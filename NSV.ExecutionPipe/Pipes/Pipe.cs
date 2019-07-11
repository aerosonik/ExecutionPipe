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
using NSV.ExecutionPipe.Helpers;

namespace NSV.ExecutionPipe.Pipes
{
    public abstract class Pipe<M, R> :
        IPipe<M, R>,
        IParallelPipe<M, R>,
        ISequentialPipe<M, R>,
        ILocalCache
    {
        #region Private fields

        private PipeExecutionType _type = PipeExecutionType.None;
        private bool _finished = false;
        private readonly Queue<ExecutorContainer<M, R>> _executionQueue = new Queue<ExecutorContainer<M, R>>();
        private Optional<M> _model = Optional<M>.Default;
        private Optional<Stopwatch> _stopWatch = Optional<Stopwatch>.Default;
        private Optional<List<PipeResult<R>>> _results = Optional<List<PipeResult<R>>>.Default;
        private Optional<IDictionary<object, object>> _localCache = Optional<IDictionary<object, object>>.Default;
        private Optional<ILocalCache> _externalCache = Optional<ILocalCache>.Default;
        private ExecutorContainer<M, R> _current;
        private bool _useParentalCache = false;
        private Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>> _ifConditionStack 
                = Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>>.Default;

        #endregion

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
            Executor<M, R> executor)
        {
            return AddExecutor(executor);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.AddExecutor(
            Executor<M, R> executor,
            bool addif)
        {
            return AddExecutor(executor, addif);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.AddExecutor(
            Lazy<Executor<M, R>> executor)
        {
            return AddExecutor(executor);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.AddExecutor(
            Lazy<Executor<M, R>> executor,
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
            Executor<M, R> executor)
        {
            return AddExecutor(executor);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.AddExecutor(
            Executor<M, R> executor,
            bool addif)
        {
            return AddExecutor(executor, addif);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.AddExecutor(
           Lazy<Executor<M, R>> executor)
        {
            return AddExecutor(executor);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.AddExecutor(
            Lazy<Executor<M, R>> executor,
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
            ExecutorContainer<M, R> container,
            PipeResult<R> result)
        {
            return (result.Success == ExecutionResult.Failed &&
                        container.BreakIfFailed) ||
                   (result.Break && container.AllowBreak);
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
                var container = _executionQueue.Dequeue();
                if (!RunExecutorIfConditions(container, _model))
                    continue;

                if (container.IsAsync)
                    AsyncHelper.RunSync(ExecuteSubPipeAsync(container, _results.Value));
                else
                    ExecuteSubPipe(container, _results.Value);

                var result = RunHelper(container);

                if (container.Retry.HasValue && result.Success == ExecutionResult.Failed)
                {
                    for (int i = 0; i < container.Retry.Value.Count; i++)
                    {
                        result = RunHelper(container);
                        if (result.Success == ExecutionResult.Successful)
                            break;
                    }
                }
                _results.Value.Add(result);
                if (Break(container, result))
                {
                    if (container.CreateResult != null)
                        return container.CreateResult(_model, result);
                    break;
                }
            }

            if (_results.HasValue)
                return CreateResult(_model, _results.Value.ToArray());

            return CreateResult(_model, null);
        }
        private PipeResult<R> RunHelper(ExecutorContainer<M, R> container)
        {
            if (container.IsAsync)
                return AsyncHelper.RunSync(container
                    .Executor.RunAsync(_model, container.UseStopWatch));
            else
                return container.Executor.Run(_model, container.UseStopWatch);
        }
        private PipeResult<R> RunSubPipeHelper(ExecutorContainer<M, R> container)
        {
            if (container.Executor.IsAsync)
                return AsyncHelper.RunSync(RunSubPipeAsync(container));
            else
                return RunSubPipe(container);
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
                var container = _executionQueue.Dequeue();
                if (!RunExecutorIfConditions(container, _model))
                    continue;

                if (container.IsAsync)
                    await ExecuteSubPipeAsync(container, _results.Value);
                else
                    ExecuteSubPipe(container, _results.Value);

                var result = container.IsAsync
                    ? await container.Executor.RunAsync(_model, container.UseStopWatch)
                    : container.Executor.Run(_model, container.UseStopWatch);

                if (container.Retry.HasValue && result.Success == ExecutionResult.Failed)
                {
                    for (int i = 0; i < container.Retry.Value.Count; i++)
                    {
                        result = container.Executor.IsAsync
                            ? await container.Executor.RunAsync(_model, container.UseStopWatch)
                            : container.Executor.Run(_model, container.UseStopWatch);
                        if (result.Success == ExecutionResult.Successful)
                            break;
                    }
                }
                _results.Value.Add(result);

                if (Break(container, result))
                {
                    if (container.CreateResult != null)
                        return container.CreateResult(_model, result);
                    break;
                }
            }

            if (_results.HasValue)
                return CreateResult(_model, _results.Value.ToArray());

            return CreateResult(_model, null);
        }

        private PipeResult<R>[] RunParallel()
        {
            return _executionQueue
                .AsParallel()
                .Where(x => RunExecutorIfConditions(x, _model))
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .SelectMany(x => RunExecutorForParallel(x))
                .ToArray();
        }

        private async Task<PipeResult<R>[]> RunParallelAsync()
        {
            var allInvocations = _executionQueue
                .Where(x => RunExecutorIfConditions(x, _model));
            var asyncInvocations = allInvocations
                .Where(x => x.Executor.IsAsync)
                .Select(x => RunExecutorForParallelTaskAsync(x))
                .Concat(allInvocations
                    .Where(x => !x.Executor.IsAsync)
                    .Select(x => RunExecutorForParallelTask(x)));

            var results = await Task.WhenAll(asyncInvocations);
            return results.SelectMany(x => x).ToArray();
        }

        private async Task<PipeResult<R>[]> RunExecutorForParallelTaskAsync(ExecutorContainer<M, R> container)
        {
            var subPipeResult = await RunSubPipeAsync(container);
            var result = await container.Executor
                .RunAsync(_model, container.UseStopWatch);
            if (container.Retry.HasValue && result.Success == ExecutionResult.Failed)
            {
                for (int i = 0; i < container.Retry.Value.Count; i++)
                {
                    result = await container.Executor
                        .RunAsync(_model, container.UseStopWatch);
                    if (result.Success == ExecutionResult.Successful)
                        break;
                }
            }
            return subPipeResult.Success == ExecutionResult.Initial
                    ? new[] { result }
                    : new[] { result, subPipeResult };
        }

        private Task<PipeResult<R>[]> RunExecutorForParallelTask(ExecutorContainer<M, R> container)
        {
            return Task.Run(() =>
            {
                return RunExecutorForParallel(container);
            });
        }

        private PipeResult<R>[] RunExecutorForParallel(ExecutorContainer<M, R> container)
        {
            var subPipeResult = RunSubPipeHelper(container);
            var result = RunHelper(container);
            if (container.Retry.HasValue && result.Success == ExecutionResult.Failed)
            {
                for (int i = 0; i < container.Retry.Value.Count; i++)
                {
                    result = RunHelper(container);
                    if (result.Success == ExecutionResult.Successful)
                        break;
                }
            }
            return subPipeResult.Success == ExecutionResult.Initial
                ? new[] { result }
                : new[] { result, subPipeResult };
        }

        private void ExecuteSubPipe(
            ExecutorContainer<M, R> container,
            List<PipeResult<R>> results)
        {
            var subPipeResult = RunSubPipe(container);
            if (subPipeResult.Success != ExecutionResult.Initial)
                results.Add(subPipeResult);
        }

        private async Task ExecuteSubPipeAsync(
            ExecutorContainer<M, R> container,
            List<PipeResult<R>> results)
        {
            var subPipeResult = await RunSubPipeAsync(container);
            if (subPipeResult.Success != ExecutionResult.Initial)
                results.Add(subPipeResult);
        }

        private PipeResult<R> RunSubPipe(ExecutorContainer<M, R> container)
        {
            if (container.Executor is IPipeExecutor<M, R> pipeItem)
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
        private async Task<PipeResult<R>> RunSubPipeAsync(ExecutorContainer<M, R> container)
        {
            if (container.Executor is IPipeExecutor<M, R> pipeItem)
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
        private Pipe<M, R> AddExecutor(Executor<M, R> executor, bool addif = true)
        {
            if (!addif)
                return this;

            if (IfConstantConditions())
            {
                var container = new ExecutorContainer<M,R>(executor);
                container.LocalCache = this;
                container.ExecuteConditions = GetCalculatedConditions();
                _executionQueue.Enqueue(container);
                _current = container;
            }
            return this;
        }
        private Pipe<M, R> AddExecutor(Lazy<Executor<M, R>> executor, bool addif = true)
        {
            if (!addif)
                return this;

            if (IfConstantConditions())
            {
                var container = new ExecutorContainer<M, R>(executor);
                container.LocalCache = this;
                container.ExecuteConditions = GetCalculatedConditions();
                _executionQueue.Enqueue(container);
                _current = container;
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
            if (_current.Executor is IPipeExecutor<M, R> && pipe != null)
            {
                var current = (IPipeExecutor<M, R>)_current.Executor;
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
        private bool RunExecutorIfConditions(ExecutorContainer<M, R> container, M model)
        {
            return container.ExecuteConditions.HasValue
                ? container.ExecuteConditions.Value.All(x => x(model))
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
