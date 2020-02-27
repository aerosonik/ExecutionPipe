using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Containers.Async
{
    public class AsyncSemaphoreContainer<M, R> : IAsyncContainer<M, R>
    {
        private readonly IAsyncContainer<M, R> _container;
        private readonly SemaphoreSlim _semaphore;
        public AsyncSemaphoreContainer(
            IAsyncContainer<M, R> container,
            string semaphoreName)
        {
            _container = container;
            _semaphore = PipeManager.GetSemaphore(semaphoreName);
        }

        public async Task<PipeResult<R>> RunAsync(M model, IPipeCache cache)
        {
            var result = PipeResult<R>.Default;

            if (_semaphore == null)
                return await _container.RunAsync(model, cache);

            await _semaphore.WaitAsync();
            try
            {
                result = await _container.RunAsync(model, cache);
            }
            finally
            {
                _semaphore.Release();
            }
            return result;
        }
    }

    public class AsyncSemaphoreContainer<M> : IAsyncContainer<M>
    {
        private readonly IAsyncContainer<M> _container;
        private readonly SemaphoreSlim _semaphore;
        public AsyncSemaphoreContainer(
            IAsyncContainer<M> container,
            string semaphoreName)
        {
            _container = container;
            _semaphore = PipeManager.GetSemaphore(semaphoreName);
        }

        public async Task<PipeResult> RunAsync(M model, IPipeCache cache)
        {
            var result = PipeResult.Default;

            if (_semaphore == null)
                return await _container.RunAsync(model, cache);

            await _semaphore.WaitAsync();
            try
            {
                result = await _container.RunAsync(model, cache);
            }
            finally
            {
                _semaphore.Release();
            }
            return result;
        }
    }

    public class AsyncSemaphoreContainer : IAsyncContainer
    {
        private readonly IAsyncContainer _container;
        private readonly SemaphoreSlim _semaphore;
        public AsyncSemaphoreContainer(
            IAsyncContainer container,
            string semaphoreName)
        {
            _container = container;
            _semaphore = PipeManager.GetSemaphore(semaphoreName);
        }

        public async Task<PipeResult> RunAsync(IPipeCache cache)
        {
            var result = PipeResult.Default;

            if (_semaphore == null)
                return await _container.RunAsync(cache);

            await _semaphore.WaitAsync();
            try
            {
                result = await _container.RunAsync(cache);
            }
            finally
            {
                _semaphore.Release();
            }
            return result;
        }
    }

}
