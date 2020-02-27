using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Containers;
using NSV.ExecutionPipe.Containers.Async;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Pipes
{
    internal interface IPipeSettings<M, R>
    {
        void SetCache(Func<IPipeCache> cache);

        //void SetExecutors(
        //    Queue<(ExecutorSettings<M, R> Settings, 
        //    IAsyncContainer<M, R> Container)> queue,
        //    Optional<(ExecutorSettings<M, R> Settings,
        //        IAsyncContainer<M, R> Container)> defaultExecutor);
        void SetExecutors(
            (ExecutorSettings<M, R> Settings,IAsyncContainer<M, R> Container)[] queue,
            Optional<(ExecutorSettings<M, R> Settings,
                IAsyncContainer<M, R> Container)> defaultExecutor);

        void SetReturn(Func<M, PipeResult<R>[], PipeResult<R>> handler);

        void SetStopWatch();
    }
}
