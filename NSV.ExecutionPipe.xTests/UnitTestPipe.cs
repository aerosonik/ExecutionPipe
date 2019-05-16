using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using Xunit;

namespace NSV.ExecutionPipe.xTests
{
    public class UnitTestPipe
    {
        [Fact]
        public void Pipe_AsParallel()
        {
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            var parallelPipe = testPipe.AsParallel();
            Assert.IsAssignableFrom<IParallelPipe<TestModel, TestResult>>(parallelPipe);
        }

        [Fact]
        public void Pipe_AsSequential()
        {
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            var parallelPipe = testPipe.AsSequential();
            Assert.IsAssignableFrom<ISequentialPipe<TestModel, TestResult>>(parallelPipe);
        }

        [Fact]
        public void Pipe_Execution_Sequential()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var model = new TestModel
            {
                Id = 1,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseModel(model)
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.Run();
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(testPipe.Elapsed.TotalMilliseconds > 6000);
        }

        [Fact]
        public void Pipe_Execution_Parallel()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var model = new TestModel
            {
                Id = 1,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseModel(model)
                .UseStopWatch()
                .AsParallel()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.Run();
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(testPipe.Elapsed.TotalMilliseconds < 6000);
        }

        [Fact]
        public void Pipe_Execution_Sequential_Break()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var model = new TestModel
            {
                Id = 1,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseModel(model)
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                    .SetAllowBreak()
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.Run();
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(testPipe.Elapsed.TotalMilliseconds < 6000);
        }

        [Fact]
        public void Pipe_Execution_Sequential_Skip()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var model = new TestModel
            {
                Id = 1,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseModel(model)
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                    .SetSkipIf(x => x.Id > 1)
                .Finish();

            var result = testPipe.Run();
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(testPipe.Elapsed.TotalMilliseconds < 6000);
        }

        [Fact]
        public void Pipe_Execution_Sequential_BreakIfFailed()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var model = new TestModel
            {
                Id = 1,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseModel(model)
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                    .SetLabel(nameof(TestExecutor1))
                    .SetResultHandler((m, r) => { return r; })
                    .SetBreakIfFailed()
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.Run();
            Assert.True(result.Success == ExecutionResult.Failed);
            Assert.Equal(1111, result.Value.Value.Id);
            Assert.Equal(nameof(TestExecutor1), result.Label);
            Assert.Equal(ExecutionResult.Failed, result.Success);
            Assert.True(testPipe.Elapsed.TotalMilliseconds < 6000/2);
        }

        [Fact]
        public void Pipe_Execution_Sequential_BreakIfFailed_UseModel()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var model = new TestModel
            {
                Id = 1,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                    .SetSkipIf(x => x.Id > 1)
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.UseModel(model).Run();
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(testPipe.Elapsed.TotalMilliseconds < 6000);
        }

        [Fact]
        public void Pipe_Execution_Sequential_MutableModel()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var model = new TestModel
            {
                Id = 0,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseModel(model)
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.Run();
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(model.Id == 3);
        }

        [Fact]
        public void Pipe_Execution_Sequential_SubPipe()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var pipeExecutor = new TestPipeExecutor();
            var testSubPipe = new TestSubPipe();
            var model = new TestModel
            {
                Id = 0,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseModel(model)
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .AddExecutor(pipeExecutor)
                    .SetSubPipe(testSubPipe, _ => true)
                .Finish();

            var result = testPipe.Run();
            Assert.True(testPipe.Elapsed.TotalMilliseconds > 14000);
            Assert.True(model.Id == 7);
        }

        [Fact]
        public void Pipe_Execution_Sequential_SubPipe_Skip()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var pipeExecutor = new TestPipeExecutor();
            var testSubPipe = new TestSubPipe();
            var model = new TestModel
            {
                Id = 0,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseModel(model)
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .AddExecutor(pipeExecutor)
                    .SetSubPipe(testSubPipe, _ => false)
                .Finish();

            var result = testPipe.Run();
            Assert.True(testPipe.Elapsed.TotalMilliseconds < 9000);
            Assert.True(model.Id == 4);
        }
    }
}
