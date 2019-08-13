using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.PipeLines.Interfaces;

namespace NSV.ExecutionPipe
{
    public interface ISyncExecutor<M, R>: ICacheContainer
    {
        PipeResult<R> ExecuteSync(M model);
    }
}
