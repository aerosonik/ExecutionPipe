using System;
using System.Threading.Tasks;
using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;

namespace NSV.ExecutionPipe.Containers.Async
{
    public class AsyncExecutorContainer<M, R>: IAsyncContainer<M, R>
    {
        private readonly Func<IAsyncExecutor<M, R>> _executor;
        private IAsyncExecutor<M, R> _executorInstance;

        public AsyncExecutorContainer(Func<IAsyncExecutor<M, R>> executor)
        {
            _executor = executor;
        }

        public async Task<PipeResult<R>> RunAsync(M model, IPipeCache cache)
        {
            if (_executorInstance == null)
                _executorInstance = _executor();

            return await _executorInstance.ExecuteAsync(model, cache);
        }
    }

    public class AsyncExecutorContainer<M>: IAsyncContainer<M>
    {
        private readonly Func<IAsyncExecutor<M>> _executor;
        private IAsyncExecutor<M> _executorInstance;
        public AsyncExecutorContainer(Func<IAsyncExecutor<M>> executor)
        {
            _executor = executor;
        }

        public async Task<PipeResult> RunAsync(M model, IPipeCache cache)
        {
            if (_executorInstance == null)
                _executorInstance = _executor();

            return await _executorInstance.ExecuteAsync(model, cache);
        }
    }

    public class AsyncExecutorContainer : IAsyncContainer
    {
        private readonly Func<IAsyncExecutor> _executor;
        private IAsyncExecutor _executorInstance;
        public AsyncExecutorContainer(Func<IAsyncExecutor> executor)
        {
            _executor = executor;
        }

        public async Task<PipeResult> RunAsync(IPipeCache cache)
        {
            if (_executorInstance == null)
                _executorInstance = _executor();

            return await _executorInstance.ExecuteAsync(cache);
        }
    }

}
