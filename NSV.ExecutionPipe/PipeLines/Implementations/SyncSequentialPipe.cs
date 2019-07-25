using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.PipeLines.Interfaces;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Diagnostics;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
    public abstract class SyncSequentialPipe<M, R> :
        AbstractSyncPipe<M, R>,
        ISyncSequentialPipe<M, R>,
        ISyncSequentialExecutorBuilder<M, R>
    {
        #region ISyncSequentialPipe<M, R>
        public ISyncSequentialExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor)
        {
            AddExecutor(executor);
            return this;
        }

        public ISyncSequentialExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor, bool addif)
        {
            AddExecutor(executor, addif);
            return this;
        }

        public ISyncSequentialExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor)
        {
            AddExecutor(executor);
            return this;
        }

        public ISyncSequentialExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor, bool addif)
        {
            AddExecutor(executor, addif);
            return this;
        }

        public ISyncSequentialPipe<M, R> Build()
        {
            _finished = true;
            return this;
        }

        public abstract PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);

        public PipeResult<R> ExecuteSync(M model, ILocalCache cache = null)
        {
            return RunSync(model, cache, RunPipeSequential);
        }

        public ISyncSequentialPipe<M, R> If(bool condition)
        {
            IfCondition(condition);
            return this;
        }

        public ISyncSequentialPipe<M, R> If(Func<M, bool> condition)
        {
            IfCondition(condition);
            return this;
        }
        public ISyncSequentialPipe<M, R> EndIf()
        {
            EndIfCondition();
            return this;
        }
        public ISyncSequentialPipe<M, R> UseLocalCache()
        {
            LocalCache();
            return this;
        }

        public ISyncSequentialPipe<M, R> UseLocalCacheThreadSafe()
        {
            LocalCacheThreadSafe();
            return this;
        }

        public ISyncSequentialPipe<M, R> UseStopWatch()
        {
            _stopWatch = new Stopwatch();
            return this;
        }

        #endregion

        #region ISyncSequentialExecutorBuilder<M, R>
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.AllowBreak()
        {
            _current.SetAllowBreak();
            return this;
        }
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.BreakIfFailed()
        {
            _current.SetBreakIfFailed();
            return this;
        }
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.Label(string label)
        {
            _current.SetLabel(label);
            return this;
        }
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            _current.SetResultHandler(handler);
            return this;
        }
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.RetryIfFailed(int count, int timeOutMilliseconds)
        {
            _current.SetRetryIfFailed(count, timeOutMilliseconds);
            return this;
        }
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.UseStopWatch()
        {
            //_current.SetUseStopWatch();
            _current = new SyncExecutorContainerStopWatch<M,R>(_current);
            return this;
        }
        ISyncSequentialExecutorBuilder<M, R> ISyncSequentialExecutorBuilder<M, R>.UseRestrictedExecution(int maxCount)
        {
            //_current.SetUseStopWatch();
            _current = new SyncExecutorContainerSemaforeSlim<M, R>(_current, maxCount);
            return this;
        }
        ISyncSequentialPipe<M, R> ISyncSequentialExecutorBuilder<M, R>.Build()
        {
            if (_current == null)
                throw new ArgumentException("Current ExecutorContainer is null");

            _executionQueue.Enqueue(_current);
            return this;
        }
        #endregion

        #region Private
        private PipeResult<R> RunPipeSequential()
        {
            while (_executionQueue.Count > 0)
            {
                var container = _executionQueue.Dequeue();
                if (!RunExecutorIfConditions(container, _model))
                    continue;

                var result = container.Run(_model);

                if (container.Retry.HasValue && result.Success == ExecutionResult.Failed)
                {
                    for (int i = 0; i < container.Retry.Value.Count; i++)
                    {
                        result = container.Run(_model);
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
