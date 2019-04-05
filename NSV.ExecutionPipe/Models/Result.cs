using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Models
{
    public struct PipeResult<T>
    {
        public bool Break { get; set; }
        public ExecutionResult Success { get; set; }
        public Optional<string[]> Errors { get; set; }
        public Optional<Exception[]> Exceptions { get; set; }
        public Optional<T> Value { get; set; }

        public static PipeResult<T> Default
        {
            get
            {
                return new PipeResult<T>
                {
                    Value = Optional<T>.Default,
                    Break = false,
                    Errors = Optional<string[]>.Default,
                    Exceptions = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Initial
                };
            }
        }

        public static PipeResult<T> DefaultSuccessBreak
        {
            get
            {
                return new PipeResult<T>
                {
                    Value = Optional<T>.Default,
                    Break = true,
                    Errors = Optional<string[]>.Default,
                    Exceptions = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Successful
                };
            }
        }

        public static PipeResult<T> DefaultSuccessful
        {
            get
            {
                return new PipeResult<T>
                {
                    Value = Optional<T>.Default,
                    Break = false,
                    Errors = Optional<string[]>.Default,
                    Exceptions = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Successful
                };
            }
        }

        public static PipeResult<T> DefaultUnSuccessfulBreak
        {
            get
            {
                return new PipeResult<T>
                {
                    Value = Optional<T>.Default,
                    Break = true,
                    Errors = Optional<string[]>.Default,
                    Exceptions = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Unsuccessful
                };
            }
        }
        public static PipeResult<T> DefaultUnSuccessful
        {
            get
            {
                return new PipeResult<T>
                {
                    Value = Optional<T>.Default,
                    Break = true,
                    Errors = Optional<string[]>.Default,
                    Exceptions = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Unsuccessful
                };
            }
        }
    }

    public enum ExecutionResult
    {
        Initial,
        Successful,
        Unsuccessful
    }

    public static class ResultExtensions {
        public static PipeResult<T> SetValue<T>(this PipeResult<T> pipeResult, T value)
        {
            pipeResult.Value = value;
            return pipeResult;
        }

        public static PipeResult<T> SetBreak<T>(this PipeResult<T> pipeResult, bool isbreak)
        {
            pipeResult.Break = isbreak;
            return pipeResult;
        }

        public static PipeResult<T> SetErrors<T>(this PipeResult<T> pipeResult, string[] errors)
        {
            pipeResult.Errors = errors;
            return pipeResult;
        }

        public static PipeResult<T> SetError<T>(this PipeResult<T> pipeResult, string error)
        {
            pipeResult.Errors = new string[] { error };
            return pipeResult;
        }

        public static PipeResult<T> SetException<T>(this PipeResult<T> pipeResult, Exception[] exceptions)
        {
            pipeResult.Exceptions = exceptions;
            return pipeResult;
        }

        public static PipeResult<T> SetException<T>(this PipeResult<T> pipeResult, Exception exception)
        {
            pipeResult.Exceptions = new Exception[] { exception };
            return pipeResult;
        }

        public static PipeResult<T> SetSuccessful<T>(this PipeResult<T> pipeResult)
        {
            pipeResult.Success = ExecutionResult.Successful;
            return pipeResult;
        }

        public static PipeResult<T> SetUnSuccessful<T>(this PipeResult<T> pipeResult)
        {
            pipeResult.Success = ExecutionResult.Unsuccessful;
            return pipeResult;
        }

        public static PipeResult<T> SetException<T>(this PipeResult<T> pipeResult, ExecutionResult result)
        {
            pipeResult.Success = result;
            return pipeResult;
        }
    }
}
