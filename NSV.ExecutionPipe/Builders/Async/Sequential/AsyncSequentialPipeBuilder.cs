using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using NSV.ExecutionPipe.Pipes.Async.Sequential;

namespace NSV.ExecutionPipe.Builders.Async.Sequential
{
    internal class AsyncSequentialPipeBuilder<M, R> :  
        AsynPipeBuilder<M, R>,
        IAsyncSequentialPipeBuilder<M, R>
    {
        private readonly AsyncSequentialContainerBuilder<M, R> _containerBuilder;
        private readonly IServiceProvider _provider;
        internal AsyncSequentialPipeBuilder()
        {
            _pipe = new AsyncSequentialPipe<M, R>();
            _containerBuilder = new AsyncSequentialContainerBuilder<M, R>(this, _queueBuilder);
        }
        internal AsyncSequentialPipeBuilder(IServiceProvider provider): this()
        {
            _provider = provider;
        }

        #region Executor

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor<TAsyncExecutor>()
        {
            if (_provider == null)
                throw new ArgumentNullException("Service provider is NULL, required executor can't be resolved ");

            var executor = _provider.GetRequiredService<TAsyncExecutor>();
            if(executor == null)
                throw new ArgumentNullException("Required executor wasn't registered ");

            return _containerBuilder.Executor(() => executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            IAsyncExecutor<M, R> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(() => executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            IAsyncExecutor<M, R> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(() => executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            Func<IAsyncExecutor<M, R>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            Func<IAsyncExecutor<M, R>> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            Func<M, Task<PipeResult<R>>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            Func<M, Task<PipeResult<R>>> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            Func<M, PipeResult<R>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            Func<M, PipeResult<R>> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Executor(
            Func<M, IPipeCache, PipeResult<R>> executor, 
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.ExecutorSkip();

            return _containerBuilder.Executor(executor);
        }

        #endregion

        #region Default

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            IAsyncExecutor<M, R> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(() => executor);
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            IAsyncExecutor<M, R> executor,
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(() => executor);
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            Func<IAsyncExecutor<M, R>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(executor);
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            Func<IAsyncExecutor<M, R>> executor,
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(executor);
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            Func<M, Task<PipeResult<R>>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(executor);
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            Func<M, Task<PipeResult<R>>> executor,
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(executor);
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(executor);
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            Func<M, IPipeCache, Task<PipeResult<R>>> executor,
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(executor);
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            Func<M, PipeResult<R>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(executor);
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            Func<M, PipeResult<R>> executor,
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(executor);
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            Func<M, IPipeCache, PipeResult<R>> executor)
        {
            if (!_queueBuilder.IfConstantConditions())
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(executor);
        }

        IAsyncSequentialDefaultExecutorBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Default(
            Func<M, IPipeCache, PipeResult<R>> executor,
            bool addif)
        {
            if (!_queueBuilder.IfConstantConditions() || !addif)
                return _containerBuilder.DefaultSkip();

            return _containerBuilder.Default(executor);
        }

        #endregion

        IAsyncSequentialPipeBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.Cache(
            bool threadSafe)
        {
            if (threadSafe)
                _pipe.SetCache(PipeCacheObject.GetThreadSafeCache());
            else
                _pipe.SetCache(PipeCacheObject.GetCache());

            return this;
        }

        IAsyncSequentialPipeBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.If(
            bool condition)
        {
            _queueBuilder.AddIfCondition(condition);
            return this; 
        }

        IAsyncSequentialPipeBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.If(
            Func<M, bool> condition)
        {
            _queueBuilder.AddFuncIfCondition(condition);
            return this;
        }

        IAsyncSequentialPipeBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.EndIf()
        {
            _queueBuilder.RemoveIfCondition();
            return this;
        }

        IAsyncSequentialPipeBuilder<M, R> IAsyncSequentialPipeBuilder<M, R>.StopWatch(
            bool use)
        {
            if(use)
                _pipe.SetStopWatch();

            return this;
        }

        IAsyncPipe<M, R> IAsyncSequentialPipeBuilder<M, R>.Return(
            Func<M, PipeResult<R>[], PipeResult<R>> handler)
        {
            return Return(handler);
        }
    }
}
