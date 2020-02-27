using NSV.ExecutionPipe.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Pipes.Async.Parallel
{
    internal class AsyncParallelPipe<M, R> : AsyncPipe<M, R>
    {
        protected override async Task<PipeResult<R>> RunAsync(M model)
        {
            var results = _queue
                  .AsParallel()
                  .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                  .Where(x => IfConditions(x.Settings, model))
                  .Select(async x =>
                  {
                      var result = await x.Container.RunAsync(model, Cache);
                      result.Label = x.Settings.Label;
                      return result;
                  });
            var parallelResults = await Task.WhenAll(results);
            if (DefaultExecutor.HasValue &&
                IfConditions(DefaultExecutor.Value.Settings, model))
            {
                Array.Resize(ref parallelResults, parallelResults.Length + 1);
                parallelResults[parallelResults.Length - 1] = await DefaultExecutor
                    .Value.Container.RunAsync(model, Cache);
            }

            return _returnHandler(model, parallelResults);
        }
    }
}
