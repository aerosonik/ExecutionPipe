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
        private Optional<ExecutorContainer<M, R>> _defaultExecutor = Optional<ExecutorContainer<M, R>>.Default;

        #endregion

        #region IBaseExecutor<M,R>
        public string Label { get; set; }
        public bool IsAsync { get; set; }
        public IBaseExecutor<M, R> UseCache(ILocalCache cache)
        {
            if (_useParentalCache)
                _externalCache = new Optional<ILocalCache>(cache);
            return this;
        }
        public PipeResult<R> Execute(M model)
        {
            SetModel(model);
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

        public async Task<PipeResult<R>> ExecuteAsync(M model)
        {
            SetModel(model);
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
        ISequentialPipe<M, R> ISequentialPipe<M, R>.SetDefault()
        {
            return SetDefault(true);
        }

        ISequentialPipe<M, R> ISequentialPipe<M, R>.AddExecutor(
            IBaseExecutor<M, R> executor)
        {
            return AddExecutor(executor);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.AddExecutor(
            IBaseExecutor<M, R> executor,
            bool addif)
        {
            return AddExecutor(executor, addif);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.AddExecutor(
            Func<IBaseExecutor<M, R>> executor)
        {
            return AddExecutor(executor);
        }
        ISequentialPipe<M, R> ISequentialPipe<M, R>.AddExecutor(
            Func<IBaseExecutor<M, R>> executor,
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
            IBaseExecutor<M, R> executor)
        {
            return AddExecutor(executor);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.AddExecutor(
            IBaseExecutor<M, R> executor,
            bool addif)
        {
            return AddExecutor(executor, addif);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.AddExecutor(
           Func<IBaseExecutor<M, R>> executor)
        {
            return AddExecutor(executor);
        }
        IParallelPipe<M, R> IParallelPipe<M, R>.AddExecutor(
            Func<IBaseExecutor<M, R>> executor,
            bool addif)
        {
            return AddExecutor(executor, addif);
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
                    if (_defaultExecutor.HasValue && !container.IsDefault)
                    {
                        var defaultResult = RunHelper(_defaultExecutor.Value);
                        _results.Value.Add(defaultResult);
                        break;
                    }
                    if (container.CreateResult.HasValue)
                        return container.CreateResult.Value(_model, result);
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
                return AsyncHelper.RunSync(container.RunAsync(_model));
            else
                return container.Run(_model);
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

                var result = container.IsAsync
                    ? await container.RunAsync(_model)
                    : container.Run(_model);

                if (container.Retry.HasValue && result.Success == ExecutionResult.Failed)
                {
                    for (int i = 0; i < container.Retry.Value.Count; i++)
                    {
                        result = container.IsAsync
                            ? await container.RunAsync(_model)
                            : container.Run(_model);
                        if (result.Success == ExecutionResult.Successful)
                            break;
                    }
                }
                _results.Value.Add(result);

                if (Break(container, result))
                {
                    if (container.CreateResult != null)
                        return container.CreateResult.Value(_model, result);
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
                .Select(x => RunExecutorForParallel(x))
                .ToArray();
        }

        private async Task<PipeResult<R>[]> RunParallelAsync()
        {
            var allInvocations = _executionQueue
                .Where(x => RunExecutorIfConditions(x, _model));
            var asyncInvocations = allInvocations
                .Where(x => x.IsAsync)
                .Select(x => RunExecutorForParallelTaskAsync(x))
                .Concat(allInvocations
                    .Where(x => !x.IsAsync)
                    .Select(x => RunExecutorForParallelTask(x)));

            return await Task.WhenAll(asyncInvocations);
        }

        private async Task<PipeResult<R>> RunExecutorForParallelTaskAsync(ExecutorContainer<M, R> container)
        {
            //var subPipeResult = await RunSubPipeAsync(container);
            var result = await container.RunAsync(_model);
            if (container.Retry.HasValue && result.Success == ExecutionResult.Failed)
            {
                for (int i = 0; i < container.Retry.Value.Count; i++)
                {
                    result = await container.RunAsync(_model);
                    if (result.Success == ExecutionResult.Successful)
                        break;
                }
            }
            return result;
        }

        private Task<PipeResult<R>> RunExecutorForParallelTask(ExecutorContainer<M, R> container)
        {
            return Task.Run(() =>
            {
                return RunExecutorForParallel(container);
            });
        }

        private PipeResult<R> RunExecutorForParallel(ExecutorContainer<M, R> container)
        {
            //var subPipeResult = RunSubPipeHelper(container);
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
            return result;
        }

        private Pipe<M, R> AddExecutor(
            IBaseExecutor<M, R> executor, 
            bool addif = true)
        {
            Func<IBaseExecutor<M, R>> executorFunc = () => executor;
            return AddExecutor(executorFunc, addif);
        }

        private Pipe<M, R> AddExecutor(
            Func<IBaseExecutor<M, R>> executor, 
            bool addif = true)
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

        private Pipe<M,R> SetDefault(bool isDefault)
        {
            _current.IsDefault = isDefault;
            _defaultExecutor = _current;
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
        private Pipe<M, R> SetRetryIfFailed(int count, int timeOutMilliseconds)
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

        private void SetModel(M model = default)
        {
            if (_model.HasValue)
                return;

            _model = model;
        }

        #endregion
    }
}
