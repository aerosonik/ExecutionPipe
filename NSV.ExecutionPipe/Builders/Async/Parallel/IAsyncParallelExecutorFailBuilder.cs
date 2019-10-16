using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders.Async.Parallel
{
    public interface IAsyncParallelExecutorFailBuilder<M, R>
    {
        IAsyncParallelExecutorFailBuilder<M, R> Retry(int count, int timeOutMilliseconds);
        IAsyncParallelExecutorFailBuilder<M, R> Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncParallelExecutorBuilder<M, R> Set();
    }

    public interface IAsyncParallelExecutorFailBuilder<M>
    {
        IAsyncParallelExecutorFailBuilder<M> Retry(int count, int timeOutMilliseconds);
        IAsyncParallelExecutorFailBuilder<M> Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncParallelExecutorBuilder<M> Set();
    }

    public interface IAsyncParallelExecutorFailBuilder
    {
        IAsyncParallelExecutorFailBuilder Retry(int count, int timeOutMilliseconds);
        IAsyncParallelExecutorFailBuilder Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncParallelExecutorBuilder Set();
    }
}
