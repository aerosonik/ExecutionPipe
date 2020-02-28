using NSV.ExecutionPipe.Builders;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NSV.ExecutionPipe.xTests.V2
{
    public class UnitTestsAsyncParallelPipe
    {
        [Fact]
        public async Task ParallelPipeBuild()
        {
            var pipe = PipeBuilder
               .AsyncParallelPipe<IntModel, bool>()
               .Cache(true)
               .Executor(x =>
               {
                   x.Integer += 1;
                   return PipeResult<bool>
                        .DefaultSuccessful.SetValue(true);
               })
                   .Label("FirstExecutor")
                   .Add()
               .If(x => x.Integer > 0)
                   .Executor(x =>
                   {
                       x.Integer += 1;
                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   })
                        .Label("SecondExecutor")
                        .Restricted(10, "SecondExecutor")
                        .StopWatch()
                        .ExecuteIf(x => x.Integer > 1)
                        .IfFail()
                            .Retry(3, 1000).Set()
                        .Add()
               .EndIf()
               .Default((x, cache) =>
               {
                   x.Integer += 100;
                   return PipeResult<bool>
                        .DefaultSuccessful.SetValue(true);
               })
                    .Label("Default")
                    .Add()
               .Return((model, results) =>
               {
                   if (model.Integer >= 102)
                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   return PipeResult<bool>
                        .DefaultUnSuccessful;
               });

            Assert.IsAssignableFrom<IAsyncPipe<IntModel, bool>>(pipe);
            var integer = new IntModel { Integer = 10 };
            var result = await pipe.ExecuteAsync(integer);
            Assert.True(result.Value.Value);
            Assert.Equal(ExecutionResult.Successful, result.Success);
        }
    }
}
