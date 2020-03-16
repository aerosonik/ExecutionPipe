using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;

namespace NSV.ExecutionPipe.Pool
{
    internal class AsyncExecutionPool<M, R> : IAsyncExecutionPool<M, R>
    {
        private readonly int _initialCount = 0;
        private readonly int _macCount = 0;
        private readonly int _increaseRatio = 2;
        private int _currentCount = 0;
        private readonly ConcurrentStack<IAsyncPipe<M, R>> _stack;
        private readonly Func<IAsyncPipe<M, R>> _factory;
        private readonly object _increaseLock = new object();
        private readonly object _decreaseLock = new object();

        public int PoolSize => _currentCount;

        internal AsyncExecutionPool(
            Func<IAsyncPipe<M, R>> factory,
            int initialCount,
            int maxCount,
            int increaseRatio = 2)
        {
            _stack = new ConcurrentStack<IAsyncPipe<M, R>>();
            _factory = factory;
            _initialCount = initialCount;
            _macCount = maxCount;
            _increaseRatio = increaseRatio;
            InitPool();
        }

        public async Task<PipeResult<R>> ExecuteAsync(M model)
        {
            var pipe = GetPipe();
            var result = await pipe.ExecuteAsync(model);
            ReturnPipe(pipe);
            return result;
        }

        private void InitPool()
        {
            for (int i = 0; i < _initialCount; i++)
            {
                _stack.Push(_factory());
                _currentCount++;
            }
        }

        private void IncreasePool()
        {
            lock (_increaseLock)
            {
                var count = _currentCount * _increaseRatio - _currentCount;
                for (int i = 0; i < count; i++)
                {
                    if (_currentCount + i > _macCount)
                        break;
                    _stack.Push(_factory());
                    _currentCount++;
                }
            }
        }

        private void DecreasePool()
        {
            lock (_decreaseLock)
            {
                if (_currentCount < _initialCount * _increaseRatio)
                    return;

                var count = _currentCount / _increaseRatio;

                for (int i = 0; i < count; i++)
                {
                    if (_stack.TryPop(out var _))
                       _currentCount--;
                }
            }
        }

        private IAsyncPipe<M, R> GetPipe()
        {
            while (true)
            {
                if (_stack.TryPop(out var pipe))
                    return pipe;
                IncreasePool();
            }
        }

        private void ReturnPipe(IAsyncPipe<M, R> pipe)
        {
            _stack.Push(pipe);
            if (_currentCount > _initialCount)
                DecreasePool();
        }
    }
}
