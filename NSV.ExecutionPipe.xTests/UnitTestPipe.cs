using NSV.ExecutionPipe.Pipes;
using System;
using Xunit;

namespace NSV.ExecutionPipe.xTests
{
    public class UnitTestPipe
    {
        private Pipe<TestModel, TestResult> TestPipe;

        [Fact]
        public void Pipe_AsParallel()
        {
            //TestPipe = new Pipe<>()
        }
    }
}
