using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;

namespace NSV.ExecutionPipe.Containers.Async
{
    public class AsyncReTryContainer<M, R> : IAsyncContainer<M, R>
    {
        private readonly IAsyncContainer<M, R> _container;
        private int _count;
        private int _timeOutMilliseconds;

        public AsyncReTryContainer(
            IAsyncContainer<M, R> container,
            int count, 
            int timeOutMilliseconds)
        {
            _container = container;
            _count = count;
            _timeOutMilliseconds = timeOutMilliseconds;
        }

        public async Task<PipeResult<R>> RunAsync(M model, IPipeCache cache)
        {
            var result = await _container.RunAsync(model, cache);
            if (result.Success == ExecutionResult.Failed &&
                _count > 0)
            {
                return await ReTryAsync(model, cache);
            }
            return result;
        }

        private async Task<PipeResult<R>> ReTryAsync(M model, IPipeCache cache)
        {
            for (int i = 0; i < _count-1; i++)
            {
                var result = await _container.RunAsync(model, cache);
                if (result.Success == ExecutionResult.Successful)
                    return result;

                await Task.Delay(_timeOutMilliseconds);
            }
            return await _container.RunAsync(model, cache);
        }
    }
}
