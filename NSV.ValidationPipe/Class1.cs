using NSV.ExecutionPipe;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NSV.ValidationPipe
{
    public interface IValidationPipe<M>: IValidator<M>
    {
        IValidatorCreator<M> NewValidator();
        IValidationPipeCreator<M> NewValidtionPipe();

        IValidationPipe<M> AddValidator(
            IValidator<M> validator,
            Expression<Func<M, object>> field);
        IValidationPipe<M> AddValidatorForEach<M1>(
            IValidator<M1> validator,
            Expression<Func<M, IEquatable<M1>>> collection);
        
        IValidationPipe<M> AddValidationPipe<M1>(
            IValidationPipe<M1> pipe,
            Expression<Func<M, M1>> field);

        IValidationPipe<M> SetSkikpIf(Func<M, bool> condition);
        IValidationPipe<M> SetValidateIf(Func<M, bool> condition);

        IValidationPipe<M> AsParallel();
    }

    public interface IValidatorCreator<M>
    {
        IValidatorCreator<M1> Field<M1>(Expression<Func<M, M1>> field);
        IValidatorCreator<M1> Collection<M1>(Expression<Func<M, IEquatable<M1>>> field);

        IValidatorCreator<M1> Validator<M1>(IValidator<M1> validator);
        IValidatorCreator<M> When<M1>(Func<M1, bool> condition);
        IValidatorCreator<M> When(Func<M, bool> condition);
        IValidationPipe<M> Add();
    }

    public interface IValidationPipeCreator<M>
    {
        IValidationPipeCreator<M> ForField<M1>(Expression<Func<M, M1>> field);
        IValidationPipeCreator<M> ForCollection<M1>(Expression<Func<M, IEquatable<M1>>> field);
    }

    public interface IValidator<M>
    {
        string FieldPath { get; set; }
        bool IsAsync { get; set; }
        Expression<Func<M, object>> Field { get; set; }
        ValidateResult Validate();
        Task<ValidateResult> ValidateAsync();
    }

    public struct ValidateResult
    {
        public bool IsValid { get; set; }
        public string FieldPath { get; set; }
        public string ErrorMessage { get; set; }
        public Optional<ValidateResult[]> SubResults { get; set; }

        public static ValidateResult DefaultValid
        {
            get
            {
                return new ValidateResult
                {
                    IsValid = true,
                    SubResults = Optional<ValidateResult[]>.Default
                };
            }
        }
    }
}
