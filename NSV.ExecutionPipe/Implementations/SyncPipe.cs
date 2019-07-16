using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;

namespace NSV.ExecutionPipe
{
    public abstract class SyncParallelPipe<M, R> :
        AbstractSyncPipe<M,R>,
        ISyncParallelPipe<M, R>,
        ISyncParallelExecutorBuilder<M, R>
    {
        public ISyncParallelExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor)
        {
            AddParallelExecutor(executor);
            return this;
        }

        public ISyncParallelExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor, bool addif)
        {
            AddParallelExecutor(executor, addif);
            return this;
        }

        public ISyncParallelExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor)
        {
            AddParallelExecutor(executor);
            return this;
        }

        public ISyncParallelExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor, bool addif)
        {
            AddParallelExecutor(executor, addif);
            return this;
        }

        public ISyncParallelPipe<M, R> Build()
        {
            _finished = true;
            return this;
        }

        public abstract PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);

        public PipeResult<R> ExecuteSync(M model, ILocalCache cache = null)
        {
            return RunSync(model, cache, RunPipeParallel);
        }

        public ISyncParallelPipe<M, R> If(bool condition)
        {
            IfCondition(condition);
            return this;
        }

        public ISyncParallelPipe<M, R> If(Func<M, bool> condition)
        {
            IfCondition(condition);
            return this;
        }
        public ISyncParallelPipe<M, R> EndIf()
        {
            EndIfCondition();
            return this;
        }

        public ISyncParallelPipe<M, R> UseLocalCacheThreadSafe()
        {
            LocalCacheThreadSafe();
            return this;
        }

        public ISyncParallelPipe<M, R> UseStopWatch()
        {
            _stopWatch = new Stopwatch();
            return this;
        }

        #region ISyncParallelExecutorBuilder<M,R>
        ISyncParallelPipe<M, R> ISyncParallelExecutorBuilder<M, R>.Build()
        {
            return this;
        }

        ISyncParallelExecutorBuilder<M, R> ISyncParallelExecutorBuilder<M, R>.Label(string label)
        {
            _current.SetLabel(label);
            return this;
        }

        ISyncParallelExecutorBuilder<M, R> ISyncParallelExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            _current.SetResultHandler(handler);
            return this;
        }

        ISyncParallelExecutorBuilder<M, R> ISyncParallelExecutorBuilder<M, R>.UseStopWatch()
        {
            _current.SetUseStopWatch();
            return this;
        }
        #endregion

        #region Private

        private PipeResult<R> RunPipeParallel()
        {
            var results = _executionQueue
                .AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .Where(x => RunExecutorIfConditions(x, _model))          
                .Select(x =>
                {
                    var result = x.Run(_model);
                    if (x.Retry.HasValue && result.Success == ExecutionResult.Failed)
                    {
                        for (int i = 0; i < x.Retry.Value.Count; i++)
                        {
                            result = x.Run(_model);
                            if (result.Success == ExecutionResult.Successful)
                                break;
                        }
                    }
                    return result;
                })
                .ToArray();
            return CreatePipeResult(_model, results);
        }

        #endregion 
    }
    public abstract class SyncSequentialPipe<M, R> :
        AbstractSyncPipe<M, R>,
        ISyncSequentialPipe<M, R>,
        ISyncSequentialExecutorBuilder<M, R>
    {
        public ISyncSequentialExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor)
        {
           AddSequentialExecutor(executor);
            return this;
        }

        public ISyncSequentialExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor, bool addif)
        {
            AddSequentialExecutor(executor, addif);
            return this;
        }

        public ISyncSequentialExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor)
        {
            AddSequentialExecutor(executor);
            return this;
        }

        public ISyncSequentialExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor, bool addif)
        {
            AddSequentialExecutor(executor, addif);
            return this;
        }

        public ISyncSequentialPipe<M, R> Build()
        {
            _finished = true;
            return this;
        }

        public abstract PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);

        public PipeResult<R> ExecuteSync(M model, ILocalCache cache = null)
        {
            return RunSync(model, cache, RunPipeSequential);
        }

        public ISyncSequentialPipe<M, R> If(bool condition)
        {
            IfCondition(condition);
            return this;
        }

        public ISyncSequentialPipe<M, R> If(Func<M, bool> condition)
        {
            IfCondition(condition);
            return this;
        }
        public ISyncSequentialPipe<M, R> EndIf()
        {
            EndIfCondition();
            return this;
        }
        public ISyncSequentialPipe<M, R> UseLocalCache()
        {
            LocalCache();
            return this;
        }

        public ISyncSequentialPipe<M, R> UseLocalCacheThreadSafe()
        {
            LocalCacheThreadSafe();
            return this;
        }

        public ISyncSequentialPipe<M, R> UseStopWatch()
        {
            _stopWatch = new Stopwatch();
            return this;
        }

        #region ISyncSequentialExecutorBuilder<M, R>
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.AllowBreak()
        {
            _current.SetAllowBreak();
            return this;
        }
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.BreakIfFailed()
        {
            _current.SetBreakIfFailed();
            return this;
        }
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.Label(string label)
        {
            _current.SetLabel(label);
            return this;
        }
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            _current.SetResultHandler(handler);
            return this;
        }
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.RetryIfFailed(int count, int timeOutMilliseconds)
        {
            _current.SetRetryIfFailed(count, timeOutMilliseconds);
            return this;
        }
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.UseStopWatch()
        {
            _current.SetUseStopWatch();
            return this;
        }
        ISyncSequentialPipe<M, R> ISyncSequentialExecutorBuilder<M, R>.Build()
        {
            return this;
        }
        #endregion

        #region Private
        private PipeResult<R> RunPipeSequential()
        {
            while (_executionQueue.Count > 0)
            {
                var container = _executionQueue.Dequeue();
                if (!RunExecutorIfConditions(container, _model))
                    continue;

                var result = container.Run(_model);

                if (container.Retry.HasValue && result.Success == ExecutionResult.Failed)
                {
                    for (int i = 0; i < container.Retry.Value.Count; i++)
                    {
                        result = container.Run(_model);
                        if (result.Success == ExecutionResult.Successful)
                            break;
                    }
                }
                _results.Value.Add(result);
                if (Break(container, result))
                {
                    if (container.CreateResult.HasValue)
                        return container.CreateResult.Value(_model, result);
                    break;
                }
            }
            if (_results.HasValue)
                return CreatePipeResult(_model, _results.Value.ToArray());

            return CreatePipeResult(_model, null);
        }
        #endregion
    }
    public abstract class AsyncParallelPipe<M, R> :
        AbstractAsyncPipe<M, R>,
        IAsyncParallelPipe<M, R>,
        IAsyncParallelExecutorBuilder<M, R>
    {
        public IAsyncParallelExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor)
        {
            AddParallelExecutor(executor);
            return this;
        }

        public IAsyncParallelExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor, bool addif)
        {
            AddParallelExecutor(executor, addif);
            return this;
        }

        public IAsyncParallelExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor)
        {
            AddParallelExecutor(executor);
            return this;
        }

        public IAsyncParallelExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor, bool addif)
        {
            AddParallelExecutor(executor, addif);
            return this;
        }

        public IAsyncParallelPipe<M, R> Build()
        {
            _finished = true;
            return this;
        }

        public abstract PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);

        public IAsyncParallelPipe<M, R> EndIf()
        {
            EndIfCondition();
            return this;
        }

        public Task<PipeResult<R>> ExecuteAsync(M model, ILocalCache cache = null)
        {
            return RunAsync(model, cache, RunPipeParallelAsync);
        }

        public IAsyncParallelPipe<M, R> If(bool condition)
        {
            IfCondition(condition);
            return this;
        }

        public IAsyncParallelPipe<M, R> If(Func<M, bool> condition)
        {
            IfCondition(condition);
            return this;
        }

        public IAsyncParallelPipe<M, R> UseLocalCacheThreadSafe()
        {
            LocalCacheThreadSafe();
            return this;
        }

        public IAsyncParallelPipe<M, R> UseStopWatch()
        {
            _stopWatch = new Stopwatch();
            return this;
        }

        #region IAsyncParallelExecutorBuilder<M, R>
        IAsyncParallelPipe<M, R> IAsyncParallelExecutorBuilder<M, R>.Build()
        {
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.Label(string label)
        {
            _current.SetLabel(label);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            _current.SetResultHandler(handler);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.UseStopWatch()
        {
            _current.SetUseStopWatch();
            return this;
        }
        #endregion

        #region Private

        private async Task<PipeResult<R>> RunPipeParallelAsync()
        {
            var results = _executionQueue
                .AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .Where(x => RunExecutorIfConditions(x, _model))
                .Select(async x =>
                {
                    var result = await x.RunAsync(_model);
                    if (x.Retry.HasValue && result.Success == ExecutionResult.Failed)
                    {
                        for (int i = 0; i < x.Retry.Value.Count; i++)
                        {
                            result = await x.RunAsync(_model);
                            if (result.Success == ExecutionResult.Successful)
                                break;
                        }
                    }
                    return result;
                })
                .ToArray();

            return CreatePipeResult(_model, await Task.WhenAll(results));
        }

        #endregion 
    }
    public abstract class AsyncSequentialPipe<M, R> :
        AbstractAsyncPipe<M, R>,
        IAsyncSequentialPipe<M, R>,
        IAsyncSequentialExecutorBuilder<M, R>
    {
        public IAsyncSequentialExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor)
        {
            AddSequentialExecutor(executor);
            return this;
        }

        public IAsyncSequentialExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor, bool addif)
        {
            AddSequentialExecutor(executor, addif);
            return this;
        }

        public IAsyncSequentialExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor)
        {
            AddSequentialExecutor(executor);
            return this;
        }

        public IAsyncSequentialExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor, bool addif)
        {
            AddSequentialExecutor(executor, addif);
            return this;
        }

        public IAsyncSequentialPipe<M, R> Build()
        {
            _finished = true;
            return this;
        }

        public abstract PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);

        public IAsyncSequentialPipe<M, R> EndIf()
        {
            EndIfCondition();
            return this;
        }

        public Task<PipeResult<R>> ExecuteAsync(M model, ILocalCache cache = null)
        {
            return RunAsync(model, cache, RunPipeSequentialAsync);
        }

        public IAsyncSequentialPipe<M, R> If(bool condition)
        {
            IfCondition(condition);
            return this;
        }

        public IAsyncSequentialPipe<M, R> If(Func<M, bool> condition)
        {
            IfCondition(condition);
            return this;
        }

        public IAsyncSequentialPipe<M, R> UseLocalCache()
        {
            LocalCache();
            return this;
        }

        public IAsyncSequentialPipe<M, R> UseLocalCacheThreadSafe()
        {
            LocalCacheThreadSafe();
            return this;
        }

        public IAsyncSequentialPipe<M, R> UseStopWatch()
        {
            _stopWatch = new Stopwatch();
            return this;
        }

        #region IAsyncSequentialExecutorBuilder<M,R>
        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.AllowBreak()
        {
            _current.SetAllowBreak();
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.BreakIfFailed()
        {
            _current.SetBreakIfFailed();
            return this;
        }

        IAsyncSequentialPipe<M, R> IAsyncSequentialExecutorBuilder<M, R>.Build()
        {
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.Label(string label)
        {
            _current.SetLabel(label);
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            _current.SetResultHandler(handler);
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.RetryIfFailed(int count, int timeOutMilliseconds)
        {
            _current.SetRetryIfFailed(count, timeOutMilliseconds);
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.UseStopWatch()
        {
            _current.SetUseStopWatch();
            return this;
        }
        #endregion

        #region Private
        private async Task<PipeResult<R>> RunPipeSequentialAsync()
        {
            while (_executionQueue.Count > 0)
            {
                var container = _executionQueue.Dequeue();
                if (!RunExecutorIfConditions(container, _model))
                    continue;

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
                _results.Value.Add(result);
                if (Break(container, result))
                {
                    if (container.CreateResult.HasValue)
                        return container.CreateResult.Value(_model, result);
                    break;
                }
            }
            if (_results.HasValue)
                return CreatePipeResult(_model, _results.Value.ToArray());

            return CreatePipeResult(_model, null);
        }
        #endregion
    }

    public abstract class AbstractPipe<M,R>: ILocalCache
    {
        protected bool _finished = false;
        protected Optional<M> _model = Optional<M>.Default;
        protected Optional<Stopwatch> _stopWatch = Optional<Stopwatch>.Default;
        protected Optional<List<PipeResult<R>>> _results = Optional<List<PipeResult<R>>>.Default;
        protected Optional<IDictionary<object, object>> _localCache = Optional<IDictionary<object, object>>.Default;
        protected Optional<ILocalCache> _externalCache = Optional<ILocalCache>.Default;
        protected Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>> _ifConditionStack
                = Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>>.Default;
        protected bool _useParentalCache = false;

        protected bool IfConstantConditions()
        {
            if (!_ifConditionStack.HasValue)
                return true;

            return !_ifConditionStack.Value
                .Where(x => x.constant.HasValue)
                .Any(x => !x.constant.Value);
        }
        protected Optional<Func<M, bool>[]> GetCalculatedConditions()
        {
            if (!_ifConditionStack.HasValue || !_ifConditionStack.Value.Any(x => x.calculated.HasValue))
                return Optional<Func<M, bool>[]>.Default;


            return Optional<Func<M, bool>[]>.SetValue(_ifConditionStack.Value
                        .Where(x => x.calculated.HasValue)
                        .Select(x => x.calculated.Value)
                        .ToArray());
        }

        protected void IfCondition(bool condition)
        {
            if (!_ifConditionStack.HasValue)
                _ifConditionStack = new Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>();
            var ifCondition = (Optional<Func<M, bool>>.Default, condition);
            _ifConditionStack.Value.Push(ifCondition);
        }
        protected void IfCondition(Func<M, bool> condition)
        {
            if (condition == null)
                return;

            if (!_ifConditionStack.HasValue)
                _ifConditionStack = new Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>();
            var ifCondition = (condition, Optional<bool>.Default);
            _ifConditionStack.Value.Push(ifCondition);
        }
        protected void EndIfCondition()
        {
            if (!_ifConditionStack.HasValue || !_ifConditionStack.Value.Any())
                throw new Exception("Redundant EndIf");

            _ifConditionStack.Value.Pop();
        }
        public void LocalCacheThreadSafe()
        {
            if (_useParentalCache)
                return;

            if (_finished)
                throw new Exception(Const.PipeIsAlreadyFinished);

            _localCache = new ConcurrentDictionary<object, object>();
        }
        public void LocalCache()
        {
            if (_useParentalCache)
                return;

            if (_finished)
                throw new Exception(Const.PipeIsAlreadyFinished);

            _localCache = new Dictionary<object, object>();
        }
        internal bool RunExecutorIfConditions(BaseExecutorContainer<M, R> container, M model)
        {
            return container.ExecuteConditions.HasValue
                ? container.ExecuteConditions.Value.All(x => x(model))
                : true;
        }

        internal bool Break(BaseExecutorContainer<M, R> container, PipeResult<R> result)
        {
            return (result.Success == ExecutionResult.Failed &&
                        container.BreakIfFailed) ||
                   (result.Break && container.AllowBreak);
        }

        protected PipeResult<R>  RunSync(M model, ILocalCache cache, Func<PipeResult<R>> func)
        {
            _model = model;
            if (_useParentalCache)
                _externalCache = Optional<ILocalCache>.SetValue(cache);

            if (!_stopWatch.HasValue)
                return func();

            _stopWatch.Value.Start();
            var result = func();
            _stopWatch.Value.Stop();
            return result.SetElapsed(_stopWatch.Value.Elapsed);
        }

        protected async Task<PipeResult<R>> RunAsync(M model, ILocalCache cache, Func<Task<PipeResult<R>>> func)
        {
            _model = model;
            if (_useParentalCache)
                _externalCache = Optional<ILocalCache>.SetValue(cache);

            if (!_stopWatch.HasValue)
                return await func();

            _stopWatch.Value.Start();
            var result = await func();
            _stopWatch.Value.Stop();
            return result.SetElapsed(_stopWatch.Value.Elapsed);
        }

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
            if (_useParentalCache && _externalCache.HasValue)
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

    }

    public abstract class AbstractSyncPipe<M, R> : AbstractPipe<M, R>
    {
        internal readonly Queue<SyncExecutorContainer<M, R>> _executionQueue
            = new Queue<SyncExecutorContainer<M, R>>();
        internal SyncExecutorContainer<M, R> _current;

        protected void AddParallelExecutor(ISyncExecutor<M, R> executor, bool addif = true)
        {
            if (!addif)
                return;

            if (IfConstantConditions())
            {
                var container = new SyncExecutorContainer<M, R>(executor)
                {
                    LocalCache = this,
                    ExecuteConditions = GetCalculatedConditions()
                };
                _executionQueue.Enqueue(container);
                _current = container;
            }
        }
        protected void AddParallelExecutor(Func<ISyncExecutor<M, R>> executor, bool addif = true)
        {
            if (!addif)
                return;

            if (IfConstantConditions())
            {
                var container = new SyncExecutorContainer<M, R>(executor)
                {
                    LocalCache = this,
                    ExecuteConditions = GetCalculatedConditions()
                };
                _executionQueue.Enqueue(container);
                _current = container;
            }
        }

        protected void AddSequentialExecutor(ISyncExecutor<M, R> executor, bool addif = true)
        {
            AddParallelExecutor(executor, addif);
        }
        protected void AddSequentialExecutor(Func<ISyncExecutor<M, R>> executor, bool addif = true)
        {
            AddParallelExecutor(executor, addif);
        }
   
    }

    public abstract class AbstractAsyncPipe<M, R> : AbstractPipe<M, R>
    {
        internal readonly Queue<AsyncExecutorContainer<M, R>> _executionQueue 
            = new Queue<AsyncExecutorContainer<M, R>>();
        internal AsyncExecutorContainer<M, R> _current;

        protected void AddParallelExecutor(IAsyncExecutor<M, R> executor, bool addif = true)
        {
            if (!addif)
                return;

            if (IfConstantConditions())
            {
                var container = new AsyncExecutorContainer<M, R>(executor)
                {
                    LocalCache = this,
                    ExecuteConditions = GetCalculatedConditions()
                };
                _executionQueue.Enqueue(container);
                _current = container;
            }
        }
        protected void AddParallelExecutor(Func<IAsyncExecutor<M, R>> executor, bool addif = true)
        {
            if (!addif)
                return;

            if (IfConstantConditions())
            {
                var container = new AsyncExecutorContainer<M, R>(executor)
                {
                    LocalCache = this,
                    ExecuteConditions = GetCalculatedConditions()
                };
                _executionQueue.Enqueue(container);
                _current = container;
            }
        }

        protected void AddSequentialExecutor(IAsyncExecutor<M, R> executor, bool addif = true)
        {
            AddParallelExecutor(executor, addif);
        }
        protected void AddSequentialExecutor(Func<IAsyncExecutor<M, R>> executor, bool addif = true)
        {
            AddParallelExecutor(executor, addif);
        }
    }
}
