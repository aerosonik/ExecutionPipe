using NSV.ExecutionPipe.Builders;
using NSV.ExecutionPipe.Pipes;
using NSV.ExecutionPipe.Pool;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NSV.ExecutionPipe.xTests.V2
{
    public class UnitTestsExecutionPool
    {
        //private IAsyncPipe<IntModel, int> CreatePipe()
        //{
        //    return PipeBuilder.AsyncPipe<IntModel, int>()
        //        .Executor(async (model, cache) => 
        //        {
                    
        //        })
        //        .Add()
        //        .Return((model, results) => results[0]);
        //}
        [Fact]
        public async Task ExecutionPoolInit()
        {
            //var pool = ExecutionPoolFactory.CreateExecutionPool
        }
    }
}
