using NSV.ExecutionPipe.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Pipes.Async.Sequential
{
    internal class AsyncSequentialPipe<M, R> : AsyncPipe<M, R>
    {
        protected override async Task<PipeResult<R>> RunAsync(M model)
        {
            for (int i = 0; i < _queue.Length; i++)
            {
                var (settings, container) = _queue[i];
                if (!IfConditions(settings, model))
                    continue;

                var result = await container.RunAsync(model, Cache);
                result.Label = settings.Label;

                if (IfBreak(settings, result))
                {
                    if (result.Success == ExecutionResult.Successful &&
                        settings.OkReturn.HasValue)
                    {
                        _results.Add(settings.OkReturn.Value(model, result));
                        break;
                    }

                    if (result.Success == ExecutionResult.Failed &&
                        settings.FailedReturn.HasValue)
                    {
                        _results.Add(settings.FailedReturn.Value(model, result));
                        break;
                    }

                    _results.Add(result);
                    break;
                }
                _results.Add(result);
            }

            if (DefaultExecutor.HasValue &&
                IfConditions(DefaultExecutor.Value.Settings, model))
            {
                var defaultResult = await DefaultExecutor.Value
                    .Container.RunAsync(model, Cache);
                defaultResult.SetLabel(DefaultExecutor.Value.Settings.Label);

                _results.Add(defaultResult);
            }

            return _results.Any()
                 ? _returnHandler(model, _results.ToArray())
                 : _returnHandler(model, new PipeResult<R>[] { });
        }
    }
}
