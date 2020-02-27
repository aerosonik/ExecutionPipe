namespace NSV.ExecutionPipe.Builders.Async.Sequential
{
    public interface IAsyncSequentialDefaultExecutorBuilder<M, R>
    {
        IAsyncSequentialDefaultExecutorBuilder<M, R> Retry(int count, int timeOutMilliseconds);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncSequentialDefaultExecutorBuilder<M, R> StopWatch();
        IAsyncSequentialDefaultExecutorBuilder<M, R> StopWatch(bool condition);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Restricted(
            int initialCount,
            string key);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Restricted(
            bool condition,
            int initialCount,
            string key);
        IAsyncSequentialDefaultExecutorBuilder<M, R> Label(string label);
        IAsynPipeBuilder<M, R> Add();
    }

    public interface IAsyncSequentialDefaultExecutorBuilder<M>
    {
        IAsyncSequentialDefaultExecutorBuilder<M> Retry(int count, int timeOutMilliseconds);
        IAsyncSequentialDefaultExecutorBuilder<M> Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncSequentialDefaultExecutorBuilder<M> StopWatch();
        IAsyncSequentialDefaultExecutorBuilder<M> StopWatch(bool condition);
        IAsyncSequentialDefaultExecutorBuilder<M> Restricted(int initialCount);
        IAsyncSequentialDefaultExecutorBuilder<M> Restricted(bool condition, int initialCount);
        IAsyncSequentialDefaultExecutorBuilder<M> Label(string label);
        IAsynPipeBuilder<M> Add();
    }

    public interface IAsyncSequentialDefaultExecutorBuilder
    {
        IAsyncSequentialDefaultExecutorBuilder Retry(int count, int timeOutMilliseconds);
        IAsyncSequentialDefaultExecutorBuilder Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncSequentialDefaultExecutorBuilder StopWatch();
        IAsyncSequentialDefaultExecutorBuilder StopWatch(bool condition);
        IAsyncSequentialDefaultExecutorBuilder Restricted(int initialCount);
        IAsyncSequentialDefaultExecutorBuilder Restricted(bool condition, int initialCount);
        IAsyncSequentialDefaultExecutorBuilder Label(string label);
        IAsynPipeBuilder Add();
    }
}
