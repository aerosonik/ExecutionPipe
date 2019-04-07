using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Executors
{
    public abstract class Executor<M, R> : IExecutor<M, R>
    {
        public ILocalCache LocalCache { get; set; }

        public bool BreakIfFailed { get; set; }

        public bool AllowBreak { get; set; }

        public Optional<M> Model { get; set; }

        public Func<M, PipeResult<R>, PipeResult<R>> CreateResult { get; set; }

        public Func<M, bool> SkipCondition { get; set; }

        public bool IsAsync { get; set; } = true;

        public abstract PipeResult<R> Execute();

        public abstract Task<PipeResult<R>> ExecuteAsync();

    }
}
