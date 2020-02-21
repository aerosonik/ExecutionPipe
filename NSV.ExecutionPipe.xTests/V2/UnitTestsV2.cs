﻿using NSV.ExecutionPipe.Builders;
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
        public async Task CreatePipeTest()
        {
            var pipe = PipeBuilder
                .AsyncPipe<int, bool>()
                .Cache(false)
                .Executor(x => 
                    { 
                        x +=1; 
                        return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true); 
                    })
                    .Label("FirstExecutor")
                    .Add()
                .If(x => x > 0)
                    .Executor(x => 
                        { 
                            x +=1; 
                            return PipeResult<bool>
                                .DefaultSuccessful.SetValue(true); 
                        })
                        .Label("SecondExecutor")
                        //.Restricted(0,10, "SecondExecutor")
                        //.StopWatch()
                        //.ExecuteIf(x => x >1)
                        //.IfFail()
                        //    .Retry(3,1000).Break(false).Set()
                        //.IfOk()
                        //    .Return((m,r) => r).Set()
                        .Add()
                .EndIf()
                .Default(x =>
                    {
                        x += 100;
                        return PipeResult<bool>
                            .DefaultSuccessful.SetValue(true);
                    })
                    .Label("Default")
                    .Add()
                .Return((model, results) => results.Last());

            Assert.IsAssignableFrom<IAsyncPipe<int, bool>>(pipe);
            var result = await pipe.ExecuteAsync(10);
            Assert.Equal("Default",result.Label);
        }
    }
}
