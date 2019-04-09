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

        public Optional<M> Model { get; set; }

        public Func<M, PipeResult<R>, PipeResult<R>> CreateResult { get; set; }

        public Func<M, bool> SkipCondition { get; set; }

        public bool IsAsync { get; set; } = true;

        public string Label { get; set; } = string.Empty;

        public Optional<RetryModel> Retry { get; set; } = Optional<RetryModel>.Default;

        public bool UseStopWatch { get; set; }      

        public abstract PipeResult<R> Execute();

        public abstract Task<PipeResult<R>> ExecuteAsync();

        public PipeResult<R> Run()
        {
            if (!UseStopWatch)
                return Execute().SetLabel(Label);

            var sw = new Stopwatch();
            sw.Start();
            var result = Execute();
            sw.Stop();
            return result.SetElapsed(sw.Elapsed).SetLabel(Label);
        }

        public async Task<PipeResult<R>> RunAsync()
        {
            if (!UseStopWatch)
                return (await ExecuteAsync()).SetLabel(Label);

            var sw = new Stopwatch();
            sw.Start();
            var result = await ExecuteAsync();
            sw.Stop();
            return result.SetElapsed(sw.Elapsed).SetLabel(Label);
        }
    }
}
