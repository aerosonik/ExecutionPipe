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
        public override PipeResult<TestResult> Execute(TestModel model)
        {
            Thread.Sleep(model.Milliseconds);
            model.Id += 1;
            var testResult = new TestResult
            {
                Id = 1,
                Text = "Test Result 1"
            };
            return PipeResult<TestResult>.DefaultUnSuccessful.SetValue(testResult);
        }

        public override async Task<PipeResult<TestResult>> ExecuteAsync(TestModel model)
        {
            await Task.Delay(model.Milliseconds);
            model.Id += 1;
            var testResult = new TestResult
            {
                Id = 1,
                Text = "Test Result 1"
            };
            return PipeResult<TestResult>.DefaultSuccessful.SetValue(testResult);
        }
    }

    public class TestExecutor2 : Executor<TestModel, TestResult>
    {
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
}
