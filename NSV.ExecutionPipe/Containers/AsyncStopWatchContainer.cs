﻿using NSV.ExecutionPipe.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Containers
{
    public class AsyncStopWatchContainer<M, R> : IAsyncContainer<M, R>
    {
        private readonly IAsyncContainer<M, R> _container;

        public AsyncStopWatchContainer(IAsyncContainer<M, R> container)
        {
            _container = container;
        }

        public async Task<PipeResult<R>> RunAsync(M model)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = await _container.RunAsync(model);
            stopWatch.Stop();
            result.SetElapsed(stopWatch.Elapsed);
            return result;
        }
    }

    public class AsyncStopWatchContainer<M> : IAsyncContainer<M>
    {
        private readonly IAsyncContainer<M> _container;

        public AsyncStopWatchContainer(IAsyncContainer<M> container)
        {
            _container = container;
        }

        public async Task<PipeResult> RunAsync(M model)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = await _container.RunAsync(model);
            stopWatch.Stop();
            result.SetElapsed(stopWatch.Elapsed);
            return result;
        }
    }

    public class AsyncStopWatchContainer : IAsyncContainer
    {
        private readonly IAsyncContainer _container;

        public AsyncStopWatchContainer(IAsyncContainer container)
        {
            _container = container;
        }

        public async Task<PipeResult> RunAsync()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = await _container.RunAsync();
            stopWatch.Stop();
            result.SetElapsed(stopWatch.Elapsed);
            return result;
        }
    }
}
