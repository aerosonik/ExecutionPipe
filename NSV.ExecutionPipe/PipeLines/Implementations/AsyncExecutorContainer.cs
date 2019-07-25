using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
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
}
