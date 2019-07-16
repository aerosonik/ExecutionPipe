using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    public interface IAsyncSequentialExecutorBuilder<M, R>
    {
        IAsyncSequentialExecutorBuilder<M, R> BreakIfFailed();
        IAsyncSequentialExecutorBuilder<M, R> AllowBreak();
        IAsyncSequentialExecutorBuilder<M, R> UseStopWatch();
        IAsyncSequentialExecutorBuilder<M, R> Label(string label);
        IAsyncSequentialExecutorBuilder<M, R> RetryIfFailed(int count, int timeOutMilliseconds);
        IAsyncSequentialExecutorBuilder<M, R> ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
        IAsyncSequentialPipe<M, R> Build();
    }
}
