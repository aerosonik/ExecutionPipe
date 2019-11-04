using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Containers;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;

namespace NSV.ExecutionPipe.Pipes.Async
{
    internal abstract class AsyncPipe<M, R> : 
        PipeSettings<M,R>,
        IAsyncPipe<M, R>, 
        IAsyncExecutor<M, R>
    {
        #region IAsyncPipe<M, R>
        async Task<PipeResult<R>> IAsyncPipe<M, R>.ExecuteAsync(M model)
        {
            if (!_useStopWatch)
                return await RunAsync(model);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = await RunAsync(model);
            stopWatch.Stop();
            result.SetElapsed(stopWatch.Elapsed);
            return result;
        }

        IAsyncExecutor<M, R> IAsyncPipe<M, R>.ToExecutor()
        {
            return this;
        }
        #endregion

        #region IAsyncExecutor<M, R>
        async Task<PipeResult<R>> IAsyncExecutor<M, R>.ExecuteAsync(
            M model,
            IPipeCache pipeCache)
        {
            Cache = pipeCache;
            return await RunAsync(model);
        }

        #endregion

        protected abstract Task<PipeResult<R>> RunAsync(M model);

        protected bool IfConditions(ExecutorSettings<M, R> settings, M model)
        {
            return settings.ExecuteConditions.HasValue
                ? settings.ExecuteConditions.Value.All(x => x(model))
                : true;
        }

        protected bool IfBreak(
           ExecutorSettings<M, R> settings,
           PipeResult<R> result)
        {
            if (settings.FailedBreak &&
                result.Success == ExecutionResult.Failed &&
                result.Break)
                return true;

            if (settings.OkBreak &&
                result.Success == ExecutionResult.Successful &&
                result.Break)
                return true;

            return false;
        }

    }
}
