using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NSV.ExecutionPipe;
using NSV.ExecutionPipe.Pipes;

namespace NSV.ValidationPipe
{
   internal class FieldValidatorCreator<TModel, TField> :
        IFieldValidatorCreator<TModel, TField>,
        IValidatorCreator<TModel, TField>,
        IFieldValidatorExecutor<TModel>
    {
        private readonly Optional<Func<TModel, bool>[]> _ifConditions = Optional<Func<TModel, bool>[]>.Default;
        private readonly Expression<Func<TModel, TField>> _field;
        private readonly Expression<Func<TModel, IEnumerable<TField>>> _collectionField;
        private readonly PipeValidator<TModel> _pipe;
        private readonly bool _isCollectionField = false;
        private bool _asParallel = false;
        private Func<TModel, bool> _when = (_) => true;
        private string _path = string.Empty;

        private List<ValidatorStruture<TField>> _queue;
        private ValidatorStruture<TField> _current;

        internal FieldValidatorCreator(
            PipeValidator<TModel> pipe,
            Expression<Func<TModel, TField>> field,
            params Func<TModel, bool>[] ifConditions) : this(pipe)
        {
            _field = field;
        }
        internal FieldValidatorCreator(
            PipeValidator<TModel> pipe,
            Expression<Func<TModel, IEnumerable<TField>>> field,
            params Func<TModel, bool>[] ifConditions) : this(pipe)
        {
            _collectionField = field;
            _isCollectionField = true;
        }
        private FieldValidatorCreator(
            PipeValidator<TModel> pipe, 
            params Func<TModel, bool>[] ifConditions)
        {
            _pipe = pipe;
            _current = new ValidatorStruture<TField>();
            _ifConditions = ifConditions;
        }

        public IPipeValidator<TModel> Add()
        {
            if (_queue != null && _current.StructureType != ValidatorStrutureType.Default)
            {
                _queue.Add(_current);
                _current = new ValidatorStruture<TField>();
            }
            _pipe.AddFieldValidator(this);
            return _pipe;
        }

        //take effect only if collection
        public IFieldValidatorCreator<TModel, TField> AsParallel()
        {
            _asParallel = true;
            return this;
        }

        public IValidatorCreator<TModel, TField> Must(Func<TField, bool> must)
        {
            CheckQueueAndCurrentValidator();
            _current = new ValidatorStruture<TField>(must);
            return this;
        }

        public IValidatorCreator<TModel, TField> Must(Func<TField, Task<bool>> must)
        {
            CheckQueueAndCurrentValidator();
            _current = new ValidatorStruture<TField>(must);
            return this;
        }

        IFieldValidatorCreator<TModel, TField> IValidatorCreator<TModel, TField>.WithMessage(string message)
        {
            _current.Message = message;
            return this;
        }

        public IFieldValidatorCreator<TModel, TField> Path(string path)
        {
            _path = path;
            return this;
        }

        public IFieldValidatorCreator<TModel, TField> Set(IValidator<TField> validator)
        {
            CheckQueueAndCurrentValidator();
            _current = new ValidatorStruture<TField>(validator);
            return this;
        }

        public IFieldValidatorCreator<TModel, TField> Set(IValidatorAsync<TField> validatorAsync)
        {
            CheckQueueAndCurrentValidator();
            _current = new ValidatorStruture<TField>(validatorAsync);
            return this;
        }

        public IFieldValidatorCreator<TModel, TField> When(Func<TModel, bool> condition)
        {
            _when = condition;
            return this;
        }

        async Task<ValidateResult> IFieldValidatorExecutor<TModel>.ExecuteValidationAsync(TModel model)
        {
            if (!CheckIfConditions(model))
                return ValidateResult.DefaultValid;

            if (_isCollectionField)
            {
                IEnumerable<TField> fields = _collectionField.Compile().Invoke(model);
                Task<ValidateResult>[] tasks = null;
                if (_asParallel)
                {
                    tasks = fields
                        .AsParallel()
                        .Select(async x => await InvokeValidationForField(x, _current))
                        .ToArray();
                }
                else
                {
                    tasks = fields
                        .Select(async x => await InvokeValidationForField(x, _current))
                        .ToArray();
                }
                var results = new ValidateResult();
                results.SubResults = await Task.WhenAll(tasks);
                results.SubResults = results.SubResults
                    .Value.SelectMany(x => x.SubResults.Value)
                    .ToArray();
                if (results.SubResults.Value.Any(x => !x.IsValid))
                    results.IsValid = false;
                return results;
            }
            else
            {
                TField field = _field.Compile().Invoke(model);
                return await InvokeValidationForField(field, _current);
            }
        }

        private async Task<ValidateResult> InvokeValidationForField(
            TField field,
            ValidatorStruture<TField> current)
        {
            if (_queue != null && _queue.Count > 0)
            {
                return await InvokeValidationForQueue(field);
            }
            else
            {
                return await InvokeValidationForCurrent(field, current);
            }
        }

        private async Task<ValidateResult> InvokeValidationForQueue(TField field)
        {
            var tasks = _queue
                .Where(x => x.StructureType == ValidatorStrutureType.FuncAsync)
                .Select(async x =>
                {
                    var result = await x.MustAsync(field);
                    return result
                        ? ValidateResult.DefaultValid
                        : ValidateResult.DefaultFailed
                            .SetErrorMessage(x.Message)
                            .SetPath(_path);
                }).Union(_queue
                    .Where(x => x.StructureType == ValidatorStrutureType.ValidatorAsync)
                    .Select(async x =>
                    {
                        var result = await x.ValidatorAsync.ValidateAsync(field);
                        if (!result.IsValid)
                            result.SetPath(_path);
                        return result;
                    }));

            var results = new ValidateResult();
            results.SubResults = _queue
                .Where(x => x.StructureType == ValidatorStrutureType.Func)
                .Select(x =>
                {
                    var result = x.Must(field);
                    return result
                        ? ValidateResult.DefaultValid
                        : ValidateResult.DefaultFailed
                            .SetErrorMessage(x.Message)
                            .SetPath(_path);
                }).Union(_queue
                        .Where(x => x.StructureType == ValidatorStrutureType.Validator)
                        .Select(x =>
                        {
                            var result = x.Validator.Validate(field);
                            if (!result.IsValid)
                                result.SetPath(_path);
                            return result;
                        }))
                .ToArray();

            var subResults = await Task.WhenAll(tasks);

            results.SubResults = results.SubResults.Value.Concat(subResults).ToArray();
            if (results.SubResults.Value.Any(x => !x.IsValid))
                results.IsValid = false;

            return results;
        }

        private async Task<ValidateResult> InvokeValidationForCurrent(
            TField field,
            ValidatorStruture<TField> current)
        {
            switch (current.StructureType)
            {
                case ValidatorStrutureType.Func:
                    if (!_current.Must(field))
                        return ValidateResult.DefaultFailed
                            .SetErrorMessage(current.Message)
                            .SetPath(_path);
                    return ValidateResult.DefaultValid;

                case ValidatorStrutureType.FuncAsync:
                    if (!await _current.MustAsync(field))
                        return ValidateResult.DefaultFailed
                            .SetErrorMessage(current.Message)
                            .SetPath(_path);
                    return ValidateResult.DefaultValid;

                case ValidatorStrutureType.Validator:
                    var result = current.Validator.Validate(field);
                    if (!result.IsValid)
                        result.SetPath(_path);
                    return result;

                case ValidatorStrutureType.ValidatorAsync:
                    var asyncResult = await current.ValidatorAsync.ValidateAsync(field);
                    if (!asyncResult.IsValid)
                        asyncResult.SetPath(_path);
                    return asyncResult;

                default:
                    return ValidateResult.DefaultValid;
            }
        }

        private void CheckQueueAndCurrentValidator()
        {
            if (_current.StructureType != ValidatorStrutureType.Default)
            {
                if (_queue == null)
                    _queue = new List<ValidatorStruture<TField>>();
                _queue.Add(_current);
            }
        }

        private bool CheckIfConditions(TModel model)
        {
            if (!_ifConditions.HasValue && _when(model))
                return true;

            if (_ifConditions.Value.Select(x => x(model)).All(x => x) && _when(model))
                return true;

            return false;
        }
    }

