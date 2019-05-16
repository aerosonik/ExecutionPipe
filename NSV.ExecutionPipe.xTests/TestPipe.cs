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

    public class TestSubPipe : Pipe<TestModel, TestResult>
    {
        public TestSubPipe()
        {
            var subExecutor1 = new TestSubExecutor1();
            var subExecutor2 = new TestSubExecutor2();
            var subExecutor3 = new TestSubExecutor3();

            UseStopWatch()
                .AsSequential()
                .AddExecutor(subExecutor1)
                    .SetLabel(nameof(TestSubExecutor1))
                .AddExecutor(subExecutor2)
                    .SetLabel(nameof(TestSubExecutor2))
                .AddExecutor(subExecutor3)
                    .SetLabel(nameof(TestSubExecutor3))
                .Finish();
        }
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
