using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.Containers
{
    public abstract class ExecutorSettings<M,R>
    {
        public ExecutorSettings(
            bool allowBreak,
            Func<M, PipeResult<R>, PipeResult<R>> returnFunc,
            Func<M, bool>[] executeConditions)
        {
            Break = allowBreak;
            Return = returnFunc;
            ExecuteConditions = executeConditions;
        }

        public bool Break { get; }

        public Optional<Func<M, PipeResult<R>, PipeResult<R>>> Return { get; }
            = Optional<Func<M, PipeResult<R>, PipeResult<R>>>.Default;

        public Optional<Func<M, bool>[]> ExecuteConditions { get;  }
    }
}
