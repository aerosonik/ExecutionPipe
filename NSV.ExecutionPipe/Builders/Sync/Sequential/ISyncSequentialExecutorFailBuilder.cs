using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.Builders.Sync.Sequential
{
    public interface ISyncSequentialExecutorFailBuilder<M, R>
    {
        ISyncSequentialExecutorFailBuilder<M, R> Retry(int count, int timeOutMilliseconds);
        ISyncSequentialExecutorFailBuilder<M, R> Retry(bool condition, int count, int timeOutMilliseconds);
        ISyncSequentialExecutorFailBuilder<M, R> Break(bool condition); // allowed by default
        ISyncSequentialExecutorFailBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);
        ISyncSequentialExecutorBuilder<M, R> Set();
    }

    public interface ISyncSequentialExecutorFailBuilder<M>
    {
        ISyncSequentialExecutorFailBuilder<M> Retry(int count, int timeOutMilliseconds);
        ISyncSequentialExecutorFailBuilder<M> Retry(bool condition, int count, int timeOutMilliseconds);
        ISyncSequentialExecutorFailBuilder<M> Break(bool condition); // allowed by default
        ISyncSequentialExecutorFailBuilder<M> Return(Func<M, PipeResult, PipeResult> handler);
        ISyncSequentialExecutorBuilder<M> Set();
    }

    public interface ISyncSequentialExecutorFailBuilder
    {
        ISyncSequentialExecutorFailBuilder Retry(int count, int timeOutMilliseconds);
        ISyncSequentialExecutorFailBuilder Retry(bool condition, int count, int timeOutMilliseconds);
        ISyncSequentialExecutorFailBuilder Break(bool condition); // allowed by default
        ISyncSequentialExecutorFailBuilder Return(Func<PipeResult, PipeResult> handler);
        ISyncSequentialExecutorBuilder Set();
    }
}
