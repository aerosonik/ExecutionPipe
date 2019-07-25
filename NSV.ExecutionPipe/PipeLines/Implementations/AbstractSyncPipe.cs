using System;
using System.Collections.Generic;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
    public abstract class AbstractSyncPipe<M, R> : AbstractPipe<M, R>
    {
        internal readonly Queue<ISyncExecutorContainer<M, R>> _executionQueue
            = new Queue<SyncExecutorContainer<M, R>>();
        internal ISyncExecutorContainer<M, R> _current;

        protected void AddExecutor(ISyncExecutor<M, R> executor, bool addif = true)
        {
            SetContainer<SyncExecutorContainer<M, R>, ISyncExecutor<M, R>>(
                () => new SyncExecutorContainer<M, R>(executor),
                addif,
               // _executionQueue,
                _current);
        }
        protected void AddExecutor(Func<ISyncExecutor<M, R>> executor, bool addif = true)
        {
            SetContainer<ISyncExecutorContainer<M, R>, ISyncExecutor<M, R>>(
                () => new SyncExecutorContainer<M, R>(executor),
                addif,
                //_executionQueue,
                _current);
        }
    }
}
