using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
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
}
