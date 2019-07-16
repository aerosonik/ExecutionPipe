using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    public interface IAsyncParallelExecutorBuilder<M, R>
    {
        IAsyncParallelExecutorBuilder<M, R> UseStopWatch();
        IAsyncParallelExecutorBuilder<M, R> Label(string label);
        IAsyncParallelExecutorBuilder<M, R> ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
        IAsyncParallelPipe<M, R> Build();
    }
}
