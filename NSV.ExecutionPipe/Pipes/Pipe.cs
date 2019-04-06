using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace NSV.ExecutionPipe.Pipes
{
    public abstract class Pipe<M, R> : IPipe<M, R>, IParallelPipe<M, R>, ISequentialPipe<M, R>, ILocalCache
    {
        private PipeExecutionType _type = PipeExecutionType.None;
        private bool _finished = false;
        private readonly Queue<IExecutor<M, R>> _executionQueue = new Queue<IExecutor<M, R>>();
        private Optional<M> _model = Optional<M>.Default;
        private Optional<Stopwatch> _stopWatch = Optional<Stopwatch>.Default;
        private Optional<List<PipeResult<R>>> _results = Optional<List<PipeResult<R>>>.Default;
        private Optional<IDictionary<object, object>> _localCache = Optional<IDictionary<object, object>>.Default;
        private IExecutor<M, R> _current;
        public TimeSpan Elapsed { get; private set; }

        public IParallelPipe<M, R> AsParallel()
        {
            _type = PipeExecutionType.Parallel;
            return this;
        }

        public ISequentialPipe<M, R> AsSequential()
        {
            _type = PipeExecutionType.Sequential;
            _results = new List<PipeResult<R>>();
            return this;
        }

        #region AddExecutor(IExecutor<M, R> executor)
        private Pipe<M, R> AddExecutor(IExecutor<M, R> executor)
        {
            executor.LocalCache = this;
            executor.Model = _model.Value;
            _executionQueue.Enqueue(executor);
            _current = executor;
            return this;
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.AddExecutor(IExecutor<M, R> executor)
        {
            return AddExecutor(executor);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.AddExecutor(IExecutor<M, R> executor)
        {
            return AddExecutor(executor);
        }
        #endregion

        #region SetModel(M model)
        private Pipe<M, R> SetModel(M model)
        {
            _current.Model = model;
            return this;
        }

        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetModel(M model)
        {
            return SetModel(model);
        }

        IParallelPipe<M, R> IParallelPipe<M, R>.SetModel(M model)
        {
            return SetModel(model);
        }
        #endregion

        #region SetSkipIf(Func<M, bool> condition)
        private Pipe<M, R> SetSkipIf(Func<M, bool> condition)
        {
            _current.SkipCondition = condition;
            return this;
        }

        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetSkipIf(Func<M, bool> condition)
        {
            return SetSkipIf(condition);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.SetSkipIf(Func<M, bool> condition)
        {
            return SetSkipIf(condition);
        }
        #endregion

        #region SetBreakIfFailed(bool value = true)
        private Pipe<M, R> SetBreakIfFailed(bool value = true)
        {
            _current.BreakIfFailed = value;
            return this;
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetBreakIfFailed()
        {
            return SetBreakIfFailed(true);
        }

        #endregion

        #region SetAllowBreak(bool value = true)
        private ISequentialPipe<M, R> SetAllowBreak(bool value = true)
        {
            _current.AllowBreak = value;
            return this;
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetAllowBreak()
        {
            return SetAllowBreak(true);
        }
        #endregion

        #region SetSubPipe(IPipe<M, R> pipe, Func<M, bool> condition)
        private Pipe<M, R> SetSubPipe(IPipe<M, R> pipe, Func<M, bool> condition = null)
        {
            if (_current is IPipeExecutor<M, R> && pipe != null)
            {
                var current = (IPipeExecutor<M, R>)_current;
                current.Pipe = pipe;
                current.PipeExecutionCondition = condition;
            }
            return this;
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetSubPipe(IPipe<M, R> pipe, Func<M, bool> condition)
        {
            return SetSubPipe(pipe, condition);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.SetSubPipe(IPipe<M, R> pipe, Func<M, bool> condition)
        {
            return SetSubPipe(pipe, condition);
        }
        #endregion

        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            _current.CreateResult = handler;
            return this;
        }


        public IPipe<M, R> UseStopWatch()
        {
            _stopWatch = new Stopwatch();
            return this;
        }

        public IPipe<M, R> UseModel(M model = default)
        {
            _model = model;
            return this;
        }

        public IPipe<M, R> UseLocalCacheThreadSafe()
        {
            _localCache = new ConcurrentDictionary<object, object>();
            return this;
        }

        public IPipe<M, R> UseLocalCache()
        {
            _localCache = new Dictionary<object, object>();
            return this;
        }

        public IPipe<M, R> Finish()
        {
            if (_finished)
                throw new Exception(Const.PipeIsAlreadyFinished);

            _finished = true;
            return this;
        }

        public object GetObject(object key)
        {
            if (!_localCache.HasValue)
                return null;

            if (_localCache.Value.TryGetValue(key, out var value))
                return value;

            return null;
        }

        public void SetObject(object key, object value)
        {
            if (!_localCache.HasValue)
                return;

            _localCache.Value.Add(key, value);
        }

        public ILocalCache GetLocalCacheObject()
        {
            return this;
        }


        public abstract PipeResult<R> CreateResult(
            M model,
            PipeResult<R>[] results);

        public PipeResult<R> Run()
        {

            if (_type == PipeExecutionType.None)
                throw new Exception(Const.PipeNotConfigured);

            if (!_stopWatch.HasValue)
                return RunPipe();

            _stopWatch.Value.Start();
            var result = RunPipe();
            _stopWatch.Value.Stop();
            Elapsed = _stopWatch.Value.Elapsed;
            return result;
        }

        public async Task<PipeResult<R>> RunAsync()
        {
            if (_type == PipeExecutionType.None)
                throw new Exception(Const.PipeNotConfigured);

            if (!_stopWatch.HasValue)
                return await RunPipeAsync();

            _stopWatch.Value.Start();
            var result = await RunPipeAsync();
            _stopWatch.Value.Stop();
            Elapsed = _stopWatch.Value.Elapsed;
            return result;
        }

        #region Private
        private bool Break(
            IExecutor<M, R> item,
            PipeResult<R> result)
        {
            return (result.Success == ExecutionResult.Unsuccessful &&
                        item.BreakIfFailed) ||
                   (result.Break && item.AllowBreak);
        }

        private PipeResult<R> RunPipe()
        {
            _current = null;

            if (_type == PipeExecutionType.Parallel)
            {
                var results = RunParallel();
                return CreateResult(_model, results.ToArray());
            }

            while (_executionQueue.Count > 0)
            {
                var item = _executionQueue.Dequeue();
                if (item.SkipCondition != null && item.SkipCondition(item.Model))
                    continue;

                ExecuteSubPipe(item, _results.Value);

                var result = item.Execute();
                _results.Value.Add(result);

                if (Break(item, result))
                {
                    if (item.CreateResult != null)
                        return item.CreateResult(_model, result);
                    break;
                }
            }

            if (_results.HasValue)
                return CreateResult(_model, _results.Value.ToArray());

            return CreateResult(_model, null);
        }

        private async Task<PipeResult<R>> RunPipeAsync()
        {
            _current = null;

            if (_type == PipeExecutionType.Parallel)
            {
                var results = await RunParallelAsync();
                return CreateResult(_model, results);
            }

            while (_executionQueue.Count > 0)
            {
                var item = _executionQueue.Dequeue();
                if (item.SkipCondition != null && item.SkipCondition(item.Model))
                    continue;

                await ExecuteSubPipeAsync(item, _results.Value);

                var result = await item.ExecuteAsync();
                _results.Value.Add(result);

                if (Break(item, result))
                {
                    if (item.CreateResult != null)
                        return item.CreateResult(_model, result);
                    break;
                }
            }

            if (_results.HasValue)
                return CreateResult(_model, _results.Value.ToArray());

            return CreateResult(_model, null);
        }

        private PipeResult<R>[] RunParallel()
        {
            var paralelResults = _executionQueue.AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .SelectMany(x =>
                {
                    var pipeResult = x.Execute();
                    var subPipeResult = RunSubPipe(x);
                    return subPipeResult.Success == ExecutionResult.Initial
                        ? new[] { pipeResult }
                        : new[] { pipeResult, subPipeResult };

                }).ToList();

            return paralelResults.ToArray();
        }

        private async Task<PipeResult<R>[]> RunParallelAsync()
        {
            var paralelResults = _executionQueue.AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .Select(async x =>
                {
                    var pipeResult = await x.ExecuteAsync();
                    var subPipeResult = await RunSubPipeAsync(x);
                    return subPipeResult.Success == ExecutionResult.Initial
                        ? new[] { pipeResult }
                        : new[] { pipeResult, subPipeResult };
                });

            var result = await Task.WhenAll(paralelResults);
            return result.SelectMany(x => x).ToArray();
        }

        private void ExecuteSubPipe(IExecutor<M, R> executor, List<PipeResult<R>> results)
        {
            var subPipeResult = RunSubPipe(executor);
            if (subPipeResult.Success != ExecutionResult.Initial)
                results.Add(subPipeResult);
        }

        private async Task ExecuteSubPipeAsync(IExecutor<M, R> executor, List<PipeResult<R>> results)
        {
            var subPipeResult = await RunSubPipeAsync(executor);
            if (subPipeResult.Success != ExecutionResult.Initial)
                results.Add(subPipeResult);
        }

        private PipeResult<R> RunSubPipe(IExecutor<M, R> executor)
        {
            if (executor is IPipeExecutor<M, R> pipeItem)
            {
                if (pipeItem.PipeExecutionCondition != null &&
                pipeItem.PipeExecutionCondition(_model) ||
                pipeItem.PipeExecutionCondition == null)
                {
                    return pipeItem.Pipe.Run();
                }
            }
            return PipeResult<R>.Default;
        }

        private async Task<PipeResult<R>> RunSubPipeAsync(IExecutor<M, R> executor)
        {
            if (executor is IPipeExecutor<M, R> pipeItem)
            {
                if (pipeItem.PipeExecutionCondition != null &&
                pipeItem.PipeExecutionCondition(_model) ||
                pipeItem.PipeExecutionCondition == null)
                {
                    return await pipeItem.Pipe.RunAsync();
                }
            }
            return PipeResult<R>.Default;
        }
        #endregion

    }
}
