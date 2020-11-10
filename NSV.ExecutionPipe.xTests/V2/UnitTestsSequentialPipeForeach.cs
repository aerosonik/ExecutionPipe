using NSV.ExecutionPipe.Builders;
using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NSV.ExecutionPipe.xTests.V2
{
    public class UnitTestsSequentialPipeForeach
    {
        #region Executors
        private class Executor1 : IAsyncExecutor<ForeachModel, int>
        {
            public Task<PipeResult<int>> ExecuteAsync(ForeachModel model, IPipeCache pipeCache = null)
            {
                throw new NotImplementedException();
            }
        }
        private class FroreachExecutor1 : IAsyncExecutor<EnumerableModel, int>
        {
            public Task<PipeResult<int>> ExecuteAsync(EnumerableModel model, IPipeCache pipeCache = null)
            {
                throw new NotImplementedException();
            }
        }
        private class FroreachExecutor2 : IAsyncExecutor<EnumerableModel, int>
        {
            public Task<PipeResult<int>> ExecuteAsync(EnumerableModel model, IPipeCache pipeCache = null)
            {
                throw new NotImplementedException();
            }
        }
        private class FroreachExecutor3 : IAsyncExecutor<EnumerableModel, int>
        {
            public Task<PipeResult<int>> ExecuteAsync(EnumerableModel model, IPipeCache pipeCache = null)
            {
                throw new NotImplementedException();
            }
        }
        private class DefaultForeachExecutor : IAsyncExecutor<EnumerableModel, int>
        {
            public Task<PipeResult<int>> ExecuteAsync(EnumerableModel model, IPipeCache pipeCache = null)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        [Fact]
        public async Task Foreach()
        {
            var foreachlabel = "FOREACH";
            var pipe = PipeBuilder
                .AsyncPipe<ForeachModel, int>()
                .Executor(new Executor1()).Label("e1").Add()
                .Foreach(x => x.Enumerated) // AsyncPipe<EnumerableModel, int>()
                    .Executor(new FroreachExecutor1()).Label("fe1").Add()
                    .Executor(new FroreachExecutor2()).Label("fe2").Add()
                    .Executor(new FroreachExecutor3()).Label("fe3").Add()
                    .EndForeach((model,results)=>
                    {
                        return PipeResult<int>
                            .DefaultSuccessful
                            .SetLabel(foreachlabel)
                            .SetValue(1);
                    })
                .Return((m, r) => { return r.FirstOrDefault(x => x.Label == foreachlabel); });
            var model = new ForeachModel
            {
                Integer = 32,
                Enumerated = new[] 
                {
                    new EnumerableModel{Title = "t1", Flag = true, Value = 10 },
                    new EnumerableModel{Title = "t2", Flag = true, Value = 20 },
                    new EnumerableModel{Title = "t3", Flag = true, Value = 30 },
                }
            };
            var result = await pipe.ExecuteAsync(model);
            Assert.Equal(1, result);
        }
    }
}
