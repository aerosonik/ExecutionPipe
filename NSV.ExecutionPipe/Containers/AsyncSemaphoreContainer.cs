using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.PipeLines;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Containers
{
    public class AsyncSemaphoreContainer<M, R> : IAsyncContainer<M, R>
    {
        private readonly IAsyncContainer<M, R> _container;
        private readonly string _semaphoreName;
        public AsyncSemaphoreContainer(
            IAsyncContainer<M, R> container,
            string semaphoreName)
        {
            _container = container;
            _semaphoreName = semaphoreName;
        }

        public async Task<PipeResult<R>> RunAsync(M model)
        {
            var result = PipeResult<R>.Default;

            var semaphore = PipeManager.GetSemaphore(_semaphoreName);
            if (semaphore == null)
                return await _container.RunAsync(model);

            await semaphore.WaitAsync();
            try
            {
                result = await _container.RunAsync(model);
            }
            finally
            {
                semaphore.Release();
            }
            return result;
        }
    }

    public class AsyncSemaphoreContainer<M> : IAsyncContainer<M>
    {
        private readonly IAsyncContainer<M> _container;
        private readonly string _semaphoreName;
        public AsyncSemaphoreContainer(
            IAsyncContainer<M> container,
            string semaphoreName)
        {
            _container = container;
            _semaphoreName = semaphoreName;
        }

        public async Task<PipeResult> RunAsync(M model)
        {
            var result = PipeResult.Default;

            var semaphore = PipeManager.GetSemaphore(_semaphoreName);
            if (semaphore == null)
                return await _container.RunAsync(model);

            await semaphore.WaitAsync();
            try
            {
                result = await _container.RunAsync(model);
            }
            finally
            {
                semaphore.Release();
            }
            return result;
        }
    }

    public class AsyncSemaphoreContainer : IAsyncContainer
    {
        private readonly IAsyncContainer _container;
        private readonly string _semaphoreName;
        public AsyncSemaphoreContainer(
            IAsyncContainer container,
            string semaphoreName)
        {
            _container = container;
            _semaphoreName = semaphoreName;
        }

        public async Task<PipeResult> RunAsync()
        {
            var result = PipeResult.Default;

            var semaphore = PipeManager.GetSemaphore(_semaphoreName);
            if (semaphore == null)
                return await _container.RunAsync();

            await semaphore.WaitAsync();
            try
            {
                result = await _container.RunAsync();
            }
            finally
            {
                semaphore.Release();
            }
            return result;
        }
    }

}
