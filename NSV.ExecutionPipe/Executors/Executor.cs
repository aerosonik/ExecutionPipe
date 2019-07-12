using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    public abstract class Executor<M, R> : IExecutor<M, R>
    {
        public ILocalCache LocalCache { get; set; }
        public bool IsAsync { get; set; } = true;
        public IBaseExecutor<M, R> UseCache(ILocalCache cache)
        {
            LocalCache = cache;
            return this;
        }
        public abstract PipeResult<R> Execute(M model);
        public abstract Task<PipeResult<R>> ExecuteAsync(M model);
    }
}
