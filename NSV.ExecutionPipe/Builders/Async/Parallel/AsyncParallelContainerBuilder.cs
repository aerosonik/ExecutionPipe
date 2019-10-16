using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Containers;
using NSV.ExecutionPipe.Containers.Async;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Builders.Async.Parallel
{
    internal class AsyncParallelContainerBuilder<M, R> :
        IAsyncParallelExecutorBuilder<M, R>,
        IAsyncParallelExecutorFailBuilder<M, R>
    {
        #region Private Fields
        private readonly IAsyncParallelPipeBuilder<M, R> _asyncPipeBuilder;
        private ExecutorSettings<M, R> _currentExecutorSettings;
        private IAsyncContainer<M, R> _currentContainer;
        private bool _skipCurrentExecutor = false;
        private AsyncConditionalQueueBuilder<M, R> _conditionalQueueBuilder;
        #endregion

        #region C-tor
        internal AsyncParallelContainerBuilder(
            AsyncParallelPipeBuilder<M, R> asyncPipeBuilder,
            AsyncConditionalQueueBuilder<M, R> executeConditions)
        {
            _asyncPipeBuilder = asyncPipeBuilder;
            _conditionalQueueBuilder = executeConditions;
        }
        #endregion

        #region Set Executor
        public IAsyncParallelExecutorBuilder<M, R> ExecutorSkip()
        {
            _skipCurrentExecutor = true;
            return this;
        }

        public IAsyncParallelExecutorBuilder<M, R> Executor(
            Func<IAsyncExecutor<M, R>> executor)
        {
            _currentContainer = new AsyncExecutorContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
            return this;
        }
        public IAsyncParallelExecutorBuilder<M, R> Executor(
            Func<M, Task<PipeResult<R>>> executor)
        {
            _currentContainer = new AsyncFuncContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
            return this;
        }
        public IAsyncParallelExecutorBuilder<M, R> Executor(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            _currentContainer = new AsyncFuncCacheContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
            return this;
        }
        public IAsyncParallelExecutorBuilder<M, R> Executor(
            Func<M, PipeResult<R>> executor)
        {
            _currentContainer = new AsyncFuncContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
            return this;
        }
        public IAsyncParallelExecutorBuilder<M, R> Executor(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            _currentContainer = new AsyncFuncCacheContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
            return this;
        }
        #endregion

        #region IAsyncParallelExecutorBuilder<M, R>
        IAsyncParallelPipeBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.Add()
        {
            if (_skipCurrentExecutor)
                return _asyncPipeBuilder;

            _currentExecutorSettings
                .ExecuteConditions = _conditionalQueueBuilder.GetFuncIfConditions();

            _conditionalQueueBuilder.Enque(
                _currentExecutorSettings,
                _currentContainer);

            _skipCurrentExecutor = false;
            _currentContainer = null;
            _currentExecutorSettings = null;

            return _asyncPipeBuilder;
        }
        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.ExecuteIf(
            Func<M, bool> condition)
        {
            if(_skipCurrentExecutor)
                return this;

            _conditionalQueueBuilder.AddFuncIfCondition(condition);
            return this;
        }

        IAsyncParallelExecutorFailBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.IfFail()
        {
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.Label(
            string label)
        {
            if (_skipCurrentExecutor)
                return this;

            _currentExecutorSettings.Label = label;
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.Restricted(
            int minCount, 
            int maxCount, 
            string key)
        {
            if (_skipCurrentExecutor)
                return this;

            PipeManager.SetSemaphore(minCount, maxCount, key);
            _currentContainer = new AsyncSemaphoreContainer<M, R>(_currentContainer, key);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.Restricted(
            bool condition, 
            int minCount, 
            int maxCount, 
            string key)
        {
            if (_skipCurrentExecutor || !condition)
                return this;

            PipeManager.SetSemaphore(minCount, maxCount, key);
            _currentContainer = new AsyncSemaphoreContainer<M, R>(_currentContainer, key);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.StopWatch()
        {
            if (_skipCurrentExecutor)
                return this;

            _currentContainer = new AsyncStopWatchContainer<M, R>(_currentContainer);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.StopWatch(
            bool condition)
        {
            if (_skipCurrentExecutor || !condition)
                return this;

            _currentContainer = new AsyncStopWatchContainer<M, R>(_currentContainer);
            return this;
        }


        #endregion

        #region IAsyncParallelExecutorFailBuilder<M, R>
        IAsyncParallelExecutorFailBuilder<M, R> IAsyncParallelExecutorFailBuilder<M, R>.Retry(
            int count, 
            int timeOutMilliseconds)
        {
            if (_skipCurrentExecutor)
                return this;

            _currentContainer = new AsyncReTryContainer<M, R>(_currentContainer, count, timeOutMilliseconds);
            return this;
        }

        IAsyncParallelExecutorFailBuilder<M, R> IAsyncParallelExecutorFailBuilder<M, R>.Retry(
            bool condition, 
            int count, 
            int timeOutMilliseconds)
        {
            if (_skipCurrentExecutor || !condition)
                return this;

            _currentContainer = new AsyncReTryContainer<M, R>(_currentContainer, count, timeOutMilliseconds);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorFailBuilder<M, R>.Set()
        {
            return this;
        }
        #endregion
    }
}
