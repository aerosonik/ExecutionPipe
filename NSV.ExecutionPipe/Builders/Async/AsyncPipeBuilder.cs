using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders.Async
{
    internal abstract class AsynPipeBuilder<M, R>: IAsynPipeBuilder<M, R>
    {
        protected AsyncConditionalQueueBuilder<M, R> _queueBuilder = new AsyncConditionalQueueBuilder<M, R>();
        protected IPipeSettings<M, R> _pipe;
        public IAsyncPipe<M, R> Return(Func<M, PipeResult<R>[], PipeResult<R>> rersultHandler)
        {
            _pipe.SetExecutors(_queueBuilder.GetArray(), _queueBuilder.GetDefault());
            _pipe.SetReturn(rersultHandler);
            return (IAsyncPipe<M, R>)_pipe;
        }
    }

    public class AsynPipeBuilder<M>: IAsynPipeBuilder<M>
    {
        public IAsyncPipe<M> Return(Func<M, PipeResult[], PipeResult> rersultHandler)
        {
            throw new NotImplementedException();
        }
    }

    public class AsynPipeBuilder: IAsynPipeBuilder
    {
        public IAsyncPipe Return(Func<PipeResult[], PipeResult> rersultHandler)
        {
            throw new NotImplementedException();
        }
    }
}
