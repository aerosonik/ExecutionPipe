using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Containers.Async
{
    public interface IAsyncContainer<M,R>
    {
        Task<PipeResult<R>> RunAsync(M model, IPipeCache cache);
    }

    public interface IAsyncContainer<M>
    {
        Task<PipeResult> RunAsync(M model, IPipeCache cache);
    }

    public interface IAsyncContainer
    {
        Task<PipeResult> RunAsync(IPipeCache cache);
    }
}
