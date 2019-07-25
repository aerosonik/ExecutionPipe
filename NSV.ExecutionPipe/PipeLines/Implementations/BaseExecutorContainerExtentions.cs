using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
    internal static class BaseExecutorContainerExtentions
    {
        internal static void SetAllowBreak<M, R>(this IBaseExecutorContainer<M, R> current, bool value = true)
        {
            current.AllowBreak = value;
        }
        internal static void SetBreakIfFailed<M, R>(this IBaseExecutorContainer<M, R> current, bool value = true)
        {
            current.BreakIfFailed = value;
        }
        internal static void SetLabel<M, R>(this IBaseExecutorContainer<M, R> current, string value)
        {
            current.Label = value;
        }
        //internal static void SetUseStopWatch<M, R>(this IBaseExecutorContainer<M, R> current, bool value = true)
        //{
        //    current.UseStopWatch = value;
        //}
        internal static void SetRetryIfFailed<M, R>(this IBaseExecutorContainer<M, R> current, int count, int timeOutMilliseconds)
        {
            current.Retry = new RetryModel
            {
                Count = count,
                TimeOutMilliseconds = timeOutMilliseconds
            };
        }
        internal static void SetResultHandler<M, R>(this IBaseExecutorContainer<M, R> current, Func<M, PipeResult<R>, PipeResult<R>> value)
        {
            current.CreateResult = value;
        }
    }
}
