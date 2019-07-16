using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe
{
    public interface IAsyncExecutor<M, R>
    {
        Task<PipeResult<R>> ExecuteAsync(M model, ILocalCache cache = null);
    }
    public interface ISyncExecutor<M, R>
    {
        PipeResult<R> ExecuteSync(M model, ILocalCache cache = null);
    }

    #region SyncSequentialPipe<M, R>
    public interface ISyncSequentialPipe<M, R> : ISyncExecutor<M, R>
    {
        PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);
        ISyncSequentialPipe<M, R> UseStopWatch();
        ISyncSequentialPipe<M, R> UseLocalCacheThreadSafe();
        ISyncSequentialPipe<M, R> UseLocalCache();
        ISyncSequentialExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor);
        ISyncSequentialExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor, bool addif);
        ISyncSequentialExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor);
        ISyncSequentialExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor, bool addif);
        ISyncSequentialPipe<M, R> If(bool condition);
        ISyncSequentialPipe<M, R> If(Func<M, bool> condition);
        ISyncSequentialPipe<M, R> EndIf();
        ISyncSequentialPipe<M, R> Build();
    }
    public interface ISyncSequentialExecutorBuilder<M, R>
    {
        ISyncSequentialExecutorBuilder<M, R> BreakIfFailed();
        ISyncSequentialExecutorBuilder<M, R> AllowBreak();
        ISyncSequentialExecutorBuilder<M, R> UseStopWatch();
        ISyncSequentialExecutorBuilder<M, R> Label(string label);
        ISyncSequentialExecutorBuilder<M, R> RetryIfFailed(int count, int timeOutMilliseconds);
        ISyncSequentialExecutorBuilder<M, R> ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
        ISyncSequentialPipe<M, R> Build();
    }
    #endregion

    #region AsyncSequentialPipe<M, R>
    public interface IAsyncSequentialPipe<M, R> : IAsyncExecutor<M, R>
    {
        PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);
        IAsyncSequentialPipe<M, R> UseStopWatch();
        IAsyncSequentialPipe<M, R> UseLocalCacheThreadSafe();
        IAsyncSequentialPipe<M, R> UseLocalCache();
        IAsyncSequentialExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor);
        IAsyncSequentialExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor, bool addif);
        IAsyncSequentialExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor);
        IAsyncSequentialExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor, bool addif);
        IAsyncSequentialPipe<M, R> If(bool condition);
        IAsyncSequentialPipe<M, R> If(Func<M, bool> condition);
        IAsyncSequentialPipe<M, R> EndIf();
        IAsyncSequentialPipe<M, R> Build();
    }
    public interface IAsyncSequentialExecutorBuilder<M, R>
    {
        IAsyncSequentialExecutorBuilder<M, R> BreakIfFailed();
        IAsyncSequentialExecutorBuilder<M, R> AllowBreak();
        IAsyncSequentialExecutorBuilder<M, R> UseStopWatch();
        IAsyncSequentialExecutorBuilder<M, R> Label(string label);
        IAsyncSequentialExecutorBuilder<M, R> RetryIfFailed(int count, int timeOutMilliseconds);
        IAsyncSequentialExecutorBuilder<M, R> ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
        IAsyncSequentialPipe<M, R> Build();
    }

    #endregion

    #region SyncParallelPipe<M, R>
    public interface ISyncParallelPipe<M, R> : ISyncExecutor<M,R>
    {
        PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);
        ISyncParallelPipe<M, R> UseStopWatch();
        ISyncParallelPipe<M, R> UseLocalCacheThreadSafe();
        ISyncParallelExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor);
        ISyncParallelExecutorBuilder<M, R> Add(ISyncExecutor<M, R> executor, bool addif);
        ISyncParallelExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor);
        ISyncParallelExecutorBuilder<M, R> Add(Func<ISyncExecutor<M, R>> executor, bool addif);
        ISyncParallelPipe<M, R> If(bool condition);
        ISyncParallelPipe<M, R> If(Func<M, bool> condition);
        ISyncParallelPipe<M, R> EndIf();
        ISyncParallelPipe<M, R> Build();
    }
    public interface ISyncParallelExecutorBuilder<M, R>
    {
        ISyncParallelExecutorBuilder<M, R> UseStopWatch();
        ISyncParallelExecutorBuilder<M, R> Label(string label);
        ISyncParallelExecutorBuilder<M, R> ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
        ISyncParallelPipe<M, R> Build();
    }
    #endregion

    #region AsyncParallelPipe<M, R>
    public interface IAsyncParallelPipe<M, R> : IAsyncExecutor<M,R>
    {
        PipeResult<R> CreatePipeResult(M model, PipeResult<R>[] results);
        IAsyncParallelPipe<M, R> UseStopWatch();
        IAsyncParallelPipe<M, R> UseLocalCacheThreadSafe();
        IAsyncParallelExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor);
        IAsyncParallelExecutorBuilder<M, R> Add(IAsyncExecutor<M, R> executor, bool addif);
        IAsyncParallelExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor);
        IAsyncParallelExecutorBuilder<M, R> Add(Func<IAsyncExecutor<M, R>> executor, bool addif);
        IAsyncParallelPipe<M, R> If(bool condition);
        IAsyncParallelPipe<M, R> If(Func<M, bool> condition);
        IAsyncParallelPipe<M, R> EndIf();
        IAsyncParallelPipe<M, R> Build();
    }
    public interface IAsyncParallelExecutorBuilder<M, R>
    {
        IAsyncParallelExecutorBuilder<M, R> UseStopWatch();
        IAsyncParallelExecutorBuilder<M, R> Label(string label);
        IAsyncParallelExecutorBuilder<M, R> ResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler);
        IAsyncParallelPipe<M, R> Build();
    }

    #endregion
}
