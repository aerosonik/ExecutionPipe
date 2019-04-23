using NSV.ExecutionPipe;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ValidationPipe
{
    public abstract class PipeValidator<TModel> : IPipeValidator<TModel>
    {
        #region Private Fields
        private bool _asParallel = false;
        private Queue<IFieldValidatorExecutor<TModel>> _validators;
        private Optional<ILocalCache> _externalCache = Optional<ILocalCache>.Default;
        private Optional<IDictionary<object, object>> _localCache = Optional<IDictionary<object, object>>.Default;
        private Optional<Stack<Func<TModel, bool>>> _ifConditionStack = Optional<Stack<Func<TModel, bool>>>.Default;
        #endregion

        #region IPipeValidator<TModel>
        public IPipeValidator<TModel> AsParallel()
        {
            _asParallel = true;
            return this;
        }

        public IFieldValidatorCreator<TModel, TField> For<TField>(
            Expression<Func<TModel, TField>> field)
        {
            if (!_ifConditionStack.HasValue || !_ifConditionStack.Value.Any())
                return new FieldValidatorCreator<TModel, TField>(this, field);

            return new FieldValidatorCreator<TModel, TField>(this, field, _ifConditionStack.Value.ToArray());
        }

        public IFieldValidatorCreator<TModel, TField> ForCollection<TField>(
            Expression<Func<TModel, IEnumerable<TField>>> field)
        {
            if (!_ifConditionStack.HasValue || !_ifConditionStack.Value.Any())
                return new FieldValidatorCreator<TModel, TField>(this, field);

            return new FieldValidatorCreator<TModel, TField>(this, field, _ifConditionStack.Value.ToArray());
        }

        public IPipeValidator<TModel> If(Func<TModel, bool> condition)
        {
            if (condition == null)
                return this;

            if (!_ifConditionStack.HasValue)
                _ifConditionStack = new Stack<Func<TModel, bool>>();

            _ifConditionStack.Value.Push(condition);
            return this;
        }
        public IPipeValidator<TModel> EndIf()
        {
            if (!_ifConditionStack.HasValue || !_ifConditionStack.Value.Any())
                throw new Exception("Redundant EndIf");

            _ifConditionStack.Value.Pop();
            return this;
        }

        public IPipeValidator<TModel> EndAllIf()
        {
            if (!_ifConditionStack.HasValue || !_ifConditionStack.Value.Any())
                throw new Exception("Redundant EndAllIf");

            _ifConditionStack.Value.Clear();
            return this;
        }

        public IPipeValidator<TModel> ImportLocalCache(ILocalCache cache)
        {
            CheckCacheObject();

            _externalCache = new Optional<ILocalCache>(cache);
            return this;
        }

        public IPipeValidator<TModel> UseLocalCacheThreadSafe()
        {
            CheckCacheObject();

            _localCache = new ConcurrentDictionary<object, object>();
            return this;
        }

        public IPipeValidator<TModel> UseLocalCache()
        {
            CheckCacheObject();

            _localCache = new Dictionary<object, object>();
            return this;
        }

        public async Task<ValidateResult> ExecuteAsync(TModel model)
        {
            if (_ifConditionStack.HasValue && _ifConditionStack.Value.Any())
                throw new Exception("Expected EndIf or EndAllIf operators");
            if (_ifConditionStack.HasValue)
                _ifConditionStack = Optional<Stack<Func<TModel, bool>>>.Default;

            ValidateResult[] results = null;
            if (_asParallel)
            {
                var resultTasks = _validators
                    .AsParallel()
                    .Select(async x => await x.ExecuteValidationAsync(model))
                    .ToArray();
                results = await Task.WhenAll(resultTasks);
            }
            else
            {
                var resultList = new List<ValidateResult>();
                while (_validators.Count > 0)
                {
                    var item = _validators.Dequeue();
                    resultList.Add(await item.ExecuteValidationAsync(model));
                }
                results = results.ToArray();
            }
            var result = ValidateResult.DefaultValid;
            result.SubResults = results.SelectMany(x => x.SubResults.Value).ToArray();
            if (result.SubResults.Value.Any(x => !x.IsValid))
                result.IsValid = false;
            return result;
        }
        #endregion

        #region IValidator<TField>, IValidatorAsync<TField>
        public abstract ValidateResult Validate(TModel model);
        public abstract Task<ValidateResult> ValidateAsync(TModel model);
        #endregion

        #region ILocalCache
        public object GetObject(object key)
        {
            if (!_localCache.HasValue && _externalCache.HasValue)
                return _externalCache.Value.GetObject(key);

            if (_localCache.HasValue && !_externalCache.HasValue)
                if (_localCache.Value.TryGetValue(key, out var value))
                    return value;

            return null;
        }

        public void SetObject(object key, object value)
        {
            if (!_localCache.HasValue && _externalCache.HasValue)
            {
                _externalCache.Value.SetObject(key, value);
                return;
            }
            if (_localCache.HasValue && !_externalCache.HasValue)
            {
                _localCache.Value.Add(key, value);
                return;
            }
        }

        public ILocalCache GetLocalCacheObject()
        {
            return this;
        }
        #endregion

        #region Private methods
        private void CheckCacheObject()
        {
            if (_externalCache.HasValue)
                throw new Exception("External cache already in use");

            if (_localCache.HasValue)
                throw new Exception("local cache already in use");
        }
        #endregion

        internal void AddFieldValidator(IFieldValidatorExecutor<TModel> validator)
        {
            if (_validators == null)
                _validators = new Queue<IFieldValidatorExecutor<TModel>>();

            _validators.Enqueue(validator);
        }
    }
}
