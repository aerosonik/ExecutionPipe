using NSV.ExecutionPipe.Builders.Async.Sequential;
using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Builders.Async.Sequential
{
    public interface IAsyncSequentialPipeBuilder<M,R>
    {
        IAsyncSequentialPipeBuilder<M, R> Cache(bool threadSafe);
        IAsyncSequentialPipeBuilder<M, R> StopWatch(bool use);

        IAsyncSequentialExecutorBuilder<M, R> Executor(IAsyncExecutor<M, R> executor);
        IAsyncSequentialExecutorBuilder<M, R> Executor(IAsyncExecutor<M, R> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<IAsyncExecutor<M, R>> executor);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<IAsyncExecutor<M, R>> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<M, Task<PipeResult<R>>> executor);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<M, Task<PipeResult<R>>> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<M, IPipeCache, Task<PipeResult<R>>> executor);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<M, IPipeCache, Task<PipeResult<R>>> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<M, IPipeCache, PipeResult<R>> executor);
        IAsyncSequentialExecutorBuilder<M, R> Executor(Func<M, IPipeCache, PipeResult<R>> executor, bool addif);

        IAsyncSequentialPipeBuilder<M, R> If(bool condition);
        IAsyncSequentialPipeBuilder<M, R> If(Func<M, bool> condition);
        IAsyncSequentialPipeBuilder<M, R> EndIf();

        IAsyncPipe<M, R> Return(Func<M, PipeResult<R>[], PipeResult<R>> rersultHandler);
    }

    public interface IAsyncSequentialPipeBuilder<M>
    {
        IAsyncSequentialPipeBuilder<M> Parallel(bool asParallel); // false by default
        IAsyncSequentialPipeBuilder<M> Cache(bool threadSafe); // false by default
        IAsyncSequentialPipeBuilder<M> StopWatch(bool use); // false by default
        IAsyncSequentialPipeBuilder<M> Return(M model, PipeResult[] results);

        IAsyncSequentialExecutorBuilder<M> Executor(IAsyncExecutor<M> executor);
        IAsyncSequentialExecutorBuilder<M> Executor(IAsyncExecutor<M> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M> Executor(Func<IAsyncExecutor<M>> executor);
        IAsyncSequentialExecutorBuilder<M> Executor(Func<IAsyncExecutor<M>> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M> Executor(Func<M, Task<PipeResult>> executor);
        IAsyncSequentialExecutorBuilder<M> Executor(Func<M, Task<PipeResult>> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M> Executor(Func<M, IPipeCache, Task<PipeResult>> executor);
        IAsyncSequentialExecutorBuilder<M> Executor(Func<M, IPipeCache, Task<PipeResult>> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M> Executor(Func<M, PipeResult> executor);
        IAsyncSequentialExecutorBuilder<M> Executor(Func<M, PipeResult> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M> Executor(Func<M, IPipeCache, PipeResult> executor);
        IAsyncSequentialExecutorBuilder<M> Executor(Func<M, IPipeCache, PipeResult> executor, bool addif);

        IAsyncSequentialPipeBuilder<M> If(bool condition);
        IAsyncSequentialPipeBuilder<M> If(Func<M, bool> condition);
        IAsyncSequentialPipeBuilder<M> EndIf();

        IAsyncExecutor<M> ToAsyncExecutor();
    }

    public interface IAsyncSequentialPipeBuilder
    {
        IAsyncSequentialPipeBuilder Parallel(bool asParallel); // false by default
        IAsyncSequentialPipeBuilder Cache(bool threadSafe); // false by default
        IAsyncSequentialPipeBuilder StopWatch(bool use); // false by default
        IAsyncSequentialPipeBuilder Return(PipeResult[] results);

        IAsyncSequentialExecutorBuilder Executor(IAsyncExecutor executor);
        IAsyncSequentialExecutorBuilder Executor(IAsyncExecutor executor, bool addif);
        IAsyncSequentialExecutorBuilder Executor(Func<IAsyncExecutor> executor);
        IAsyncSequentialExecutorBuilder Executor(Func<IAsyncExecutor> executor, bool addif);
        IAsyncSequentialExecutorBuilder Executor(Func<Task<PipeResult>> executor);
        IAsyncSequentialExecutorBuilder Executor(Func<Task<PipeResult>> executor, bool addif);
        IAsyncSequentialExecutorBuilder Executor(Func<IPipeCache, Task<PipeResult>> executor);
        IAsyncSequentialExecutorBuilder Executor(Func<IPipeCache, Task<PipeResult>> executor, bool addif);
        IAsyncSequentialExecutorBuilder Executor(Func<PipeResult> executor);
        IAsyncSequentialExecutorBuilder Executor(Func<PipeResult> executor, bool addif);
        IAsyncSequentialExecutorBuilder Executor(Func<IPipeCache, PipeResult> executor);
        IAsyncSequentialExecutorBuilder Executor(Func<IPipeCache, PipeResult> executor, bool addif);

        IAsyncSequentialPipeBuilder If(bool condition);
        IAsyncSequentialPipeBuilder EndIf();

        IAsyncExecutor ToAsyncExecutor();
    }
}
