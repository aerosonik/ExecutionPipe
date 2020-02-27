using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders.Sync.Parallel
{
    public interface ISyncParallelPipeBuilder<M, R>
    {
        ISyncParallelPipeBuilder<M, R> Parallel(bool asParallel); // false by default
        ISyncParallelPipeBuilder<M, R> Cache(bool threadSafe); // false by default
        ISyncParallelPipeBuilder<M, R> StopWatch(bool use); // false by default
        ISyncParallelPipeBuilder<M, R> Return(M model, PipeResult<R>[] results);

        ISyncParallelExecutorBuilder<M, R> Executor(ISyncExecutor<M, R> executor);
        ISyncParallelExecutorBuilder<M, R> Executor(ISyncExecutor<M, R> executor, bool addif);
        ISyncParallelExecutorBuilder<M, R> Executor(Func<ISyncExecutor<M, R>> executor);
        ISyncParallelExecutorBuilder<M, R> Executor(Func<ISyncExecutor<M, R>> executor, bool addif);
        ISyncParallelExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor);
        ISyncParallelExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor, bool addif);

        ISyncParallelPipeBuilder<M, R> If(bool condition);
        ISyncParallelPipeBuilder<M, R> If(Func<M, bool> condition);
        ISyncParallelPipeBuilder<M, R> EndIf();

        ISyncExecutor<M, R> ToSyncExecutor();
    }

    public interface ISyncParallelPipeBuilder<M>
    {
        ISyncParallelPipeBuilder<M> Parallel(bool asParallel); // false by default
        ISyncParallelPipeBuilder<M> Cache(bool threadSafe); // false by default
        ISyncParallelPipeBuilder<M> StopWatch(bool use); // false by default
        ISyncParallelPipeBuilder<M> Return(M model, PipeResult[] results);

        ISyncParallelExecutorBuilder<M> Executor(ISyncExecutor<M> executor);
        ISyncParallelExecutorBuilder<M> Executor(ISyncExecutor<M> executor, bool addif);
        ISyncParallelExecutorBuilder<M> Executor(Func<ISyncExecutor<M>> executor);
        ISyncParallelExecutorBuilder<M> Executor(Func<ISyncExecutor<M>> executor, bool addif);
        ISyncParallelExecutorBuilder<M> Executor(Func<M, PipeResult> executor);
        ISyncParallelExecutorBuilder<M> Executor(Func<M, PipeResult> executor, bool addif);

        ISyncParallelPipeBuilder<M> If(bool condition);
        ISyncParallelPipeBuilder<M> If(Func<M, bool> condition);
        ISyncParallelPipeBuilder<M> EndIf();

        ISyncExecutor<M> ToSyncExecutor();
    }

    public interface ISyncParallelPipeBuilder
    {
        ISyncParallelPipeBuilder Parallel(bool asParallel); // false by default
        ISyncParallelPipeBuilder Cache(bool threadSafe); // false by default
        ISyncParallelPipeBuilder StopWatch(bool use); // false by default
        ISyncParallelPipeBuilder Return(PipeResult[] results);

        ISyncParallelExecutorBuilder Executor(ISyncExecutor executor);
        ISyncParallelExecutorBuilder Executor(ISyncExecutor executor, bool addif);
        ISyncParallelExecutorBuilder Executor(Func<ISyncExecutor> executor);
        ISyncParallelExecutorBuilder Executor(Func<ISyncExecutor> executor, bool addif);
        ISyncParallelExecutorBuilder Executor(Func<PipeResult> executor);
        ISyncParallelExecutorBuilder Executor(Func<PipeResult> executor, bool addif);

        ISyncParallelPipeBuilder If(bool condition);
        ISyncParallelPipeBuilder EndIf();

        ISyncExecutor ToSyncExecutor();
    }
}
