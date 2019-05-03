using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSV.ExecutionPipe.xTests
{
    public class TestPipe : Pipe<TestModel, TestResult>
    {
        public override PipeResult<TestResult> CreateResult(
            TestModel model,
            PipeResult<TestResult>[] results)
        {
            if (results != null && results.Length > 0)
            {
                return new PipeResult<TestResult>
                {
                    Errors = results.AllErrors(),

                    Exceptions = results.AllExceptions(),

                    Success = results.AnySuccess(),

                    Value = results
                        .Where(x => x.Success != ExecutionResult.Initial)
                        .Where(x => x.Value.HasValue)
                        .FirstOrDefault().Value.Value 
                            ?? Optional<TestResult>.Default  
                };
            }
            return PipeResult<TestResult>.Default;
        }
    }
}
