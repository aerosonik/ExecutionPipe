using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    public interface IExecutor<M,R> : IBaseExecutor<M, R>
    {
        ILocalCache LocalCache { get; set; }
    }
}
