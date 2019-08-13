using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.PipeLines.Infrastructure;
using NSV.ExecutionPipe.PipeLines.Interfaces;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.PipeLines.Implementations
{
    public abstract class AbstractPipe<M, R>: ICacheContainer
    {
        protected bool _finished = false;
        protected Optional<M> _model = Optional<M>.Default;
        protected Optional<Stopwatch> _stopWatch = Optional<Stopwatch>.Default;
        protected Optional<List<PipeResult<R>>> _results = Optional<List<PipeResult<R>>>.Default;
        protected Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>> _ifConditionStack
                = Optional<Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>>.Default;

        public IPipeCache Cache { get; set; } = PipeCacheObject.GetDefaultCache();

        protected bool IfConstantConditions()
        {
            if (!_ifConditionStack.HasValue)
                return true;

            return !_ifConditionStack.Value
                .Where(x => x.constant.HasValue)
                .Any(x => !x.constant.Value);
        }
        protected Optional<Func<M, bool>[]> GetCalculatedConditions()
        {
            if (!_ifConditionStack.HasValue || !_ifConditionStack.Value.Any(x => x.calculated.HasValue))
                return Optional<Func<M, bool>[]>.Default;


            return Optional<Func<M, bool>[]>.SetValue(_ifConditionStack.Value
                        .Where(x => x.calculated.HasValue)
                        .Select(x => x.calculated.Value)
                        .ToArray());
        }
        protected void IfCondition(bool condition)
        {
            if (!_ifConditionStack.HasValue)
                _ifConditionStack = new Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>();
            var ifCondition = (Optional<Func<M, bool>>.Default, condition);
            _ifConditionStack.Value.Push(ifCondition);
        }
        protected void IfCondition(Func<M, bool> condition)
        {
            if (condition == null)
                return;

            if (!_ifConditionStack.HasValue)
                _ifConditionStack = new Stack<(Optional<Func<M, bool>> calculated, Optional<bool> constant)>();
            var ifCondition = (condition, Optional<bool>.Default);
            _ifConditionStack.Value.Push(ifCondition);
        }
        protected void EndIfCondition()
        {
            if (!_ifConditionStack.HasValue || !_ifConditionStack.Value.Any())
                throw new Exception("Redundant EndIf");

            _ifConditionStack.Value.Pop();
        }
        public void LocalCacheThreadSafe()
        {
            Cache = PipeCacheObject.GetCache();
        }
        public void LocalCache()
        {
            Cache = PipeCacheObject.GetThreadSafeCache();
        }
        internal bool RunExecutorIfConditions(IBaseExecutorContainer<M, R> container, M model)
        {
            return container.ExecuteConditions.HasValue
                ? container.ExecuteConditions.Value.All(x => x(model))
                : true;
        }
        internal bool Break(IBaseExecutorContainer<M, R> container, PipeResult<R> result)
        {
            return (result.Success == ExecutionResult.Failed &&
                        container.BreakIfFailed) ||
                   (result.Break && container.AllowBreak);
        }
        protected PipeResult<R> RunSync(M model, IPipeCache cache, Func<PipeResult<R>> func)
        {
            _model = model;
            if (_useParentalCache)
                _externalCache = Optional<IPipeCache>.SetValue(cache);

            if (!_stopWatch.HasValue)
                return func();

            _stopWatch.Value.Start();
            var result = func();
            _stopWatch.Value.Stop();
            return result.SetElapsed(_stopWatch.Value.Elapsed);
        }
        protected async Task<PipeResult<R>> RunAsync(M model, IPipeCache cache, Func<Task<PipeResult<R>>> func)
        {
            _model = model;
            if (_useParentalCache)
                _externalCache = Optional<IPipeCache>.SetValue(cache);

            if (!_stopWatch.HasValue)
                return await func();

            _stopWatch.Value.Start();
            var result = await func();
            _stopWatch.Value.Stop();
            return result.SetElapsed(_stopWatch.Value.Elapsed);
        }

        internal void SetContainer<TContainer, TExecutor>(
            Func<TContainer> containerFunc,
            bool addif,
            //Queue<TContainer> executionQueue,
            TContainer current)
            where TContainer : BaseExecutorContainer<M, R>
        {
            if (!addif)
                return;

            if (IfConstantConditions())
            {
                var container = containerFunc();

                container.LocalCache = this;
                container.ExecuteConditions = GetCalculatedConditions();

                //executionQueue.Enqueue(container);
                current = container;
            }
        }
    }
}
