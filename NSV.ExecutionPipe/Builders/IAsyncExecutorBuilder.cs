using System;

namespace NSV.ExecutionPipe.Builders
{
    public interface IAsyncExecutorBuilder<M, R>
    {
        IAsyncExecutorFailBuilder<M, R> IfFail();
        IAsyncExecutorOkBuilder<M, R> IfOk();
        IAsyncExecutorBuilder<M, R> StopWatch();
        IAsyncExecutorBuilder<M, R> StopWatch(bool condition);
        IAsyncExecutorBuilder<M, R> Restricted(int maxCount);
        IAsyncExecutorBuilder<M, R> Restricted(bool condition, int maxCount);
        IAsyncExecutorBuilder<M, R> Label(string label);
        IAsyncExecutorBuilder<M, R> ExecuteIf(Func<M, bool> condition);
        IAsyncPipeBuilder<M, R> Add();
    }

    public interface IAsyncExecutorBuilder<M>
    {
        IAsyncExecutorFailBuilder<M> IfFail();
        IAsyncExecutorOkBuilder<M> IfOk();
        IAsyncExecutorBuilder<M> StopWatch();
        IAsyncExecutorBuilder<M> StopWatch(bool condition);
        IAsyncExecutorBuilder<M> Restricted(int maxCount);
        IAsyncExecutorBuilder<M> Restricted(bool condition, int maxCount);
        IAsyncExecutorBuilder<M> Label(string label);
        IAsyncExecutorBuilder<M> ExecuteIf(Func<M, bool> condition);
        IAsyncPipeBuilder<M> Add();
    }

    public interface IAsyncExecutorBuilder
    {
        IAsyncExecutorFailBuilder IfFail();
        IAsyncExecutorOkBuilder IfOk();
        IAsyncExecutorBuilder StopWatch();
        IAsyncExecutorBuilder StopWatch(bool condition);
        IAsyncExecutorBuilder Restricted(int maxCount);
        IAsyncExecutorBuilder Restricted(bool condition, int maxCount);
        IAsyncExecutorBuilder Label(string label);
        IAsyncPipeBuilder Add();
    }

}
