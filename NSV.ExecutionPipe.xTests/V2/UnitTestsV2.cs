using NSV.ExecutionPipe.Builders;
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
        public async Task AsyncPipeAddAllTypesExecutors()
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
                //.Executor(execIncTwo1, false).Label("exec_21").Add() // not in pipe
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
                    if (results.Length < 5)
                        return PipeResult<int>
                            .DefaultUnSuccessful
                            .SetValue(model.Integer);
                    if (results.Sum(x => x.Value) >= 136)
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

        }
    }
}
