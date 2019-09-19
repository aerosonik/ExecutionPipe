using NSV.ExecutionPipe.Models;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Containers
{
    public interface IAsyncContainer<M,R>
    {
        Task<PipeResult<R>> RunAsync(M model);
    }

    public interface IAsyncContainer<M>
    {
        Task<PipeResult> RunAsync(M model);
    }

    public interface IAsyncContainer
    {
        Task<PipeResult> RunAsync();
    }
}
