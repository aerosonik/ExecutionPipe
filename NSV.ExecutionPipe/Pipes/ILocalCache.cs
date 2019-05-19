using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Pipes
{
    public interface ILocalCache
    {
        T GetObject<T>(object key);

        void SetObject<T>(object key, T value);

        ILocalCache GetLocalCacheObject();
    }
}
