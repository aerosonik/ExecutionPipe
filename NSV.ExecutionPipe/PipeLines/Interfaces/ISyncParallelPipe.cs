using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    public interface ISyncParallelPipe<M, R> : ISyncExecutor<M, R>
    {
        PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);
        ISyncParallelPipe<M, R> UseStopWatch();
        ISyncParallelPipe<M, R> UseLocalCacheThreadSafe();
        ISyncParallelExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor);
        ISyncParallelExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor, bool addif);
        ISyncParallelExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor);
        ISyncParallelExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor, bool addif);
        ISyncParallelPipe<M, R> If(bool condition);
        ISyncParallelPipe<M, R> If(Func<M, bool> condition);
        ISyncParallelPipe<M, R> EndIf();
        ISyncParallelPipe<M, R> Build();
    }
}
