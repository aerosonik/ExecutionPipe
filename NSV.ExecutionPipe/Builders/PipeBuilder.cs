using NSV.ExecutionPipe.Builders.Async;
using NSV.ExecutionPipe.Builders.Async.Parallel;
using NSV.ExecutionPipe.Builders.Async.Sequential;
using NSV.ExecutionPipe.Builders.Sync;
using NSV.ExecutionPipe.Builders.Sync.Sequential;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Builders
{
    public class PipeBuilder
    {
        #region Async

        #region Sequential
        public static IAsyncSequentialPipeBuilder<M, R> AsyncPipe<M,R>()
        {
            return new AsyncSequentialPipeBuilder<M, R>();
        }
        public static IAsyncSequentialPipeBuilder<M> AsyncPipe<M>()
        {
            //return new AsyncSequentialPipeBuilder<M>();
            throw new NotImplementedException("PipeBuilder.AsyncPipe<M>()");
        }
        public static IAsyncSequentialPipeBuilder AsyncPipe()
        {
            //return new AsyncSequentialPipeBuilder();
            throw new NotImplementedException("PipeBuilder.AsyncPipe()");
        }
        #endregion

        #region Parallel

        public static IAsyncParallelPipeBuilder<M, R> AsyncParallelPipe<M, R>()
        {
            return new AsyncParallelPipeBuilder<M, R>();
        }
        public static IAsyncParallelPipeBuilder<M> AsyncParallelPipe<M>()
        {
            //return new AsyncSequentialPipeBuilder<M>();
            throw new NotImplementedException("PipeBuilder.AsyncParallelPipe<M>()");
        }
        public static IAsyncParallelPipeBuilder AsyncParallelPipe()
        {
            //return new AsyncSequentialPipeBuilder();
            throw new NotImplementedException("PipeBuilder.AsyncParallelPipe()");
        }

        #endregion

        #endregion

        #region Sync

        #region Sequential
        public static ISyncSequentialPipeBuilder<M, R> SyncPipe<M, R>()
        {
            throw new NotImplementedException("PipeBuilder.SyncPipe<M,R>()");
            //return new AsyncSequentialPipeBuilder<M, R>();
        }
        public static ISyncSequentialPipeBuilder<M> SyncPipe<M>()
        {
            //return new AsyncSequentialPipeBuilder<M>();
            throw new NotImplementedException("PipeBuilder.SyncPipe<M>()");
        }
        public static ISyncSequentialPipeBuilder SyncPipe()
        {
            //return new AsyncSequentialPipeBuilder();
            throw new NotImplementedException("PipeBuilder.SyncPipe()");
        }
        #endregion

        #region Parallel
        #endregion

        #endregion

    }
}
