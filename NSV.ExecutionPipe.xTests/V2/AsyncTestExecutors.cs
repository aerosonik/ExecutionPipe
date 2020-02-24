using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.xTests.V2
{
    public class IntModelIncrementOneEtor : IAsyncExecutor<IntModel, int>
    {
        public async Task<PipeResult<int>> ExecuteAsync(
            IntModel model, 
            IPipeCache pipeCache = null)
        {
            model.Integer += 1;
            return PipeResult<int>
                .DefaultSuccessful
                .SetValue(model.Integer);
        }
    }

    public class IntModelIncrementTwoEtor : IAsyncExecutor<IntModel, int>
    {
        public async Task<PipeResult<int>> ExecuteAsync(
            IntModel model,
            IPipeCache pipeCache = null)
        {
            model.Integer += 2;
            return PipeResult<int>
                .DefaultSuccessful
                .SetValue(model.Integer);
        }
    }

    public class DefaultEtor : IAsyncExecutor<IntModel, int>
    {
        public async Task<PipeResult<int>> ExecuteAsync(
            IntModel model,
            IPipeCache pipeCache = null)
        {
            model.Integer += 100;
            return PipeResult<int>
                .DefaultSuccessful
                .SetValue(model.Integer);
        }
    }

    public static class ExecutorFactory
    {
        public static Func<IAsyncExecutor<IntModel, int>> GetFuncIntModelIncrementOneEtor()
        {
            return () => new IntModelIncrementOneEtor();
        }

        public static Func<IAsyncExecutor<IntModel, int>> GetFuncIntModelIncrementTwoEtor()
        {
            return () => new IntModelIncrementTwoEtor();
        }

        public static Func<IAsyncExecutor<IntModel, int>> GetFuncDefaultEtor()
        {
            return () => new DefaultEtor();
        }
    }
}
