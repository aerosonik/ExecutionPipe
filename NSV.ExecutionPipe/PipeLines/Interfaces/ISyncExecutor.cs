using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;

namespace NSV.ExecutionPipe
{
    public interface ISyncExecutor<M, R>
    {
        PipeResult<R> ExecuteSync(M model, ILocalCache cache = null);
    }
}
