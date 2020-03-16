using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Pipes
{
    public interface IAsyncPipe<M, R>
    {
        Task<PipeResult<R>> ExecuteAsync(M model);

        IAsyncExecutor<M, R> ToExecutor();       
    }

    public interface IAsyncPipe<M>
    {
        Task<PipeResult> ExecuteAsync(M model);
    }

    public interface IAsyncPipe
    {
        Task<PipeResult> ExecuteAsync();
    }
}
