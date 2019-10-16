using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    public interface IAsyncExecutor<M, R>
    {
        Task<PipeResult<R>> ExecuteAsync(M model, IPipeCache pipeCache = null);
    }

    public interface IAsyncExecutor<M>
    {
        Task<PipeResult> ExecuteAsync(M model, IPipeCache pipeCache = null);
    }

    public interface IAsyncExecutor
    {
        Task<PipeResult> ExecuteAsync(IPipeCache pipeCache = null);
    }
}
