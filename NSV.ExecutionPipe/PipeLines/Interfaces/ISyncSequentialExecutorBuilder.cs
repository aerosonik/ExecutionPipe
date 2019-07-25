using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    public interface ISyncSequentialExecutorBuilder<M, R>
    {
        ISyncSequentialExecutorBuilder<M, R> BreakIfFailed();
        ISyncSequentialExecutorBuilder<M, R> BreakIfFailed(bool condition);
        ISyncSequentialExecutorBuilder<M, R> AllowBreak();
        ISyncSequentialExecutorBuilder<M, R> AllowBreak(bool condition);
        ISyncSequentialExecutorBuilder<M, R> UseStopWatch();
        ISyncSequentialExecutorBuilder<M, R> UseStopWatch(bool condition);
        ISyncSequentialExecutorBuilder<M, R> UseRestrictedExecution(int maxCount);
        ISyncSequentialExecutorBuilder<M, R> UseRestrictedExecution(bool condition, int maxCount);
        ISyncSequentialExecutorBuilder<M, R> Label(string label);
        ISyncSequentialExecutorBuilder<M, R> RetryIfFailed(int count, int timeOutMilliseconds);
        ISyncSequentialExecutorBuilder<M, R> RetryIfFailed(bool condition, int count, int timeOutMilliseconds);
        ISyncSequentialExecutorBuilder<M, R> ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
        ISyncSequentialExecutorBuilder<M, R> ExecuteIf(Func<M, bool> condition);
        ISyncSequentialPipe<M, R> Add();
    }
}
