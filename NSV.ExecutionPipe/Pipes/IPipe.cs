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

    }

    public interface ISequentialPipe<M,R>: IBasePipe<M, R>
    {
        ISequentialPipe<M, R> SetBreakIfFailed(bool value = true);
        ISequentialPipe<M, R> SetAllowBreak(bool value = true);
        ISequentialPipe<M, R> SetResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
    }

    public interface IParallelPipe<M, R>: IBasePipe<M, R>
    {

    }

    public interface IBasePipe<M, R>
    {
        IBasePipe<M, R> AddExecutor(IExecutor<M, R> executor);
        IBasePipe<M, R> SetModel(M model);
        IBasePipe<M, R> SetSkipIf(Func<M, bool> condition);
        IBasePipe<M, R> SetSubPipe(IPipe<M, R> pipe, Func<M, bool> condition);

        IBasePipe<M, R> UseStopWatch();
        IBasePipe<M, R> UseModel(M model = default);
        IBasePipe<M, R> UseLocalCacheThreadSafe();
        IBasePipe<M, R> UseLocalCache();

        IPipe<M, R> Finish();
    }
 }
