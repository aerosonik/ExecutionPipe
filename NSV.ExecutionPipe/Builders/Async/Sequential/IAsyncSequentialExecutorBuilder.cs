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
            int initialCount,
            string key);
        IAsyncSequentialExecutorBuilder<M, R> Restricted(
            bool condition,
            int initialCount,
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
        IAsyncSequentialExecutorBuilder<M> Restricted(int initialCount);
        IAsyncSequentialExecutorBuilder<M> Restricted(bool condition, int initialCount);
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
        IAsyncSequentialExecutorBuilder Restricted(int initialCount);
        IAsyncSequentialExecutorBuilder Restricted(bool condition, int initialCount);
        IAsyncSequentialExecutorBuilder Label(string label);
        IAsyncSequentialPipeBuilder Add();
    }

    public interface IAsyncSequentialExecutorBuilder<M, R, TReturnInterface>
    {
        //IAsyncSequentialExecutorFailBuilder<M, R> IfFail();
        //IAsyncSequentialExecutorOkBuilder<M, R> IfOk();
        IAsyncSequentialExecutorBuilder<M, R, TReturnInterface> StopWatch();
        IAsyncSequentialExecutorBuilder<M, R, TReturnInterface> StopWatch(bool condition);
        IAsyncSequentialExecutorBuilder<M, R, TReturnInterface> Restricted(
            int initialCount,
            string key);
        IAsyncSequentialExecutorBuilder<M, R, TReturnInterface> Restricted(
            bool condition,
            int initialCount,
            string key);
        IAsyncSequentialExecutorBuilder<M, R, TReturnInterface> Label(string label);
        IAsyncSequentialExecutorBuilder<M, R, TReturnInterface> ExecuteIf(Func<M, bool> condition);
        TReturnInterface Add();
    }
}
