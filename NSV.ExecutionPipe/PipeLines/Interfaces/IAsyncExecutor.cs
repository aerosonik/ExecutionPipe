using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe
{
    public interface IAsyncExecutor<M, R>
    {
        Task<PipeResult<R>> ExecuteAsync(M model, ILocalCache cache = null);
    }
}
