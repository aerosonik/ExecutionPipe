using NSV.ExecutionPipe.Cache;
using NSV.ExecutionPipe.Containers;
using NSV.ExecutionPipe.Containers.Async;
using NSV.ExecutionPipe.Models;
using System;
using System.Collections.Generic;

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

        protected Func<IPipeCache> _cacheFunc = () => null;

        protected IPipeCache _cacheObject;

        protected List<PipeResult<R>> _results;

        protected Func<M, PipeResult<R>[], PipeResult<R>> _returnHandler;

        protected bool _useStopWatch = false;

        protected (
            ExecutorSettings<M, R> Settings,
            IAsyncContainer<M, R> Container
            )[] _queue;

        protected Optional<(ExecutorSettings<M, R> Settings,
                IAsyncContainer<M, R> Container)> DefaultExecutor;

        #region IPipeSettings<M, R>

        void IPipeSettings<M, R>.SetCache(Func<IPipeCache> cache)
        {
            _cacheFunc = cache;
        }

        void IPipeSettings<M, R>.SetExecutors((
            ExecutorSettings<M, R> Settings,
            IAsyncContainer<M, R> Container)[] queue,
            Optional<(ExecutorSettings<M, R> Settings,
                IAsyncContainer<M, R> Container)> defaultExecutor)
        {
            _queue = queue;
            DefaultExecutor = defaultExecutor;
        }

        void IPipeSettings<M, R>.SetReturn(
            Func<M, PipeResult<R>[], 
            PipeResult<R>> handler)
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
