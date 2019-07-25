using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
    internal abstract class ExecutorContainerSettings<M, R> :
        IExecutorContainerSettings<M, R>
    {
        public ILocalCache Cache { get; set; }
        public bool BreakIfFailed { get; set; }
        public bool AllowBreak { get; set; }
        public string Label { get; set; } = string.Empty;
        public bool UseStopWatch { get; set; }
        public Optional<Func<M, PipeResult<R>, PipeResult<R>>> CreateResult { get; set; }
            = Optional<Func<M, PipeResult<R>, PipeResult<R>>>.Default;
        public Optional<RetryModel> Retry { get; set; } = Optional<RetryModel>.Default;
        public Optional<Func<M, bool>[]> ExecuteConditions { get; set; }
        public bool UseRestrictedMode { get; set; }
        public int RestrictedModeMaxThreads { get; set; }
    }

    internal class SyncExecutorContainerSettings<M, R> : 
        ExecutorContainerSettings<M, R>,
        ISyncExecutorContainerSettings<M,R>
    {
        public Func<ISyncExecutor<M, R>> Executor { get; set; }
    }
    internal class AsyncExecutorContainerSettings<M, R> :
        ExecutorContainerSettings<M, R>,
        IAsyncExecutorContainerSettings<M, R>
    {
        public Func<IAsyncExecutor<M, R>> Executor { get; set; }
    }

    internal interface IExecutorContainerSettings<M, R>
    {
        ILocalCache Cache { get; set; }
        bool BreakIfFailed { get; set; }
        bool AllowBreak { get; set; }
        string Label { get; set; }
        bool UseStopWatch { get; set; }
        bool UseRestrictedMode { get; set; }
        int RestrictedModeMaxThreads { get; set; }
        Optional<Func<M, PipeResult<R>, PipeResult<R>>> CreateResult { get; set; }
        Optional<RetryModel> Retry { get; set; }
        Optional<Func<M, bool>[]> ExecuteConditions { get; set; }
    }

    internal interface ISyncExecutorContainerSettings<M, R> : 
        IExecutorContainerSettings<M, R>
    {
        Func<ISyncExecutor<M, R>> Executor { get; set; }
    }

    internal interface IAsyncExecutorContainerSettings<M, R> :
        IExecutorContainerSettings<M, R>
    {
        Func<IAsyncExecutor<M, R>> Executor { get; set; }
    }

}
