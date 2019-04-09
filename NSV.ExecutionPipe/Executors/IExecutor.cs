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

        bool UseStopWatch { get; set; }

        Optional<M> Model { get; set; }

        Func<M, bool> SkipCondition { get; set; }

        PipeResult<R> Run();

        Task<PipeResult<R>> RunAsync();

        PipeResult<R> Execute();

        Task<PipeResult<R>> ExecuteAsync();

        Func<M, PipeResult<R>, PipeResult<R>> CreateResult { get; set; }
    }
}
