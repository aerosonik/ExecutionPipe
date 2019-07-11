using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    public interface IExecutor<M,R>
    {
        ILocalCache LocalCache { get; set; }

        bool IsAsync { get; set; }

        PipeResult<R> Execute(M model);

        Task<PipeResult<R>> ExecuteAsync(M model);
    }
}
