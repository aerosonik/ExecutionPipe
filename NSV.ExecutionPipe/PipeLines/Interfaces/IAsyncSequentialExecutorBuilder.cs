using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    public interface IAsyncSequentialExecutorBuilder<M, R>
    {
        IAsyncSequentialExecutorBuilder<M, R> BreakIfFailed();
        IAsyncSequentialExecutorBuilder<M, R> BreakIfFailed(bool condition);
        IAsyncSequentialExecutorBuilder<M, R> AllowBreak();
        IAsyncSequentialExecutorBuilder<M, R> AllowBreak(bool condition);
        IAsyncSequentialExecutorBuilder<M, R> UseStopWatch();
        IAsyncSequentialExecutorBuilder<M, R> UseStopWatch(bool condition);
        IAsyncSequentialExecutorBuilder<M, R> UseRestrictedExecution(int maxCount);
        IAsyncSequentialExecutorBuilder<M, R> UseRestrictedExecution(bool condition, int maxCount);
        IAsyncSequentialExecutorBuilder<M, R> Label(string label);
        IAsyncSequentialExecutorBuilder<M, R> RetryIfFailed(int count, int timeOutMilliseconds);
        IAsyncSequentialExecutorBuilder<M, R> RetryIfFailed(bool condition, int count, int timeOutMilliseconds);
        IAsyncSequentialExecutorBuilder<M, R> ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
        IAsyncSequentialExecutorBuilder<M, R> ExecuteIf(Func<M, bool> condition);
        IAsyncSequentialPipe<M, R> Add();
    }
}
