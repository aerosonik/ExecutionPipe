using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Containers;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Builders
{
    public class AsyncContainerBuilder<M,R> : 
        //ContainerSettings,
        IAsyncExecutorBuilder<M,R>,
        IAsyncExecutorOkBuilder<M,R>,
        IAsyncExecutorFailBuilder<M,R>
    {
        #region Private Fields
        private struct ContainerBuilderSettings
        {
            public ContainerBuilderSettings(object executor)
            {
                Executor = executor;
                Break = false;
                StopWatch = false;
                Semaphore = Optional<SemaphoreModel>.Default;
                Return = Optional<Func<M, PipeResult<R>, PipeResult<R>>>.Default;
                ExecuteConditions = Optional<Func<M, bool>[]>.Default;
            }
            public bool Break { get; set; }
            public bool StopWatch { get; set; }
            public Optional<SemaphoreModel> Semaphore { get; set; }
            public Optional<Func<M, PipeResult<R>, PipeResult<R>>> Return { get; set; }
            public Optional<Func<M, bool>[]> ExecuteConditions { get; set; }
            public object Executor { get; set; }
        }
        private ContainerBuilderSettings _settings;
        private readonly IAsyncContainerQueueEnque<M, R> _asyncContainerQueueEnque;
        private readonly IAsyncPipeBuilder<M, R> _asyncPipeBuilder;
        #endregion

        #region C-tor
        public AsyncContainerBuilder(
           IAsyncContainerQueueEnque<M, R> asyncContainerQueueEnque,
           IAsyncPipeBuilder<M, R> asyncPipeBuilder)
        {
            _asyncContainerQueueEnque = asyncContainerQueueEnque;
            _asyncPipeBuilder = asyncPipeBuilder;
        }
        #endregion

        #region Set Executor
        public void Executor(Func<IAsyncExecutor<M, R>> executor)
        {
            _settings = new ContainerBuilderSettings(executor);
        }
        public void Executor(Func<M, Task<PipeResult<R>>> executor)
        {
            _settings = new ContainerBuilderSettings(executor);
        }
        public void Executor(Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            _settings = new ContainerBuilderSettings(executor);
        }
        public void Executor(Func<M, PipeResult<R>> executor)
        {
            _settings = new ContainerBuilderSettings(executor);
        }
        public void Executor(Func<M, IPipeCache, PipeResult<R>> executor)
        {
            _settings = new ContainerBuilderSettings(executor);
        }
        #endregion

        #region IAsyncExecutorFailBuilder<M,R>
        IAsyncExecutorFailBuilder<M, R> IAsyncExecutorFailBuilder<M, R>.Break(bool condition)
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorFailBuilder<M, R> IAsyncExecutorFailBuilder<M, R>.Retry(int count, int timeOutMilliseconds)
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorFailBuilder<M, R> IAsyncExecutorFailBuilder<M, R>.Retry(bool condition, int count, int timeOutMilliseconds)
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorFailBuilder<M, R> IAsyncExecutorFailBuilder<M, R>.Return(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorBuilder<M, R> IAsyncExecutorFailBuilder<M, R>.Set()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IAsyncExecutorOkBuilder<M,R>
        IAsyncExecutorOkBuilder<M, R> IAsyncExecutorOkBuilder<M, R>.Break(bool condition)
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorOkBuilder<M, R> IAsyncExecutorOkBuilder<M, R>.Return(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorBuilder<M, R> IAsyncExecutorOkBuilder<M, R>.Set()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IAsyncExecutorBuilder<M,R>
        IAsyncPipeBuilder<M, R> IAsyncExecutorBuilder<M, R>.Add()
        {
            IAsyncContainer<M, R> container;
            switch (_settings.Executor)
            {
                case Func<IAsyncExecutor<M, R>> executor:
                    container = new AsyncExecutorContainer<M, R>(
                        executor, Label, Cache, Retry);
                    break;

                case Func<M, Task<PipeResult<R>>> executor:
                    container = new AsyncFuncContainer<M, R>(executor, Label, Cache, Retry);
                    break;
                case Func<M, PipeResult<R>> executor:
                    container = new AsyncFuncContainer<M, R>(executor, Label, Cache, Retry);
                    break;

                case Func<M, IPipeCache, Task<PipeResult<R>>> executor:
                    container = new AsyncFuncCacheContainer<M, R>(executor, Label, Cache, Retry);
                    break;
                case Func<M, IPipeCache, PipeResult<R>> executor:
                    container = new AsyncFuncCacheContainer<M, R>(executor, Label, Cache, Retry);
                    break;

                default:
                    var defaultExecutor = _settings.Executor as Func<IAsyncExecutor<M, R>>;
                    container = new AsyncExecutorContainer<M, R>(defaultExecutor, Label, Cache, Retry);
                    break;
            }

            if(StopWatch)
                container = new AsyncStopWatchContainer<M,R>(container);
            if (Semaphore.HasValue)
                container = new AsyncSemaphoreContainer<M, R>(container, Semaphore.Value.Name);

            var queueContainer = new QueueAsyncContainer<M, R>(
                Break, 
                Return, 
                ExecuteConditions,
                container);

            _asyncContainerQueueEnque.Enque(queueContainer);

            return _asyncPipeBuilder;
        }

        IAsyncExecutorBuilder<M, R> IAsyncExecutorBuilder<M, R>.ExecuteIf(
            Func<M, bool> condition)
        {
            ExecuteConditions
            throw new NotImplementedException();
        }

        IAsyncExecutorFailBuilder<M, R> IAsyncExecutorBuilder<M, R>.IfFail()
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorOkBuilder<M, R> IAsyncExecutorBuilder<M, R>.IfOk()
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorBuilder<M, R> IAsyncExecutorBuilder<M, R>.Label(
            string label)
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorBuilder<M, R> IAsyncExecutorBuilder<M, R>.Restricted(
            int maxCount)
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorBuilder<M, R> IAsyncExecutorBuilder<M, R>.Restricted(
            bool condition,
            int maxCount)
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorBuilder<M, R> IAsyncExecutorBuilder<M, R>.StopWatch()
        {
            throw new NotImplementedException();
        }

        IAsyncExecutorBuilder<M, R> IAsyncExecutorBuilder<M, R>.StopWatch(
            bool condition)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
