using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Pool
{
    public interface IAsyncExecutionPool<M,R>
    {
        public Task<PipeResult<R>> ExecuteAsync(M model);
    }
}
