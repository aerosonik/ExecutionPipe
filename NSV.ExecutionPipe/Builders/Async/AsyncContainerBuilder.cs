using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Containers;
using NSV.ExecutionPipe.Containers.Async;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Builders.Async
{
    internal abstract class AsyncContainerBuilder<M,R>
    {
        protected bool _skipCurrentExecutor = false;
        protected ExecutorSettings<M, R> _currentExecutorSettings;
        protected IAsyncContainer<M, R> _currentContainer;
        protected readonly AsyncConditionalQueueBuilder<M, R> _conditionalQueueBuilder;
        protected bool _executeIf = false;

        #region C-tor
        internal AsyncContainerBuilder(AsyncConditionalQueueBuilder<M, R> executeConditions)
        {
            _conditionalQueueBuilder = executeConditions;
        }
        #endregion

        protected void SetExecutorSkip()
        {
            _skipCurrentExecutor = true;
        }

        protected void SetExecutor(
            Func<IAsyncExecutor<M, R>> executor,
            bool isDefault = false)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncExecutorContainer<M, R>(executor);
            _currentExecutorSettings = isDefault
                 ? new ExecutorSettings<M, R> { Label = "Default" }
                 : new ExecutorSettings<M, R>();
        }
        protected void SetExecutor(
            Func<M, Task<PipeResult<R>>> executor,
            bool isDefault = false)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncContainer<M, R>(executor);
            _currentExecutorSettings = isDefault
                ? new ExecutorSettings<M, R> { Label = "Default" }
                : new ExecutorSettings<M, R>();
        }
        protected void SetExecutor(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor,
            bool isDefault = false)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncCacheContainer<M, R>(executor);
            _currentExecutorSettings = isDefault
                ? new ExecutorSettings<M, R> { Label = "Default" }
                : new ExecutorSettings<M, R>();
        }
        protected void SetExecutor(
            Func<M, PipeResult<R>> executor,
            bool isDefault = false)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncContainer<M, R>(executor);
            _currentExecutorSettings = isDefault
                 ? new ExecutorSettings<M, R> { Label = "Default" }
                 : new ExecutorSettings<M, R>();
        }
        protected void SetExecutor(
            Func<M, IPipeCache, PipeResult<R>> executor,
            bool isDefault = false)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncCacheContainer<M, R>(executor);
            _currentExecutorSettings = isDefault
                 ? new ExecutorSettings<M, R> { Label = "Default" }
                 : new ExecutorSettings<M, R>();
        }

        protected void Retry(
            int count, int timeOutMilliseconds)
        {
            if (_skipCurrentExecutor)
                return;

            _currentContainer = new AsyncReTryContainer<M, R>(_currentContainer, count, timeOutMilliseconds);
        }

        protected void Retry(
            bool condition,
            int count,
            int timeOutMilliseconds)
        {
            if (_skipCurrentExecutor || !condition)
                return;

            _currentContainer = new AsyncReTryContainer<M, R>(_currentContainer, count, timeOutMilliseconds);
        }

        protected void StopWatch()
        {
            if (_skipCurrentExecutor)
                return;

            _currentContainer = new AsyncStopWatchContainer<M, R>(_currentContainer);
        }

        protected void StopWatch(
            bool condition)
        {
            if (_skipCurrentExecutor || !condition)
                return;

            _currentContainer = new AsyncStopWatchContainer<M, R>(_currentContainer);
        }

        protected void Restricted(
            bool condition,
            int initialCount,
            string key)
        {
            if (_skipCurrentExecutor || !condition)
                return;

            PipeManager.SetSemaphore(initialCount, key);
            _currentContainer = new AsyncSemaphoreContainer<M, R>(_currentContainer, key);
        }
        protected void Restricted(
            int initialCount,
            string key)
        {
            if (_skipCurrentExecutor)
                return;

            PipeManager.SetSemaphore(initialCount, key);
            _currentContainer = new AsyncSemaphoreContainer<M, R>(_currentContainer, key);
        }

        protected void Label(string label)
        {
            if (_skipCurrentExecutor)
                return;

            _currentExecutorSettings.Label = label;
        }

        protected void AddExecutor(
            bool defaultExecutor = false)
        {
            _currentExecutorSettings
                .ExecuteConditions = _conditionalQueueBuilder.GetFuncIfConditions();

            if (defaultExecutor)
                _conditionalQueueBuilder.SetDefault(
                    _currentExecutorSettings,
                    _currentContainer);
            else
                _conditionalQueueBuilder.Enque(
                    _currentExecutorSettings,
                    _currentContainer);

            if (_executeIf)
            {
                _conditionalQueueBuilder.RemoveIfCondition();
                _executeIf = false;
            }

            _skipCurrentExecutor = false;
            _currentContainer = null;
            _currentExecutorSettings = null;
        }
    }

    internal abstract class AsyncContainerBuilder<M>
    {
    }

    internal abstract class AsyncContainerBuilder
    {
    }
}
