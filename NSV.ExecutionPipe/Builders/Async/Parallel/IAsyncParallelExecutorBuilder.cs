using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders.Async.Parallel
{
    public interface IAsyncParallelExecutorBuilder<M, R>
    {
        IAsyncParallelExecutorFailBuilder<M, R> IfFail();
        IAsyncParallelExecutorBuilder<M, R> StopWatch();
        IAsyncParallelExecutorBuilder<M, R> StopWatch(bool condition);
        IAsyncParallelExecutorBuilder<M, R> Restricted(
            int initialCount,
            string key);
        IAsyncParallelExecutorBuilder<M, R> Restricted(
            bool condition,
            int initialCount,
            string key);
        IAsyncParallelExecutorBuilder<M, R> Label(string label);
        IAsyncParallelExecutorBuilder<M, R> ExecuteIf(Func<M, bool> condition);
        IAsyncParallelPipeBuilder<M, R> Add();
    }

    public interface IAsyncParallelExecutorBuilder<M>
    {
        IAsyncParallelExecutorFailBuilder<M> IfFail();
        IAsyncParallelExecutorBuilder<M> StopWatch();
        IAsyncParallelExecutorBuilder<M> StopWatch(bool condition);
        IAsyncParallelExecutorBuilder<M> Restricted(int initialCount);
        IAsyncParallelExecutorBuilder<M> Label(string label);
        IAsyncParallelExecutorBuilder<M> ExecuteIf(Func<M, bool> condition);
        IAsyncParallelPipeBuilder<M> Add();
    }

    public interface IAsyncParallelExecutorBuilder
    {
        IAsyncParallelExecutorFailBuilder IfFail();
        IAsyncParallelExecutorBuilder StopWatch();
        IAsyncParallelExecutorBuilder StopWatch(bool condition);
        IAsyncParallelExecutorBuilder Restricted(int initialCount);
        IAsyncParallelExecutorBuilder Label(string label);
        IAsyncParallelPipeBuilder Add();
    }
}
