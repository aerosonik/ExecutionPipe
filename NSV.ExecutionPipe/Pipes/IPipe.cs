using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Pipes
{
    public interface IPipe<M, R>
    {
        TimeSpan Elapsed { get; }
        PipeResult<R> Run();
        Task<PipeResult<R>> RunAsync();
        PipeResult<R> CreateResult(M model, PipeResult<R>[] results);
        IParallelPipe<M, R> AsParallel();
        ISequentialPipe<M, R> AsSequential();

        IPipe<M, R> UseStopWatch();
        IPipe<M, R> UseModel(M model = default);
        IPipe<M, R> UseLocalCacheThreadSafe();
        IPipe<M, R> UseLocalCache();
    }

    public interface ISequentialPipe<M,R>: IBasePipe<M, R>
    {
        ISequentialPipe<M, R> SetBreakIfFailed();
        ISequentialPipe<M, R> SetAllowBreak();
        ISequentialPipe<M, R> SetResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);

        ISequentialPipe<M, R> AddExecutor(IExecutor<M, R> executor);
        ISequentialPipe<M, R> SetModel(M model);
        ISequentialPipe<M, R> SetSkipIf(Func<M, bool> condition);
        ISequentialPipe<M, R> SetSubPipe(IPipe<M, R> pipe, Func<M, bool> condition);
        ISequentialPipe<M, R> SetUseStopWatch();
        ISequentialPipe<M, R> SetLabel(string label);
    }

    public interface IParallelPipe<M, R>: IBasePipe<M, R>
    {
        IParallelPipe<M, R> AddExecutor(IExecutor<M, R> executor);
        IParallelPipe<M, R> SetModel(M model);
        IParallelPipe<M, R> SetSkipIf(Func<M, bool> condition);
        IParallelPipe<M, R> SetSubPipe(IPipe<M, R> pipe, Func<M, bool> condition);
        IParallelPipe<M, R> SetUseStopWatch();
        IParallelPipe<M, R> SetLabel(string label);
    }

    public interface IBasePipe<M, R>
    {
        IPipe<M, R> Finish();
    }
}
