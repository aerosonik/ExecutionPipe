using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    public interface IAsyncParallelPipe<M, R> : IAsyncExecutor<M, R>
    {
        PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);
        IAsyncParallelPipe<M, R> UseStopWatch();
        IAsyncParallelPipe<M, R> UseStopWatch(bool usestopwatch);
        IAsyncParallelPipe<M, R> UseLocalCacheThreadSafe();

        IAsyncParallelExecutorBuilder<M, R> Executor(IAsyncExecutor<M, R> executor);
        IAsyncParallelExecutorBuilder<M, R> Executor(IAsyncExecutor<M, R> executor, bool addif);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<IAsyncExecutor<M, R>> executor);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<IAsyncExecutor<M, R>> executor, bool addif);

        IAsyncParallelPipe<M, R> If(bool condition);
        IAsyncParallelPipe<M, R> If(Func<M, bool> condition);
        IAsyncParallelPipe<M, R> EndIf();
        IAsyncExecutor<M, R> Build();
    }
}
