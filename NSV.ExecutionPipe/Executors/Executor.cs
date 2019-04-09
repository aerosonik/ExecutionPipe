using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    public abstract class Executor<M, R> : IExecutor<M, R>
    {
        public ILocalCache LocalCache { get; set; }

        public bool BreakIfFailed { get; set; }

        public bool AllowBreak { get; set; }

        public Func<M, PipeResult<R>, PipeResult<R>> CreateResult { get; set; }

        public Func<M, bool> SkipCondition { get; set; }

        public bool IsAsync { get; set; } = true;

        public string Label { get; set; } = string.Empty;

        public Optional<RetryModel> Retry { get; set; } = Optional<RetryModel>.Default;

        public bool UseStopWatch { get; set; }      

        public abstract PipeResult<R> Execute(M model);

        public abstract Task<PipeResult<R>> ExecuteAsync(M model);

        public PipeResult<R> Run(M model)
        {
            if (!UseStopWatch)
                return Execute(model).SetLabel(Label);

            var sw = new Stopwatch();
            sw.Start();
            var result = Execute(model);
            sw.Stop();
            return result.SetElapsed(sw.Elapsed).SetLabel(Label);
        }

        public async Task<PipeResult<R>> RunAsync(M model)
        {
            if (!UseStopWatch)
                return (await ExecuteAsync(model)).SetLabel(Label);

            var sw = new Stopwatch();
            sw.Start();
            var result = await ExecuteAsync(model);
            sw.Stop();
            return result.SetElapsed(sw.Elapsed).SetLabel(Label);
        }
    }
}
