using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.Builders.Async.Sequential
{
    public interface IAsyncSequentialExecutorOkBuilder<M, R>
    {
        IAsyncSequentialExecutorOkBuilder<M, R> Break(bool condition); // allowed by default
        IAsyncSequentialExecutorOkBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);
        IAsyncSequentialExecutorBuilder<M, R> Set();
    }

    public interface IAsyncSequentialExecutorOkBuilder<M>
    {
        IAsyncSequentialExecutorOkBuilder<M> Break(bool condition); // allowed by default
        IAsyncSequentialExecutorOkBuilder<M> Return(Func<M, PipeResult, PipeResult> handler);
        IAsyncSequentialExecutorBuilder<M> Set();
    }

    public interface IAsyncSequentialExecutorOkBuilder
    {
        IAsyncSequentialExecutorOkBuilder Break(bool condition); // allowed by default
        IAsyncSequentialExecutorOkBuilder Return(Func<PipeResult, PipeResult> handler);
        IAsyncSequentialExecutorBuilder Set();
    }
}
