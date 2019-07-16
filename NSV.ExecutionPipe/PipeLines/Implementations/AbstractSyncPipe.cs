using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
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
}
