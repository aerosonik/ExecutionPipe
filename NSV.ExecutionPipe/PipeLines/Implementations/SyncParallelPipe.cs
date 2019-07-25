using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.PipeLines.Interfaces;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Diagnostics;
using System.Linq;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
    public abstract class SyncParallelPipe<M, R> :
       AbstractSyncPipe<M, R>,
       ISyncParallelPipe<M, R>,
       ISyncParallelExecutorBuilder<M, R>
    {
        #region ISyncParallelPipe<M, R>
        public ISyncParallelExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor)
        {
            AddExecutor(executor);
            return this;
        }

        public ISyncParallelExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor, bool addif)
        {
            AddExecutor(executor, addif);
            return this;
        }

        public ISyncParallelExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor)
        {
            AddExecutor(executor);
            return this;
        }

        public ISyncParallelExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor, bool addif)
        {
            AddExecutor(executor, addif);
            return this;
        }

        public ISyncParallelPipe<M, R> Build()
        {
            _finished = true;
            return this;
        }

        public abstract PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);

        public PipeResult<R> ExecuteSync(M model, ILocalCache cache = null)
        {
            return RunSync(model, cache, RunPipeParallel);
        }

        public ISyncParallelPipe<M, R> If(bool condition)
        {
            IfCondition(condition);
            return this;
        }

        public ISyncParallelPipe<M, R> If(Func<M, bool> condition)
        {
            IfCondition(condition);
            return this;
        }
        public ISyncParallelPipe<M, R> EndIf()
        {
            EndIfCondition();
            return this;
        }

        public ISyncParallelPipe<M, R> UseLocalCacheThreadSafe()
        {
            LocalCacheThreadSafe();
            return this;
        }

        public ISyncParallelPipe<M, R> UseStopWatch()
        {
            _stopWatch = new Stopwatch();
            return this;
        }

        #endregion

        #region ISyncParallelExecutorBuilder<M,R>
        ISyncParallelPipe<M, R> ISyncParallelExecutorBuilder<M, R>.Build()
        {
            if (_current == null)
                throw new ArgumentException("Current ExecutorContainer is null");

            _executionQueue.Enqueue(_current);
            return this;
        }

        ISyncParallelExecutorBuilder<M, R> ISyncParallelExecutorBuilder<M, R>.Label(string label)
        {
            _current.SetLabel(label);
            return this;
        }

        ISyncParallelExecutorBuilder<M, R> ISyncParallelExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            _current.SetResultHandler(handler);
            return this;
        }

        ISyncParallelExecutorBuilder<M, R> ISyncParallelExecutorBuilder<M, R>.UseStopWatch()
        {
            _current.SetUseStopWatch();
            return this;
        }
        #endregion

        #region Private

        private PipeResult<R> RunPipeParallel()
        {
            var results = _executionQueue
                .AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .Where(x => RunExecutorIfConditions(x, _model))
                .Select(x =>
                {
                    var result = x.Run(_model);
                    if (x.Retry.HasValue && result.Success == ExecutionResult.Failed)
                    {
                        for (int i = 0; i < x.Retry.Value.Count; i++)
                        {
                            result = x.Run(_model);
                            if (result.Success == ExecutionResult.Successful)
                                break;
                        }
                    }
                    return result;
                })
                .ToArray();
            return CreatePipeResult(_model, results);
        }

        #endregion 
    }
}
