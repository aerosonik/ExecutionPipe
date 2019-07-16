using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
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
