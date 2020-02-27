using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders.Sync.Parallel
{
    public interface ISyncParallelExecutorBuilder<M, R>
    {
        ISyncParallelExecutorFailBuilder<M, R> IfFail();
        ISyncParallelExecutorOkBuilder<M, R> IfOk();
        ISyncParallelExecutorBuilder<M, R> StopWatch();
        ISyncParallelExecutorBuilder<M, R> StopWatch(bool condition);
        ISyncParallelExecutorBuilder<M, R> Restricted(int maxCount);
        ISyncParallelExecutorBuilder<M, R> Restricted(bool condition, int maxCount);
        ISyncParallelExecutorBuilder<M, R> Label(string label);
        ISyncParallelExecutorBuilder<M, R> ExecuteIf(Func<M, bool> condition);
        ISyncParallelPipeBuilder<M, R> Add();
    }

    public interface ISyncParallelExecutorBuilder<M>
    {
        ISyncParallelExecutorFailBuilder<M> IfFail();
        ISyncParallelExecutorOkBuilder<M> IfOk();
        ISyncParallelExecutorBuilder<M> StopWatch();
        ISyncParallelExecutorBuilder<M> StopWatch(bool condition);
        ISyncParallelExecutorBuilder<M> Restricted(int maxCount);
        ISyncParallelExecutorBuilder<M> Restricted(bool condition, int maxCount);
        ISyncParallelExecutorBuilder<M> Label(string label);
        ISyncParallelExecutorBuilder<M> ExecuteIf(Func<M, bool> condition);
        ISyncParallelPipeBuilder<M> Add();
    }

    public interface ISyncParallelExecutorBuilder
    {
        ISyncParallelExecutorFailBuilder IfFail();
        ISyncParallelExecutorOkBuilder IfOk();
        ISyncParallelExecutorBuilder StopWatch();
        ISyncParallelExecutorBuilder StopWatch(bool condition);
        ISyncParallelExecutorBuilder Restricted(int maxCount);
        ISyncParallelExecutorBuilder Restricted(bool condition, int maxCount);
        ISyncParallelExecutorBuilder Label(string label);
        ISyncParallelPipeBuilder Add();
    }
}
