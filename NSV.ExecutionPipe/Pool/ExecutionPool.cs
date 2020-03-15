using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;

namespace NSV.ExecutionPipe.Pool
{
    internal class ExecutionPool<M,R>: IAsyncExecutionPool<M,R>
    {
        private readonly int _initialCount = 0;
        private readonly int _macCount = 0;
        private readonly int _increaseRatio = 2;
        private int _currentCount = 0;
        private readonly ConcurrentStack<IAsyncPipe<M, R>> _stack;
        private readonly Func<IAsyncPipe<M, R>> _fabric;
        private readonly object _getPipeLock = new object();

        public async Task<PipeResult<R>> ExecuteAsync(M model)
        {
            var pipe = GetPipe();
            var result = await pipe.ExecuteAsync(model);
            _stack.Push(pipe);
            return result;
        }

        internal ExecutionPool(
            Func<IAsyncPipe<M, R>> fabric,
            int initialCount,
            int maxCount,
            int increaseRatio = 2)
        {
            _stack = new ConcurrentStack<IAsyncPipe<M, R>>();
            _fabric = fabric;
            _initialCount = initialCount;
            _macCount = maxCount;
            
            InitPool();
        }

        private void InitPool()
        {
            for(int i = 0; i< _initialCount; i++)
            {
                _stack.Push(_fabric());
            }
            _currentCount += _initialCount;
        }

        private void IncreasePool()
        {
            var count = _currentCount * _increaseRatio - _currentCount;

            for (int i = 0; i < count; i++)
            {
                _stack.Push(_fabric());
            }
            _currentCount += count;
        }

        private void DecreasePool()
        {
            var count = 0;
            if (_currentCount >= _initialCount * _increaseRatio)
                count = _currentCount / _increaseRatio;
            else
                return;

            var decreseCount = 0;
            for (int i = 0; i < count; i++)
            {
                if (_stack.TryPop(out var pipe))
                    decreseCount += 1;
            }
            _currentCount -= decreseCount;
        }

        private IAsyncPipe<M, R> GetPipe()
        {
            lock (_getPipeLock)
            {
                IAsyncPipe<M, R> pipe = null;
                if (_stack.TryPop(out pipe))
                    return pipe;

                while (pipe == null)
                {
                    IncreasePool();
                    if (_stack.TryPop(out pipe))
                        break;
                }
                return pipe;
            }
        }
    }
}
