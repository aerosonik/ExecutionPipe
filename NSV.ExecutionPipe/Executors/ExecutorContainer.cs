using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
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
        internal Func<M, PipeResult<R>, PipeResult<R>> CreateResult { get; set; }
        internal Optional<RetryModel> Retry { get; set; } = Optional<RetryModel>.Default;
        internal Optional<Func<M, bool>[]> ExecuteConditions { get; set; }
        internal bool IsAsync { get { return Executor.IsAsync; } }
        internal Executor<M, R> Executor
        {
            get
            {
                if (_executor == null)
                {
                    _executor= _lazyExecutor.Value;
                    _executor.LocalCache = LocalCache;
                    _executor.Label = Label;
                }
                return _executor;
            }
        }
        private Lazy<Executor<M, R>> _lazyExecutor;
        private Executor<M, R> _executor;
        internal ExecutorContainer(Executor<M, R> executor)
        {
            _lazyExecutor = new Lazy<Executor<M, R>>(() => executor );
        }
        internal ExecutorContainer(Lazy<Executor<M, R>> executor)
        {
            _lazyExecutor = executor;
        }
    }
}
