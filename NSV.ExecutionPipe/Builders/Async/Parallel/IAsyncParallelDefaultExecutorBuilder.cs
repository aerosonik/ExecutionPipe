namespace NSV.ExecutionPipe.Builders.Async.Parallel
{
    public interface IAsyncParallelDefaultExecutorBuilder<M, R>
    {
        IAsyncParallelDefaultExecutorBuilder<M, R> Retry(int count, int timeOutMilliseconds);
        IAsyncParallelDefaultExecutorBuilder<M, R> Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncParallelDefaultExecutorBuilder<M, R> StopWatch();
        IAsyncParallelDefaultExecutorBuilder<M, R> StopWatch(bool condition);
        IAsyncParallelDefaultExecutorBuilder<M, R> Restricted(
            int minCount,
            int maxCount,
            string key);
        IAsyncParallelDefaultExecutorBuilder<M, R> Restricted(
            bool condition,
            int minCount,
            int maxCount,
            string key);
        IAsyncParallelDefaultExecutorBuilder<M, R> Label(string label);
        IAsynPipeBuilder<M, R> Add();
    }

    public interface IAsyncParallelDefaultExecutorBuilder<M>
    {
        IAsyncParallelDefaultExecutorBuilder<M> Retry(int count, int timeOutMilliseconds);
        IAsyncParallelDefaultExecutorBuilder<M> Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncParallelDefaultExecutorBuilder<M> StopWatch();
        IAsyncParallelDefaultExecutorBuilder<M> StopWatch(bool condition);
        IAsyncParallelDefaultExecutorBuilder<M> Restricted(
            int minCount,
            int maxCount,
            string key);
        IAsyncParallelDefaultExecutorBuilder<M> Restricted(
            bool condition,
            int minCount,
            int maxCount,
            string key);
        IAsyncParallelDefaultExecutorBuilder<M> Label(string label);
        IAsynPipeBuilder<M> Add();
    }

    public interface IAsyncParallelDefaultExecutorBuilder
    {
        IAsyncParallelDefaultExecutorBuilder Retry(int count, int timeOutMilliseconds);
        IAsyncParallelDefaultExecutorBuilder Retry(bool condition, int count, int timeOutMilliseconds);
        IAsyncParallelDefaultExecutorBuilder StopWatch();
        IAsyncParallelDefaultExecutorBuilder StopWatch(bool condition);
        IAsyncParallelDefaultExecutorBuilder Restricted(
            int minCount,
            int maxCount,
            string key);
        IAsyncParallelDefaultExecutorBuilder Restricted(
            bool condition,
            int minCount,
            int maxCount,
            string key);
        IAsyncParallelDefaultExecutorBuilder Label(string label);
        IAsynPipeBuilder Add();
    }
}
