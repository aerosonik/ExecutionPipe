using NSV.ExecutionPipe.Containers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders
{
    public interface IAsyncContainerQueueEnque<M,R>
    {
        void Enque(QueueAsyncContainer<M, R> container);
    }
}
