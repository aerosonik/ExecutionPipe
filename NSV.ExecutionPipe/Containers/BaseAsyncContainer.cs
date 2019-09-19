using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;
using System;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Containers
{
    public abstract class BaseAsyncContainer<M,R> : BaseContainer
    {
        protected BaseAsyncContainer(string label, IPipeCache cache, RetryModel retry)
            : base(label, cache, retry) { }

        protected async Task<PipeResult<R>> ReTryAsync(
            M model, 
            Func<M, Task<PipeResult<R>>> func,
            RetryModel retry,
            PipeResult<R> result)
        {
            for (int i = 0; i < retry.Count; i++)
            {
                result = await func(model);
                if (result.Success == ExecutionResult.Successful)
                    return result;
                await Task.Delay(retry.TimeOutMilliseconds);
            }
            return result;
        }

        protected async Task<PipeResult<R>> ReTryAsync(
            M model,
            Func<M, IPipeCache, Task<PipeResult<R>>> func,
            RetryModel retry,
            PipeResult<R> result)
        {
            for (int i = 0; i < retry.Count; i++)
            {
                result = await func(model, _cache);
                if (result.Success == ExecutionResult.Successful)
                    return result;
                await Task.Delay(retry.TimeOutMilliseconds);
            }
            return result;
        }
    }

    public abstract class BaseAsyncContainer<M> : BaseContainer
    {
        protected BaseAsyncContainer(string label, IPipeCache cache, RetryModel retry)
            : base(label, cache, retry) { }

        protected async Task<PipeResult> ReTryAsync(
            M model,
            Func<M, Task<PipeResult>> func,
            RetryModel retry,
            PipeResult result)
        {
            for (int i = 0; i < retry.Count; i++)
            {
                result = await func(model);
                if (result.Success == ExecutionResult.Successful)
                    return result;
                await Task.Delay(retry.TimeOutMilliseconds);
            }
            return result;
        }

        protected async Task<PipeResult> ReTryAsync(
            M model,
            Func<M, IPipeCache, Task<PipeResult>> func,
            RetryModel retry,
            PipeResult result)
        {
            for (int i = 0; i < retry.Count; i++)
            {
                result = await func(model, _cache);
                if (result.Success == ExecutionResult.Successful)
                    return result;
                await Task.Delay(retry.TimeOutMilliseconds);
            }
            return result;
        }
    }

    public abstract class BaseAsyncContainer : BaseContainer
    {
        protected BaseAsyncContainer(string label, IPipeCache cache, RetryModel retry)
            : base(label, cache, retry) { }

        protected async Task<PipeResult> ReTryAsync(
            Func<Task<PipeResult>> func,
            RetryModel retry,
            PipeResult result)
        {
            for (int i = 0; i < retry.Count; i++)
            {
                result = await func();
                if (result.Success == ExecutionResult.Successful)
                    return result;
                await Task.Delay(retry.TimeOutMilliseconds);
            }
            return result;
        }

        protected async Task<PipeResult> ReTryAsync(
            Func<IPipeCache, Task<PipeResult>> func,
            RetryModel retry,
            PipeResult result)
        {
            for (int i = 0; i < retry.Count; i++)
            {
                result = await func(_cache);
                if (result.Success == ExecutionResult.Successful)
                    return result;
                await Task.Delay(retry.TimeOutMilliseconds);
            }
            return result;
        }
    }

    public abstract class BaseContainer
    {
        protected BaseContainer(string label, IPipeCache cache, RetryModel retry)
        {
            _label = label;
            _cache = cache;
            _retry = retry;
        }
        protected readonly string _label;
        protected readonly IPipeCache _cache;
        protected readonly Optional<RetryModel> _retry = Optional<RetryModel>.Default;
    }
}
