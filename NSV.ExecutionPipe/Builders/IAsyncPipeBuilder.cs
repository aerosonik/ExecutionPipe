using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Builders
{
    public interface IAsyncPipeBuilder<M,R>
    {
        IAsyncPipeBuilder<M, R> Parallel(bool asParallel); // false by default
        IAsyncPipeBuilder<M, R> Cache(bool threadSafe); // false by default
        IAsyncPipeBuilder<M, R> StopWatch(bool use); // false by default
        IAsyncPipeBuilder<M, R> Return(M model, PipeResult<R>[] results);

        IAsyncExecutorBuilder<M, R> Executor(IAsyncExecutor<M, R> executor);
        IAsyncExecutorBuilder<M, R> Executor(IAsyncExecutor<M, R> executor, bool addif);
        IAsyncExecutorBuilder<M, R> Executor(Func<IAsyncExecutor<M, R>> executor);
        IAsyncExecutorBuilder<M, R> Executor(Func<IAsyncExecutor<M, R>> executor, bool addif);
        IAsyncExecutorBuilder<M, R> Executor(Func<M, Task<PipeResult<R>>> executor);
        IAsyncExecutorBuilder<M, R> Executor(Func<M, Task<PipeResult<R>>> executor, bool addif);
        IAsyncExecutorBuilder<M, R> Executor(Func<M, IPipeCache, Task<PipeResult<R>>> executor);
        IAsyncExecutorBuilder<M, R> Executor(Func<M, IPipeCache, Task<PipeResult<R>>> executor, bool addif);
        IAsyncExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor);
        IAsyncExecutorBuilder<M, R> Executor(Func<M, PipeResult<R>> executor, bool addif);
        IAsyncExecutorBuilder<M, R> Executor(Func<M, IPipeCache, PipeResult<R>> executor);
        IAsyncExecutorBuilder<M, R> Executor(Func<M, IPipeCache, PipeResult<R>> executor, bool addif);


        IAsyncPipeBuilder<M, R> If(bool condition);
        IAsyncPipeBuilder<M, R> If(Func<M, bool> condition);
        IAsyncPipeBuilder<M, R> EndIf();

        IAsyncExecutor<M, R> ToAsyncExecutor();
    }

    public interface IAsyncPipeBuilder<M>
    {
        IAsyncPipeBuilder<M> Parallel(bool asParallel); // false by default
        IAsyncPipeBuilder<M> Cache(bool threadSafe); // false by default
        IAsyncPipeBuilder<M> StopWatch(bool use); // false by default
        IAsyncPipeBuilder<M> Return(M model, PipeResult[] results);

        IAsyncExecutorBuilder<M> Executor(IAsyncExecutor<M> executor);
        IAsyncExecutorBuilder<M> Executor(IAsyncExecutor<M> executor, bool addif);
        IAsyncExecutorBuilder<M> Executor(Func<IAsyncExecutor<M>> executor);
        IAsyncExecutorBuilder<M> Executor(Func<IAsyncExecutor<M>> executor, bool addif);
        IAsyncExecutorBuilder<M> Executor(Func<M, Task<PipeResult>> executor);
        IAsyncExecutorBuilder<M> Executor(Func<M, Task<PipeResult>> executor, bool addif);
        IAsyncExecutorBuilder<M> Executor(Func<M, IPipeCache, Task<PipeResult>> executor);
        IAsyncExecutorBuilder<M> Executor(Func<M, IPipeCache, Task<PipeResult>> executor, bool addif);
        IAsyncExecutorBuilder<M> Executor(Func<M, PipeResult> executor);
        IAsyncExecutorBuilder<M> Executor(Func<M, PipeResult> executor, bool addif);
        IAsyncExecutorBuilder<M> Executor(Func<M, IPipeCache, PipeResult> executor);
        IAsyncExecutorBuilder<M> Executor(Func<M, IPipeCache, PipeResult> executor, bool addif);

        IAsyncPipeBuilder<M> If(bool condition);
        IAsyncPipeBuilder<M> If(Func<M, bool> condition);
        IAsyncPipeBuilder<M> EndIf();

        IAsyncExecutor<M> ToAsyncExecutor();
    }

    public interface IAsyncPipeBuilder
    {
        IAsyncPipeBuilder Parallel(bool asParallel); // false by default
        IAsyncPipeBuilder Cache(bool threadSafe); // false by default
        IAsyncPipeBuilder StopWatch(bool use); // false by default
        IAsyncPipeBuilder Return(PipeResult[] results);

        IAsyncExecutorBuilder Executor(IAsyncExecutor executor);
        IAsyncExecutorBuilder Executor(IAsyncExecutor executor, bool addif);
        IAsyncExecutorBuilder Executor(Func<IAsyncExecutor> executor);
        IAsyncExecutorBuilder Executor(Func<IAsyncExecutor> executor, bool addif);
        IAsyncExecutorBuilder Executor(Func<Task<PipeResult>> executor);
        IAsyncExecutorBuilder Executor(Func<Task<PipeResult>> executor, bool addif);
        IAsyncExecutorBuilder Executor(Func<IPipeCache, Task<PipeResult>> executor);
        IAsyncExecutorBuilder Executor(Func<IPipeCache, Task<PipeResult>> executor, bool addif);
        IAsyncExecutorBuilder Executor(Func<PipeResult> executor);
        IAsyncExecutorBuilder Executor(Func<PipeResult> executor, bool addif);
        IAsyncExecutorBuilder Executor(Func<IPipeCache, PipeResult> executor);
        IAsyncExecutorBuilder Executor(Func<IPipeCache, PipeResult> executor, bool addif);

        IAsyncPipeBuilder If(bool condition);
        IAsyncPipeBuilder EndIf();

        IAsyncExecutor ToAsyncExecutor();
    }
}
