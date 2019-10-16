using NSV.ExecutionPipe.Models;
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

            return _returnHandler(model, await Task.WhenAll(results));
        }
    }
}
