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
        public override PipeResult<TestResult> CreateResult(TestModel model, PipeResult<TestResult>[] results)
        {
            if (results != null && results.Length > 0)
            {
                var success = results
                    .Any(x => x.Success == ExecutionResult.Successful)
                    ? ExecutionResult.Successful
                    : results
                        .All(x => x.Success == ExecutionResult.Initial)
                        ? ExecutionResult.Initial
                        : ExecutionResult.Unsuccessful;

                return new PipeResult<TestResult>
                {
                    Errors = results
                        .Where(x => x.Success != ExecutionResult.Initial)
                        .Where(x => x.Errors.HasValue)
                        .SelectMany(x => x.Errors.Value)
                        .ToArray(),

                    Exceptions = results
                        .Where(x => x.Success != ExecutionResult.Initial)
                        .Where(x => x.Exceptions.HasValue)
                        .SelectMany(x => x.Exceptions.Value)
                        .ToArray(),

                    Success = success,

                    Value = results
                        .Where(x => x.Success != ExecutionResult.Initial)
                        .Where(x => x.Value.HasValue)
                        .FirstOrDefault().Value.Value ?? Optional<TestResult>.Default  
                };
            }
            return PipeResult<TestResult>.Default;
        }
    }
}
