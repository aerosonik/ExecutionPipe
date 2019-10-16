using System;

namespace NSV.ExecutionPipe.Builders.Async.Sequential
{
    public interface IAsyncSequentialExecutorBuilder<M, R>
    {
        IAsyncSequentialExecutorFailBuilder<M, R> IfFail();
        IAsyncSequentialExecutorOkBuilder<M, R> IfOk();
        IAsyncSequentialExecutorBuilder<M, R> StopWatch();
        IAsyncSequentialExecutorBuilder<M, R> StopWatch(bool condition);
        IAsyncSequentialExecutorBuilder<M, R> Restricted(
            int minCount,
            int maxCount,
            string key);
        IAsyncSequentialExecutorBuilder<M, R> Restricted(
            bool condition, 
            int minCount,
            int maxCount,
            string key);
        IAsyncSequentialExecutorBuilder<M, R> Label(string label);
        IAsyncSequentialExecutorBuilder<M, R> ExecuteIf(Func<M, bool> condition);
        IAsyncSequentialPipeBuilder<M, R> Add();
    }

    public interface IAsyncSequentialExecutorBuilder<M>
    {
        IAsyncSequentialExecutorFailBuilder<M> IfFail();
        IAsyncSequentialExecutorOkBuilder<M> IfOk();
        IAsyncSequentialExecutorBuilder<M> StopWatch();
        IAsyncSequentialExecutorBuilder<M> StopWatch(bool condition);
        IAsyncSequentialExecutorBuilder<M> Restricted(int maxCount);
        IAsyncSequentialExecutorBuilder<M> Restricted(bool condition, int maxCount);
        IAsyncSequentialExecutorBuilder<M> Label(string label);
        IAsyncSequentialExecutorBuilder<M> ExecuteIf(Func<M, bool> condition);
        IAsyncSequentialPipeBuilder<M> Add();
    }

    public interface IAsyncSequentialExecutorBuilder
    {
        IAsyncSequentialExecutorFailBuilder IfFail();
        IAsyncSequentialExecutorOkBuilder IfOk();
        IAsyncSequentialExecutorBuilder StopWatch();
        IAsyncSequentialExecutorBuilder StopWatch(bool condition);
        IAsyncSequentialExecutorBuilder Restricted(int maxCount);
        IAsyncSequentialExecutorBuilder Restricted(bool condition, int maxCount);
        IAsyncSequentialExecutorBuilder Label(string label);
        IAsyncSequentialPipeBuilder Add();
    }

}
