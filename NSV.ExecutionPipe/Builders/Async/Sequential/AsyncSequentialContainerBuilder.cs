using NSV.ExecutionPipe.Builders.Async;
using NSV.ExecutionPipe.Builders.Async.Sequential;
using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Containers;
using NSV.ExecutionPipe.Containers.Async;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.PipeLines;
using System;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Builders
{
    internal class AsyncSequentialContainerBuilder<M, R> :
        IAsyncSequentialExecutorBuilder<M, R>,
        IAsyncSequentialExecutorOkBuilder<M, R>,
        IAsyncSequentialExecutorFailBuilder<M, R>
    {
        #region Private Fields
        private readonly IAsyncSequentialPipeBuilder<M, R> _asyncPipeBuilder;
        private ExecutorSettings<M, R> _currentExecutorSettings;
        private IAsyncContainer<M, R> _currentContainer;
        private bool _skipCurrentExecutor = false;
        private AsyncConditionalQueueBuilder<M, R> _conditionalQueueBuilder;
        #endregion

        #region C-tor
        internal AsyncSequentialContainerBuilder(
            AsyncSequentialPipeBuilder<M, R> asyncPipeBuilder,
            AsyncConditionalQueueBuilder<M, R> executeConditions)
        {
            _asyncPipeBuilder = asyncPipeBuilder;
            _conditionalQueueBuilder = executeConditions;
        }
        #endregion

        #region Set Executor
        public IAsyncSequentialExecutorBuilder<M, R> ExecutorSkip()
        {
            _skipCurrentExecutor = true;
            return this;
        }

        public IAsyncSequentialExecutorBuilder<M, R> Executor(
            Func<IAsyncExecutor<M, R>> executor)
        {
            _currentContainer = new AsyncExecutorContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
            return this;
        }
        public IAsyncSequentialExecutorBuilder<M, R> Executor(
            Func<M, Task<PipeResult<R>>> executor)
        {
            _currentContainer = new AsyncFuncContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
            return this;
        }
        public IAsyncSequentialExecutorBuilder<M, R> Executor(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            _currentContainer = new AsyncFuncCacheContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
            return this;
        }
        public IAsyncSequentialExecutorBuilder<M, R> Executor(
            Func<M, PipeResult<R>> executor)
        {
            _currentContainer = new AsyncFuncContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
            return this;
        }
        public IAsyncSequentialExecutorBuilder<M, R> Executor(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            _currentContainer = new AsyncFuncCacheContainer<M, R>(executor);
            _currentExecutorSettings = new ExecutorSettings<M, R>();
            return this;
        }
        #endregion

        #region IAsyncExecutorFailBuilder<M,R>
        IAsyncSequentialExecutorFailBuilder<M, R> IAsyncSequentialExecutorFailBuilder<M, R>.Break(bool condition)
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
            if (_skipCurrentExecutor)
                return this;

            _currentContainer = new AsyncReTryContainer<M, R>(_currentContainer, count, timeOutMilliseconds);
            return this;
        }

        IAsyncSequentialExecutorFailBuilder<M, R> IAsyncSequentialExecutorFailBuilder<M, R>.Retry(
            bool condition, 
            int count, 
            int timeOutMilliseconds)
        {
            if (_skipCurrentExecutor || !condition)
                return this;

            _currentContainer = new AsyncReTryContainer<M, R>(_currentContainer, count, timeOutMilliseconds);
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

        #region IAsyncExecutorOkBuilder<M,R>
        IAsyncSequentialExecutorOkBuilder<M, R> IAsyncSequentialExecutorOkBuilder<M, R>.Break(bool condition)
        {
            if (_skipCurrentExecutor)
                return this;

            _currentExecutorSettings.OkBreak = condition;
            return this;
        }

        IAsyncSequentialExecutorOkBuilder<M, R> IAsyncSequentialExecutorOkBuilder<M, R>.Return(Func<M, PipeResult<R>, PipeResult<R>> handler)
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

        #region IAsyncExecutorBuilder<M,R>
        IAsyncSequentialPipeBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.Add()
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

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.ExecuteIf(
            Func<M, bool> condition)
        {
            if (_skipCurrentExecutor)
                return this;

            _conditionalQueueBuilder.AddFuncIfCondition(condition);
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
            if (_skipCurrentExecutor)
                return this;

            _currentExecutorSettings.Label = label;
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.Restricted(
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

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.Restricted(
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

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.StopWatch()
        {
            if (_skipCurrentExecutor)
                return this;

            _currentContainer = new AsyncStopWatchContainer<M, R>(_currentContainer);
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.StopWatch(
            bool condition)
        {
            if (_skipCurrentExecutor || !condition)
                return this;

            _currentContainer = new AsyncStopWatchContainer<M, R>(_currentContainer);
            return this;
        }
        #endregion
    }
}
