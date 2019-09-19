using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;

namespace NSV.ExecutionPipe.Containers
{
    public abstract class ContainerSettings//<M,R>
    {
        protected bool Break { get; set; } = false;

        //public Optional<Func<M, PipeResult<R>, PipeResult<R>>> Return { get; set; }

        //public Optional<Func<M, bool>[]> ExecuteConditions { get; set; }

        protected Optional<RetryModel> Retry { get; set; } 
            = Optional<RetryModel>.Default;

        protected string Label { get; set; } = string.Empty;

        protected bool StopWatch { get; set; } = false;

        protected Optional<SemaphoreModel> Semaphore { get; set; }
            = Optional<SemaphoreModel>.Default;

        protected IPipeCache Cache { get; set; }
    }
}
