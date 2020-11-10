using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Builders.Async.Sequential
{
    public interface IAsyncSequentialPipeBuilder<M,R>
    {
        IAsyncSequentialPipeBuilder<M, R> Cache(bool threadSafe);
        IAsyncSequentialPipeBuilder<M, R> StopWatch(bool use);

        IAsyncSequentialExecutorBuilder<M, R> Executor<TAsyncExecutor>() 
            where TAsyncExecutor: IAsyncExecutor<M, R>;
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

        IAsyncSequentialForeachBuilder<M, TEnumerator, R, IAsyncSequentialPipeBuilder<M, R>> Foreach<TEnumerator>(Expression<Func<M, IEnumerable<TEnumerator>>> enumerated);

        //IAsyncSequentialForeachBuilder<M, TEnumerator, R> Foreach<TEnumerator>(IAsyncExecutor<M, R> executor, Expression<Func<M, IEnumerable<TEnumerator>>> iterator);
        //IAsyncSequentialForeachBuilder<M, TEnumerator, R> Foreach<TEnumerator>(IAsyncExecutor<M, R> executor, Expression<Func<M, IEnumerable<TEnumerator>>> iterator, bool addif);

        IAsyncSequentialPipeBuilder<M, R> If(bool condition);
        IAsyncSequentialPipeBuilder<M, R> If(Func<M, bool> condition);
        IAsyncSequentialPipeBuilder<M, R> EndIf();

        IAsyncPipe<M, R> Return(Func<M, PipeResult<R>[], PipeResult<R>> rersultHandler);

        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(IAsyncExecutor<M, R> executor);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(IAsyncExecutor<M, R> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(Func<IAsyncExecutor<M, R>> executor);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(Func<IAsyncExecutor<M, R>> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(Func<M, Task<PipeResult<R>>> executor);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(Func<M, Task<PipeResult<R>>> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(Func<M, IPipeCache, Task<PipeResult<R>>> executor);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(Func<M, IPipeCache, Task<PipeResult<R>>> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(Func<M, PipeResult<R>> executor);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(Func<M, PipeResult<R>> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(Func<M, IPipeCache, PipeResult<R>> executor);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Default(Func<M, IPipeCache, PipeResult<R>> executor, bool addif);
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

        IAsyncPipe<M> Return(Func<M, PipeResult[], PipeResult> rersultHandler);

        IAsyncSequentialDefaultExecutorBuilder<M> Default(IAsyncExecutor<M> executor);
        IAsyncSequentialDefaultExecutorBuilder<M> Default(IAsyncExecutor<M> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder<M> Default(Func<IAsyncExecutor<M>> executor);
        IAsyncSequentialDefaultExecutorBuilder<M> Default(Func<IAsyncExecutor<M>> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder<M> Default(Func<M, Task<PipeResult>> executor);
        IAsyncSequentialDefaultExecutorBuilder<M> Default(Func<M, Task<PipeResult>> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder<M> Default(Func<M, IPipeCache, Task<PipeResult>> executor);
        IAsyncSequentialDefaultExecutorBuilder<M> Default(Func<M, IPipeCache, Task<PipeResult>> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder<M> Default(Func<M, PipeResult> executor);
        IAsyncSequentialDefaultExecutorBuilder<M> Default(Func<M, PipeResult> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder<M> Default(Func<M, IPipeCache, PipeResult> executor);
        IAsyncSequentialDefaultExecutorBuilder<M> Default(Func<M, IPipeCache, PipeResult> executor, bool addif);
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

        IAsyncSequentialDefaultExecutorBuilder Default(IAsyncExecutor executor);
        IAsyncSequentialDefaultExecutorBuilder Default(IAsyncExecutor executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder Default(Func<IAsyncExecutor> executor);
        IAsyncSequentialDefaultExecutorBuilder Default(Func<IAsyncExecutor> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder Default(Func<Task<PipeResult>> executor);
        IAsyncSequentialDefaultExecutorBuilder Default(Func<Task<PipeResult>> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder Default(Func<IPipeCache, Task<PipeResult>> executor);
        IAsyncSequentialDefaultExecutorBuilder Default(Func<IPipeCache, Task<PipeResult>> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder Default(Func<PipeResult> executor);
        IAsyncSequentialDefaultExecutorBuilder Default(Func<PipeResult> executor, bool addif);
        IAsyncSequentialDefaultExecutorBuilder Default(Func<IPipeCache, PipeResult> executor);
        IAsyncSequentialDefaultExecutorBuilder Default(Func<IPipeCache, PipeResult> executor, bool addif);
    }
}
