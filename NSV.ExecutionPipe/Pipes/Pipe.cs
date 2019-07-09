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
    public abstract class Pipe<M, R> :
        IPipe<M, R>,
        IParallelPipe<M, R>,
        ISequentialPipe<M, R>,
        ILocalCache
    {
        private PipeExecutionType _type = PipeExecutionType.None;
        private bool _finished = false;
        private readonly Queue<IExecutor<M, R>> _executionQueue = new Queue<IExecutor<M, R>>();
        private Optional<M> _model = Optional<M>.Default;
        private Optional<Stopwatch> _stopWatch = Optional<Stopwatch>.Default;
        private Optional<List<PipeResult<R>>> _results = Optional<List<PipeResult<R>>>.Default;
        private Optional<IDictionary<object, object>> _localCache = Optional<IDictionary<object, object>>.Default;
        private Optional<ILocalCache> _externalCache = Optional<ILocalCache>.Default;
        private IExecutor<M, R> _current;
        private bool _useParentalCache = false;
        private Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>> _ifConditionStack 
                = Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>>.Default;

        #region IPipe<M, R>

        public TimeSpan Elapsed { get; private set; }

        public IParallelPipe<M, R> AsParallel()
        {
            if (_finished)
                throw new Exception(Const.PipeIsAlreadyFinished);

            _type = PipeExecutionType.Parallel;
            return this;
        }

        public ISequentialPipe<M, R> AsSequential()
        {
            if (_finished)
                throw new Exception(Const.PipeIsAlreadyFinished);

            _type = PipeExecutionType.Sequential;
            _results = new List<PipeResult<R>>();
            return this;
        }

        public IPipe<M, R> UseStopWatch()
        {
            _stopWatch = new Stopwatch();
            return this;
        }

        public IPipe<M, R> UseModel(M model = default)
        {
            if (_model.HasValue)
                return this;

            _model = model;
            return this;
        }

        public IPipe<M, R> UseLocalCacheThreadSafe()
        {
            if (_useParentalCache)
                return this;

            if (_finished)
                throw new Exception(Const.PipeIsAlreadyFinished);

            _localCache = new ConcurrentDictionary<object, object>();
            return this;
        }

        public IPipe<M, R> UseLocalCache()
        {
            if (_useParentalCache)
                return this;

            if (_finished)
                throw new Exception(Const.PipeIsAlreadyFinished);

            _localCache = new Dictionary<object, object>();
            return this;
        }

        public IPipe<M, R> UseParentalCache()
        {
            _useParentalCache = true;
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

        #endregion

        #region IBasePipe<M, R>

        public IPipe<M, R> Finish()
        {
            if (_finished)
                throw new Exception(Const.PipeIsAlreadyFinished);

            _finished = true;
            return this;
        }

        #endregion

        #region ILocalCache

        public T GetObject<T>(object key)
        {
            if (_useParentalCache && _externalCache.HasValue)
                return _externalCache.Value.GetObject<T>(key);

            if (_localCache.HasValue && !_useParentalCache)
                if (_localCache.Value.TryGetValue(key, out var value))
                    return (T)value;

            return default(T);
        }

        public void SetObject<T>(object key, T value)
        {
            if(_useParentalCache && _externalCache.HasValue)
            {
                _externalCache.Value.SetObject(key, value);
                return;
            }
            if (_localCache.HasValue && !_useParentalCache)
            {
                _localCache.Value.Add(key, value);
                return;
            }
        }

        public ILocalCache GetLocalCacheObject()
        {
            return this;
        }

        #endregion

        #region ISequentialPipe<M,R> Explicitly

        ISequentialPipe<M, R> ISequentialPipe<M, R>.AddExecutor(
            IExecutor<M, R> executor)
        {
            return AddExecutor(executor);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.AddExecutor(
           IExecutor<M, R> executor,
            bool addif)
        {
            return AddExecutor(executor, addif);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetBreakIfFailed()
        {
            return SetBreakIfFailed(true);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetAllowBreak()
        {
            return SetAllowBreak(true);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetSubPipe(
            IPipe<M, R> pipe, 
            Func<M, bool> condition)
        {
            return SetSubPipe(pipe, condition);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetResultHandler(
            Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            _current.CreateResult = handler;
            return this;
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetUseStopWatch()
        {
            return SetUseStopWatch();
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetLabel(string label)
        {
            return SetLabel(label);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetRetryIfFailed(
            int count,
            int timeOutMilliseconds)
        {
            return SetRetryIfFailed(count, timeOutMilliseconds);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.If(bool condition)
        {
            return If(condition);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.If(Func<M, bool> condition)
        {
            return If(condition);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.EndIf()
        {
            return EndIf();
        }
        #endregion

        #region IParallelPipe<M, R> Explicitly

        IParallelPipe<M, R> IParallelPipe<M, R>.AddExecutor(
            IExecutor<M, R> executor)
        {
            return AddExecutor(executor);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.AddExecutor(
            IExecutor<M, R> executor,
            bool addif)
        {
            return AddExecutor(executor, addif);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.SetSubPipe(
            IPipe<M, R> pipe, Func<M, bool> condition)
        {
            return SetSubPipe(pipe, condition);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.SetUseStopWatch()
        {
            return SetUseStopWatch();
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.SetLabel(
            string label)
        {
            return SetLabel(label);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.If(bool condition)
        {
            return If(condition);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.If(Func<M, bool> condition)
        {
            return If(condition);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.EndIf()
        {
            return EndIf();
        }

        #endregion

        #region Private Methods

        private bool Break(
            IExecutor<M, R> item,
            PipeResult<R> result)
        {
            return (result.Success == ExecutionResult.Failed &&
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
                if (!RunExecutorIfConditions(item, _model))
                    continue;

                if (item.IsAsync)
                    ExecuteSubPipeAsync(item, _results.Value).Wait();
                else
                    ExecuteSubPipe(item, _results.Value);

                var result = RunHelper(item);

                if (item.Retry.HasValue && result.Success == ExecutionResult.Failed)
                {
                    for (int i = 0; i < item.Retry.Value.Count; i++)
                    {
                        result = RunHelper(item);
                        if (result.Success == ExecutionResult.Successful)
                            break;
                    }
                }
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
        private PipeResult<R> RunHelper(IExecutor<M,R> executor)
        {
            if (executor.IsAsync)
                return executor.RunAsync(_model).Result;
            else
                return executor.Run(_model);
        }
        private PipeResult<R> RunSubPipeHelper(IExecutor<M, R> executor)
        {
            if (executor.IsAsync)
                return RunSubPipeAsync(executor).Result;
            else
                return RunSubPipe(executor);
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
                if (!RunExecutorIfConditions(item, _model))
                    continue;

                if (item.IsAsync)
                    await ExecuteSubPipeAsync(item, _results.Value);
                else
                    ExecuteSubPipe(item, _results.Value);

                var result = item.IsAsync
                    ? await item.RunAsync(_model)
                    : item.Run(_model);

                if (item.Retry.HasValue && result.Success == ExecutionResult.Failed)
                {
                    for (int i = 0; i < item.Retry.Value.Count; i++)
                    {
                        result = item.IsAsync
                            ? await item.RunAsync(_model)
                            : item.Run(_model);
                        if (result.Success == ExecutionResult.Successful)
                            break;
                    }
                }
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
                    var pipeResult = RunHelper(x); //x.Run(_model);
                    var subPipeResult = RunSubPipeHelper(x);
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
                    var pipeResult = x.IsAsync
                        ? await x.RunAsync(_model)
                        : x.Run(_model);//await x.RunAsync(_model);
                    var subPipeResult = x.IsAsync
                        ? await RunSubPipeAsync(x)
                        : RunSubPipe(x);
                    return subPipeResult.Success == ExecutionResult.Initial
                        ? new[] { pipeResult }
                        : new[] { pipeResult, subPipeResult };
                });

            var result = await Task.WhenAll(paralelResults);
            return result.SelectMany(x => x).ToArray();
        }

        private void ExecuteSubPipe(
            IExecutor<M, R> executor,
            List<PipeResult<R>> results)
        {
            var subPipeResult = RunSubPipe(executor);
            if (subPipeResult.Success != ExecutionResult.Initial)
                results.Add(subPipeResult);
        }

        private async Task ExecuteSubPipeAsync(
            IExecutor<M, R> executor,
            List<PipeResult<R>> results)
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
                    return pipeItem
                        .Pipe
                        .UseModel(_model)
                        .Run();
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
                    var result = await pipeItem
                        .Pipe
                        .UseModel(_model)
                        .RunAsync()
                        .ConfigureAwait(false);
                    pipeItem.SubPipeResult = result;
                    return result;
                }
            }
            return PipeResult<R>.Default;
        }
        private Pipe<M, R> AddExecutor(IExecutor<M, R> executor, bool addif = true)
        {
            if (!addif)
                return this;

            if (IfConstantConditions())
            {
                (executor as Executor<M, R>).ExecuteConditions = GetCalculatedConditions();
                executor.LocalCache = this;
                _executionQueue.Enqueue(executor);
                _current = executor;
            }
            return this;
        }
        private Pipe<M, R> SetBreakIfFailed(bool value = true)
        {
            _current.BreakIfFailed = value;
            return this;
        }
        private ISequentialPipe<M, R> SetAllowBreak(bool value = true)
        {
            _current.AllowBreak = value;
            return this;
        }
        private Pipe<M, R> SetSubPipe(IPipe<M, R> pipe, Func<M, bool> condition = null)
        {
            if (_current is IPipeExecutor<M, R> && pipe != null)
            {
                var current = (IPipeExecutor<M, R>)_current;
                current.Pipe = pipe;
                current.PipeExecutionCondition = condition;

                if (current.Pipe is Pipe<M, R> pipeInstance)
                    pipeInstance._externalCache = this;
            }
            return this;
        }
        private Pipe<M, R> SetUseStopWatch()
        {
            _current.UseStopWatch = true;
            return this;
        }
        private Pipe<M, R> SetLabel(string label)
        {
            _current.Label = label;
            return this;
        }
        Pipe<M, R> SetRetryIfFailed(int count, int timeOutMilliseconds)
        {
            _current.Retry = new RetryModel
            {
                Count = count,
                TimeOutMilliseconds = timeOutMilliseconds
            };
            return this;
        }
        private Pipe<M, R> If(bool condition)
        {
            if (!_ifConditionStack.HasValue)
                _ifConditionStack = new Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>();
            var ifCondition = (Optional<Func<M, bool>>.Default, condition);
            _ifConditionStack.Value.Push(ifCondition);
            return this;
        }
        private Pipe<M, R> If(Func<M, bool> condition)
        {
            if (condition == null)
                return this;

            if (!_ifConditionStack.HasValue)
                _ifConditionStack = new Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>();
            var ifCondition = (condition, Optional<bool>.Default);
            _ifConditionStack.Value.Push(ifCondition);
            return this;
        }
        private Pipe<M, R> EndIf()
        {
            if (!_ifConditionStack.HasValue || !_ifConditionStack.Value.Any())
                throw new Exception("Redundant EndIf");

            _ifConditionStack.Value.Pop();
            return this;
        }
        private bool IfConstantConditions()
        {
            if (!_ifConditionStack.HasValue)
                return true;

            return !_ifConditionStack.Value
                .Where(x => x.constant.HasValue)
                .Any(x => !x.constant.Value);             
        }
        private Optional<Func<M, bool>[]> GetCalculatedConditions()
        {
            if (!_ifConditionStack.HasValue || !_ifConditionStack.Value.Any(x => x.calculated.HasValue))
                return Optional<Func<M, bool>[]>.Default;


            return Optional<Func<M, bool>[]>.SetValue(_ifConditionStack.Value
                        .Where(x => x.calculated.HasValue)
                        .Select(x => x.calculated.Value)
                        .ToArray());
        }
        private bool RunExecutorIfConditions(IExecutor<M, R> iexecutor, M model)
        {
            var executor = (Executor<M, R>)iexecutor;
            return executor.ExecuteConditions.HasValue
                ? executor.ExecuteConditions.Value.All(x => x(model))
                : true;
        }

        #endregion

        #region Internal methods

        internal void SetParentalCache(ILocalCache cache)
        {
            if (_useParentalCache)
                _externalCache = new Optional<ILocalCache>(cache);
        }

        #endregion

    }
}
