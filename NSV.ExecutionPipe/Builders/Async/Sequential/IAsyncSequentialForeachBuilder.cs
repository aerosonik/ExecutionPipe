using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NSV.ExecutionPipe.Builders.Async.Sequential
{
    public interface IAsyncSequentialForeachBuilder<M, TEnumerator, R, TReturnInterface>
    {
        IAsyncSequentialExecutorBuilder<TEnumerator, R, IAsyncSequentialForeachBuilder<M, TEnumerator, R, TReturnInterface>> Executor(
            IAsyncExecutor<TEnumerator, R> executor);
        IAsyncSequentialForeachBuilder<TEnumerator,TChildEnumerator, R, IAsyncSequentialForeachBuilder<M, TEnumerator, R, TReturnInterface>> Foreach<TChildEnumerator>(
            Expression<Func<TEnumerator, IEnumerable<TEnumerator>>> enumerated);
        TReturnInterface EndForeach(Func<TEnumerator, PipeResult<R>[], PipeResult<R>> handler);
    }
}
