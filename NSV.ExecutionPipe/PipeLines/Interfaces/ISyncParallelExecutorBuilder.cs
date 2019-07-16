using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    public interface ISyncParallelExecutorBuilder<M, R>
    {
        ISyncParallelExecutorBuilder<M, R> UseStopWatch();
        ISyncParallelExecutorBuilder<M, R> Label(string label);
        ISyncParallelExecutorBuilder<M, R> ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
        ISyncParallelPipe<M, R> Build();
    }
}
