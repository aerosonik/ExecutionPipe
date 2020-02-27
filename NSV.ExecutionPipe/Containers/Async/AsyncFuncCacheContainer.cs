using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Containers.Async
{
    public class AsyncFuncCacheContainer<M, R> : IAsyncContainer<M, R>
    {
        private readonly Func<M, IPipeCache, Task<PipeResult<R>>> _executor;

        public AsyncFuncCacheContainer(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            _executor = executor;
        }

        public AsyncFuncCacheContainer(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            _executor = (m, c) => Task.FromResult(executor(m, c));
        }

        public async Task<PipeResult<R>> RunAsync(M model, IPipeCache cache)
        {
            var result = (await _executor(model, cache));
            return result;//.SetLabel(_label);
        }
    }

    public class AsyncFuncCacheContainer<M> : IAsyncContainer<M>
    {
        private readonly Func<M, IPipeCache, Task<PipeResult>> _executor;

        public AsyncFuncCacheContainer(
            Func<M, IPipeCache, Task<PipeResult>> executor)
        {
            _executor = executor;
        }

        public AsyncFuncCacheContainer(
            Func<M, IPipeCache, PipeResult> executor)
        {
            _executor = (m, c) => Task.FromResult(executor(m, c));
        }

        public async Task<PipeResult> RunAsync(M model, IPipeCache cache)
        {
            var result = (await _executor(model, cache));

            return result;//.SetLabel(_label);
        }
    }

    public class AsyncFuncCacheContainer : IAsyncContainer
    {
        private readonly Func<IPipeCache, Task<PipeResult>> _executor;

        public AsyncFuncCacheContainer(
            Func<IPipeCache, Task<PipeResult>> executor)
        {
            _executor = executor;
        }

        public AsyncFuncCacheContainer(
            Func<IPipeCache, PipeResult> executor)
        {
            _executor = (c) => Task.FromResult(executor(c));
        }

        public async Task<PipeResult> RunAsync(IPipeCache cache)
        {
            var result = (await _executor(cache));

            return result;//.SetLabel(_label);
        }
    }
}
