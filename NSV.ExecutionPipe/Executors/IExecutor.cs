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

        bool BreakIfFailed { get; set; }

        bool AllowBreak { get; set; }

        bool IsAsync { get; set; }

        string Label { get; set; }

        Optional<RetryModel> Retry { get; set; }

        bool UseStopWatch { get; set; }

        PipeResult<R> Run(M model);

        Task<PipeResult<R>> RunAsync(M model);

        PipeResult<R> Execute(M model);

        Task<PipeResult<R>> ExecuteAsync(M model);

        Func<M, PipeResult<R>, PipeResult<R>> CreateResult { get; set; }
    }
}
