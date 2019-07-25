using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
    internal class SyncExecutorContainer<M, R> : ISyncExecutorContainer<M, R>
    {
        private ISyncExecutorContainerSettings<M, R> _settings { get; set; }
        internal SyncExecutorContainer(ISyncExecutorContainerSettings<M, R> settings)
        {
            _settings = settings;
        }

        public PipeResult<R> Run(M model)
        {
            return _settings
                .Executor()
                .ExecuteSync(model, _settings.Cache)
                .SetLabel(_settings.Label);
        }
    }
    internal interface ISyncExecutorContainer<M, R>
    {
        PipeResult<R> Run(M model);
    }

    internal class SyncExecutorContainerStopWatch<M, R> :
        ISyncExecutorContainer<M, R>
    {
        private ISyncExecutorContainer<M, R> _container;
        internal SyncExecutorContainerStopWatch(ISyncExecutorContainer<M, R> container)
        {
            _container = container;
        }

        public PipeResult<R> Run(M model)
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = _container.Run(model);
            sw.Stop();
            return result.SetElapsed(sw.Elapsed);
        }
    }

    internal class SyncExecutorContainerSemaphoreSlim<M, R> :
        ISyncExecutorContainer<M, R>
    {
        private ISyncExecutorContainer<M, R> _container;
        private IExecutorContainerSettings<M, R> _settings { get; set; }
        internal SyncExecutorContainerSemaphoreSlim(
            ISyncExecutorContainer<M, R> container, 
            IExecutorContainerSettings<M, R> settings)
        {
            _container = container;
            _settings = settings;
        }

        public PipeResult<R> Run(M model)
        {
            PipeResult<R> result = PipeResult<R>.DefaultUnSuccessful;

            var semafore = new SemaphoreSlim(0, _settings.RestrictedModeMaxThreads);
            semafore.Wait();
            try
            {
                result = _container.Run(model);
            }
            finally
            {
                semafore.Release();
            }
            return result;
        }
    }

}

