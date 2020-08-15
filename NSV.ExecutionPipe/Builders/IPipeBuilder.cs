using NSV.ExecutionPipe.Builders.Async.Parallel;
using NSV.ExecutionPipe.Builders.Async.Sequential;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders
{
    public interface IPipeBuilder<M, R>
    {
        IAsyncSequentialPipeBuilder<M, R> AsyncPipe();

        IAsyncParallelPipeBuilder<M, R> AsyncParallelPipe();
    }
}
