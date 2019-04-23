using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ValidationPipe
{
    public interface IPipeValidationCreator<TModel, TField>
    {
        IPipeValidationCreator<TModel, TField> Path(string path);
        IPipeValidationCreator<TModel, TField> Pipe(IPipeValidator<TField> pipe);
        IPipeValidationCreator<TModel, TField> When(Func<TModel, bool> condition);
        IPipeValidator<TModel> Add();
    }
}
