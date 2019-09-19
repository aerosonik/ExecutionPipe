using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;
using System;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Containers
{
    public class AsyncFuncContainer<M, R> :
        BaseAsyncContainer<M, R>,
        IAsyncContainer<M, R>
    {
        private readonly Func<M, Task<PipeResult<R>>> _executor;

        public AsyncFuncContainer(
            Func<M, Task<PipeResult<R>>> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = executor;
        }

        public AsyncFuncContainer(
            Func<M, PipeResult<R>> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = (model) => Task.FromResult(executor(model));
        }

        public async Task<PipeResult<R>> RunAsync(M model)
        {
            var result = (await _executor(model));

            if (result.Success == ExecutionResult.Failed &&
                _retry.HasValue)
            {
                result = await ReTryAsync(model, _executor, _retry, result);
            }
            return result.SetLabel(_label);
        }
    }

    public class AsyncFuncContainer<M> :
        BaseAsyncContainer<M>,
        IAsyncContainer<M>
    {
        private readonly Func<M, Task<PipeResult>> _executor;

        public AsyncFuncContainer(
            Func<M, Task<PipeResult>> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = executor;
        }

        public AsyncFuncContainer(
            Func<M, PipeResult> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = (model) => Task.FromResult(executor(model));
        }

        public async Task<PipeResult> RunAsync(M model)
        {
            var result = (await _executor(model));

            if (result.Success == ExecutionResult.Failed &&
                _retry.HasValue)
            {
                result = await ReTryAsync(model, _executor, _retry, result);
            }
            return result.SetLabel(_label);
        }
    }

    public class AsyncFuncContainer :
        BaseAsyncContainer,
        IAsyncContainer
    {
        private readonly Func<Task<PipeResult>> _executor;

        public AsyncFuncContainer(
            Func<Task<PipeResult>> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = executor;
        }

        public AsyncFuncContainer(
            Func<PipeResult> executor,
            string label,
            IPipeCache cache,
            RetryModel retry) : base(label, cache, retry)
        {
            _executor = () => Task.FromResult(executor());
        }

        public async Task<PipeResult> RunAsync()
        {
            var result = (await _executor());

            if (result.Success == ExecutionResult.Failed &&
                _retry.HasValue)
            {
                result = await ReTryAsync(_executor, _retry, result);
            }
            return result.SetLabel(_label);
        }
    }
}
