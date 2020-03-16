using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Pool
{
    public class ExecutionPoolFactory
    {
        public static IAsyncExecutionPool<M, R> CreateExecutionPool<M, R>(
           Func<IAsyncPipe<M, R>> factory,
           int initialCount,
           int maxCount,
           int increaseRatio = 2)
        {
           return new AsyncExecutionPool<M, R>(
                factory,
                initialCount,
                maxCount,
                increaseRatio);
        }
    }
}
