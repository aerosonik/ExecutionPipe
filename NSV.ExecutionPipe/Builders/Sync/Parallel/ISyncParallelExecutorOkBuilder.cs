using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders.Sync.Parallel
{
    public interface ISyncParallelExecutorOkBuilder<M, R>
    {
        ISyncParallelExecutorOkBuilder<M, R> Break(bool condition); // allowed by default
        ISyncParallelExecutorOkBuilder<M, R> Return(Func<M, PipeResult<R>, PipeResult<R>> handler);
        ISyncParallelExecutorBuilder<M, R> Set();
    }

    public interface ISyncParallelExecutorOkBuilder<M>
    {
        ISyncParallelExecutorOkBuilder<M> Break(bool condition); // allowed by default
        ISyncParallelExecutorOkBuilder<M> Return(Func<M, PipeResult, PipeResult> handler);
        ISyncParallelExecutorBuilder<M> Set();
    }

    public interface ISyncParallelExecutorOkBuilder
    {
        ISyncParallelExecutorOkBuilder Break(bool condition); // allowed by default
        ISyncParallelExecutorOkBuilder Return(Func<PipeResult, PipeResult> handler);
        ISyncParallelExecutorBuilder Set();
    }
}
