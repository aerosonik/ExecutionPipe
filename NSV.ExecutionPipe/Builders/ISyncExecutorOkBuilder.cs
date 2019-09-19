using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.Builders
{
    public interface ISyncExecutorOkBuilder<M, R>
    {
        ISyncExecutorOkBuilder<M, R> Break(bool condition); // allowed by default
        ISyncExecutorOkBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);
        ISyncExecutorBuilder<M, R> Set();
    }

    public interface ISyncExecutorOkBuilder<M>
    {
        ISyncExecutorOkBuilder<M> Break(bool condition); // allowed by default
        ISyncExecutorOkBuilder<M> Return(Func<M, PipeResult, PipeResult> handler);
        ISyncExecutorBuilder<M> Set();
    }

    public interface ISyncExecutorOkBuilder
    {
        ISyncExecutorOkBuilder Break(bool condition); // allowed by default
        ISyncExecutorOkBuilder Return(Func<PipeResult, PipeResult> handler);
        ISyncExecutorBuilder Set();
    }
}
