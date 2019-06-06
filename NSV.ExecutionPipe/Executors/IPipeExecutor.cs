using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;

namespace NSV.ExecutionPipe.Executors
{
    public interface IPipeExecutor<M, R>
    {
        Func<M, bool> PipeExecutionCondition { get; set; }
        IPipe<M, R> Pipe { get; set; }
        PipeResult<R> SubPipeResult { get; set; }
    }
}
