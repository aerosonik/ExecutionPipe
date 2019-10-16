using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.Builders.Sync.Sequential
{
    public interface ISyncSequentialExecutorOkBuilder<M, R>
    {
        ISyncSequentialExecutorOkBuilder<M, R> Break(bool condition); // allowed by default
        ISyncSequentialExecutorOkBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);
        ISyncSequentialExecutorBuilder<M, R> Set();
    }

    public interface ISyncSequentialExecutorOkBuilder<M>
    {
        ISyncSequentialExecutorOkBuilder<M> Break(bool condition); // allowed by default
        ISyncSequentialExecutorOkBuilder<M> Return(Func<M, PipeResult, PipeResult> handler);
        ISyncSequentialExecutorBuilder<M> Set();
    }

    public interface ISyncSequentialExecutorOkBuilder
    {
        ISyncSequentialExecutorOkBuilder Break(bool condition); // allowed by default
        ISyncSequentialExecutorOkBuilder Return(Func<PipeResult, PipeResult> handler);
        ISyncSequentialExecutorBuilder Set();
    }
}
