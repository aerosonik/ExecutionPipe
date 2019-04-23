using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSV.ValidationPipe
{
    public interface IFieldValidatorCreator<TModel, TField>
    {
        IFieldValidatorCreator<TModel, TField> Path(string path);
        IFieldValidatorCreator<TModel, TField> When(Func<TModel, bool> condition);
        IFieldValidatorCreator<TModel, TField> Set(IValidator<TField> validator);
        IFieldValidatorCreator<TModel, TField> Set(IValidatorAsync<TField> validator);
        IFieldValidatorCreator<TModel, TField> AsParallel();

        IValidatorCreator<TModel, TField> Must(Func<TField, bool> must);
        IValidatorCreator<TModel, TField> Must(Func<TField, Task<bool>> must);
        IPipeValidator<TModel> Add();
    }

    public interface IValidatorCreator<TModel, TField>
    {
        IFieldValidatorCreator<TModel, TField> WithMessage(string message);
    }
}
