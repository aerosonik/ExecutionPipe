using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;

namespace NSV.ExecutionPipe.Executors
{
    public interface ISyncExecutor<M, R> 
    {
        PipeResult<R> ExecuteAsync(M model, IPipeCache pipeCache = null);
    }

    public interface ISyncExecutor<M>
    {
        PipeResult ExecuteAsync(M model, IPipeCache pipeCache = null);
    }

    public interface ISyncExecutor
    {
        PipeResult ExecuteAsync(IPipeCache pipeCache = null);
    }
}
