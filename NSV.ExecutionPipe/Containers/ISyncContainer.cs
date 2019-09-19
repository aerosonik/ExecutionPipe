using NSV.ExecutionPipe.Models;

namespace NSV.ExecutionPipe.Containers
{
    public interface ISyncContainer<M, R>
    {
        PipeResult<R> Run(M model);
    }

    public interface ISyncContainer<M>
    {
        PipeResult Run(M model);
    }

    public interface ISyncContainer
    {
        PipeResult Run();
    }
}
