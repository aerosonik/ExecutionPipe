using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NSV.ExecutionPipe.Builders;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;

namespace NSV.ExecutionPipe.Pool
{
    internal class AsyncExecutionPool<M, R> : IAsyncExecutionPool<M, R>
    {
        private readonly int _initialCount = 0;
        private readonly int _maxCount = 0;
        private readonly int _increaseRatio = 2;
        private int _currentCount = 0;
        private readonly ConcurrentStack<IAsyncPipe<M, R>> _stack;
        private readonly Func<IPipeBuilder<M, R>, IAsyncPipe<M, R>> _factory;
        private readonly IPipeBuilder<M, R> _builder;
        private readonly object _increaseLock = new object();
        private readonly object _decreaseLock = new object();

        public int PoolSize => _currentCount;

        internal AsyncExecutionPool(
            IPipeBuilder<M, R> builder,
            Func<IPipeBuilder<M, R>,IAsyncPipe<M, R>> factory,
            int initialCount,
            int maxCount,
            int increaseRatio = 2)
        {
            _stack = new ConcurrentStack<IAsyncPipe<M, R>>();
            _builder = builder;
            _factory = factory;
            _initialCount = initialCount;
            _maxCount = maxCount;
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
                _stack.Push(_factory(_builder));
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
                    if (_currentCount + i >= _maxCount)
                        break;
                    _stack.Push(_factory(_builder));
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
                    if (!_stack.TryPop(out var _))
                        return;
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
                if(_currentCount < _maxCount)
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
