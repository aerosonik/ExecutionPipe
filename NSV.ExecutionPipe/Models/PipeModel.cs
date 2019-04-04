using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace NSV.ExecutionPipe.Implementations
{
    public class PipeConcurrentModel<T>
    {
        private readonly T _model;
        private readonly object _lock = new object();

        public PipeConcurrentModel(T model)
        {
            _model = model;
        }

        public void Set<V>(Expression<Func<T,V>> property, V value)
        {
            lock (_lock)
            {
                ((property.Body as MemberExpression)?.Member as PropertyInfo)
                    .SetValue(_model, value);
            }
        }

        public object Get(Expression<Func<T, object>> property)
        {
            lock (_lock)
            {
                return ((property.Body as MemberExpression)?.Member as PropertyInfo)
                    .GetValue(_model);
            }
        }
    }

    public class PipeModel<T>
    {
        public PipeModel(T model)
        {
            Model = model;
        }

        public T Model;

        public static implicit operator T(PipeModel<T> pipeModel)
        {
            return pipeModel.Model;
        }

        public static implicit operator PipeModel<T>(T model)
        {
            return new PipeModel<T>(model);
        }

    }
}
