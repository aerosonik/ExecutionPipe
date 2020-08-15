using System;
using Microsoft.Extensions.DependencyInjection;
using NSV.ExecutionPipe.Builders;
using NSV.ExecutionPipe.Pipes;

namespace NSV.ExecutionPipe.Pool
{
    public static class PoolExtensions
    {
        public static IServiceCollection AddAsyncExecutionPool<M, R>(
            this IServiceCollection services,
            Func<IPipeBuilder<M, R>, IAsyncPipe<M, R>> factory,
            int initialCount,
            int maxCount,
            int increaseRatio = 2)
        {
            services.AddSingleton<IPipeBuilder<M, R>>(provider =>
                 new InternalPipeBuilder<M, R>(provider));

            services.AddSingleton<IAsyncExecutionPool<M, R>>(provider => 
                new AsyncExecutionPool<M,R>(
                    provider.GetRequiredService<IPipeBuilder<M, R>>(),
                    factory,
                    initialCount,
                    maxCount,
                    increaseRatio));

            return services;
        }
    }
}
