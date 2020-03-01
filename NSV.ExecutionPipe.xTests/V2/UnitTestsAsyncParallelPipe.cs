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
        public async Task ParallelPipeExecute()
        {
            var pipe = PipeBuilder
               .AsyncParallelPipe<IntModel, bool>()
               .Cache(true)
               .StopWatch(true)
               .Executor(async x =>
                   {
                       await Task.Delay(1000);
                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   })
                   .Label("exc 1")
                   .Add()
                .Executor(async x =>
                    {
                        await Task.Delay(1000);
                        return PipeResult<bool>
                             .DefaultSuccessful.SetValue(true);
                    })
                   .Label("exc 2")
                   .Add()
               .Executor(async x =>
                    {
                        await Task.Delay(1000);
                        return PipeResult<bool>
                             .DefaultSuccessful.SetValue(true);
                    })
                   .Label("exc 3")
                   .Add()
               .Executor(async x =>
                    {
                        await Task.Delay(1000);
                        return PipeResult<bool>
                             .DefaultSuccessful.SetValue(true);
                    })
                   .Label("exc 4")
                   .Add()
               .Default((x, cache) =>
                   {
                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   })
                   .Add()
               .Return((model, results) =>
                   {
                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   });

            Assert.IsAssignableFrom<IAsyncPipe<IntModel, bool>>(pipe);
            var integer = new IntModel { Integer = 10 };
            var result = await pipe.ExecuteAsync(integer);
            Assert.True(result.Value.Value);
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Elapsed < TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task ParallelPipeExecuteIF()
        {
            var pipe = PipeBuilder
               .AsyncParallelPipe<IntModel, bool>()
               .Cache(true)
               .StopWatch(true)
               .Executor(async x =>
                   {
                       await Task.Delay(1000);
                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   })
                   .ExecuteIf(x => x.Integer > 100)
                   .Label("exc 1")
                   .Add()
                .Executor(async x =>
                    {
                        await Task.Delay(1000);
                        return PipeResult<bool>
                             .DefaultSuccessful.SetValue(true);
                    },false)
                   .Label("exc 2")
                   .Add()
               .Executor(async x =>
                   {
                       await Task.Delay(1000);
                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   })
                   .Label("exc 3")
                   .Add()
               .Executor(async x =>
                   {
                       await Task.Delay(1000);
                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   })
                   .Label("exc 4")
                   .Add()
               .Executor(async x =>
                    {
                        await Task.Delay(1000);
                        return PipeResult<bool>
                             .DefaultSuccessful.SetValue(true);
                    })
                   .Label("exc 5")
                   .Add()
               .Default((x, cache) =>
                   {
                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   })
                   .Add()
               .Return((model, results) =>
                   {
                       Assert.True(results.Length == 4);
                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   });

            Assert.IsAssignableFrom<IAsyncPipe<IntModel, bool>>(pipe);
            var integer = new IntModel { Integer = 10 };
            var result = await pipe.ExecuteAsync(integer);
            Assert.True(result.Value.Value);
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Elapsed < TimeSpan.FromSeconds(2));
        }

        //public async Task ParallelPipeExecuteIF()
        //{
        //    var pipe = PipeBuilder
        //       .AsyncParallelPipe<IntModel, bool>()
        //       .Cache(true)
        //       .StopWatch(true)
        //       .Executor(async x =>
        //       {
        //           await Task.Delay(1000);
        //           return PipeResult<bool>
        //                .DefaultSuccessful.SetValue(true);
        //       })
        //           .ExecuteIf(x => x.Integer > 100)
        //           .Label("exc 1")
        //           .IfFail().
        //           .Add()
        //        .Executor(async x =>
        //        {
        //            await Task.Delay(1000);
        //            return PipeResult<bool>
        //                 .DefaultSuccessful.SetValue(true);
        //        }, false)
        //           .Label("exc 2")
        //           .Add()
        //       .Executor(async x =>
        //       {
        //           await Task.Delay(1000);
        //           return PipeResult<bool>
        //                .DefaultSuccessful.SetValue(true);
        //       })
        //           .Label("exc 3")
        //           .Add()
        //       .Executor(async x =>
        //       {
        //           await Task.Delay(1000);
        //           return PipeResult<bool>
        //                .DefaultSuccessful.SetValue(true);
        //       })
        //           .Label("exc 4")
        //           .Add()
        //       .Executor(async x =>
        //       {
        //           await Task.Delay(1000);
        //           return PipeResult<bool>
        //                .DefaultSuccessful.SetValue(true);
        //       })
        //           .Label("exc 5")
        //           .Add()
        //       .Default((x, cache) =>
        //       {
        //           return PipeResult<bool>
        //                .DefaultSuccessful.SetValue(true);
        //       })
        //           .Add()
        //       .Return((model, results) =>
        //       {
        //           Assert.True(results.Length == 4);
        //           return PipeResult<bool>
        //                .DefaultSuccessful.SetValue(true);
        //       });

        //    Assert.IsAssignableFrom<IAsyncPipe<IntModel, bool>>(pipe);
        //    var integer = new IntModel { Integer = 10 };
        //    var result = await pipe.ExecuteAsync(integer);
        //    Assert.True(result.Value.Value);
        //    Assert.Equal(ExecutionResult.Successful, result.Success);
        //    Assert.True(result.Elapsed < TimeSpan.FromSeconds(2));
        //}
    }
}
