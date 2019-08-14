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
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.Execute(model);
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(testPipe.Elapsed.TotalMilliseconds > 6000);
        }

        [Fact]
        public void Pipe_Execution_Sequential_If()
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
                .If(false)
                    .AddExecutor(executor3)
                .EndIf()
                .Finish();

            var result = testPipe.Execute(model);
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(testPipe.Elapsed.TotalMilliseconds < 6000);
            Assert.True(model.Id == 3);
        }
        [Fact]
        public void Pipe_Execution_Sequential_Default()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var defaultExecutor = new TestExecutorDefault();
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
                    .SetAllowBreak()
                    .SetBreakIfFailed()
                    .SetResultHandler((m, r) => r)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .AddExecutor(defaultExecutor)
                    .SetDefault()
                .Finish();

            var result = testPipe.Execute(model);
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(testPipe.Elapsed.TotalMilliseconds < 6000);
            Assert.True(model.Id == 0);
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
                .UseStopWatch()
                //.UseLocalCacheThreadSafe()
                .AsParallel()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.Execute(model);
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
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                    .SetAllowBreak()
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.Execute(model);
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
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .If(x => x.Id <= 1)
                    .AddExecutor(executor3)//.SetSkipIf(x => x.Id > 1)
                .EndIf()
                .Finish();

            var result = testPipe.Execute(model);
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
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                    .SetLabel(nameof(TestExecutor1))
                    .SetResultHandler((m, r) => { return r; })
                    .SetBreakIfFailed()
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.Execute(model);
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
                .If(x => x.Id <= 1)
                    .AddExecutor(executor2)//.SetSkipIf(x => x.Id > 1)
                .EndIf()
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.Execute(model);
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
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .Finish();

            var result = testPipe.Execute(model);
            Assert.Equal(ExecutionResult.Successful, result.Success);
            Assert.True(model.Id == 3);
        }

        [Fact]
        public void Pipe_Execution_Sequential_SubPipe()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var testSubPipe = new TestSubPipe();
            var model = new TestModel
            {
                Id = 0,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .AddExecutor(testSubPipe)
                .Finish();

            var result = testPipe.Execute(model);
            Assert.True(testPipe.Elapsed.TotalMilliseconds > 12000);
            Assert.True(model.Id == 6);
        }

        [Fact]
        public void Pipe_Execution_Sequential_SubPipe_Skip()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            //var pipeExecutor = new TestPipeExecutor();
            var testSubPipe = new TestSubPipe();
            var model = new TestModel
            {
                Id = 0,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .AddExecutor(testSubPipe, false)
                .Finish();

            var result = testPipe.Execute(model);
            Assert.True(testPipe.Elapsed.TotalMilliseconds < 7000);
            Assert.True(model.Id == 3);
        }

        [Fact]
        public void Pipe_Execution_Sequential_SubPipe_Cache()
        {
            var executor1 = new TestExecutor1();
            var executor2 = new TestExecutor2();
            var executor3 = new TestExecutor3();
            var testSubPipe = new TestSubPipe();
            var model = new TestModel
            {
                Id = 0,
                Name = "model 1",
                Milliseconds = 2000
            };
            Pipe<TestModel, TestResult> testPipe = new TestPipe();
            testPipe
                .UseLocalCache()
                .UseStopWatch()
                .AsSequential()
                .AddExecutor(executor1)
                .AddExecutor(executor2)
                .AddExecutor(executor3)
                .AddExecutor(testSubPipe)
                .Finish();

            var result = testPipe.Execute(model);
            Assert.True(testPipe.Elapsed.TotalMilliseconds > 12000);
            Assert.True(model.Id == 6);
            Assert.True(result.Value.Value.Id == 33);
            Assert.True(result.Value.Value.Text == "first cache object");
        }
    }
}
