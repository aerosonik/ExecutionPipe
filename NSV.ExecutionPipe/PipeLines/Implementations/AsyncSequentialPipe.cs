using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.PipeLines.Interfaces;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
    public abstract class AsyncSequentialPipe<M, R> :
        AbstractAsyncPipe<M, R>,
        IAsyncSequentialPipe<M, R>,
        IAsyncSequentialExecutorBuilder<M, R>
    {
        #region IAsyncSequentialPipe<M, R>
        public IAsyncSequentialExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor)
        {
            AddSequentialExecutor(executor);
            return this;
        }

        public IAsyncSequentialExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor, bool addif)
        {
            AddSequentialExecutor(executor, addif);
            return this;
        }

        public IAsyncSequentialExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor)
        {
            AddSequentialExecutor(executor);
            return this;
        }

        public IAsyncSequentialExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor, bool addif)
        {
            AddSequentialExecutor(executor, addif);
            return this;
        }

        public IAsyncSequentialPipe<M, R> Build()
        {
            _finished = true;
            return this;
        }

        public abstract PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);

        public IAsyncSequentialPipe<M, R> EndIf()
        {
            EndIfCondition();
            return this;
        }

        public Task<PipeResult<R>> ExecuteAsync(M model, ILocalCache cache = null)
        {
            return RunAsync(model, cache, RunPipeSequentialAsync);
        }

        public IAsyncSequentialPipe<M, R> If(bool condition)
        {
            IfCondition(condition);
            return this;
        }

        public IAsyncSequentialPipe<M, R> If(Func<M, bool> condition)
        {
            IfCondition(condition);
            return this;
        }

        public IAsyncSequentialPipe<M, R> UseLocalCache()
        {
            LocalCache();
            return this;
        }

        public IAsyncSequentialPipe<M, R> UseLocalCacheThreadSafe()
        {
            LocalCacheThreadSafe();
            return this;
        }

        public IAsyncSequentialPipe<M, R> UseStopWatch()
        {
            _stopWatch = new Stopwatch();
            return this;
        }
        #endregion

        #region IAsyncSequentialExecutorBuilder<M,R>
        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.AllowBreak()
        {
            _current.SetAllowBreak();
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.BreakIfFailed()
        {
            _current.SetBreakIfFailed();
            return this;
        }

        IAsyncSequentialPipe<M, R> IAsyncSequentialExecutorBuilder<M, R>.Build()
        {
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.Label(string label)
        {
            _current.SetLabel(label);
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            _current.SetResultHandler(handler);
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.RetryIfFailed(int count, int timeOutMilliseconds)
        {
            _current.SetRetryIfFailed(count, timeOutMilliseconds);
            return this;
        }

        IAsyncSequentialExecutorBuilder<M, R> IAsyncSequentialExecutorBuilder<M, R>.UseStopWatch()
        {
            _current.SetUseStopWatch();
            return this;
        }
        #endregion

        #region Private
        private async Task<PipeResult<R>> RunPipeSequentialAsync()
        {
            while (_executionQueue.Count > 0)
            {
                var container = _executionQueue.Dequeue();
                if (!RunExecutorIfConditions(container, _model))
                    continue;

                var result = await container.RunAsync(_model);

                if (container.Retry.HasValue && result.Success == ExecutionResult.Failed)
                {
                    for (int i = 0; i < container.Retry.Value.Count; i++)
                    {
                        result = await container.RunAsync(_model);
                        if (result.Success == ExecutionResult.Successful)
                            break;
                    }
                }
                _results.Value.Add(result);
                if (Break(container, result))
                {
                    if (container.CreateResult.HasValue)
                        return container.CreateResult.Value(_model, result);
                    break;
                }
            }
            if (_results.HasValue)
                return CreatePipeResult(_model, _results.Value.ToArray());

            return CreatePipeResult(_model, null);
        }
        #endregion
    }
}
