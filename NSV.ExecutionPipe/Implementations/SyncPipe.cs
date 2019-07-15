using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;

namespace NSV.ExecutionPipe
{
    public abstract class SyncParallelPipe<M, R> :
        ISyncParallelPipe<M, R>,
        ISyncParallelExecutorBuilder<M, R>
    {
        public ISyncParallelExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor)
        {
            throw new NotImplementedException();
        }

        public ISyncParallelExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor, bool addif)
        {
            throw new NotImplementedException();
        }

        public ISyncParallelExecutorBuilder<M, R> Add(Lazy<ISyncExecutor<M, R>> executor)
        {
            throw new NotImplementedException();
        }

        public ISyncParallelExecutorBuilder<M, R> Add(Lazy<ISyncExecutor<M, R>> executor, bool addif)
        {
            throw new NotImplementedException();
        }

        public ISyncParallelPipe<M, R> Build()
        {
            throw new NotImplementedException();
        }

        public PipeResult<R> CreateResult(M model, PipeResult<R>[] results, TimeSpan elapsed)
        {
            throw new NotImplementedException();
        }

        public ISyncParallelPipe<M, R> EndIf()
        {
            throw new NotImplementedException();
        }

        public PipeResult<R> ExecuteSync(M model, ILocalCache cache = null)
        {
            throw new NotImplementedException();
        }

        public ISyncParallelPipe<M, R> If(bool condition)
        {
            throw new NotImplementedException();
        }

        public ISyncParallelPipe<M, R> If(Func<M, bool> condition)
        {
            throw new NotImplementedException();
        }

        public ISyncParallelPipe<M, R> UseLocalCacheThreadSafe()
        {
            throw new NotImplementedException();
        }

        public ISyncParallelPipe<M, R> UseStopWatch()
        {
            throw new NotImplementedException();
        }

        ISyncParallelPipe<M, R> ISyncParallelExecutorBuilder<M, R>.Build()
        {
            throw new NotImplementedException();
        }

        ISyncParallelExecutorBuilder<M, R> ISyncParallelExecutorBuilder<M, R>.Label(string label)
        {
            throw new NotImplementedException();
        }

        ISyncParallelExecutorBuilder<M, R> ISyncParallelExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            throw new NotImplementedException();
        }

        ISyncParallelExecutorBuilder<M, R> ISyncParallelExecutorBuilder<M, R>.UseStopWatch()
        {
            throw new NotImplementedException();
        }
    }
    public abstract class SyncSequentialPipe<M, R> :
        ISyncSequentialPipe<M, R>,
        ISyncSequentialExecutorBuilder<M, R>
    {
        public ISyncSequentialExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor)
        {
            throw new NotImplementedException();
        }

        public ISyncSequentialExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor, bool addif)
        {
            throw new NotImplementedException();
        }

        public ISyncSequentialExecutorBuilder<M, R> Add(Lazy<ISyncExecutor<M, R>> executor)
        {
            throw new NotImplementedException();
        }

        public ISyncSequentialExecutorBuilder<M, R> Add(Lazy<ISyncExecutor<M, R>> executor, bool addif)
        {
            throw new NotImplementedException();
        }

        public ISyncSequentialPipe<M, R> Build()
        {
            throw new NotImplementedException();
        }

        public PipeResult<R> CreateResult(M model, PipeResult<R>[] results, TimeSpan elapsed)
        {
            throw new NotImplementedException();
        }

        public ISyncSequentialPipe<M, R> EndIf()
        {
            throw new NotImplementedException();
        }

        public PipeResult<R> ExecuteSync(M model, ILocalCache cache = null)
        {
            throw new NotImplementedException();
        }

        public ISyncSequentialPipe<M, R> If(bool condition)
        {
            throw new NotImplementedException();
        }

        public ISyncSequentialPipe<M, R> If(Func<M, bool> condition)
        {
            throw new NotImplementedException();
        }

        public ISyncSequentialPipe<M, R> UseLocalCache()
        {
            throw new NotImplementedException();
        }

        public ISyncSequentialPipe<M, R> UseLocalCacheThreadSafe()
        {
            throw new NotImplementedException();
        }

        public ISyncSequentialPipe<M, R> UseStopWatch()
        {
            throw new NotImplementedException();
        }

        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.AllowBreak()
        {
            throw new NotImplementedException();
        }

        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.BreakIfFailed()
        {
            throw new NotImplementedException();
        }

        ISyncSequentialPipe<M, R> ISyncSequentialExecutorBuilder<M, R>.Build()
        {
            throw new NotImplementedException();
        }

        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.Label(string label)
        {
            throw new NotImplementedException();
        }

        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            throw new NotImplementedException();
        }

        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.RetryIfFailed(int count, int timeOutMilliseconds)
        {
            throw new NotImplementedException();
        }

        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.UseStopWatch()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class AsyncParallelPipe<M, R> :
        IAsyncParallelPipe<M, R>,
        IAsyncParallelExecutorBuilder<M, R>
    {
        public IAsyncParallelExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor)
        {
            throw new NotImplementedException();
        }

        public IAsyncParallelExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor, bool addif)
        {
            throw new NotImplementedException();
        }

        public IAsyncParallelExecutorBuilder<M, R> Add(Lazy<IAsyncExecutor<M, R>> executor)
        {
            throw new NotImplementedException();
        }

        public IAsyncParallelExecutorBuilder<M, R> Add(Lazy<IAsyncExecutor<M, R>> executor, bool addif)
        {
            throw new NotImplementedException();
        }

        public IAsyncParallelPipe<M, R> Build()
        {
            throw new NotImplementedException();
        }

        public PipeResult<R> CreateResult(M model, PipeResult<R>[] results, TimeSpan elapsed)
        {
            throw new NotImplementedException();
        }

        public IAsyncParallelPipe<M, R> EndIf()
        {
            throw new NotImplementedException();
        }

        public Task<PipeResult<R>> ExecuteAsync(M model, ILocalCache cache = null)
        {
            throw new NotImplementedException();
        }

        public IAsyncParallelPipe<M, R> If(bool condition)
        {
            throw new NotImplementedException();
        }

        public IAsyncParallelPipe<M, R> If(Func<M, bool> condition)
        {
            throw new NotImplementedException();
        }

        public IAsyncParallelPipe<M, R> UseLocalCacheThreadSafe()
        {
            throw new NotImplementedException();
        }

        public IAsyncParallelPipe<M, R> UseStopWatch()
        {
            throw new NotImplementedException();
        }

        IAsyncParallelPipe<M, R> IAsyncParallelExecutorBuilder<M, R>.Build()
        {
            throw new NotImplementedException();
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.Label(string label)
        {
            throw new NotImplementedException();
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            throw new NotImplementedException();
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.UseStopWatch()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class AsyncSequentialPipe<M, R> :
        IAsyncSequentialPipe<M, R>,
        IAsyncSequentialExecutorBuilder<M, R>
    {
        public IAsyncSequentialExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor)
        {
            throw new NotImplementedException();
        }

        public IAsyncSequentialExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor, bool addif)
        {
            throw new NotImplementedException();
        }

        public IAsyncSequentialExecutorBuilder<M, R> Add(Lazy<IAsyncExecutor<M, R>> executor)
        {
            throw new NotImplementedException();
        }

        public IAsyncSequentialExecutorBuilder<M, R> Add(Lazy<IAsyncExecutor<M, R>> executor, bool addif)
        {
            throw new NotImplementedException();
        }

        public IAsyncSequentialPipe<M, R> Build()
        {
            throw new NotImplementedException();
        }

        public PipeResult<R> CreateResult(M model, PipeResult<R>[] results, TimeSpan elapsed)
        {
            throw new NotImplementedException();
        }

        public IAsyncSequentialPipe<M, R> EndIf()
        {
            throw new NotImplementedException();
        }

        public Task<PipeResult<R>> ExecuteAsync(M model, ILocalCache cache = null)
        {
            throw new NotImplementedException();
        }

        public IAsyncSequentialPipe<M, R> If(bool condition)
        {
            throw new NotImplementedException();
        }

        public IAsyncSequentialPipe<M, R> If(Func<M, bool> condition)
        {
            throw new NotImplementedException();
        }

        public IAsyncSequentialPipe<M, R> UseLocalCache()
        {
            throw new NotImplementedException();
        }

        public IAsyncSequentialPipe<M, R> UseLocalCacheThreadSafe()
        {
            throw new NotImplementedException();
        }

        public IAsyncSequentialPipe<M, R> UseStopWatch()
        {
            throw new NotImplementedException();
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.AllowBreak()
        {
            throw new NotImplementedException();
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.BreakIfFailed()
        {
            throw new NotImplementedException();
        }

        IAsyncSequentialPipe<M, R> IAsyncSequentialExecutorBuilder<M, R>.Build()
        {
            throw new NotImplementedException();
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.Label(string label)
        {
            throw new NotImplementedException();
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            throw new NotImplementedException();
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.RetryIfFailed(int count, int timeOutMilliseconds)
        {
            throw new NotImplementedException();
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.UseStopWatch()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class AbstractPipe<M,R>
    {
        protected readonly Queue<ExecutorContainer<M, R>> _executionQueue = new Queue<ExecutorContainer<M, R>>();
        protected Optional<M> _model = Optional<M>.Default;
        protected Optional<Stopwatch> _stopWatch = Optional<Stopwatch>.Default;
        protected Optional<List<PipeResult<R>>> _results = Optional<List<PipeResult<R>>>.Default;
        protected Optional<IDictionary<object, object>> _localCache = Optional<IDictionary<object, object>>.Default;
        protected Optional<ILocalCache> _externalCache = Optional<ILocalCache>.Default;
        protected ExecutorContainer<M, R> _current;
        private Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>> _ifConditionStack
                = Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>>.Default;
    }
}
