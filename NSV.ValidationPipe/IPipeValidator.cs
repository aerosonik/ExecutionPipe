using NSV.ExecutionPipe;
using NSV.ExecutionPipe.Pipes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NSV.ValidationPipe
{
    public interface IPipeValidator<TModel>: 
        IValidator<TModel>, 
        IValidatorAsync<TModel>,
        ILocalCache
    {
        IPipeValidator<TModel> If(Func<TModel, bool> condition);
        IPipeValidator<TModel> EndIf();
        IPipeValidator<TModel> EndAllIf();

        IFieldValidatorCreator<TModel, TField> For<TField>(
            Expression<Func<TModel, TField>> field);
        IFieldValidatorCreator<TModel, TField> ForCollection<TField>(
            Expression<Func<TModel, IEnumerable<TField>>> field);

        IPipeValidator<TModel> AsParallel();

        IPipeValidator<TModel> ImportLocalCache(ILocalCache cache);
        IPipeValidator<TModel> UseLocalCacheThreadSafe();
        IPipeValidator<TModel> UseLocalCache();

        Task<ValidateResult> ExecuteAsync(TModel model);

    }


    public interface IFieldValidatorExecutor<TModel>
    {
        Task<ValidateResult> ExecuteValidationAsync(TModel model);
    }

    public interface IValidator<TField>
    {
        ValidateResult Validate(TField field);
    }

    public interface IValidatorAsync<TField>
    {
        Task<ValidateResult> ValidateAsync(TField field);
    }

   
}
