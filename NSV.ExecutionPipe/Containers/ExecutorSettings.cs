using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.Containers
{
    public class ExecutorSettings<M,R> : BaseExecutorSettings
    {
        public Optional<Func<M, PipeResult<R>, PipeResult<R>>> FailedReturn { get; set; }
            = Optional<Func<M, PipeResult<R>, PipeResult<R>>>.Default;

        public Optional<Func<M, PipeResult<R>, PipeResult<R>>> OkReturn { get; set; }
           = Optional<Func<M, PipeResult<R>, PipeResult<R>>>.Default;

        public Optional<Func<M, bool>[]> ExecuteConditions { get; set; }
            = Optional<Func<M, bool>[]>.Default;
    }

    public class ExecutorSettings<M> : BaseExecutorSettings
    {
        public Optional<Func<M, PipeResult, PipeResult>> FailedReturn { get; set; }
            = Optional<Func<M, PipeResult, PipeResult>>.Default;

        public Optional<Func<M, PipeResult, PipeResult>> OkReturn { get; set; }
           = Optional<Func<M, PipeResult, PipeResult>>.Default;

        public Optional<Func<M, bool>[]> ExecuteConditions { get; set; }
            = Optional<Func<M, bool>[]>.Default;
    }

    public class ExecutorSettings : BaseExecutorSettings
    {
        public Optional<Func<PipeResult, PipeResult>> FailedReturn { get; set; }
            = Optional<Func<PipeResult, PipeResult>>.Default;

        public Optional<Func<PipeResult, PipeResult>> OkReturn { get; set; }
            = Optional<Func<PipeResult, PipeResult>>.Default;

        //public Optional<Func<M, bool>[]> ExecuteConditions { get; }
        //    = Optional<Func<M, bool>[]>.Default;
    }

    public abstract class BaseExecutorSettings
    {
        public string Label { get; set; }
        public bool FailedBreak { get; set; }
        public bool OkBreak { get; set; }
    }

}
