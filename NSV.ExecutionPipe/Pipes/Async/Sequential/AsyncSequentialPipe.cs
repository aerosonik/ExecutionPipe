using NSV.ExecutionPipe.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Pipes.Async.Sequential
{
    internal class AsyncSequentialPipe<M, R> : AsyncPipe<M, R>
    {
        protected override async Task<PipeResult<R>> RunAsync(M model)
        {
            while (_queue.Count > 0)
            {
                var (settings, container) = _queue.Dequeue();
                if (!IfConditions(settings, model))
                    continue;

                var result = await container.RunAsync(model, Cache);
                result.Label = settings.Label;

                _results.Add(result);

                if (IfBreak(settings, result))
                {
                    if (result.Success == ExecutionResult.Successful &&
                        settings.OkReturn.HasValue)
                        return settings.OkReturn.Value(model, result);

                    if (result.Success == ExecutionResult.Failed &&
                        settings.FailedReturn.HasValue)
                        return settings.FailedReturn.Value(model, result);

                    break;
                }
            }

            return _results.Any()
                 ? _returnHandler(model, _results.ToArray())
                 : _returnHandler(model, new PipeResult<R>[] { });
        }
    }
}
