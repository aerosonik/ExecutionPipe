using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    //public interface IAsyncSequentialExecutorBuilder<M, R>
    //{
    //    IAsyncSequentialExecutorBuilder<M, R> BreakIfFailed();
    //    IAsyncSequentialExecutorBuilder<M, R> BreakIfFailed(bool condition);
    //    IAsyncSequentialExecutorBuilder<M, R> AllowBreak();
    //    IAsyncSequentialExecutorBuilder<M, R> AllowBreak(bool condition);
    //    IAsyncSequentialExecutorBuilder<M, R> UseStopWatch();
    //    IAsyncSequentialExecutorBuilder<M, R> UseStopWatch(bool condition);
    //    IAsyncSequentialExecutorBuilder<M, R> UseRestrictedExecution(int maxCount);
    //    IAsyncSequentialExecutorBuilder<M, R> UseRestrictedExecution(bool condition, int maxCount);
    //    IAsyncSequentialExecutorBuilder<M, R> Label(string label);
    //    IAsyncSequentialExecutorBuilder<M, R> RetryIfFailed(int count, int timeOutMilliseconds);
    //    IAsyncSequentialExecutorBuilder<M, R> RetryIfFailed(bool condition, int count, int timeOutMilliseconds);
    //    IAsyncSequentialExecutorBuilder<M, R> ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
    //    IAsyncSequentialExecutorBuilder<M, R> ExecuteIf(Func<M, bool> condition);
    //    IAsyncSequentialPipe<M, R> Add();
    //}

    public interface IAsyncExecutorBuilder<M, R>
    {
        IAsyncExecutorFailBuilder<M, R> IfFail();
        IAsyncExecutorOkBuilder<M, R> IfOk();
        IAsyncExecutorBuilder<M, R> StopWatch();
        IAsyncExecutorBuilder<M, R> StopWatch(bool condition);
        IAsyncExecutorBuilder<M, R> Restricted(int maxCount);
        IAsyncExecutorBuilder<M, R> Restricted(bool condition, int maxCount);
        IAsyncExecutorBuilder<M, R> Label(string label);
        IAsyncExecutorBuilder<M, R> ExecuteIf(Func<M, bool> condition);
        IAsyncSequentialPipe<M, R> Add();
    }

    public interface IAsyncExecutorFailBuilder<M, R>
    {
        IAsyncExecutorFailBuilder<M, R> Retry(int count, int timeOutMilliseconds);
        IAsyncExecutorFailBuilder<M, R> Retry(bool condition, int count, int timeOutMilliseconds);

        IAsyncExecutorFailBuilder<M, R> Break(bool condition); // allowed by default

        IAsyncExecutorFailBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);

        IAsyncSequentialPipe<M, R> Set();
    }

    public interface IAsyncExecutorOkBuilder<M, R>
    {
        IAsyncExecutorOkBuilder<M, R> Break(bool condition); // allowed by default

        IAsyncExecutorOkBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);

        IAsyncSequentialPipe<M, R> Set();
    }

}