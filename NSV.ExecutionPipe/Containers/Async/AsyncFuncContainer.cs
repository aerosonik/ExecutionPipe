using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;
using System;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Containers.Async
{
    public class AsyncFuncContainer<M, R> : IAsyncContainer<M, R>
    {
        private readonly Func<M, Task<PipeResult<R>>> _executor;

        public AsyncFuncContainer(Func<M, Task<PipeResult<R>>> executor)
        {
            _executor = executor;
        }

        public AsyncFuncContainer(Func<M, PipeResult<R>> executor)
        {
            _executor = (model) => Task.FromResult(executor(model));
        }

        public async Task<PipeResult<R>> RunAsync(M model, IPipeCache cache)
        {
            var result = (await _executor(model));
            return result;//.SetLabel(_label);
        }
    }

    public class AsyncFuncContainer<M> : IAsyncContainer<M>
    {
        private readonly Func<M, Task<PipeResult>> _executor;

        public AsyncFuncContainer(Func<M, Task<PipeResult>> executor)
        {
            _executor = executor;
        }

        public AsyncFuncContainer(Func<M, PipeResult> executor)
        {
            _executor = (model) => Task.FromResult(executor(model));
        }

        public async Task<PipeResult> RunAsync(M model, IPipeCache cache)
        {
            var result = (await _executor(model));
            return result;//.SetLabel(_label);
        }
    }

    public class AsyncFuncContainer : IAsyncContainer
    {
        private readonly Func<Task<PipeResult>> _executor;

        public AsyncFuncContainer(Func<Task<PipeResult>> executor)
        {
            _executor = executor;
        }

        public AsyncFuncContainer(Func<PipeResult> executor)
        {
            _executor = () => Task.FromResult(executor());
        }

        public async Task<PipeResult> RunAsync(IPipeCache cache)
        {
            var result = (await _executor());
            return result;//.SetLabel(_label);
        }
    }
}
