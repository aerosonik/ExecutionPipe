using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe
{
    public struct Optional<T>
    {
        public bool HasValue { get; private set; }
        public T Value { get; }

        public Optional(T value)
        {
            Value = value;
            if (value == null)
                HasValue = false;
            else
                HasValue = true;
        }

        private Optional(T value, bool hasValue)
        {
            Value = value;
            HasValue = hasValue;
        }

        public static implicit operator T(Optional<T> option)
        {
            return option.Value;
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static Optional<T> Default
        {
            get
            {
                return new Optional<T>(default, false);
            }
        }

        //public override bool Equals(object obj)
        //{
        //    var label = (T)obj;
        //    return Value.Equals(label);
        //}
    }
}