    internal struct ValidatorStruture<TField>
    {
        public ValidatorStruture(ValidatorStrutureType validatorStrutType = ValidatorStrutureType.Default)
        {
            StructureType = validatorStrutType;
            Message = null;
            Must = null;
            MustAsync = null;
            Validator = null;
            ValidatorAsync = null;
        }
        public ValidatorStruture(
            Func<TField, bool> must)
        {
            StructureType = ValidatorStrutureType.Func;
            Must = must;
            Message = null;

            MustAsync = null;
            Validator = null;
            ValidatorAsync = null;
        }
        public ValidatorStruture(
            Func<TField, Task<bool>> mustAsync)
        {
            StructureType = ValidatorStrutureType.FuncAsync;
            MustAsync = mustAsync;
            Message = null;

            Must = null;
            Validator = null;
            ValidatorAsync = null;
        }
        public ValidatorStruture(IValidator<TField> validator)
        {
            StructureType = ValidatorStrutureType.Validator;
            Validator = validator;

            Message = null;
            Must = null;
            MustAsync = null;
            ValidatorAsync = null;
        }
        public ValidatorStruture(IValidatorAsync<TField> validatorAsync)
        {
            StructureType = ValidatorStrutureType.ValidatorAsync;
            ValidatorAsync = validatorAsync;

            Message = null;
            Must = null;
            MustAsync = null;
            Validator = null;
        }
        public ValidatorStrutureType StructureType { get; }
        public string Message { get; set; }
        public Func<TField, bool> Must { get; }
        public Func<TField, Task<bool>> MustAsync { get; }
        public IValidator<TField> Validator { get; }
        public IValidatorAsync<TField> ValidatorAsync { get; }
    }

    internal enum ValidatorStrutureType
    {
        Default,
        Func,
        FuncAsync,
        Validator,
        ValidatorAsync
    }

}
