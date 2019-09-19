using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Containers
{
    public class AsyncFuncCacheContainer<M, R> :
        BaseAsyncContainer<M, R>,
        IAsyncContainer<M, R>
    {
        private readonly Func<M, IPipeCache, Task<PipeResult<R>>> _executor;

        public AsyncFuncCacheContainer(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = executor;
        }

        public AsyncFuncCacheContainer(
            Func<M, IPipeCache, PipeResult<R>> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = (m, c) => Task.FromResult(executor(m, c));
        }

        public async Task<PipeResult<R>> RunAsync(M model)
        {
            var result = (await _executor(model, _cache));

            if (result.Success == ExecutionResult.Failed &&
                _retry.HasValue)
            {
                result = await ReTryAsync(model, _executor, _retry, result);
            }
            return result.SetLabel(_label);
        }
    }

    public class AsyncFuncCacheContainer<M> :
        BaseAsyncContainer<M>,
        IAsyncContainer<M>
    {
        private readonly Func<M, IPipeCache, Task<PipeResult>> _executor;

        public AsyncFuncCacheContainer(
            Func<M, IPipeCache, Task<PipeResult>> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = executor;
        }

        public AsyncFuncCacheContainer(
            Func<M, IPipeCache, PipeResult> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = (m, c) => Task.FromResult(executor(m, c));
        }

        public async Task<PipeResult> RunAsync(M model)
        {
            var result = (await _executor(model, _cache));

            if (result.Success == ExecutionResult.Failed &&
                _retry.HasValue)
            {
                result = await ReTryAsync(model, _executor, _retry, result);
            }
            return result.SetLabel(_label);
        }
    }

    public class AsyncFuncCacheContainer :
        BaseAsyncContainer,
        IAsyncContainer
    {
        private readonly Func<IPipeCache, Task<PipeResult>> _executor;

        public AsyncFuncCacheContainer(
            Func<IPipeCache, Task<PipeResult>> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = executor;
        }

        public AsyncFuncCacheContainer(
            Func<IPipeCache, PipeResult> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = (c) => Task.FromResult(executor(c));
        }

        public async Task<PipeResult> RunAsync()
        {
            var result = (await _executor(_cache));

            if (result.Success == ExecutionResult.Failed &&
                _retry.HasValue)
            {
                result = await ReTryAsync(_executor, _retry, result);
            }
            return result.SetLabel(_label);
        }
    }
}
