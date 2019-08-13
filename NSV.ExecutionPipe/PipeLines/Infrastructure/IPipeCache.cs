using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.PipeLines.Infrastructure
{
    public interface IPipeCache
    {
        T Get<T>(object key);

        void Set<T>(object key, T value);

        void Delete(object key);
    }
}
