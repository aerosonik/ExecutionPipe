using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Containers
{
    public class QueueAsyncContainer<M,R> : ExecutorSettings<M, R>
    {
        public QueueAsyncContainer(
            bool allowBreak,
            Func<M, PipeResult<R>, PipeResult<R>> returnFunc,
            Func<M, bool>[] executeConditions,
            IAsyncContainer<M, R> container)
            :base(allowBreak, returnFunc, executeConditions)
        {
            Container = container;
        }
        public IAsyncContainer<M, R> Container { get; }
    }
}
