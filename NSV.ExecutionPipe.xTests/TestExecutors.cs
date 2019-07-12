using NSV.ExecutionPipe.Executors;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.xTests
{
    public class TestExecutor1 : Executor<TestModel, TestResult>
    {
        public TestExecutor1()
        {
            IsAsync = true;
            //IsAsync = false;
        }
        public override PipeResult<TestResult> Execute(TestModel model)
        {
            throw new NotImplementedException();
            //Thread.Sleep(model.Milliseconds);
            //model.Id += 1;
            //var testResult = new TestResultB
            //{
            //    Id = 111,
            //    Text = "Test Result 1"
            //};
            //return PipeResult<TestResult>.DefaultUnSuccessful
            //    .SetValue(testResult);
        }

        public override async Task<PipeResult<TestResult>> ExecuteAsync(TestModel model)
        {
            LocalCache.SetObject("key", "first cache object");

            await Task.Delay(model.Milliseconds);
            model.Id += 1;
            var testResult = new TestResult
            {
                Id = 1111,
                Text = "Test Result 1"
            };
            return PipeResult<TestResult>.DefaultUnSuccessful.SetValue(testResult);
        }
    }

    public class TestExecutor2 : Executor<TestModel, TestResult>
    {
        public TestExecutor2()
        {
            IsAsync = false;
        }
        public override PipeResult<TestResult> Execute(TestModel model)
        {
            Thread.Sleep(model.Milliseconds);
            model.Id += 1;
            var testResult = new TestResult
            {
                Id = 2,
                Text = "Test Result 2"
            };
            return PipeResult<TestResult>.DefaultSuccessfulBreak.SetValue(testResult);
        }

        public override async Task<PipeResult<TestResult>> ExecuteAsync(TestModel model)
        {
            await Task.Delay(model.Milliseconds);
            model.Id += 1;
            var testResult = new TestResult
            {
                Id = 2,
                Text = "Test Result 2"
            };
            return PipeResult<TestResult>.DefaultSuccessfulBreak.SetValue(testResult);
        }
    }

    public class TestExecutor3 : Executor<TestModel, TestResult>
    {
        public TestExecutor3()
        {
            IsAsync = false;
        }
        public override PipeResult<TestResult> Execute(TestModel model)
        {
            Thread.Sleep(model.Milliseconds);
            model.Id += 1;
            var testResult = new TestResult
            {
                Id = 3,
                Text = "Test Result 3"
            };
            return PipeResult<TestResult>.DefaultSuccessful.SetValue(testResult);
        }

        public override async Task<PipeResult<TestResult>> ExecuteAsync(TestModel model)
        {
            await Task.Delay(model.Milliseconds);
            model.Id += 1;
            var testResult = new TestResult
            {
                Id = 3,
                Text = "Test Result 3"
            };
            return PipeResult<TestResult>.DefaultSuccessful.SetValue(testResult);
        }
    }

    //public class TestPipeExecutor : PipeExecutor<TestModel, TestResult>
    //{
    //    public TestPipeExecutor()
    //    {
    //        IsAsync = true;
    //    }
    //    public override PipeResult<TestResult> Execute(TestModel model)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override async Task<PipeResult<TestResult>> ExecuteAsync(TestModel model)
    //    {
    //        //TODO: add to readme this feature
    //        if (SubPipeResult.Break)
    //            return SubPipeResult;

    //        await Task.Delay(model.Milliseconds);
    //        model.Id += 1;
    //        var testResult = new TestResult
    //        {
    //            Id = 4,
    //            Text = "Test Result 4"
    //        };
    //        return PipeResult<TestResult>.DefaultSuccessful.SetValue(testResult);
    //    }
    //}

    public class TestSubExecutor1 : Executor<TestModel, TestResult>
    {
        public TestSubExecutor1()
        {
            IsAsync = true;
        }
        public override PipeResult<TestResult> Execute(TestModel model)
        {
            throw new NotImplementedException();
        }

        public override async Task<PipeResult<TestResult>> ExecuteAsync(TestModel model)
        {
            await Task.Delay(model.Milliseconds);
            model.Id += 1;
            var testResult = new TestResult
            {
                Id = 11,
                Text = "Test Result 11"
            };
            return PipeResult<TestResult>.DefaultSuccessful.SetValue(testResult);
        }
    }

    public class TestSubExecutor2 : Executor<TestModel, TestResult>
    {
        public TestSubExecutor2()
        {
            IsAsync = false;
        }
        public override PipeResult<TestResult> Execute(TestModel model)
        {
            Thread.Sleep(model.Milliseconds);
            model.Id += 1;
            var testResult = new TestResult
            {
                Id = 22,
                Text = "Test Result 22"
            };
            return PipeResult<TestResult>.DefaultSuccessfulBreak.SetValue(testResult);
        }

        public override Task<PipeResult<TestResult>> ExecuteAsync(TestModel model)
        {
            throw new NotImplementedException();
        }
    }

    public class TestSubExecutor3 : Executor<TestModel, TestResult>
    {
        public TestSubExecutor3()
        {
            IsAsync = false;
        }
        public override PipeResult<TestResult> Execute(TestModel model)
        {
            var val = LocalCache.GetObject<string>("key");
            Thread.Sleep(model.Milliseconds);
            model.Id += 1;
            var testResult = new TestResult
            {
                Id = 33,
                Text = val
            };
            return PipeResult<TestResult>.DefaultSuccessful.SetValue(testResult);
        }

        public override Task<PipeResult<TestResult>> ExecuteAsync(TestModel model)
        {
            throw new NotImplementedException();
        }
    }
}
