using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    public interface IBaseExecutor<M,R>
    {
        bool IsAsync { get; set; }
        IBaseExecutor<M, R> UseCache(ILocalCache cache);
        PipeResult<R> Execute(M model);
        Task<PipeResult<R>> ExecuteAsync(M model);
    }
}
