using System;
using System.Threading.Tasks;
using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;

namespace NSV.ExecutionPipe.Containers
{
    public class AsyncExecutorContainer<M, R> : 
        BaseAsyncContainer<M,R>,
        IAsyncContainer<M, R>
    {
        private readonly Func<IAsyncExecutor<M, R>> _executor;

        public AsyncExecutorContainer(
            Func<IAsyncExecutor<M, R>> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = executor;
        }

        public async Task<PipeResult<R>> RunAsync(M model)
        {
            var executor = _executor();
            executor.Cache = _cache;
            var result = await executor.ExecuteAsync(model);
            if (result.Success == ExecutionResult.Failed &&
                _retry.HasValue)
            {
                result = await ReTryAsync(model, executor.ExecuteAsync, _retry, result);
            }
            return result.SetLabel(_label);
        }
    }

    public class AsyncExecutorContainer<M> :
        BaseAsyncContainer<M>,
        IAsyncContainer<M>
    {
        private readonly Func<IAsyncExecutor<M>> _executor;

        public AsyncExecutorContainer(
            Func<IAsyncExecutor<M>> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = executor;
        }

        public async Task<PipeResult> RunAsync(M model)
        {
            var executor = _executor();
            executor.Cache = _cache;
            var result = await executor.ExecuteAsync(model);
            if (result.Success == ExecutionResult.Failed &&
                _retry.HasValue)
            {
                result = await ReTryAsync(model, executor.ExecuteAsync, _retry, result);
            }
            return result.SetLabel(_label);
        }
    }

    public class AsyncExecutorContainer :
        BaseAsyncContainer,
        IAsyncContainer
    {
        private readonly Func<IAsyncExecutor> _executor;

        public AsyncExecutorContainer(
            Func<IAsyncExecutor> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = executor;
        }

        public async Task<PipeResult> RunAsync()
        {
            var executor = _executor();
            executor.Cache = _cache;
            var result = await executor.ExecuteAsync();
            if (result.Success == ExecutionResult.Failed &&
                _retry.HasValue)
            {
                result = await ReTryAsync(executor.ExecuteAsync, _retry, result);
            }
            return result.SetLabel(_label);
        }
    }

}
