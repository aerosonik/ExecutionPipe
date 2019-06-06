using System;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;

namespace NSV.ExecutionPipe.Executors
{
    public abstract class PipeExecutor<M, R>
        : Executor<M, R>, IPipeExecutor<M, R>
    {
        public Func<M, bool> PipeExecutionCondition { get; set; }

        public IPipe<M, R> Pipe { get; set; }

        public PipeResult<R> SubPipeResult { get; set; }
    }
}
