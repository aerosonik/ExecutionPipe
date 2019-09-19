using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.Builders
{
    public interface IAsyncExecutorOkBuilder<M, R>
    {
        IAsyncExecutorOkBuilder<M, R> Break(bool condition); // allowed by default
        IAsyncExecutorOkBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);
        IAsyncExecutorBuilder<M, R> Set();
    }

    public interface IAsyncExecutorOkBuilder<M>
    {
        IAsyncExecutorOkBuilder<M> Break(bool condition); // allowed by default
        IAsyncExecutorOkBuilder<M> Return(Func<M, PipeResult, PipeResult> handler);
        IAsyncExecutorBuilder<M> Set();
    }

    public interface IAsyncExecutorOkBuilder
    {
        IAsyncExecutorOkBuilder Break(bool condition); // allowed by default
        IAsyncExecutorOkBuilder Return(Func<PipeResult, PipeResult> handler);
        IAsyncExecutorBuilder Set();
    }
}
