using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Containers;
using NSV.ExecutionPipe.Containers.Async;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Pipes
{
    internal abstract class PipeSettings<M, R>: IPipeSettings<M, R>
    {
        protected IPipeCache Cache
        {
            get
            {
                if (_cacheObject == null)
                {
                    _cacheObject = _cacheFunc();
                }
                return _cacheObject;
            }
            set
            {
                _cacheObject = value;
            }
        }
        protected Func<IPipeCache> _cacheFunc;
        protected IPipeCache _cacheObject;

        protected List<PipeResult<R>> _results = new List<PipeResult<R>>();

        protected Func<M, PipeResult<R>[], PipeResult<R>> _returnHandler;

        protected bool _useStopWatch = false;

        protected Queue<(
            ExecutorSettings<M, R> Settings,
            IAsyncContainer<M, R> Container
            )> _queue;

        #region IPipeSettings<M, R>

        void IPipeSettings<M, R>.SetCache(Func<IPipeCache> cache)
        {
            _cacheFunc = cache;
        }

        void IPipeSettings<M, R>.SetExecutors(Queue<(
            ExecutorSettings<M, R> Settings,
            IAsyncContainer<M, R> Container)> queue)
        {
            _queue = queue;
        }

        void IPipeSettings<M, R>.SetReturn(Func<M, PipeResult<R>[], PipeResult<R>> handler)
        {
            _returnHandler = handler;
        }

        void IPipeSettings<M, R>.SetStopWatch()
        {
            _useStopWatch = true;
        }
        #endregion
    }
}
