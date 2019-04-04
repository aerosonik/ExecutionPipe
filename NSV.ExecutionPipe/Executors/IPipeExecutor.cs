using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    public interface IPipeExecutor<M, R>
    {
        Func<M, bool> PipeExecutionCondition { get; set; }
        IPipe<M, R> Pipe { get; set; }
    }
}
