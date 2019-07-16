using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    public interface IAsyncSequentialPipe<M, R> : IAsyncExecutor<M, R>
    {
        PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);
        IAsyncSequentialPipe<M, R> UseStopWatch();
        IAsyncSequentialPipe<M, R> UseLocalCacheThreadSafe();
        IAsyncSequentialPipe<M, R> UseLocalCache();
        IAsyncSequentialExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor);
        IAsyncSequentialExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor);
        IAsyncSequentialExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor, bool addif);
        IAsyncSequentialPipe<M, R> If(bool condition);
        IAsyncSequentialPipe<M, R> If(Func<M, bool> condition);
        IAsyncSequentialPipe<M, R> EndIf();
        IAsyncSequentialPipe<M, R> Build();
    }
}
