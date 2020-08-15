using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders
{
    public static class PipeBuildersExtensions
    {
        public static IServiceCollection AddExecutionPipeBuilder<M,R>(
         this IServiceCollection services)
        {
            return services.AddSingleton<IPipeBuilder<M,R>>(provider => 
                new InternalPipeBuilder<M,R>(provider));
        }
    }
}
