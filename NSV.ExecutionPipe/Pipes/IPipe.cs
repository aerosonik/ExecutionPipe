﻿using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;

namespace NSV.ExecutionPipe.Pipes
{
    public interface IPipe<M, R>: IBaseExecutor<M, R>
    {
        string Label { get; set; }
        TimeSpan Elapsed { get; }
        PipeResult<R> CreateResult(M model, PipeResult<R>[] results);
        IParallelPipe<M, R> AsParallel();
        ISequentialPipe<M, R> AsSequential();
        IPipe<M, R> UseStopWatch();
        IPipe<M, R> UseLocalCacheThreadSafe();
        IPipe<M, R> UseLocalCache();
        IPipe<M, R> UseParentalCache();
    }

    public interface ISequentialPipe<M,R>: IBasePipe<M, R>
    {
        ISequentialPipe<M, R> SetBreakIfFailed();
        ISequentialPipe<M, R> SetAllowBreak();
        ISequentialPipe<M, R> SetResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
        ISequentialPipe<M, R> AddExecutor(IBaseExecutor<M, R> executor);
        ISequentialPipe<M, R> AddExecutor(IBaseExecutor<M, R> executor, bool addif);
        ISequentialPipe<M, R> AddExecutor(Func<IBaseExecutor<M, R>> executor);
        ISequentialPipe<M, R> AddExecutor(Func<IBaseExecutor<M, R>> executor, bool addif);
        ISequentialPipe<M, R> SetDefault();
        ISequentialPipe<M, R> SetUseStopWatch();
        ISequentialPipe<M, R> SetLabel(string label);
        ISequentialPipe<M, R> SetRetryIfFailed(int count, int timeOutMilliseconds);
        ISequentialPipe<M, R> If(bool condition);
        ISequentialPipe<M, R> If(Func<M, bool> condition);
        ISequentialPipe<M, R> EndIf();

    }

    public interface IParallelPipe<M, R>: IBasePipe<M, R>
    {
        IParallelPipe<M, R> AddExecutor(IBaseExecutor<M, R> executor);
        IParallelPipe<M, R> AddExecutor(IBaseExecutor<M, R> executor, bool addif);
        IParallelPipe<M, R> AddExecutor(Func<IBaseExecutor<M, R>> executor);
        IParallelPipe<M, R> AddExecutor(Func<IBaseExecutor<M, R>> executor, bool addif);
        IParallelPipe<M, R> SetUseStopWatch();
        IParallelPipe<M, R> SetLabel(string label);
        IParallelPipe<M, R> If(bool condition);
        IParallelPipe<M, R> If(Func<M, bool> condition);
        IParallelPipe<M, R> EndIf();
    }

    public interface IBasePipe<M, R>
    {
        IPipe<M, R> Finish();
    }
}
