using System;

namespace NSV.ExecutionPipe.Builders.Sync.Sequential
{
    public interface ISyncSequentialExecutorBuilder<M, R>
    {
        ISyncSequentialExecutorFailBuilder<M, R> IfFail();
        ISyncSequentialExecutorOkBuilder<M, R> IfOk();
        ISyncSequentialExecutorBuilder<M, R> StopWatch();
        ISyncSequentialExecutorBuilder<M, R> StopWatch(bool condition);
        ISyncSequentialExecutorBuilder<M, R> Restricted(int maxCount);
        ISyncSequentialExecutorBuilder<M, R> Restricted(bool condition, int maxCount);
        ISyncSequentialExecutorBuilder<M, R> Label(string label);
        ISyncSequentialExecutorBuilder<M, R> ExecuteIf(Func<M, bool> condition);
        ISyncSequentialPipeBuilder<M, R> Add();
    }

    public interface ISyncSequentialExecutorBuilder<M>
    {
        ISyncSequentialExecutorFailBuilder<M> IfFail();
        ISyncSequentialExecutorOkBuilder<M> IfOk();
        ISyncSequentialExecutorBuilder<M> StopWatch();
        ISyncSequentialExecutorBuilder<M> StopWatch(bool condition);
        ISyncSequentialExecutorBuilder<M> Restricted(int maxCount);
        ISyncSequentialExecutorBuilder<M> Restricted(bool condition, int maxCount);
        ISyncSequentialExecutorBuilder<M> Label(string label);
        ISyncSequentialExecutorBuilder<M> ExecuteIf(Func<M, bool> condition);
        ISyncSequentialPipeBuilder<M> Add();
    }

    public interface ISyncSequentialExecutorBuilder
    {
        ISyncSequentialExecutorFailBuilder IfFail();
        ISyncSequentialExecutorOkBuilder IfOk();
        ISyncSequentialExecutorBuilder StopWatch();
        ISyncSequentialExecutorBuilder StopWatch(bool condition);
        ISyncSequentialExecutorBuilder Restricted(int maxCount);
        ISyncSequentialExecutorBuilder Restricted(bool condition, int maxCount);
        ISyncSequentialExecutorBuilder Label(string label);
        ISyncSequentialPipeBuilder Add();
    }
}
