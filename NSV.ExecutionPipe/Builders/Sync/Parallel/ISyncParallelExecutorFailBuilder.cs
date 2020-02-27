using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders.Sync.Parallel
{
    public interface ISyncParallelExecutorFailBuilder<M, R>
    {
        ISyncParallelExecutorFailBuilder<M, R> Retry(int count, int timeOutMilliseconds);
        ISyncParallelExecutorFailBuilder<M, R> Retry(bool condition, int count, int timeOutMilliseconds);
        ISyncParallelExecutorFailBuilder<M, R> Break(bool condition); // allowed by default
        ISyncParallelExecutorFailBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);
        ISyncParallelExecutorBuilder<M, R> Set();
    }

    public interface ISyncParallelExecutorFailBuilder<M>
    {
        ISyncParallelExecutorFailBuilder<M> Retry(int count, int timeOutMilliseconds);
        ISyncParallelExecutorFailBuilder<M> Retry(bool condition, int count, int timeOutMilliseconds);
        ISyncParallelExecutorFailBuilder<M> Break(bool condition); // allowed by default
        ISyncParallelExecutorFailBuilder<M> Return(Func<M, PipeResult, PipeResult> handler);
        ISyncParallelExecutorBuilder<M> Set();
    }

    public interface ISyncParallelExecutorFailBuilder
    {
        ISyncParallelExecutorFailBuilder Retry(int count, int timeOutMilliseconds);
        ISyncParallelExecutorFailBuilder Retry(bool condition, int count, int timeOutMilliseconds);
        ISyncParallelExecutorFailBuilder Break(bool condition); // allowed by default
        ISyncParallelExecutorFailBuilder Return(Func<PipeResult, PipeResult> handler);
        ISyncParallelExecutorBuilder Set();
    }
}
