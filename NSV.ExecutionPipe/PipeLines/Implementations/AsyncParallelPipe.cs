using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.PipeLines.Interfaces;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
    public abstract class AsyncParallelPipe<M, R> :
        AbstractAsyncPipe<M, R>,
        IAsyncParallelPipe<M, R>,
        IAsyncParallelExecutorBuilder<M, R>
    {
        #region IAsyncParallelPipe<M, R>
        public IAsyncParallelExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor)
        {
            AddParallelExecutor(executor);
            return this;
        }

        public IAsyncParallelExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor, bool addif)
        {
            AddParallelExecutor(executor, addif);
            return this;
        }

        public IAsyncParallelExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor)
        {
            AddParallelExecutor(executor);
            return this;
        }

        public IAsyncParallelExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor, bool addif)
        {
            AddParallelExecutor(executor, addif);
            return this;
        }

        public IAsyncParallelPipe<M, R> Build()
        {
            _finished = true;
            return this;
        }

        public abstract PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);

        public IAsyncParallelPipe<M, R> EndIf()
        {
            EndIfCondition();
            return this;
        }

        public Task<PipeResult<R>> ExecuteAsync(M model, ILocalCache cache = null)
        {
            return RunAsync(model, cache, RunPipeParallelAsync);
        }

        public IAsyncParallelPipe<M, R> If(bool condition)
        {
            IfCondition(condition);
            return this;
        }

        public IAsyncParallelPipe<M, R> If(Func<M, bool> condition)
        {
            IfCondition(condition);
            return this;
        }

        public IAsyncParallelPipe<M, R> UseLocalCacheThreadSafe()
        {
            LocalCacheThreadSafe();
            return this;
        }

        public IAsyncParallelPipe<M, R> UseStopWatch()
        {
            _stopWatch = new Stopwatch();
            return this;
        }

        #endregion

        #region IAsyncParallelExecutorBuilder<M, R>
        IAsyncParallelPipe<M, R> IAsyncParallelExecutorBuilder<M, R>.Build()
        {
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.Label(string label)
        {
            _current.SetLabel(label);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)
        {
            _current.SetResultHandler(handler);
            return this;
        }

        IAsyncParallelExecutorBuilder<M, R> IAsyncParallelExecutorBuilder<M, R>.UseStopWatch()
        {
            _current.SetUseStopWatch();
            return this;
        }
        #endregion

        #region Private

        private async Task<PipeResult<R>> RunPipeParallelAsync()
        {
            var results = _executionQueue
                .AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .Where(x => RunExecutorIfConditions(x, _model))
                .Select(async x =>
                {
                    var result = await x.RunAsync(_model);
                    if (x.Retry.HasValue && result.Success == ExecutionResult.Failed)
                    {
                        for (int i = 0; i < x.Retry.Value.Count; i++)
                        {
                            result = await x.RunAsync(_model);
                            if (result.Success == ExecutionResult.Successful)
                                break;
                        }
                    }
                    return result;
                })
                .ToArray();

            return CreatePipeResult(_model, await Task.WhenAll(results));
        }

        #endregion 
    }
}
