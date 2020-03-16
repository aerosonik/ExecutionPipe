using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NSV.ExecutionPipe.Pipes;

namespace NSV.ExecutionPipe.Pool
{
    public static class PoolExtensions
    {
        public static IServiceCollection AddExecutionPool<M,R>(
            this IServiceCollection services,
            Func<IAsyncPipe<M, R>> fabric,
            int initialCount,
            int maxCount,
            int increaseRatio = 2)
        {
            var pool = new AsyncExecutionPool<M, R>(
                fabric, 
                initialCount, 
                maxCount, 
                increaseRatio);
            services.AddSingleton<IAsyncExecutionPool<M, R>>(pool);
            return services;
        }
    }
}
