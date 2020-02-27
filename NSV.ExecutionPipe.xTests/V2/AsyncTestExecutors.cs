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
            pipeCache.SetSafely<int>("1", model.Integer);
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
            pipeCache.SetSafely<int>("2", model.Integer);
            return PipeResult<int>
                .DefaultSuccessful
                .SetValue(model.Integer);
        }
    }
    public class IntModelIncrementThreeEtor : IAsyncExecutor<IntModel, int>
    {
        private readonly RestrictionTester _restrictionTester;
        public IntModelIncrementThreeEtor(RestrictionTester restrictionTester)
        {
            _restrictionTester = restrictionTester;
        }
        public async Task<PipeResult<int>> ExecuteAsync(
            IntModel model,
            IPipeCache pipeCache = null)
        {
            _restrictionTester.IncrementCounter();
            await Task.Delay(100);
            model.Integer += 3;
            pipeCache.SetSafely<int>("3", model.Integer);
            _restrictionTester.DecrementCounter();
            return PipeResult<int>
                .DefaultSuccessful
                .SetValue(model.Integer);
        }
    }

    public class Delay50Etor : IAsyncExecutor<IntModel, TimeSpan>
    {
        public async Task<PipeResult<TimeSpan>> ExecuteAsync(
            IntModel model,
            IPipeCache pipeCache = null)
        {
            await Task.Delay(50);
            return PipeResult<TimeSpan>
                .DefaultSuccessful
                .SetValue(TimeSpan.FromMilliseconds(50));
        }
    }

    public class Delay100Etor : IAsyncExecutor<IntModel, TimeSpan>
    {
        public async Task<PipeResult<TimeSpan>> ExecuteAsync(
            IntModel model,
            IPipeCache pipeCache = null)
        {
            await Task.Delay(100);
            return PipeResult<TimeSpan>
                .DefaultSuccessful
                .SetValue(TimeSpan.FromMilliseconds(100));
        }
    }

    public class IntDelay50Etor : IAsyncExecutor<IntModel, double>
    {
        public async Task<PipeResult<double>> ExecuteAsync(
            IntModel model,
            IPipeCache pipeCache = null)
        {
            await Task.Delay(50);
            return PipeResult<double>
                .DefaultSuccessful
                .SetValue(50);
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

    public class DefaultCacheEtor : IAsyncExecutor<IntModel, int>
    {
        public async Task<PipeResult<int>> ExecuteAsync(
            IntModel model,
            IPipeCache pipeCache = null)
        {
            var x1 = pipeCache.GetSafely<int>("1");
            var x2 = pipeCache.GetSafely<int>("2");
            var x3 = pipeCache.GetSafely<int>("3");
            var x4 = pipeCache.GetSafely<int>("4");
            model.Integer += x1 + x2 + x3 + x4;
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
        public static Func<IAsyncExecutor<IntModel, int>> GetFuncDefaultCacheEtor()
        {
            return () => new DefaultCacheEtor();
        }
    }
}
