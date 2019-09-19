using System;

namespace NSV.ExecutionPipe.Builders
{
    public interface ISyncExecutorBuilder<M, R>
    {
        ISyncExecutorFailBuilder<M, R> IfFail();
        ISyncExecutorOkBuilder<M, R> IfOk();
        ISyncExecutorBuilder<M, R> StopWatch();
        ISyncExecutorBuilder<M, R> StopWatch(bool condition);
        ISyncExecutorBuilder<M, R> Restricted(int maxCount);
        ISyncExecutorBuilder<M, R> Restricted(bool condition, int maxCount);
        ISyncExecutorBuilder<M, R> Label(string label);
        ISyncExecutorBuilder<M, R> ExecuteIf(Func<M, bool> condition);
        ISyncPipeBuilder<M, R> Add();
    }

    public interface ISyncExecutorBuilder<M>
    {
        ISyncExecutorFailBuilder<M> IfFail();
        ISyncExecutorOkBuilder<M> IfOk();
        ISyncExecutorBuilder<M> StopWatch();
        ISyncExecutorBuilder<M> StopWatch(bool condition);
        ISyncExecutorBuilder<M> Restricted(int maxCount);
        ISyncExecutorBuilder<M> Restricted(bool condition, int maxCount);
        ISyncExecutorBuilder<M> Label(string label);
        ISyncExecutorBuilder<M> ExecuteIf(Func<M, bool> condition);
        ISyncPipeBuilder<M> Add();
    }

    public interface ISyncExecutorBuilder
    {
        ISyncExecutorFailBuilder IfFail();
        ISyncExecutorOkBuilder IfOk();
        ISyncExecutorBuilder StopWatch();
        ISyncExecutorBuilder StopWatch(bool condition);
        ISyncExecutorBuilder Restricted(int maxCount);
        ISyncExecutorBuilder Restricted(bool condition, int maxCount);
        ISyncExecutorBuilder Label(string label);
        ISyncPipeBuilder Add();
    }
}
