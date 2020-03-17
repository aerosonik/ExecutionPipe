using NSV.ExecutionPipe.Builders;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using NSV.ExecutionPipe.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

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
               .Return((model, results) =>
                   {
                       Assert.True(results.Length == 3);
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
        public async Task ParallelPipeExecuteWithIfAndDefault()
        {
            var pipe = PipeBuilder
               .AsyncParallelPipe<IntModel, bool>()
               .StopWatch(true)
               .If(x => x.Integer == 0)
                   .Executor(async x =>
                       {
                           await Task.Delay(1000);
                           return PipeResult<bool>
                                .DefaultSuccessful.SetValue(true);
                       })
                       .Label("exc 1")
                       .Add()
                    .If(x => x.Integer > 110) // in pipe, will not be executed
                        .Executor(async x =>
                            {
                                await Task.Delay(1000);
                                return PipeResult<bool>
                                     .DefaultSuccessful.SetValue(true);
                            })
                           .Label("exc 2")
                           .Add()
                   .EndIf()
                   .Executor(async x =>
                       {
                           await Task.Delay(1000);
                           return PipeResult<bool>
                                .DefaultSuccessful.SetValue(true);
                       })
                       .Label("exc 3")
                       .Add()
                   .If(false) //excluded from pipe, will not be executed
                       .Executor(async x =>
                           {
                               await Task.Delay(1000);
                               return PipeResult<bool>
                                    .DefaultSuccessful.SetValue(true);
                           })
                           .Label("exc 4")
                           .Add()
                   .EndIf()
                   .Executor(async x =>
                       {
                           await Task.Delay(1000);
                           return PipeResult<bool>
                                .DefaultSuccessful.SetValue(true);
                       })
                       .Label("exc 5")
                       .Add()
               .EndIf()
               .Default((x, cache) =>
                   {
                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   })
                   .Add()
               .Return((model, results) =>
                   {
                       Assert.True(results.Length == 4);
                       Assert.Contains(results, x => x.Label == "exc 1");
                       Assert.Contains(results, x => x.Label == "exc 3");
                       Assert.Contains(results, x => x.Label == "exc 5");
                       Assert.Contains(results, x => x.Label == "Default");
                       Assert.DoesNotContain(results, x => x.Label == "exc 2");
                       Assert.DoesNotContain(results, x => x.Label == "exc 4");

                       return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                   });

            Assert.IsAssignableFrom<IAsyncPipe<IntModel, bool>>(pipe);
            var integer = new IntModel { Integer = 0 };
            var result = await pipe.ExecuteAsync(integer);
            Assert.True(result.Value.Value);
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Elapsed < TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task ParralelSubPipe()
        {
            var subPipe = PipeBuilder
               .AsyncParallelPipe<IntModel, int>()
               .StopWatch(true)
               .Executor(async (x, cache) =>
                   {
                       cache.SetSafely<int>("sub_1", 1);
                       await Task.Delay(1000);
                       return PipeResult<int>
                            .DefaultSuccessful.SetValue(1);
                   })
                   .Label("exc 1")
                   .Add()
               .Executor(async (x, cache) =>
                    {
                        cache.SetSafely<int>("sub_2", 1);
                        await Task.Delay(1000);
                        return PipeResult<int>
                             .DefaultSuccessful.SetValue(1);
                    })
                   .Label("exc 2")
                   .Add()
               .Executor(async (x, cache) =>
                   {
                       cache.SetSafely<int>("sub_3", 1);
                       await Task.Delay(1000);
                       return PipeResult<int>
                            .DefaultSuccessful.SetValue(1);
                   })
                   .Label("exc 3")
                   .Add()
               .Return((model, results) =>
                   {
                       return PipeResult<int>
                            .DefaultSuccessful.SetValue(1);
                   })
               .ToExecutor();

            var pipe = PipeBuilder
               .AsyncPipe<IntModel, int>()
               .StopWatch(true)
               .Cache(true)
               .Executor(async (x, cache) =>
                   {
                       cache.SetSafely<int>("pipe_1", 1);
                       await Task.Delay(100);
                       return PipeResult<int>
                            .DefaultSuccessful.SetValue(1);
                   })
                   .Label("exc 1")
                   .Add()
               .Executor(async (x, cache) =>
                   {
                       cache.SetSafely<int>("pipe_2", 1);
                       await Task.Delay(100);
                       return PipeResult<int>
                            .DefaultSuccessful.SetValue(1);
                   })
                   .Label("exc 2")
                   .Add()
               .Executor(subPipe)
                   .Label("sub_pipe")
                   .Add()
               .Default((x, cache) => 
                    {
                        var sub1 = cache.GetSafely<int>("sub_1");
                        var sub2 = cache.GetSafely<int>("sub_2");
                        var sub3 = cache.GetSafely<int>("sub_3");
                        var pipe1 = cache.GetSafely<int>("pipe_1");
                        var pipe2 = cache.GetSafely<int>("pipe_2");

                        return PipeResult<int>
                           .DefaultSuccessful.SetValue(sub1 + sub2 + sub3 + pipe1 + pipe2);
                    })
                    .Add()
               .Return((model, results) =>
                   {
                       var sum = results
                        .FirstOrDefault(x => x.Label == "Default").Value.Value;

                       Assert.Contains(results, x => x.Label == "sub_pipe");
                       Assert.Equal(5, sum);

                       return PipeResult<int>
                            .DefaultSuccessful.SetValue(sum);
                   });

            Assert.IsAssignableFrom<IAsyncPipe<IntModel, int>>(pipe);
            var integer = new IntModel { Integer = 10 };
            var result = await pipe.ExecuteAsync(integer);
            Assert.Equal(5, result.Value.Value);
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Elapsed < TimeSpan.FromSeconds(2));
        }
    }
}
