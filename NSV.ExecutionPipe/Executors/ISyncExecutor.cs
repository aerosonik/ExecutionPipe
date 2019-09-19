using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;

namespace NSV.ExecutionPipe.Executors
{
    public interface ISyncExecutor<M, R> : ICacheContainer
    {
        PipeResult<R> ExecuteAsync(M model);
    }

    public interface ISyncExecutor<M> : ICacheContainer
    {
        PipeResult ExecuteAsync(M model);
    }

    public interface ISyncExecutor : ICacheContainer
    {
        PipeResult ExecuteAsync();
    }
}
