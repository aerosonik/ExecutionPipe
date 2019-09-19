using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.Builders
{
    public interface ISyncPipeBuilder<M, R>
    {
        ISyncPipeBuilder<M, R> Parallel(bool asParallel); // false by default
        ISyncPipeBuilder<M, R> Cache(bool threadSafe); // false by default
        ISyncPipeBuilder<M, R> StopWatch(bool use); // false by default
        ISyncPipeBuilder<M, R> Return(M model, PipeResult<R>[] results);

        IAsyncExecutorBuilder<M, R> Executor(ISyncExecutor<M, R> executor);
        IAsyncExecutorBuilder<M, R> Executor(ISyncExecutor<M, R> executor, bool addif);
        IAsyncExecutorBuilder<M, R> Executor(Func<ISyncExecutor<M, R>> executor);
        IAsyncExecutorBuilder<M, R> Executor(Func<ISyncExecutor<M, R>> executor, bool addif);
        IAsyncExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor);
        IAsyncExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor, bool addif);

        ISyncPipeBuilder<M, R> If(bool condition);
        ISyncPipeBuilder<M, R> If(Func<M, bool> condition);
        ISyncPipeBuilder<M, R> EndIf();

        ISyncExecutor<M, R> ToSyncExecutor();
    }

    public interface ISyncPipeBuilder<M>
    {
        IAsyncPipeBuilder<M> Parallel(bool asParallel); // false by default
        IAsyncPipeBuilder<M> Cache(bool threadSafe); // false by default
        IAsyncPipeBuilder<M> StopWatch(bool use); // false by default
        IAsyncPipeBuilder<M> Return(M model, PipeResult[] results);

        IAsyncExecutorBuilder<M> Executor(ISyncExecutor<M> executor);
        IAsyncExecutorBuilder<M> Executor(ISyncExecutor<M> executor, bool addif);
        IAsyncExecutorBuilder<M> Executor(Func<ISyncExecutor<M>> executor);
        IAsyncExecutorBuilder<M> Executor(Func<ISyncExecutor<M>> executor, bool addif);
        IAsyncExecutorBuilder<M> Executor(Func<M, PipeResult> executor);
        IAsyncExecutorBuilder<M> Executor(Func<M, PipeResult> executor, bool addif);

        IAsyncPipeBuilder<M> If(bool condition);
        IAsyncPipeBuilder<M> If(Func<M, bool> condition);
        IAsyncPipeBuilder<M> EndIf();

        ISyncExecutor<M> ToSyncExecutor();
    }

    public interface ISyncPipeBuilder
    {
        IAsyncPipeBuilder Parallel(bool asParallel); // false by default
        IAsyncPipeBuilder Cache(bool threadSafe); // false by default
        IAsyncPipeBuilder StopWatch(bool use); // false by default
        IAsyncPipeBuilder Return(PipeResult[] results);

        IAsyncExecutorBuilder Executor(ISyncExecutor executor);
        IAsyncExecutorBuilder Executor(ISyncExecutor executor, bool addif);
        IAsyncExecutorBuilder Executor(Func<ISyncExecutor> executor);
        IAsyncExecutorBuilder Executor(Func<ISyncExecutor> executor, bool addif);
        IAsyncExecutorBuilder Executor(Func<PipeResult> executor);
        IAsyncExecutorBuilder Executor(Func<PipeResult> executor, bool addif);

        IAsyncPipeBuilder If(bool condition);
        IAsyncPipeBuilder EndIf();

        ISyncExecutor ToSyncExecutor();
    }
}
