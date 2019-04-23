using NSV.ExecutionPipe;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ValidationPipe
{
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

        public static ValidateResult DefaultFailed
        {
            get
            {
                return new ValidateResult
                {
                    IsValid = false,
                    SubResults = Optional<ValidateResult[]>.Default
                };
            }
        }
    }

    public static class ValidateResultExtensions
    {
        public static ValidateResult SetErrorMessage(
            this ValidateResult result,
            string message)
        {
            result.ErrorMessage = message;
            return result;
        }

        public static ValidateResult SetPath(
            this ValidateResult result,
            string path)
        {
            result.FieldPath = path;
            return result;
        }
    }
}
