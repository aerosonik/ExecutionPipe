using System;
using System.Collections.Generic;
using System.Linq;
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
        public TimeSpan Elapsed { get; set; }
        public string Label { get; set; }
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

        public static PipeResult<T> DefaultSuccessfulBreak
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
                    Success = ExecutionResult.Failed
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
                    Break = false,
                    Errors = Optional<string[]>.Default,
                    Exceptions = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Failed
                };
            }
        }

        public PipeResult<T> SetValue(T value)
        {
            Value = value;
            return this;
        }

        public PipeResult<T> SetBreak(bool isbreak)
        {
            Break = isbreak;
            return this;
        }

        public PipeResult<T> SetErrors(string[] errors)
        {
            if (errors != null)
                Errors = errors;
            return this;
        }

        public PipeResult<T> SetError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
                Errors = new string[] { error };
            return this;
        }

        public PipeResult<T> SetException(Exception[] exceptions)
        {
            if (exceptions != null)
                Exceptions = exceptions;
            return this;
        }

        public PipeResult<T> SetException(Exception exception)
        {
            if (exception != null)
                Exceptions = new Exception[] { exception };
            return this;
        }

        public PipeResult<T> SetSuccessful()
        {
            Success = ExecutionResult.Successful;
            return this;
        }

        public PipeResult<T> SetUnSuccessful()
        {
            Success = ExecutionResult.Failed;
            return this;
        }

        public PipeResult<T> SetElapsed(TimeSpan span)
        {
            Elapsed = span;
            return this;
        }

        public PipeResult<T> SetLabel(string label)
        {
            Label = label;
            return this;
        }
    }

    public struct PipeResult
    {
        public bool Break { get; set; }
        public ExecutionResult Success { get; set; }
        public Optional<string[]> Errors { get; set; }
        public Optional<Exception[]> Exceptions { get; set; }
        public TimeSpan Elapsed { get; set; }
        public string Label { get; set; }

        public static PipeResult Default
        {
            get
            {
                return new PipeResult
                {
                    Break = false,
                    Errors = Optional<string[]>.Default,
                    Exceptions = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Initial
                };
            }
        }

        public static PipeResult DefaultSuccessfulBreak
        {
            get
            {
                return new PipeResult
                {
                    Break = true,
                    Errors = Optional<string[]>.Default,
                    Exceptions = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Successful
                };
            }
        }

        public static PipeResult DefaultSuccessful
        {
            get
            {
                return new PipeResult
                {
                    Break = false,
                    Errors = Optional<string[]>.Default,
                    Exceptions = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Successful
                };
            }
        }

        public static PipeResult DefaultUnSuccessfulBreak
        {
            get
            {
                return new PipeResult
                {
                    Break = true,
                    Errors = Optional<string[]>.Default,
                    Exceptions = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Failed
                };
            }
        }

        public static PipeResult DefaultUnSuccessful
        {
            get
            {
                return new PipeResult
                {
                    Break = false,
                    Errors = Optional<string[]>.Default,
                    Exceptions = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Failed
                };
            }
        }


        public PipeResult SetBreak(bool isbreak)
        {
            Break = isbreak;
            return this;
        }

        public PipeResult SetErrors(string[] errors)
        {
            if (errors != null)
                Errors = errors;
            return this;
        }

        public PipeResult SetError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
                Errors = new string[] { error };
            return this;
        }

        public PipeResult SetException(Exception[] exceptions)
        {
            if (exceptions != null)
                Exceptions = exceptions;
            return this;
        }

        public PipeResult SetException(Exception exception)
        {
            if (exception != null)
                Exceptions = new Exception[] { exception };
            return this;
        }

        public PipeResult SetSuccessful()
        {
            Success = ExecutionResult.Successful;
            return this;
        }

        public PipeResult SetUnSuccessful()
        {
            Success = ExecutionResult.Failed;
            return this;
        }

        public PipeResult SetElapsed(TimeSpan span)
        {
            Elapsed = span;
            return this;
        }

        public PipeResult SetLabel(string label)
        {
            Label = label;
            return this;
        }
    }

    public enum ExecutionResult
    {
        Initial,
        Successful,
        Failed
    }

    public static class ResultExtensions
    {
        public static PipeResult<T>[] AllUnSuccess<T>(this PipeResult<T>[] results)
        {
            return results.Where(x => x.Success == ExecutionResult.Failed)
                        .ToArray();
        }

        public static string[] AllErrors<T>(this PipeResult<T>[] results)
        {
            return results
                        .Where(x => x.Success == ExecutionResult.Failed &&
                                    x.Errors.HasValue)
                        .SelectMany(x => x.Errors.Value)
                        .ToArray();
        }

        public static Exception[] AllExceptions<T>(this PipeResult<T>[] results)
        {
            return results
                        .Where(x => x.Success == ExecutionResult.Failed &&
                                    x.Exceptions.HasValue)
                        .SelectMany(x => x.Exceptions.Value)
                        .ToArray();
        }

        public static ExecutionResult AllSuccess<T>(this PipeResult<T>[] results)
        {
            return results.All(x => x.Success == ExecutionResult.Successful)
                    ? ExecutionResult.Successful
                    : results
                        .All(x => x.Success == ExecutionResult.Initial)
                        ? ExecutionResult.Initial
                        : ExecutionResult.Failed;
        }

        public static ExecutionResult AllExecutedSuccess<T>(this PipeResult<T>[] results)
        {
            return results
                .Where(x => x.Success != ExecutionResult.Initial)
                .All(x => x.Success == ExecutionResult.Successful)
                    ? ExecutionResult.Successful
                    : results
                        .All(x => x.Success == ExecutionResult.Initial)
                        ? ExecutionResult.Initial
                        : ExecutionResult.Failed;
        }

        public static ExecutionResult AnySuccess<T>(this PipeResult<T>[] results)
        {
            return results.Any(x => x.Success == ExecutionResult.Successful)
                    ? ExecutionResult.Successful
                    : results
                        .All(x => x.Success == ExecutionResult.Initial)
                        ? ExecutionResult.Initial
                        : ExecutionResult.Failed;
        }

        public static bool IsAllSuccess<T>(this PipeResult<T>[] results)
        {
            return results.All(x => x.Success == ExecutionResult.Successful);
        }

        public static bool IsAllExecutedSuccess<T>(this PipeResult<T>[] results)
        {
            return results
                .Where(x => x.Success != ExecutionResult.Initial)
                .All(x => x.Success == ExecutionResult.Successful);
        }


        public static PipeResult[] AllUnSuccess(this PipeResult[] results)
        {
            return results.Where(x => x.Success == ExecutionResult.Failed)
                        .ToArray();
        }

        public static string[] AllErrors(this PipeResult[] results)
        {
            return results
                        .Where(x => x.Success == ExecutionResult.Failed &&
                                    x.Errors.HasValue)
                        .SelectMany(x => x.Errors.Value)
                        .ToArray();
        }

        public static Exception[] AllExceptions(this PipeResult[] results)
        {
            return results
                        .Where(x => x.Success == ExecutionResult.Failed &&
                                    x.Exceptions.HasValue)
                        .SelectMany(x => x.Exceptions.Value)
                        .ToArray();
        }

        public static ExecutionResult AllSuccess(this PipeResult[] results)
        {
            return results.All(x => x.Success == ExecutionResult.Successful)
                    ? ExecutionResult.Successful
                    : results
                        .All(x => x.Success == ExecutionResult.Initial)
                        ? ExecutionResult.Initial
                        : ExecutionResult.Failed;
        }

        public static ExecutionResult AllExecutedSuccess(this PipeResult[] results)
        {
            return results
                .Where(x => x.Success != ExecutionResult.Initial)
                .All(x => x.Success == ExecutionResult.Successful)
                    ? ExecutionResult.Successful
                    : results
                        .All(x => x.Success == ExecutionResult.Initial)
                        ? ExecutionResult.Initial
                        : ExecutionResult.Failed;
        }

        public static ExecutionResult AnySuccess(this PipeResult[] results)
        {
            return results.Any(x => x.Success == ExecutionResult.Successful)
                    ? ExecutionResult.Successful
                    : results
                        .All(x => x.Success == ExecutionResult.Initial)
                        ? ExecutionResult.Initial
                        : ExecutionResult.Failed;
        }

        public static bool IsAllSuccess(this PipeResult[] results)
        {
            return results.All(x => x.Success == ExecutionResult.Successful);
        }

        public static bool IsAllExecutedSuccess(this PipeResult[] results)
        {
            return results
                .Where(x => x.Success != ExecutionResult.Initial)
                .All(x => x.Success == ExecutionResult.Successful);
        }
    }
}
