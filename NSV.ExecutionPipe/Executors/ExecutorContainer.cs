using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    internal class ExecutorContainer<M, R>
    {
        private ExecutorType _executorType = ExecutorType.Executor;
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
                return _executorType == ExecutorType.Executor
                    ? _lazyExecutor.Value.IsAsync
                    : _lazyPipe.Value.IsAsync;
            }
        }
        private Lazy<Executor<M, R>> _lazyExecutor;
        private Lazy<Pipe<M, R>> _lazyPipe;

        internal ExecutorContainer(Executor<M, R> executor)
        {
            _lazyExecutor = new Lazy<Executor<M, R>>(() => executor );
            _executorType = ExecutorType.Executor;
        }
        internal ExecutorContainer(Lazy<Executor<M, R>> executor)
        {
            _lazyExecutor = executor;
            _executorType = ExecutorType.Executor;
        }
        internal ExecutorContainer(Pipe<M, R> pipe)
        {
            _lazyPipe = new Lazy<Pipe<M, R>>(() => pipe);
            _executorType = ExecutorType.Pipe;
        }
        internal ExecutorContainer(Lazy<Pipe<M, R>> pipe)
        {
            _lazyPipe = pipe;
            _executorType = ExecutorType.Pipe;
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
                return (await executor.ExecuteAsync(model).ConfigureAwait(false))
                    .SetLabel(Label);

            var sw = new Stopwatch();
            sw.Start();
            var result = await executor.ExecuteAsync(model).ConfigureAwait(false);
            sw.Stop();
            return result.SetElapsed(sw.Elapsed).SetLabel(Label);
        }

        private IBaseExecutor<M, R> InitExecutor()
        {
            IBaseExecutor<M, R> executor = null;
            if (_executorType == ExecutorType.Executor)
                executor = _lazyExecutor.Value;
            else
                executor = _lazyPipe.Value;

            return executor.UseCache(LocalCache);
        }
    }

    internal enum ExecutorType
    {
        Executor,
        Pipe
    }
}
