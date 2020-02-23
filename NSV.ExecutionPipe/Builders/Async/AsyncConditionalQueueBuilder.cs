using NSV.ExecutionPipe.Containers;
using NSV.ExecutionPipe.Containers.Async;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NSV.ExecutionPipe.Builders.Async
{
    internal class AsyncConditionalQueueBuilder<M,R>
    {
        private Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>> _ifStack
                = Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>>.Default;
        private readonly Queue<(ExecutorSettings<M, R> Settings, IAsyncContainer<M, R> Container)> _queue 
            = new Queue<(ExecutorSettings<M, R> Settings, IAsyncContainer<M, R> Container)>();
        private Optional<(ExecutorSettings<M, R> Settings, IAsyncContainer<M, R> Container)> _defaultExecutor
            = Optional<(ExecutorSettings<M, R> Settings, IAsyncContainer<M, R> Container)>.Default;

        internal void AddIfCondition(bool condition)
        {
            if (!_ifStack.HasValue)
                _ifStack = new Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>();

            var ifCondition = (Optional<Func<M, bool>>.Default, condition);
            _ifStack.Value.Push(ifCondition);
        }

        internal void AddFuncIfCondition(Func<M, bool> condition)
        {
            if (condition == null)
                return;

            if (!_ifStack.HasValue)
                _ifStack = new Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>();

            var ifCondition = (condition, Optional<bool>.Default);
            _ifStack.Value.Push(ifCondition);
        }

        internal void RemoveIfCondition()
        {
            if (!_ifStack.HasValue || !_ifStack.Value.Any())
                throw new Exception("Redundant EndIf");

            _ifStack.Value.Pop();
        }

        internal Func<M, bool>[] GetFuncIfConditions()
        {
            if (!_ifStack.HasValue || !_ifStack.Value.Any(x => x.calculated.HasValue))
                return Optional<Func<M, bool>[]>.Default;

            return _ifStack.Value
                        .Where(x => x.calculated.HasValue)
                        .Select(x => x.calculated.Value)
                        .ToArray();
        }

        internal bool IfConstantConditions()
        {
            if (!_ifStack.HasValue)
                return true;

            return !_ifStack.Value
                .Where(x => x.constant.HasValue)
                .Any(x => !x.constant.Value);
        }

        internal void Enque(
            ExecutorSettings<M, R> Settings, 
            IAsyncContainer<M, R> Container)
        {
            _queue.Enqueue((Settings, Container));
        }

        internal void SetDefault(
            ExecutorSettings<M, R> Settings,
            IAsyncContainer<M, R> Container)
        {
            _defaultExecutor = new Optional<(ExecutorSettings<M, R> Settings, 
                IAsyncContainer<M, R> Container)>((Settings, Container));
        }
        internal Optional<(ExecutorSettings<M, R> Settings, IAsyncContainer<M, R> Container)> GetDefault()
        {
            return _defaultExecutor;
        }

        internal Queue<(ExecutorSettings<M, R> Settings, IAsyncContainer<M, R> Container)> GetQueue()
        {
            return _queue;
        }

        internal (ExecutorSettings<M, R> Settings, IAsyncContainer<M, R> Container)[] GetArray()
        {
            return _queue.ToArray();
        }
    }
}
