using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    internal class ExecutorContainer<M, R>
    {
        internal ILocalCache LocalCache { get; set; }
        internal bool BreakIfFailed { get; set; }
        internal bool AllowBreak { get; set; }
        internal string Label { get; set; } = string.Empty;
        internal bool UseStopWatch { get; set; }
        internal Optional<Func<M, PipeResult<R>, PipeResult<R>>> CreateResult { get; set; } = Optional<Func<M, PipeResult<R>, PipeResult<R>>>.Default;
        internal Optional<RetryModel> Retry { get; set; } = Optional<RetryModel>.Default;
        internal Optional<Func<M, bool>[]> ExecuteConditions { get; set; }
        internal bool IsAsync
        {
            get
            {
                if (_executor == null)
                {
                    _executor = _executorFunc();
                }
                return _executor.IsAsync;
            }
        }
        private Func<IBaseExecutor<M, R>> _executorFunc;
        private IBaseExecutor<M, R> _executor;
        internal ExecutorContainer(IBaseExecutor<M, R> executor)
        {
            _executorFunc = () => executor;
        }
        internal ExecutorContainer(Func<IBaseExecutor<M, R>> executor)
        {
            _executorFunc = executor;
        }

        internal PipeResult<R> Run(M model)
        {
            var executor = InitExecutor();
            if (!UseStopWatch)
                return executor.Execute(model).SetLabel(Label);

            var sw = new Stopwatch();
            sw.Start();
            var result = executor.Execute(model);
            sw.Stop();
            return result.SetElapsed(sw.Elapsed).SetLabel(Label);
        }

        internal async Task<PipeResult<R>> RunAsync(M model)
        {
            var executor = InitExecutor();
            if (!UseStopWatch)
                return (await executor.ExecuteAsync(model)
                    .ConfigureAwait(false))
                    .SetLabel(Label);

            var sw = new Stopwatch();
            sw.Start();
            var result = await executor.ExecuteAsync(model)
                .ConfigureAwait(false);
            sw.Stop();
            return result.SetElapsed(sw.Elapsed).SetLabel(Label);
        }

        private IBaseExecutor<M, R> InitExecutor()
        {
            if (_executor == null)
                _executor = _executorFunc();

            return _executor.UseCache(LocalCache);
        }
    }
}
