using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe
{
    internal class SyncExecutorContainer<M,R> : BaseExecutorContainer<M, R>
    {
        private Func<ISyncExecutor<M, R>> _executor;
        internal SyncExecutorContainer(ISyncExecutor<M, R> executor)
        {
            _executor = () => executor;
        }
        internal SyncExecutorContainer(Func<ISyncExecutor<M, R>> executor)
        {
            _executor = executor;
        }

        internal PipeResult<R> Run(M model)
        {
            if (!UseStopWatch)
                return _executor()
                    .ExecuteSync(model, LocalCache)
                    .SetLabel(Label);

            var sw = new Stopwatch();
            sw.Start();
            var result = _executor().ExecuteSync(model, LocalCache);
            sw.Stop();
            return result.SetElapsed(sw.Elapsed).SetLabel(Label);
        }
    }

    internal class AsyncExecutorContainer<M, R> : BaseExecutorContainer<M, R>
    {
        private Func<IAsyncExecutor<M, R>> _executor;
        internal AsyncExecutorContainer(IAsyncExecutor<M, R> executor)
        {
            _executor = () => executor;
        }
        internal AsyncExecutorContainer(Func<IAsyncExecutor<M, R>> executor)
        {
            _executor = executor;
        }

        internal async Task<PipeResult<R>> RunAsync(M model)
        {
            if (!UseStopWatch)
                return (await _executor()
                    .ExecuteAsync(model, LocalCache))
                    .SetLabel(Label);

            var sw = new Stopwatch();
            sw.Start();
            var result = await _executor()
                .ExecuteAsync(model, LocalCache);
            sw.Stop();
            return result.SetElapsed(sw.Elapsed).SetLabel(Label);
        }
    }

    internal abstract class BaseExecutorContainer<M, R>
    {
        internal ILocalCache LocalCache { get; set; }
        internal bool BreakIfFailed { get; set; }
        internal bool AllowBreak { get; set; }
        internal string Label { get; set; } = string.Empty;
        internal bool UseStopWatch { get; set; }
        internal Optional<Func<M, PipeResult<R>, PipeResult<R>>> CreateResult { get; set; }
            = Optional<Func<M, PipeResult<R>, PipeResult<R>>>.Default;
        internal Optional<RetryModel> Retry { get; set; } = Optional<RetryModel>.Default;
        internal Optional<Func<M, bool>[]> ExecuteConditions { get; set; }
    }

    internal static class BaseExecutorContainerExtentions
    {
        internal static void SetAllowBreak<M,R>(this BaseExecutorContainer<M, R> current, bool value = true)
        {
            current.AllowBreak = value;
        }
        internal static void SetBreakIfFailed<M, R>(this BaseExecutorContainer<M, R> current, bool value = true)
        {
            current.BreakIfFailed = value;
        }
        internal static void SetLabel<M, R>(this BaseExecutorContainer<M, R> current, string value)
        {
            current.Label = value;
        }
        internal static void SetUseStopWatch<M, R>(this BaseExecutorContainer<M, R> current, bool value = true)
        {
            current.UseStopWatch = value;
        }
        internal static void SetRetryIfFailed<M, R>(this BaseExecutorContainer<M, R> current, int count, int timeOutMilliseconds)
        {
            current.Retry = new RetryModel
            {
                Count = count,
                TimeOutMilliseconds = timeOutMilliseconds
            };
        }

        internal static void SetResultHandler<M, R>(this BaseExecutorContainer<M, R> current, Func<M, PipeResult<R>, PipeResult<R>> value)
        {
            current.CreateResult = value;
        }
    }
}
