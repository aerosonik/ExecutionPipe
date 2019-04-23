using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NSV.ValidationPipe
{
    public static class ValidatorExtensions
    {
        //NotNull
        public static IFieldValidatorCreator<TModel, TField> NotNull<TModel, TField>(
            this IFieldValidatorCreator<TModel, TField> creator, 
            string message = null)
        {
            return creator.Must(x => x != null).WithMessage(message);
        }
        public static IValidatorCreator<TModel, TField> NotNull<TModel, TField>(
            this IFieldValidatorCreator<TModel, TField> creator)
        {
            return creator.Must(x => x != null);
        }

        #region TField = string
        //Equal
        public static IFieldValidatorCreator<TModel, string> Equal<TModel>(
            this IFieldValidatorCreator<TModel, string> creator,
            string value, string message = null)
        {
            return creator.Must(x => x.Equals(value)).WithMessage(message);
        }
        public static IValidatorCreator<TModel, string> Equal<TModel>(
            this IFieldValidatorCreator<TModel, string> creator,
            string value)
        {
            return creator.Must(x => x.Equals(value));
        }
        //NotEqual
        public static IFieldValidatorCreator<TModel, string> NotEqual<TModel>(
           this IFieldValidatorCreator<TModel, string> creator,
           string value, string message = null)
        {
            return creator.Must(x => !x.Equals(value)).WithMessage(message);
        }
        public static IValidatorCreator<TModel, string> NotEqual<TModel>(
            this IFieldValidatorCreator<TModel, string> creator,
            string value)
        {
            return creator.Must(x => !x.Equals(value));
        }
        //Contains
        public static IFieldValidatorCreator<TModel, string> Contains<TModel>(
            this IFieldValidatorCreator<TModel, string> creator,
            string value, string message = null)
        {
            return creator.Must(x => x.Contains(value)).WithMessage(message);
        }
        public static IValidatorCreator<TModel, string> Contains<TModel>(
           this IFieldValidatorCreator<TModel, string> creator,
           string value)
        {
            return creator.Must(x => x.Contains(value));
        }     
        //StartWith
        public static IFieldValidatorCreator<TModel, string> StartWith<TModel>(
            this IFieldValidatorCreator<TModel, string> creator,
            string value, string message = null)
        {
            return creator.Must(x => x.StartsWith(value)).WithMessage(message);
        }
        public static IValidatorCreator<TModel, string> StartWith<TModel>(
            this IFieldValidatorCreator<TModel, string> creator,
            string value)
        {
            return creator.Must(x => x.StartsWith(value));
        }
        //EndWith
        public static IFieldValidatorCreator<TModel, string> EndWith<TModel>(
            this IFieldValidatorCreator<TModel, string> creator,
            string value, string message = null)
        {
            return creator.Must(x => x.EndsWith(value)).WithMessage(message);
        }
        public static IValidatorCreator<TModel, string> EndWith<TModel>(
           this IFieldValidatorCreator<TModel, string> creator,
           string value)
        {
            return creator.Must(x => x.EndsWith(value));
        }
        //Regexp
        public static IFieldValidatorCreator<TModel, string> IsMatch<TModel>(
            this IFieldValidatorCreator<TModel, string> creator,
            string pattern, string message = null)
        {
            return creator.Must(x => Regex.IsMatch(x, pattern, RegexOptions.CultureInvariant))
                          .WithMessage(message);
        }
        public static IValidatorCreator<TModel, string> IsMatch<TModel>(
           this IFieldValidatorCreator<TModel, string> creator,
           string pattern)
        {
            return creator.Must(x => Regex.IsMatch(x, pattern, RegexOptions.CultureInvariant));
        }
        //IsEmpty
        public static IFieldValidatorCreator<TModel, string> IsEmpty<TModel>(
            this IFieldValidatorCreator<TModel, string> creator, string message = null)
        {
            return creator.Must(x => string.IsNullOrWhiteSpace(x))
                          .WithMessage(message);
        }
        public static IValidatorCreator<TModel, string> IsEmpty<TModel>(
           this IFieldValidatorCreator<TModel, string> creator)
        {
            return creator.Must(x => string.IsNullOrWhiteSpace(x));
        }
        //NotEmpty
        public static IFieldValidatorCreator<TModel, string> NotEmpty<TModel>(
            this IFieldValidatorCreator<TModel, string> creator, string message = null)
        {
            return creator.Must(x => !string.IsNullOrWhiteSpace(x))
                          .WithMessage(message);
        }
        public static IValidatorCreator<TModel, string> NotEmpty<TModel>(
           this IFieldValidatorCreator<TModel, string> creator)
        {
            return creator.Must(x => !string.IsNullOrWhiteSpace(x));
        }
        #endregion

        #region TField = Int
        //Equal
        public static IFieldValidatorCreator<TModel, int> Equal<TModel>(
            this IFieldValidatorCreator<TModel, int> creator,
            int value, string message = null)
        {
            return creator.Must(x => x.Equals(value)).WithMessage(message);
        }
        public static IValidatorCreator<TModel, int> Equal<TModel>(
            this IFieldValidatorCreator<TModel, int> creator,
            int value)
        {
            return creator.Must(x => x.Equals(value));
        }

        // Greater
        public static IFieldValidatorCreator<TModel, int> Greater<TModel>(
           this IFieldValidatorCreator<TModel, int> creator,
           int value, string message = null)
        {
            return creator.Must(x => x > value).WithMessage(message);
        }
        public static IValidatorCreator<TModel, int> Greater<TModel>(
            this IFieldValidatorCreator<TModel, int> creator,
            int value)
        {
            return creator.Must(x => x > value);
        }

        // GreaterOrEqual
        public static IFieldValidatorCreator<TModel, int> GreaterOrEqual<TModel>(
           this IFieldValidatorCreator<TModel, int> creator,
           int value, string message = null)
        {
            return creator.Must(x => x >= value).WithMessage(message);
        }
        public static IValidatorCreator<TModel, int> GreaterOrEqual<TModel>(
            this IFieldValidatorCreator<TModel, int> creator,
            int value)
        {
            return creator.Must(x => x >= value);
        }

        // less
        public static IFieldValidatorCreator<TModel, int> less<TModel>(
           this IFieldValidatorCreator<TModel, int> creator,
           int value, string message = null)
        {
            return creator.Must(x => x < value).WithMessage(message);
        }
        public static IValidatorCreator<TModel, int> less<TModel>(
            this IFieldValidatorCreator<TModel, int> creator,
            int value)
        {
            return creator.Must(x => x < value);
        }

        // lessOrEqual
        public static IFieldValidatorCreator<TModel, int> lessOrEqual<TModel>(
           this IFieldValidatorCreator<TModel, int> creator,
           int value, string message = null)
        {
            return creator.Must(x => x <= value).WithMessage(message);
        }
        public static IValidatorCreator<TModel, int> lessOrEqual<TModel>(
            this IFieldValidatorCreator<TModel, int> creator,
            int value)
        {
            return creator.Must(x => x <= value);
        }
        // Between
        public static IFieldValidatorCreator<TModel, int> Between<TModel>(
           this IFieldValidatorCreator<TModel, int> creator,
           int value1, int value2, string message = null)
        {
            return creator.Must(x => x > value1 && x < value2).WithMessage(message);
        }
        public static IValidatorCreator<TModel, int> Between<TModel>(
            this IFieldValidatorCreator<TModel, int> creator,
            int value1, int value2)
        {
            return creator.Must(x => x > value1 && x < value2);
        }
        #endregion

        #region TField = DateTime
        //Equal
        public static IFieldValidatorCreator<TModel, DateTime> Equal<TModel>(
            this IFieldValidatorCreator<TModel, DateTime> creator,
            DateTime value, string message = null)
        {
            return creator.Must(x => x.Equals(value)).WithMessage(message);
        }
        public static IValidatorCreator<TModel, DateTime> Equal<TModel>(
            this IFieldValidatorCreator<TModel, DateTime> creator,
            DateTime value)
        {
            return creator.Must(x => x.Equals(value));
        }
        #endregion
    }
}
