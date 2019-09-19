using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.Builders
{
    public interface ISyncExecutorFailBuilder<M, R>
    {
        ISyncExecutorFailBuilder<M, R> Retry(int count, int timeOutMilliseconds);
        ISyncExecutorFailBuilder<M, R> Retry(bool condition, int count, int timeOutMilliseconds);
        ISyncExecutorFailBuilder<M, R> Break(bool condition); // allowed by default
        ISyncExecutorFailBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);
        ISyncExecutorBuilder<M, R> Set();
    }

    public interface ISyncExecutorFailBuilder<M>
    {
        ISyncExecutorFailBuilder<M> Retry(int count, int timeOutMilliseconds);
        ISyncExecutorFailBuilder<M> Retry(bool condition, int count, int timeOutMilliseconds);
        ISyncExecutorFailBuilder<M> Break(bool condition); // allowed by default
        ISyncExecutorFailBuilder<M> Return(Func<M, PipeResult, PipeResult> handler);
        ISyncExecutorBuilder<M> Set();
    }

    public interface ISyncExecutorFailBuilder
    {
        ISyncExecutorFailBuilder Retry(int count, int timeOutMilliseconds);
        ISyncExecutorFailBuilder Retry(bool condition, int count, int timeOutMilliseconds);
        ISyncExecutorFailBuilder Break(bool condition); // allowed by default
        ISyncExecutorFailBuilder Return(Func<PipeResult, PipeResult> handler);
        ISyncExecutorBuilder Set();
    }
}
