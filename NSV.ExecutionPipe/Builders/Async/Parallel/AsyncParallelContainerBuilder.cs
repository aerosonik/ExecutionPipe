using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Builders.Async.Parallel
{
    internal class AsyncParallelContainerBuilder<M, R> :
        AsyncContainerBuilder<M, R>,
        IAsyncParallelExecutorBuilder<M, R>,
        IAsyncParallelExecutorFailBuilder<M, R>,
        IAsyncParallelDefaultExecutorBuilder<M, R>
    {
        #region Private Fields
        private readonly AsyncParallelPipeBuilder<M, R> _asyncPipeBuilder;
        #endregion

        #region C-tor
        internal AsyncParallelContainerBuilder(
            AsyncParallelPipeBuilder<M, R> asyncPipeBuilder,
            AsyncConditionalQueueBuilder<M, R> executeConditions)
            : base(executeConditions)
        {
            _asyncPipeBuilder = asyncPipeBuilder;
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
            SetExecutor(executor, true);
            return this;
        }
        public IAsyncParallelDefaultExecutorBuilder<M, R> Default(
            Func<M, Task<PipeResult<R>>> executor)
        {
            SetExecutor(executor, true);
            return this;
        }
        public IAsyncParallelDefaultExecutorBuilder<M, R> Default(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            SetExecutor(executor, true);
            return this;
        }
        public IAsyncParallelDefaultExecutorBuilder<M, R> Default(
            Func<M, PipeResult<R>> executor)
        {
            SetExecutor(executor, true);
            return this;
        }
        public IAsyncParallelDefaultExecutorBuilder<M, R> Default(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            SetExecutor(executor, true);
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

        private AsyncParallelPipeBuilder<M, R> Add(bool defaultExecutor = false)
        {
            if (_skipCurrentExecutor)
                return _asyncPipeBuilder;

            AddExecutor(defaultExecutor);

            return _asyncPipeBuilder;
        }

        #endregion
    }
}
