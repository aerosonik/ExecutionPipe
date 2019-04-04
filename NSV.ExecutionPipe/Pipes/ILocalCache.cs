using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Pipes
{
    public interface ILocalCache
    {
        object GetObject(object key);

        void SetObject(object key, object value);

        ILocalCache GetLocalCacheObject();
    }
}
