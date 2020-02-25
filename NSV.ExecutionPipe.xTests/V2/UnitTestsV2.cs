using NSV.ExecutionPipe.Builders;
using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NSV.ExecutionPipe.xTests.V2
{
    public class UnitTestsV2
    {
        [Fact]
        public async Task CreateAsyncPipeTest()
        {
            var pipe = PipeBuilder
                .AsyncPipe<IntModel, bool>()
                .Cache(false)
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
                         .Restricted(1, 10, "SecondExecutor")
                         .StopWatch()
                         .ExecuteIf(x => x.Integer > 1)
                         .IfFail()
                             .Retry(3, 1000).Break(false).Set()
                         .IfOk()
                             .Return((m, r) => r).Set()
                         .Add()
                .EndIf()
                .Default((x, cache) =>
                     {
                         x.Integer += 100;
                         //cache.Delete("key");
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

        [Fact]
        public async Task AsyncPipeAddAllTypesExecutorsWithSkip()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var execIncTwo = new IntModelIncrementTwoEtor();
            var execIncTwo1 = new IntModelIncrementTwoEtor();
            var funcExecIncOne = ExecutorFactory.GetFuncIntModelIncrementOneEtor();
            var funcExecIncTwo = ExecutorFactory.GetFuncIntModelIncrementTwoEtor();
            var funcDefault = ExecutorFactory.GetFuncDefaultEtor();

            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Executor(execIncOne).Label("exec_1").Add()
                .Executor(execIncTwo, true).Label("exec_2").Add()
                .Executor(execIncTwo1, false).Label("exec_21").Add() // skip
                .Executor(funcExecIncOne).Label("exec_3").Add()
                .Executor(funcExecIncTwo, true).Label("exec_4").Add()
                .Executor(async (model) =>
                    {
                        var value = await Task.FromResult<int>(10);
                        model.Integer += value;
                        return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(model.Integer);
                    }).Label("exec_5").Add()
                .Executor((model) =>
                     {
                         model.Integer += 20;
                         return PipeResult<int>
                                 .DefaultSuccessful
                                 .SetValue(model.Integer);
                     }).Label("exec_6").Add()
                .Default(funcDefault).Add()
                .Return((model, results) =>
                    {
                        if (results.Length < 7)
                            return PipeResult<int>
                                .DefaultUnSuccessful
                                .SetValue(model.Integer);
                        if (results.Sum(x => x.Value) < 136)
                            return PipeResult<int>
                                .DefaultUnSuccessful
                                .SetValue(model.Integer);

                        return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(model.Integer);
                    });

            var integer = new IntModel { Integer = 10 };
            var result = await pipe.ExecuteAsync(integer);

            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Value >= 136);
        }

        [Fact]
        public async Task AsyncPipeAddAllTypesWithCacheExecutors()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var execIncTwo = new IntModelIncrementTwoEtor();
            var funcDefault = ExecutorFactory.GetFuncDefaultCacheEtor();

            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Cache(false)
                .Executor(execIncOne).Label("exec_1").Add()
                .Executor(execIncTwo).Label("exec_2").Add()
                .Executor(async (model, cache) =>
                    {
                        var value = await Task.FromResult<int>(10);
                        model.Integer += value;
                        cache.SetSafely<int>("3", model.Integer);
                        return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(model.Integer);
                    }).Label("exec_5").Add()
                .Executor((model, cache) =>
                    {
                        model.Integer += 20;
                        cache.SetSafely<int>("4", model.Integer);
                        return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(model.Integer);
                    }).Label("exec_6").Add()
                .Default(funcDefault).Add()
                .Return((model, results) =>
                    {
                        if (results.Length < 5)
                            return PipeResult<int>
                                .DefaultUnSuccessful
                                .SetValue(model.Integer);
                        if (results.Sum(x => x.Value) < 83)
                            return PipeResult<int>
                                .DefaultUnSuccessful
                                .SetValue(model.Integer);

                        return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(model.Integer);
                    });

            var integer = new IntModel { Integer = 0 };
            var result = await pipe.ExecuteAsync(integer);

            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Value >= 83);
        }

        [Fact]
        public async Task AsyncPipeWithStopWatch()
        {
            var execIncOne = new Delay50Etor();
            var execIncTwo = new Delay100Etor();

            var pipe = PipeBuilder
                .AsyncPipe<IntModel, TimeSpan>()
                .StopWatch(true)
                .Executor(execIncOne).Label("exec_1").StopWatch().Add()
                .Executor(execIncTwo).Label("exec_2").StopWatch().Add()
                .Executor(async (model, cache) =>
                    {
                        await Task.Delay(150);
                        return PipeResult<TimeSpan>
                                .DefaultSuccessful
                                .SetValue(TimeSpan.FromMilliseconds(150));
                    }).StopWatch().Label("exec_5").Add()
                .Executor(async (model) =>
                    {
                        await Task.Delay(200);
                        return PipeResult<TimeSpan>
                                .DefaultSuccessful
                                .SetValue(TimeSpan.FromMilliseconds(200));
                    }).StopWatch().Label("exec_6").Add()
                .Return((model, results) =>
                    {
                        var sumElapsed = TimeSpan.FromMilliseconds(
                            results.Sum(x => x.Elapsed.TotalMilliseconds));

                        return PipeResult<TimeSpan>
                            .DefaultSuccessful
                            .SetValue(sumElapsed);
                    });

            var integer = new IntModel { Integer = 0 };
            var result = await pipe.ExecuteAsync(integer);
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Value.Value.TotalMilliseconds >= 500);
            Assert.True(result.Elapsed.TotalMilliseconds > result.Value.Value.TotalMilliseconds);
        }

        [Fact]
        public async Task AsyncPipeWithStopWatchAndRetry()
        {
            var execIncOne = new IntDelay50Etor();

            var pipe = PipeBuilder
                .AsyncPipe<IntModel, double>()
                .Cache(false)
                .StopWatch(true)
                .Executor(execIncOne)
                    .Label("exec_1")
                    .StopWatch()
                    .Add()
                .Executor(async (model, cache) =>
                    {
                        await Task.Delay(50);
                        int count = cache.GetSafely<int>("break_count") + 1;
                        cache.SetOrUpdateSafely<int>("break_count", count);
                        return PipeResult<double>
                                .DefaultUnSuccessful
                                .SetValue(50);
                    })
                    .Label("exec_2")
                    .IfFail().Retry(3,100).Set()
                    .StopWatch()
                    .Add()
                .Default((model, cache) => 
                    {
                        int count = cache.GetSafely<int>("break_count");
                        return PipeResult<double>
                               .DefaultSuccessful
                               .SetValue(count);
                    }).Add()
                .Return((model, results) =>
                    {
                        var sumElapsed = TimeSpan.FromMilliseconds(
                            results.Sum(x => x.Elapsed.TotalMilliseconds));
                        if(results.FirstOrDefault(x => x.Label == "Default").Value.Value == 4)
                            return PipeResult<double>
                                .DefaultSuccessful
                                .SetValue(sumElapsed.TotalMilliseconds);

                        return PipeResult<double>
                                .DefaultUnSuccessful
                                .SetValue(sumElapsed.TotalMilliseconds);
                    });

            var integer = new IntModel { Integer = 0 };
            var result = await pipe.ExecuteAsync(integer);
            Assert.Equal(ExecutionResult.Successful, result.Success);
            
            Assert.True(result.Value > 450);
            Assert.True(result.Elapsed.TotalMilliseconds > result.Value);
        }

        //public async Task AsyncPipeWithStopWatchAndRetry()
        //{
        //    var execIncOne = new IntDelay50Etor();

        //    var pipe = PipeBuilder
        //        .AsyncPipe<IntModel, double>()
        //        .Cache(false)
        //        .StopWatch(true)
        //        .Executor(execIncOne)
        //            .Label("exec_1")
        //            .StopWatch()
        //            .Add()
        //        .Executor(async (model, cache) =>
        //        {
        //            await Task.Delay(50);
        //            int count = cache.GetSafely<int>("break_count") + 1;
        //            cache.SetOrUpdateSafely<int>("break_count", count);
        //            return PipeResult<double>
        //                    .DefaultUnSuccessful
        //                    .SetValue(50);
        //        })
        //            .Label("exec_2")
        //            .IfFail().Retry(3, 100).Set()
        //            .StopWatch()
        //            .Add()
        //        .Default((model, cache) =>
        //        {
        //            int count = cache.GetSafely<int>("break_count");
        //            return PipeResult<double>
        //                   .DefaultSuccessful
        //                   .SetValue(count);
        //        }).Add()
        //        .Return((model, results) =>
        //        {
        //            var sumElapsed = TimeSpan.FromMilliseconds(
        //                results.Sum(x => x.Elapsed.TotalMilliseconds));
        //            if (results.FirstOrDefault(x => x.Label == "Default").Value.Value == 4)
        //                return PipeResult<double>
        //                    .DefaultSuccessful
        //                    .SetValue(sumElapsed.TotalMilliseconds);

        //            return PipeResult<double>
        //                    .DefaultUnSuccessful
        //                    .SetValue(sumElapsed.TotalMilliseconds);
        //        });

        //    var integer = new IntModel { Integer = 0 };
        //    var result = await pipe.ExecuteAsync(integer);
        //    Assert.Equal(ExecutionResult.Successful, result.Success);

        //    Assert.True(result.Value > 450);
        //    Assert.True(result.Elapsed.TotalMilliseconds > result.Value);
        //}



    }
}
