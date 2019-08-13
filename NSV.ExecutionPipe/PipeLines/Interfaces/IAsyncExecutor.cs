using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.PipeLines.Interfaces;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe
{
    public interface IAsyncExecutor<M, R>: ICacheContainer
    {
        Task<PipeResult<R>> ExecuteAsync(M model);
    }
}
