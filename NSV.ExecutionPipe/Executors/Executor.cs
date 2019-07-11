using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    public abstract class Executor<M, R> : IExecutor<M, R>
    {
        public ILocalCache LocalCache { get; set; }
        public bool IsAsync { get; set; } = true;
        public string Label { get; set; } = string.Empty;
        public abstract PipeResult<R> Execute(M model);
        public abstract Task<PipeResult<R>> ExecuteAsync(M model);
        internal PipeResult<R> Run(M model, bool useStopWatch)
        {
            if (!useStopWatch)
                return Execute(model).SetLabel(Label);

            var sw = new Stopwatch();
            sw.Start();
            var result = Execute(model);
            sw.Stop();
            return result.SetElapsed(sw.Elapsed).SetLabel(Label);
        }
        internal async Task<PipeResult<R>> RunAsync(M model, bool useStopWatch)
        {
            if (!useStopWatch)
                return (await ExecuteAsync(model).ConfigureAwait(false))
                    .SetLabel(Label);

            var sw = new Stopwatch();
            sw.Start();
            var result = await ExecuteAsync(model).ConfigureAwait(false);
            sw.Stop();
            return result.SetElapsed(sw.Elapsed).SetLabel(Label);
        }
    }
}
