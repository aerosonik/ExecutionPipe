using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Models
{
    public struct PipeResult<T>
    {
        public bool Break;
        public ExecutionResult Success { get; set; }
        public Optional<string[]> Errors { get; set; }
        public Optional<Exception[]> Exception { get; set; }
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
                    Exception = Optional<Exception[]>.Default,
                    Success = ExecutionResult.Initial
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
}
