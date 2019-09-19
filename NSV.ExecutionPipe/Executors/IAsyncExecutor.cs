using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    public interface IAsyncExecutor<M, R>: ICacheContainer
    {
        Task<PipeResult<R>> ExecuteAsync(M model);
    }

    public interface IAsyncExecutor<M> : ICacheContainer
    {
        Task<PipeResult> ExecuteAsync(M model);
    }

    public interface IAsyncExecutor : ICacheContainer
    {
        Task<PipeResult> ExecuteAsync();
    }
}
