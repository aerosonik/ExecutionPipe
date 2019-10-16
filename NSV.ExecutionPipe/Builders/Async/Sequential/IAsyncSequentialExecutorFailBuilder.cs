using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders.Async.Sequential
{
    public interface IAsyncSequentialExecutorFailBuilder<M, R>
    {
        IAsyncSequentialExecutorFailBuilder<M, R> Retry(int count, int timeOutMilliseconds);
        IAsyncSequentialExecutorFailBuilder<M, R> Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncSequentialExecutorFailBuilder<M, R> Break(bool condition); // allowed by default
        IAsyncSequentialExecutorFailBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);
        IAsyncSequentialExecutorBuilder<M, R> Set();
    }

    public interface IAsyncSequentialExecutorFailBuilder<M>
    {
        IAsyncSequentialExecutorFailBuilder<M> Retry(int count, int timeOutMilliseconds);
        IAsyncSequentialExecutorFailBuilder<M> Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncSequentialExecutorFailBuilder<M> Break(bool condition); // allowed by default
        IAsyncSequentialExecutorFailBuilder<M> Return(Func<M, PipeResult, PipeResult> handler);
        IAsyncSequentialExecutorBuilder<M> Set();
    }

    public interface IAsyncSequentialExecutorFailBuilder
    {
        IAsyncSequentialExecutorFailBuilder Retry(int count, int timeOutMilliseconds);
        IAsyncSequentialExecutorFailBuilder Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncSequentialExecutorFailBuilder Break(bool condition); // allowed by default
        IAsyncSequentialExecutorFailBuilder Return(Func<PipeResult, PipeResult> handler);
        IAsyncSequentialExecutorBuilder Set();
    }
}
