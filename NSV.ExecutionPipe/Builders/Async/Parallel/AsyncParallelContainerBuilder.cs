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
        IAsyncParallelExecutorFailBuilder<M, R>,
        IAsyncParallelDefaultExecutorBuilder<M, R>
    {
        #region Private Fields
        private readonly AsyncParallelPipeBuilder<M, R> _asyncPipeBuilder;
        private ExecutorSettings<M, R> _currentExecutorSettings;
        private IAsyncContainer<M, R> _currentContainer;
        private bool _skipCurrentExecutor = false;
        private bool _executeIf = false;
        private readonly AsyncConditionalQueueBuilder<M, R> _conditionalQueueBuilder;
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
            SetExecutorSkip();
            return this;
        }

        public IAsyncParallelExecutorBuilder<M, R> Executor(
            Func<IAsyncExecutor<M, R>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncParallelExecutorBuilder<M, R> Executor(
            Func<M, Task<PipeResult<R>>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncParallelExecutorBuilder<M, R> Executor(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncParallelExecutorBuilder<M, R> Executor(
            Func<M, PipeResult<R>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncParallelExecutorBuilder<M, R> Executor(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        #endregion

        #region Set Default Executor

        public IAsyncParallelDefaultExecutorBuilder<M, R> DefaultSkip()
        {
            SetExecutorSkip();
            return this;
        }
        public IAsyncParallelDefaultExecutorBuilder<M, R> Default(
            Func<IAsyncExecutor<M, R>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncParallelDefaultExecutorBuilder<M, R> Default(
            Func<M, Task<PipeResult<R>>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncParallelDefaultExecutorBuilder<M, R> Default(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncParallelDefaultExecutorBuilder<M, R> Default(
            Func<M, PipeResult<R>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncParallelDefaultExecutorBuilder<M, R> Default(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            SetExecutor(executor);
            return this;
        }

        #endregion

        #region IAsyncParallelExecutorBuilder<M, R>
        IAsyncParallelPipeBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.Add()
        {
            return Add();
        }
        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.ExecuteIf(
            Func<M, bool> condition)
        {
            if(_skipCurrentExecutor)
                return this;

            _conditionalQueueBuilder.AddFuncIfCondition(condition);
            _executeIf = true;
            return this;
        }

        IAsyncParallelExecutorFailBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.IfFail()
        {
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.Label(
            string label)
        {
            Label(label);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.Restricted(
            int initialCount, 
            string key)
        {
            Restricted(initialCount, key);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.Restricted(
            bool condition, 
            int initialCount, 
            string key)
        {
            Restricted(condition, initialCount, key);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.StopWatch()
        {
            StopWatch();
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.StopWatch(
            bool condition)
        {
            StopWatch(condition);
            return this;
        }


        #endregion

        #region IAsyncParallelExecutorFailBuilder<M, R>

        IAsyncParallelExecutorFailBuilder<M, R> IAsyncParallelExecutorFailBuilder<M, R>.Retry(
            int count, 
            int timeOutMilliseconds)
        {
            Retry(count, timeOutMilliseconds);
            return this;
        }

        IAsyncParallelExecutorFailBuilder<M, R> IAsyncParallelExecutorFailBuilder<M, R>.Retry(
            bool condition, 
            int count, 
            int timeOutMilliseconds)
        {
            Retry(count, timeOutMilliseconds);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorFailBuilder<M, R>.Set()
        {
            return this;
        }

        #endregion

        #region  IAsyncParallelDefaultExecutorBuilder<M, R>

        IAsyncParallelDefaultExecutorBuilder<M, R> IAsyncParallelDefaultExecutorBuilder<M, R>.Retry(
            int count, 
            int timeOutMilliseconds)
        {
            Retry(count, timeOutMilliseconds);
            return this;
        }

        IAsyncParallelDefaultExecutorBuilder<M, R> IAsyncParallelDefaultExecutorBuilder<M, R>.Retry(
            bool condition, 
            int count, 
            int timeOutMilliseconds)
        {
            Retry(condition, count, timeOutMilliseconds);
            return this;
        }

        IAsyncParallelDefaultExecutorBuilder<M, R> IAsyncParallelDefaultExecutorBuilder<M, R>.StopWatch()
        {
            StopWatch();
            return this;
        }

        IAsyncParallelDefaultExecutorBuilder<M, R> IAsyncParallelDefaultExecutorBuilder<M, R>.StopWatch(
            bool condition)
        {
            StopWatch(condition);
            return this;
        }

        IAsyncParallelDefaultExecutorBuilder<M, R> IAsyncParallelDefaultExecutorBuilder<M, R>.Restricted(
            int initialCount, 
            string key)
        {
            Restricted(initialCount, key);
            return this;
        }

        IAsyncParallelDefaultExecutorBuilder<M, R> IAsyncParallelDefaultExecutorBuilder<M, R>.Restricted(
            bool condition, 
            int initialCount, 
            string key)
        {
            Restricted(condition, initialCount, key);
            return this;
        }

        IAsyncParallelDefaultExecutorBuilder<M, R> IAsyncParallelDefaultExecutorBuilder<M, R>.Label(
            string label)
        {
            Label(label);
            return this;
        }

        IAsynPipeBuilder<M, R> IAsyncParallelDefaultExecutorBuilder<M, R>.Add()
        {
            return Add(true);
        }

        #endregion

        #region private

        private void SetExecutorSkip()
        {
            _skipCurrentExecutor = true;
        }

        private void SetExecutor(
            Func<IAsyncExecutor<M, R>> executor)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncExecutorContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
        }

        private void SetExecutor(
            Func<M, Task<PipeResult<R>>> executor)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
        }

        private void SetExecutor(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncCacheContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
        }

        private void SetExecutor(
            Func<M, PipeResult<R>> executor)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
        }

        private void SetExecutor(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncCacheContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
        }

        private void Retry(
            int count, 
            int timeOutMilliseconds)
        {
            if (_skipCurrentExecutor)
                return;

            _currentContainer = new AsyncReTryContainer<M, R>(_currentContainer, count, timeOutMilliseconds);
        }

        private void Retry(
            bool condition, 
            int count, 
            int timeOutMilliseconds)
        {
            if (_skipCurrentExecutor || !condition)
                return;

            _currentContainer = new AsyncReTryContainer<M, R>(_currentContainer, count, timeOutMilliseconds);
        }

        private void StopWatch()
        {
            if (_skipCurrentExecutor)
                return;

            _currentContainer = new AsyncStopWatchContainer<M, R>(_currentContainer);
        }

        private void StopWatch(
            bool condition)
        {
            if (_skipCurrentExecutor || !condition)
                return;

            _currentContainer = new AsyncStopWatchContainer<M, R>(_currentContainer);
        }

        private void Restricted(
            bool condition, 
            int initialCount, 
            string key)
        {
            if (_skipCurrentExecutor || !condition)
                return;

            PipeManager.SetSemaphore(initialCount, key);
            _currentContainer = new AsyncSemaphoreContainer<M, R>(_currentContainer, key);
        }

        private void Restricted(
            int initialCount, 
            string key)
        {
            if (_skipCurrentExecutor)
                return;

            PipeManager.SetSemaphore(initialCount, key);
            _currentContainer = new AsyncSemaphoreContainer<M, R>(_currentContainer, key);
        }

        private void Label(string label)
        {
            if (_skipCurrentExecutor)
                return;

            _currentExecutorSettings.Label = label;
        }

        private AsyncParallelPipeBuilder<M, R> Add(bool defaultExecutor = false)
        {
            if (_skipCurrentExecutor)
                return _asyncPipeBuilder;

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

            return _asyncPipeBuilder;
        }

        #endregion
    }
}
