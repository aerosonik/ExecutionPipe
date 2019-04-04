using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NSV.ExecutionPipe.Pipes;

namespace NSV.ExecutionPipe.Executors
{
    public abstract class PipeExecutor<M, R>
        : Executor<M, R>, IPipeExecutor<M, R>
    {
        public Func<M, bool> PipeExecutionCondition { get; set; }
        public IPipe<M, R> Pipe { get; set; }
    }
}
