using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    public interface IAsyncSequentialPipe<M, R> : IAsyncExecutor<M, R>
    {
        PipeResult<R> Return(M model, PipeResult<R>[] results);
        IAsyncSequentialPipe<M, R> SetStopWatch();
        IAsyncSequentialPipe<M, R> SetStopWatch(bool usestopwatch);
        IAsyncSequentialPipe<M, R> SetCache(bool isThreadSafe);

        IAsyncSequentialExecutorBuilder<M, R> Executor(IAsyncExecutor<M, R> executor);
        IAsyncSequentialExecutorBuilder<M, R> Executor(IAsyncExecutor<M, R> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<IAsyncExecutor<M, R>> executor);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<IAsyncExecutor<M, R>> executor, bool addif);

        IAsyncSequentialPipe<M, R> If(bool condition);
        IAsyncSequentialPipe<M, R> If(Func<M, bool> condition);
        IAsyncSequentialPipe<M, R> EndIf();
        IAsyncExecutor<M, R> End();//Build();
    }
}
