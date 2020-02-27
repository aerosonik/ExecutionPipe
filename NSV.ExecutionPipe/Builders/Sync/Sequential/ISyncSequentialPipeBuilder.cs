using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.Builders.Sync.Sequential
{
    public interface ISyncSequentialPipeBuilder<M, R>
    {
        ISyncSequentialPipeBuilder<M, R> Parallel(bool asParallel); // false by default
        ISyncSequentialPipeBuilder<M, R> Cache(bool threadSafe); // false by default
        ISyncSequentialPipeBuilder<M, R> StopWatch(bool use); // false by default
        ISyncSequentialPipeBuilder<M, R> Return(M model, PipeResult<R>[] results);

        ISyncSequentialExecutorBuilder<M, R> Executor(ISyncExecutor<M, R> executor);
        ISyncSequentialExecutorBuilder<M, R> Executor(ISyncExecutor<M, R> executor, bool addif);
        ISyncSequentialExecutorBuilder<M, R> Executor(Func<ISyncExecutor<M, R>> executor);
        ISyncSequentialExecutorBuilder<M, R> Executor(Func<ISyncExecutor<M, R>> executor, bool addif);
        ISyncSequentialExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor);
        ISyncSequentialExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor, bool addif);

        ISyncSequentialPipeBuilder<M, R> If(bool condition);
        ISyncSequentialPipeBuilder<M, R> If(Func<M, bool> condition);
        ISyncSequentialPipeBuilder<M, R> EndIf();

        ISyncExecutor<M, R> ToSyncExecutor();
    }

    public interface ISyncSequentialPipeBuilder<M>
    {
        ISyncSequentialPipeBuilder<M> Parallel(bool asParallel); // false by default
        ISyncSequentialPipeBuilder<M> Cache(bool threadSafe); // false by default
        ISyncSequentialPipeBuilder<M> StopWatch(bool use); // false by default
        ISyncSequentialPipeBuilder<M> Return(M model, PipeResult[] results);

        ISyncSequentialExecutorBuilder<M> Executor(ISyncExecutor<M> executor);
        ISyncSequentialExecutorBuilder<M> Executor(ISyncExecutor<M> executor, bool addif);
        ISyncSequentialExecutorBuilder<M> Executor(Func<ISyncExecutor<M>> executor);
        ISyncSequentialExecutorBuilder<M> Executor(Func<ISyncExecutor<M>> executor, bool addif);
        ISyncSequentialExecutorBuilder<M> Executor(Func<M, PipeResult> executor);
        ISyncSequentialExecutorBuilder<M> Executor(Func<M, PipeResult> executor, bool addif);

        ISyncSequentialPipeBuilder<M> If(bool condition);
        ISyncSequentialPipeBuilder<M> If(Func<M, bool> condition);
        ISyncSequentialPipeBuilder<M> EndIf();

        ISyncExecutor<M> ToSyncExecutor();
    }

    public interface ISyncSequentialPipeBuilder
    {
        ISyncSequentialPipeBuilder Parallel(bool asParallel); // false by default
        ISyncSequentialPipeBuilder Cache(bool threadSafe); // false by default
        ISyncSequentialPipeBuilder StopWatch(bool use); // false by default
        ISyncSequentialPipeBuilder Return(PipeResult[] results);

        ISyncSequentialExecutorBuilder Executor(ISyncExecutor executor);
        ISyncSequentialExecutorBuilder Executor(ISyncExecutor executor, bool addif);
        ISyncSequentialExecutorBuilder Executor(Func<ISyncExecutor> executor);
        ISyncSequentialExecutorBuilder Executor(Func<ISyncExecutor> executor, bool addif);
        ISyncSequentialExecutorBuilder Executor(Func<PipeResult> executor);
        ISyncSequentialExecutorBuilder Executor(Func<PipeResult> executor, bool addif);

        ISyncSequentialPipeBuilder If(bool condition);
        ISyncSequentialPipeBuilder EndIf();

        ISyncExecutor ToSyncExecutor();
    }
}
