using NSV.ExecutionPipe.Builders.Async;
using NSV.ExecutionPipe.Builders.Async.Sequential;
using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Containers;
using NSV.ExecutionPipe.Containers.Async;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Builders
{
    internal class AsyncSequentialContainerBuilder<M, R> :
        AsyncContainerBuilder<M, R>,
        IAsyncSequentialExecutorBuilder<M, R>,
        IAsyncSequentialExecutorOkBuilder<M, R>,
        IAsyncSequentialExecutorFailBuilder<M, R>,
        IAsyncSequentialDefaultExecutorBuilder<M, R>
    {
        #region Private Fields
        private readonly AsyncSequentialPipeBuilder<M, R> _asyncPipeBuilder;
        #endregion

        #region C-tor
        internal AsyncSequentialContainerBuilder(
            AsyncSequentialPipeBuilder<M, R> asyncPipeBuilder,
            AsyncConditionalQueueBuilder<M, R> executeConditions)
            :base(executeConditions)
        {
            _asyncPipeBuilder = asyncPipeBuilder;
        }
        #endregion

        #region Set Executor
        public IAsyncSequentialExecutorBuilder<M, R> ExecutorSkip()
        {
            SetExecutorSkip();
            return this;
        }
        public IAsyncSequentialExecutorBuilder<M, R> Executor(
            Func<IAsyncExecutor<M, R>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncSequentialExecutorBuilder<M, R> Executor(
            Func<M, Task<PipeResult<R>>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncSequentialExecutorBuilder<M, R> Executor(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncSequentialExecutorBuilder<M, R> Executor(
            Func<M, PipeResult<R>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        public IAsyncSequentialExecutorBuilder<M, R> Executor(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            SetExecutor(executor);
            return this;
        }
        #endregion

        #region Set Default Executor

        public IAsyncSequentialDefaultExecutorBuilder<M, R> DefaultSkip()
        {
            SetExecutorSkip();
            return this;
        }
        public IAsyncSequentialDefaultExecutorBuilder<M, R> Default(
            Func<IAsyncExecutor<M, R>> executor)
        {
            SetExecutor(executor, true);
            return this;
        }
        public IAsyncSequentialDefaultExecutorBuilder<M, R> Default(
            Func<M, Task<PipeResult<R>>> executor)
        {
            SetExecutor(executor, true);
            return this;
        }
        public IAsyncSequentialDefaultExecutorBuilder<M, R> Default(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            SetExecutor(executor, true);
            return this;
        }
        public IAsyncSequentialDefaultExecutorBuilder<M, R> Default(
            Func<M, PipeResult<R>> executor)
        {
            SetExecutor(executor, true);
            return this;
        }
        public IAsyncSequentialDefaultExecutorBuilder<M, R> Default(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            SetExecutor(executor, true);
            return this;
        }

        #endregion

        #region IAsyncSequentialExecutorFailBuilder<M,R>
        IAsyncSequentialExecutorFailBuilder<M, R> IAsyncSequentialExecutorFailBuilder<M, R>.Break(
            bool condition)
        {
            if (_skipCurrentExecutor)
                return this;

            _currentExecutorSettings.FailedBreak = condition;
            return this;
        }

        IAsyncSequentialExecutorFailBuilder<M, R> IAsyncSequentialExecutorFailBuilder<M, R>.Retry(
            int count,
            int timeOutMilliseconds)
        {
            Retry(count, timeOutMilliseconds);
            return this;
        }

        IAsyncSequentialExecutorFailBuilder<M, R> IAsyncSequentialExecutorFailBuilder<M, R>.Retry(
            bool condition,
            int count,
            int timeOutMilliseconds)
        {
            Retry(condition, count, timeOutMilliseconds);
            return this;
        }

        IAsyncSequentialExecutorFailBuilder<M, R> IAsyncSequentialExecutorFailBuilder<M, R>.Return(
            Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            if (_skipCurrentExecutor)
                return this;

            _currentExecutorSettings.FailedReturn = handler;
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorFailBuilder<M, R>.Set()
        {
            return this;
        }
        #endregion

        #region IAsyncSequentialExecutorOkBuilder<M,R>
        IAsyncSequentialExecutorOkBuilder<M, R> IAsyncSequentialExecutorOkBuilder<M, R>.Break(
            bool condition)
        {
            if (_skipCurrentExecutor)
                return this;

            _currentExecutorSettings.OkBreak = condition;
            return this;
        }

        IAsyncSequentialExecutorOkBuilder<M, R> IAsyncSequentialExecutorOkBuilder<M, R>.Return(
            Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            if (_skipCurrentExecutor)
                return this;

            _currentExecutorSettings.OkReturn = handler;
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorOkBuilder<M, R>.Set()
        {
            return this;
        }
        #endregion

        #region IAsyncSequentialExecutorBuilder<M,R>

        IAsyncSequentialPipeBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.Add()
        {
            return Add();
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.ExecuteIf(
            Func<M, bool> condition)
        {
            if (_skipCurrentExecutor)
                return this;

            _conditionalQueueBuilder.AddFuncIfCondition(condition);
            _executeIf = true;
            return this;
        }

        IAsyncSequentialExecutorFailBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.IfFail()
        {
            return this;
        }

        IAsyncSequentialExecutorOkBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.IfOk()
        {
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.Label(
            string label)
        {
            Label(label);
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.Restricted(
            int initialCount,
            string key)
        {
            Restricted(initialCount, key);
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.Restricted(
            bool condition,
            int initialCount,
            string key)
        {
            Restricted(condition, initialCount, key);
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.StopWatch()
        {
            StopWatch();
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.StopWatch(
            bool condition)
        {
            StopWatch(condition);
            return this;
        }

        #endregion

        #region IAsyncSequentialDefaultExecutorBuilder<M, R>

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialDefaultExecutorBuilder<M, R>.Retry(
            int count, int timeOutMilliseconds)
        {
            Retry(count, timeOutMilliseconds);
            return this;
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialDefaultExecutorBuilder<M, R>.Retry(
            bool condition, int count, int timeOutMilliseconds)
        {
            Retry(condition, count, timeOutMilliseconds);
            return this;
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialDefaultExecutorBuilder<M, R>.StopWatch()
        {
            StopWatch();
            return this;
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialDefaultExecutorBuilder<M, R>.StopWatch(
            bool condition)
        {
            StopWatch(condition);
            return this;
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialDefaultExecutorBuilder<M, R>.Restricted(
            int initialCount, 
            string key)
        {
            Restricted(initialCount, key);
            return this;
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialDefaultExecutorBuilder<M, R>.Restricted(
            bool condition, 
            int initialCount, 
            string key)
        {
            Restricted(condition, initialCount, key);
            return this;
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialDefaultExecutorBuilder<M, R>.Label(
            string label)
        {
            Label(label);
            return this;
        }

        IAsynPipeBuilder<M, R> IAsyncSequentialDefaultExecutorBuilder<M, R>.Add()
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
            Func<IAsyncExecutor<M, R>> executor,
            bool isDefault = false)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncExecutorContainer<M, R>(executor);
            _currentExecutorSettings = isDefault
                 ? new ExecutorSettings<M, R> { Label = "Default" }
                 : new ExecutorSettings<M, R>();
        }
        private void SetExecutor(
            Func<M, Task<PipeResult<R>>> executor,
            bool isDefault = false)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncContainer<M, R>(executor);
            _currentExecutorSettings = isDefault
                ? new ExecutorSettings<M, R> { Label = "Default" }
                : new ExecutorSettings<M, R>();
        }
        private void SetExecutor(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor,
            bool isDefault = false)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncCacheContainer<M, R>(executor);
            _currentExecutorSettings = isDefault
                ? new ExecutorSettings<M, R> { Label = "Default" }
                : new ExecutorSettings<M, R>();
        }
        private void SetExecutor(
            Func<M, PipeResult<R>> executor,
            bool isDefault = false)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncContainer<M, R>(executor);
            _currentExecutorSettings = isDefault
                 ? new ExecutorSettings<M, R> { Label = "Default" }
                 : new ExecutorSettings<M, R>();
        }
        private void SetExecutor(
            Func<M, IPipeCache, PipeResult<R>> executor,
            bool isDefault = false)
        {
            _skipCurrentExecutor = false;
            _currentContainer = new AsyncFuncCacheContainer<M, R>(executor);
            _currentExecutorSettings = isDefault
                 ? new ExecutorSettings<M, R> { Label = "Default" }
                 : new ExecutorSettings<M, R>();
        }

        private void Retry(
            int count, int timeOutMilliseconds)
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

        private AsyncSequentialPipeBuilder<M, R> Add(
            bool defaultExecutor = false)
        {
            if (_skipCurrentExecutor)
                return _asyncPipeBuilder;

            AddExecutor(defaultExecutor);

            return _asyncPipeBuilder;
        }
        #endregion
    }
}
