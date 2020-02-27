using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;

namespace NSV.ExecutionPipe.Builders.Async
{
    public interface IAsynPipeBuilder<M, R>
    {
        IAsyncPipe<M, R> Return(Func<M, PipeResult<R>[], PipeResult<R>> rersultHandler);
    }

    public interface IAsynPipeBuilder<M>
    {
        IAsyncPipe<M> Return(Func<M, PipeResult[], PipeResult> rersultHandler);
    }

    public interface IAsynPipeBuilder
    {
        IAsyncPipe Return(Func<PipeResult[], PipeResult> rersultHandler);
    }
}
