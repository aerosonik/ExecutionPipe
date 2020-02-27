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
                         .Restricted(10, "SecondExecutor")
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
                    .IfFail().Retry(3, 100).Set()
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
                        if (results.FirstOrDefault(x => x.Label == "Default").Value.Value == 4)
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

        [Fact]
        public async Task AsyncPipeExecutorBreakAndReturnIfFailedWithDefault()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Cache(false)
                .Executor(execIncOne).Label("exec_1").Add()
                .Executor(async (model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                                .DefaultUnSuccessfulBreak
                                .SetValue(model.Integer);
                    })
                    .Label("exec_2")
                    .IfFail().Break(true).Return((m, r) => r).Set()
                    .Add()
                .Executor(async (model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(model.Integer);
                    })
                    .Label("exec_3")
                    .Add()
                .Default((model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                               .DefaultSuccessful
                               .SetValue(model.Integer);
                    }).Add()
                .Return((model, results) =>
                {
                    if (results.Any(x => x.Label == "Default") &&
                        !results.Any(x => x.Label == "exec_3") &&
                        results.Length == 3)
                        return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(results.Max(x => x.Value.Value));

                    return PipeResult<int>
                            .DefaultUnSuccessful
                            .SetValue(results.Max(x => x.Value.Value));
                });

            var result = await pipe.ExecuteAsync(new IntModel { Integer = 0 });
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Value.Value == 3);
        }

        [Fact]
        public async Task AsyncPipeExecutorBreakIfFailedWithDefault()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Cache(false)
                .Executor(execIncOne).Label("exec_1").Add()
                .Executor(async (model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                                .DefaultUnSuccessfulBreak
                                .SetValue(model.Integer);
                    })
                    .Label("exec_2")
                    .IfFail().Break(true).Set()
                    .Add()
                .Executor(async (model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(model.Integer);
                    })
                    .Label("exec_3")
                    .Add()
                .Default((model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                               .DefaultSuccessful
                               .SetValue(model.Integer);
                    }).Add()
                .Return((model, results) =>
                    {
                        if (results.Any(x => x.Label == "Default") &&
                            !results.Any(x => x.Label == "exec_3") &&
                            results.Length == 3)
                            return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(results.Max(x => x.Value.Value));

                        return PipeResult<int>
                                .DefaultUnSuccessful
                                .SetValue(results.Max(x => x.Value.Value));
                    });

            var result = await pipe.ExecuteAsync(new IntModel { Integer = 0 });
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Value.Value == 3);
        }

        [Fact]
        public async Task AsyncPipeExecutorBreakIfFailed()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Cache(false)
                .Executor(execIncOne).Label("exec_1").Add()
                .Executor(async (model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                                .DefaultUnSuccessfulBreak
                                .SetValue(model.Integer);
                    })
                    .Label("exec_2")
                    .IfFail().Break(true).Set()
                    .Add()
                .Executor(async (model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(model.Integer);
                    })
                    .Label("exec_3")
                    .Add()
                .Return((model, results) =>
                    {
                        if (!results.Any(x => x.Label == "exec_3") &&
                            results.Length == 2)
                            return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(results.Max(x => x.Value.Value));

                        return PipeResult<int>
                                .DefaultUnSuccessful
                                .SetValue(results.Max(x => x.Value.Value));
                    });

            var result = await pipe.ExecuteAsync(new IntModel { Integer = 0 });
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Value.Value == 2);
        }

        [Fact]
        public async Task AsyncPipeExecutorBreakAndReturnIfFailed()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Cache(false)
                .Executor(execIncOne).Label("exec_1").Add()
                .Executor(async (model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                                .DefaultUnSuccessfulBreak
                                .SetValue(model.Integer);
                    })
                    .Label("exec_2")
                    .IfFail().Break(true).Return((m, r) => r.SetValue(222)).Set()
                    .Add()
                .Executor(async (model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(model.Integer);
                    })
                    .Label("exec_3")
                    .Add()
                .Return((model, results) =>
                    {
                        Assert.True(results.FirstOrDefault(x => x.Label == "exec_2").Value.Value == 222);

                        if (!results.Any(x => x.Label == "exec_3") &&
                            results.Length == 2)
                            return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(results.Max(x => x.Value.Value));

                        return PipeResult<int>
                                .DefaultUnSuccessful
                                .SetValue(results.Max(x => x.Value.Value));
                    });

            var result = await pipe.ExecuteAsync(new IntModel { Integer = 0 });
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Value.Value == 222);
        }

        [Fact]
        public async Task AsyncPipeExecutorExecuteIf()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Cache(false)
                .Executor(execIncOne).Label("exec_1").Add()
                .Executor(async (model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                                .DefaultUnSuccessfulBreak
                                .SetValue(model.Integer);
                    })
                    .Label("exec_2")
                    .ExecuteIf(m => m.Integer > 100)
                    .Add()
                .Executor(async (model, cache) =>
                {
                    model.Integer += 1;
                    return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(model.Integer);
                })
                    .Label("exec_3")
                    .Add()
                .Return((model, results) =>
                {
                    Assert.True(!results.Any(x => x.Label == "exec_2"));

                    if (results.Any(x => x.Label == "exec_3") &&
                        results.Length == 2)
                        return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(results.Max(x => x.Value.Value));

                    return PipeResult<int>
                            .DefaultUnSuccessful
                            .SetValue(results.Max(x => x.Value.Value));
                });

            var result = await pipe.ExecuteAsync(new IntModel { Integer = 0 });
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Value.Value == 2);
        }

        [Fact]
        public async Task AsyncPipeExecutorIfOkBreak()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Cache(false)
                .Executor(execIncOne).Label("exec_1").Add()
                .Executor(async (model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                                .DefaultSuccessfulBreak
                                .SetValue(model.Integer);
                    })
                    .Label("exec_2")
                    .IfOk().Break(true).Set()
                    .Add()
                .Executor(async (model, cache) =>
                    {
                        model.Integer += 1;
                        return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(model.Integer);
                    })
                    .Label("exec_3")
                    .Add()
                .Return((model, results) =>
                    {
                        Assert.Contains(results, x => x.Label == "exec_2");
                        Assert.DoesNotContain(results, x => x.Label == "exec_3");

                        if (results.Length == 2)
                            return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(results.Max(x => x.Value.Value));

                        return PipeResult<int>
                                .DefaultUnSuccessful
                                .SetValue(results.Max(x => x.Value.Value));
                    });

            var result = await pipe.ExecuteAsync(new IntModel { Integer = 0 });
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Value.Value == 2);
        }

        [Fact]
        public async Task AsyncPipeExecutorIfOkBreakAndReturn()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Cache(false)
                .Executor(execIncOne).Label("exec_1").Add()
                .Executor(async (model, cache) =>
                {
                    model.Integer += 1;
                    return PipeResult<int>
                            .DefaultSuccessfulBreak
                            .SetValue(model.Integer);
                })
                    .Label("exec_2")
                    .IfOk().Break(true).Return((m, r) => r.SetValue(444)).Set()
                    .Add()
                .Executor(async (model, cache) =>
                {
                    model.Integer += 1;
                    return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(model.Integer);
                })
                    .Label("exec_3")
                    .Add()
                .Return((model, results) =>
                {
                    Assert.Contains(results, x => x.Label == "exec_2");
                    Assert.DoesNotContain(results, x => x.Label == "exec_3");

                    if (results.Length == 2)
                        return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(results.Max(x => x.Value.Value));

                    return PipeResult<int>
                            .DefaultUnSuccessful
                            .SetValue(results.Max(x => x.Value.Value));
                });

            var result = await pipe.ExecuteAsync(new IntModel { Integer = 0 });
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Value.Value == 444);
        }

        [Fact]
        public async Task AsyncPipeIfEndIdWithNestedIf()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Executor(execIncOne) // in pipe, will not be executed
                    .ExecuteIf(model => model.Integer > 0)
                    .Label("exec_1").Add()
                .If(model => model.Integer == 0)
                    .Executor(execIncOne).Label("exec_2").Add()
                .EndIf()
                .Executor(execIncOne).Label("exec_3").Add()
                .If(model => model.Integer > 0)
                    .Executor(execIncOne).Label("exec_3.1").Add()
                    .If(model => model.Integer > 100)
                        .Executor(execIncOne) // in pipe, will not be executed
                            .Label("exec_3.2")
                            .Add()
                    .EndIf()
                .EndIf()
                .If(model => model.Integer > 0)
                    .Executor(execIncOne).Label("exec_4").Add()
                    .If(false)
                        .Executor(execIncOne).Label("exec_4.1").Add() // excluded from pipe
                        .Executor(execIncOne).Label("exec_4.2").Add() // excluded from pipe
                    .EndIf()
                    .Executor(execIncOne)
                        .ExecuteIf(model => model.Integer > 0)
                        .Label("exec_4.3")
                        .Add()
                .EndIf()
                .Return((model, results) =>
                {
                    Assert.Contains(results, x => x.Label == "exec_2");
                    Assert.Contains(results, x => x.Label == "exec_3");
                    Assert.Contains(results, x => x.Label == "exec_3.1");
                    Assert.Contains(results, x => x.Label == "exec_4");
                    Assert.Contains(results, x => x.Label == "exec_4.3");

                    Assert.DoesNotContain(results, x => x.Label == "exec_1");
                    Assert.DoesNotContain(results, x => x.Label == "exec_3.2");
                    Assert.DoesNotContain(results, x => x.Label == "exec_4.1");
                    Assert.DoesNotContain(results, x => x.Label == "exec_4.2");

                    if (results.Length == 5)
                        return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(model.Integer);

                    return PipeResult<int>
                            .DefaultUnSuccessful
                            .SetValue(model.Integer);
                });

            var result = await pipe.ExecuteAsync(new IntModel { Integer = 0 });
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(result.Value.Value == 5);
        }

        [Fact]
        public async Task AsyncPipeMultipleExecutionsWithCache()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var execIncTwo = new IntModelIncrementTwoEtor();
            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Cache(false)
                .Executor((model, cache) =>
                    {
                        Assert.True(cache.GetSafely<int>("1") == 0);
                        Assert.True(cache.GetSafely<int>("2") == 0);
                        return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(model.Integer);
                    }).Label("exec_1").Add()
                .Executor(execIncOne).Label("exec_2").Add()
                .Executor(execIncTwo).Label("exec_3").Add()
                .Default((model, cache) =>
                    {
                        Assert.True(cache.GetSafely<int>("1") == 1);
                        Assert.True(cache.GetSafely<int>("2") == 3);
                        cache.ClearSafely();
                        return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(model.Integer);
                    }).Add()
                .Return((model, results) =>
                {
                    Assert.Contains(results, x => x.Label == "exec_1");
                    Assert.Contains(results, x => x.Label == "exec_2");
                    Assert.Contains(results, x => x.Label == "exec_3");
                    Assert.Contains(results, x => x.Label == "Default");

                    if (results.Length == 4)
                        return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(model.Integer);

                    return PipeResult<int>
                            .DefaultUnSuccessful
                            .SetValue(model.Integer);
                });

            for (int i = 0; i < 5; i++)
            {
                var result = await pipe.ExecuteAsync(new IntModel { Integer = 0 });
                Assert.Equal(ExecutionResult.Successful, result.Success);
                Assert.True(result.Value.Value == 3);
            }
        }

        [Fact]
        public async Task AsyncPipeMultipleExecutionsWithCacheAndIf()
        {
            var execIncOne = new IntModelIncrementOneEtor();
            var execIncTwo = new IntModelIncrementTwoEtor();
            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Cache(false)
                .Executor((model, cache) =>
                {
                    Assert.True(cache.GetSafely<int>("1") == 0);
                    Assert.True(cache.GetSafely<int>("2") == 0);
                    return PipeResult<int>
                        .DefaultSuccessful
                        .SetValue(model.Integer);
                }).Label("exec_1").Add()
                .Executor(execIncOne).Label("exec_2").Add()
                .If(model => model.Integer > 100)
                    .Executor((model, cache) =>
                        {
                            model.Integer += 1;
                            cache.SetSafely<int>("3", model.Integer);
                            return PipeResult<int>
                                .DefaultSuccessful
                                .SetValue(model.Integer);
                        }).Label("exec_3").Add()
                .EndIf()
                .Executor(execIncTwo).Label("exec_4").Add()
                .Default((model, cache) =>
                {
                    Assert.True(cache.GetSafely<int>("1") == 1);
                    Assert.True(cache.GetSafely<int>("2") == 3);
                    cache.ClearSafely();
                    return PipeResult<int>
                        .DefaultSuccessful
                        .SetValue(model.Integer);
                }).Add()
                .Return((model, results) =>
                {
                    Assert.Contains(results, x => x.Label == "exec_1");
                    Assert.Contains(results, x => x.Label == "exec_2");
                    Assert.Contains(results, x => x.Label == "exec_4");
                    Assert.Contains(results, x => x.Label == "Default");

                    Assert.DoesNotContain(results, x => x.Label == "exec_3");

                    if (results.Length == 4)
                        return PipeResult<int>
                            .DefaultSuccessful
                            .SetValue(model.Integer);

                    return PipeResult<int>
                            .DefaultUnSuccessful
                            .SetValue(model.Integer);
                });

            for (int i = 0; i < 5; i++)
            {
                var result = await pipe.ExecuteAsync(new IntModel { Integer = 0 });
                Assert.Equal(ExecutionResult.Successful, result.Success);
                Assert.True(result.Value.Value == 3);
            }
        }

        [Fact]
        public async Task AsyncPipeMultipleExecutionsWithRestriction()
        {
            var restriction = new RestrictionTester();
            var execIncOne = new IntModelIncrementOneEtor();
            var execIncTwo = new IntModelIncrementTwoEtor();
            var execIncThree = new IntModelIncrementThreeEtor(restriction);

            var pipe = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Executor(execIncOne).Label("exec_1").Add()
                .Executor(execIncTwo).Label("exec_2").Add()
                .Executor(execIncThree).Label("exec_3")
                    .Restricted(3, "rest_1")
                    .Add()
                .Return((model, results) =>
                {
                    Assert.Equal(3, results.Length);
                    return PipeResult<int>
                        .DefaultSuccessful
                        .SetValue(model.Integer);
                });

            var execIncOne1 = new IntModelIncrementOneEtor();
            var execIncTwo1 = new IntModelIncrementTwoEtor();
            var execIncThree1 = new IntModelIncrementThreeEtor(restriction);

            var pipe1 = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Executor(execIncOne1).Label("exec_1").Add()
                .Executor(execIncTwo1).Label("exec_2").Add()
                .Executor(execIncThree1).Label("exec_3")
                    .Restricted(3, "rest_1")
                    .Add()
                .Return((model, results) =>
                {
                    Assert.Equal(3, results.Length);
                    return PipeResult<int>
                        .DefaultSuccessful
                        .SetValue(model.Integer);
                });

            var execIncOne2 = new IntModelIncrementOneEtor();
            var execIncTwo2 = new IntModelIncrementTwoEtor();
            var execIncThree2 = new IntModelIncrementThreeEtor(restriction);

            var pipe2 = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Executor(execIncOne2).Label("exec_1").Add()
                .Executor(execIncTwo2).Label("exec_2").Add()
                .Executor(execIncThree2).Label("exec_3")
                    .Restricted(3, "rest_1")
                    .Add()
                .Return((model, results) =>
                {
                    Assert.Equal(3, results.Length);
                    return PipeResult<int>
                        .DefaultSuccessful
                        .SetValue(model.Integer);
                });

            var execIncOne3 = new IntModelIncrementOneEtor();
            var execIncTwo3 = new IntModelIncrementTwoEtor();
            var execIncThree3 = new IntModelIncrementThreeEtor(restriction);

            var pipe3 = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Executor(execIncOne3).Label("exec_1").Add()
                .Executor(execIncTwo3).Label("exec_2").Add()
                .Executor(execIncThree3).Label("exec_3")
                    .Restricted(3, "rest_1")
                    .Add()
                .Return((model, results) =>
                {
                    Assert.Equal(3, results.Length);
                    return PipeResult<int>
                        .DefaultSuccessful
                        .SetValue(model.Integer);
                });

            var execIncOne4 = new IntModelIncrementOneEtor();
            var execIncTwo4 = new IntModelIncrementTwoEtor();
            var execIncThree4 = new IntModelIncrementThreeEtor(restriction);

            var pipe4 = PipeBuilder
                .AsyncPipe<IntModel, int>()
                .Executor(execIncOne4).Label("exec_1").Add()
                .Executor(execIncTwo4).Label("exec_2").Add()
                .Executor(execIncThree4).Label("exec_3")
                    .Restricted(3, "rest_1")
                    .Add()
                .Return((model, results) =>
                {
                    Assert.Equal(3, results.Length);
                    return PipeResult<int>
                        .DefaultSuccessful
                        .SetValue(model.Integer);
                });

            var length = 20;
            var array = new int[length];

            for (int i = 0; i < length; i++)
            {
                var pipeTask = pipe.ExecuteAsync(new IntModel { Integer = 0 });
                var pipeTask1 = pipe1.ExecuteAsync(new IntModel { Integer = 0 });
                var pipeTask2 = pipe2.ExecuteAsync(new IntModel { Integer = 0 });
                var pipeTask3 = pipe3.ExecuteAsync(new IntModel { Integer = 0 });
                var pipeTask4 = pipe4.ExecuteAsync(new IntModel { Integer = 0 });

                var results = await Task.WhenAll(pipeTask, pipeTask1, pipeTask2, pipeTask3, pipeTask4);

                Assert.True(restriction.MaxCount < 4);
                array[i] = restriction.MaxCount;
            }
            var avg = array.Average();
            var max = array.Max();
        }

    }
}
