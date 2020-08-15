using NSV.ExecutionPipe.Builders.Async.Parallel;
using NSV.ExecutionPipe.Builders.Async.Sequential;
using System;

namespace NSV.ExecutionPipe.Builders
{
    internal class InternalPipeBuilder<M, R> : IPipeBuilder<M, R>
    {
        private readonly IServiceProvider _provider;
        internal InternalPipeBuilder(IServiceProvider provider)
        {
            _provider = provider;
        }
        public IAsyncParallelPipeBuilder<M, R> AsyncParallelPipe()
        {
            return new AsyncParallelPipeBuilder<M, R>(_provider);
        }

        public IAsyncSequentialPipeBuilder<M, R> AsyncPipe()
        {
            return new AsyncSequentialPipeBuilder<M, R>(_provider);
        }
    }
}
