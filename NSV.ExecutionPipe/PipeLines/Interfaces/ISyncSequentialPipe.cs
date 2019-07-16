using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    public interface ISyncSequentialPipe<M, R> : ISyncExecutor<M, R>
    {
        PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);
        ISyncSequentialPipe<M, R> UseStopWatch();
        ISyncSequentialPipe<M, R> UseLocalCacheThreadSafe();
        ISyncSequentialPipe<M, R> UseLocalCache();
        ISyncSequentialExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor);
        ISyncSequentialExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor, bool addif);
        ISyncSequentialExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor);
        ISyncSequentialExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor, bool addif);
        ISyncSequentialPipe<M, R> If(bool condition);
        ISyncSequentialPipe<M, R> If(Func<M, bool> condition);
        ISyncSequentialPipe<M, R> EndIf();
        ISyncSequentialPipe<M, R> Build();
    }
}
