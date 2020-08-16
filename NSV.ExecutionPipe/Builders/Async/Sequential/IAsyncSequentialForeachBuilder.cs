using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders.Async.Sequential
{
    public interface IAsyncSequentialForeachBuilder<M, TEnumerator, R>
    {
        IAsyncSequentialExecutorBuilder<TEnumerator, R> Executor<TAsyncExecutor>()
            where TAsyncExecutor: IAsyncExecutor<TEnumerator, R>;

        IAsyncSequentialForeachBuilder<M, TEnumerator, R> If(bool condition);
        IAsyncSequentialForeachBuilder<M, TEnumerator, R> If(Func<M, bool> condition);
        IAsyncSequentialForeachBuilder<M, TEnumerator, R> EndIf();

        IAsyncSequentialPipeBuilder<M, R> Return(Func<TEnumerator, PipeResult<R>[], PipeResult<R>> rersultHandler);
    }
}
