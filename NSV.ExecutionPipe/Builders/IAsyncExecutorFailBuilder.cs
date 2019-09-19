using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders
{
    public interface IAsyncExecutorFailBuilder<M, R>
    {
        IAsyncExecutorFailBuilder<M, R> Retry(int count, int timeOutMilliseconds);
        IAsyncExecutorFailBuilder<M, R> Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncExecutorFailBuilder<M, R> Break(bool condition); // allowed by default
        IAsyncExecutorFailBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);
        IAsyncExecutorBuilder<M, R> Set();
    }

    public interface IAsyncExecutorFailBuilder<M>
    {
        IAsyncExecutorFailBuilder<M> Retry(int count, int timeOutMilliseconds);
        IAsyncExecutorFailBuilder<M> Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncExecutorFailBuilder<M> Break(bool condition); // allowed by default
        IAsyncExecutorFailBuilder<M> Return(Func<M, PipeResult, PipeResult> handler);
        IAsyncExecutorBuilder<M> Set();
    }

    public interface IAsyncExecutorFailBuilder
    {
        IAsyncExecutorFailBuilder Retry(int count, int timeOutMilliseconds);
        IAsyncExecutorFailBuilder Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncExecutorFailBuilder Break(bool condition); // allowed by default
        IAsyncExecutorFailBuilder Return(Func<PipeResult, PipeResult> handler);
        IAsyncExecutorBuilder Set();
    }
}
