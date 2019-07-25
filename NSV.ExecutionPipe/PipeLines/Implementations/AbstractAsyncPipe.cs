using System;
using System.Collections.Generic;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
    public abstract class AbstractAsyncPipe<M, R> : AbstractPipe<M, R>
    {
        internal readonly Queue<AsyncExecutorContainer<M, R>> _executionQueue
            = new Queue<AsyncExecutorContainer<M, R>>();
        internal AsyncExecutorContainer<M, R> _current;

        protected void AddExecutor(IAsyncExecutor<M, R> executor, bool addif = true)
        {
            SetContainer<AsyncExecutorContainer<M, R>, ISyncExecutor<M, R>>(
                () => new AsyncExecutorContainer<M, R>(executor),
                addif,
                //_executionQueue,
                _current);
        }
        protected void AddExecutor(Func<IAsyncExecutor<M, R>> executor, bool addif = true)
        {
            SetContainer<AsyncExecutorContainer<M, R>, IAsyncExecutor<M, R>>(
                () => new AsyncExecutorContainer<M, R>(executor),
                addif,
                //_executionQueue,
                _current);
        }
    }
}
