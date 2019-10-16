using System;
using System.Threading.Tasks;
using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using NSV.ExecutionPipe.Pipes.Async.Parallel;

namespace NSV.ExecutionPipe.Builders.Async.Parallel
{
    internal class AsyncParallelPipeBuilder<M, R> : 
        IAsyncParallelPipeBuilder<M, R>
    {
        private readonly AsyncParallelContainerBuilder<M, R> _containerBuilder;
        private readonly AsyncConditionalQueueBuilder<M, R> _queueBuilder;
        private readonly IPipeSettings<M, R> _pipe;

        public AsyncParallelPipeBuilder()
        {
            _pipe = new AsyncParallelPipe<M, R>();
            _queueBuilder = new AsyncConditionalQueueBuilder<M, R>();
            _containerBuilder = new AsyncParallelContainerBuilder<M, R>(this, _queueBuilder);
        }

        #region Executor
        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            IAsyncExecutor<M, R> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(() => executor);
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            IAsyncExecutor<M, R> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(() => executor);
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            Func<IAsyncExecutor<M, R>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            Func<IAsyncExecutor<M, R>> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            Func<M, Task<PipeResult<R>>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            Func<M, Task<PipeResult<R>>> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            Func<M, PipeResult<R>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            Func<M, PipeResult<R>> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Executor(
            Func<M, IPipeCache, PipeResult<R>> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }
#endregion

        IAsyncParallelPipeBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.Cache(
            bool threadSafe)
        {
            if (threadSafe)
                _pipe.SetCache(PipeCacheObject.GetThreadSafeCache());
            else
                _pipe.SetCache(PipeCacheObject.GetCache());

            return this;
        }

        IAsyncParallelPipeBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.If(
            bool condition)
        {
            _queueBuilder.AddIfCondition(condition);
            return this;
        }

        IAsyncParallelPipeBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.If(
            Func<M, bool> condition)
        {
            _queueBuilder.AddFuncIfCondition(condition);
            return this;
        }

        IAsyncParallelPipeBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.EndIf()
        {
            _queueBuilder.RemoveIfCondition();
            return this;
        }

        IAsyncPipe<M, R> IAsyncParallelPipeBuilder<M, R>.Return(
            Func<M, PipeResult<R>[], PipeResult<R>> rersultHandler)
        {
            _pipe.SetExecutors(_queueBuilder.GetQueue());
            _pipe.SetReturn(rersultHandler);
            return (IAsyncPipe<M, R>)_pipe;
        }

        IAsyncParallelPipeBuilder<M, R> IAsyncParallelPipeBuilder<M, R>.StopWatch(
            bool use)
        {
            if (use)
                _pipe.SetStopWatch();

            return this;
        }
    }
}
