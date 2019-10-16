using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Builders.Async.Parallel
{
    public interface IAsyncParallelPipeBuilder<M, R>
    {
        IAsyncParallelPipeBuilder<M, R> Cache(bool threadSafe);
        IAsyncParallelPipeBuilder<M, R> StopWatch(bool use);

        IAsyncParallelExecutorBuilder<M, R> Executor(IAsyncExecutor<M, R> executor);
        IAsyncParallelExecutorBuilder<M, R> Executor(IAsyncExecutor<M, R> executor, bool addif);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<IAsyncExecutor<M, R>> executor);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<IAsyncExecutor<M, R>> executor, bool addif);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<M, Task<PipeResult<R>>> executor);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<M, Task<PipeResult<R>>> executor, bool addif);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<M, IPipeCache, Task<PipeResult<R>>> executor);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<M, IPipeCache, Task<PipeResult<R>>> executor, bool addif);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor, bool addif);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<M, IPipeCache, PipeResult<R>> executor);
        IAsyncParallelExecutorBuilder<M, R> Executor(Func<M, IPipeCache, PipeResult<R>> executor, bool addif);

        IAsyncParallelPipeBuilder<M, R> If(bool condition);
        IAsyncParallelPipeBuilder<M, R> If(Func<M, bool> condition);
        IAsyncParallelPipeBuilder<M, R> EndIf();

        IAsyncPipe<M, R> Return(Func<M, PipeResult<R>[], PipeResult<R>> rersultHandler);
    }

    public interface IAsyncParallelPipeBuilder<M>
    {
        IAsyncParallelPipeBuilder<M> Parallel(bool asParallel); // false by default
        IAsyncParallelPipeBuilder<M> Cache(bool threadSafe); // false by default
        IAsyncParallelPipeBuilder<M> StopWatch(bool use); // false by default
        IAsyncParallelPipeBuilder<M> Return(M model, PipeResult[] results);

        IAsyncParallelExecutorBuilder<M> Executor(IAsyncExecutor<M> executor);
        IAsyncParallelExecutorBuilder<M> Executor(IAsyncExecutor<M> executor, bool addif);
        IAsyncParallelExecutorBuilder<M> Executor(Func<IAsyncExecutor<M>> executor);
        IAsyncParallelExecutorBuilder<M> Executor(Func<IAsyncExecutor<M>> executor, bool addif);
        IAsyncParallelExecutorBuilder<M> Executor(Func<M, Task<PipeResult>> executor);
        IAsyncParallelExecutorBuilder<M> Executor(Func<M, Task<PipeResult>> executor, bool addif);
        IAsyncParallelExecutorBuilder<M> Executor(Func<M, IPipeCache, Task<PipeResult>> executor);
        IAsyncParallelExecutorBuilder<M> Executor(Func<M, IPipeCache, Task<PipeResult>> executor, bool addif);
        IAsyncParallelExecutorBuilder<M> Executor(Func<M, PipeResult> executor);
        IAsyncParallelExecutorBuilder<M> Executor(Func<M, PipeResult> executor, bool addif);
        IAsyncParallelExecutorBuilder<M> Executor(Func<M, IPipeCache, PipeResult> executor);
        IAsyncParallelExecutorBuilder<M> Executor(Func<M, IPipeCache, PipeResult> executor, bool addif);

        IAsyncParallelPipeBuilder<M> If(bool condition);
        IAsyncParallelPipeBuilder<M> If(Func<M, bool> condition);
        IAsyncParallelPipeBuilder<M> EndIf();

        IAsyncExecutor<M> ToAsyncExecutor();
    }

    public interface IAsyncParallelPipeBuilder
    {
        IAsyncParallelPipeBuilder Parallel(bool asParallel); // false by default
        IAsyncParallelPipeBuilder Cache(bool threadSafe); // false by default
        IAsyncParallelPipeBuilder StopWatch(bool use); // false by default
        IAsyncParallelPipeBuilder Return(PipeResult[] results);

        IAsyncParallelExecutorBuilder Executor(IAsyncExecutor executor);
        IAsyncParallelExecutorBuilder Executor(IAsyncExecutor executor, bool addif);
        IAsyncParallelExecutorBuilder Executor(Func<IAsyncExecutor> executor);
        IAsyncParallelExecutorBuilder Executor(Func<IAsyncExecutor> executor, bool addif);
        IAsyncParallelExecutorBuilder Executor(Func<Task<PipeResult>> executor);
        IAsyncParallelExecutorBuilder Executor(Func<Task<PipeResult>> executor, bool addif);
        IAsyncParallelExecutorBuilder Executor(Func<IPipeCache, Task<PipeResult>> executor);
        IAsyncParallelExecutorBuilder Executor(Func<IPipeCache, Task<PipeResult>> executor, bool addif);
        IAsyncParallelExecutorBuilder Executor(Func<PipeResult> executor);
        IAsyncParallelExecutorBuilder Executor(Func<PipeResult> executor, bool addif);
        IAsyncParallelExecutorBuilder Executor(Func<IPipeCache, PipeResult> executor);
        IAsyncParallelExecutorBuilder Executor(Func<IPipeCache, PipeResult> executor, bool addif);

        IAsyncParallelPipeBuilder If(bool condition);
        IAsyncParallelPipeBuilder EndIf();

        IAsyncExecutor ToAsyncExecutor();
    }
}
