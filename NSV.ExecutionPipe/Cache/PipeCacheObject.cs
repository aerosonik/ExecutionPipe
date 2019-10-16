using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NSV.ExecutionPipe.Cache
{
    internal class PipeCacheObject : IPipeCache
    {
        private readonly Optional<IDictionary<object, object>> _internalCache;

        private PipeCacheObject(bool useThreadSafe)
        {
            if (useThreadSafe)
                _internalCache = new ConcurrentDictionary<object, object>();
            else
                _internalCache = new Dictionary<object, object>();
        }
        //internal PipeCacheObject()
        //{
        //    _internalCache = Optional<IDictionary<object, object>>.Default;
        //}

        internal static Func<IPipeCache> GetCache()
        {
            return () => new PipeCacheObject(false);
        }
        internal static Func<IPipeCache> GetThreadSafeCache()
        {
            return () => new PipeCacheObject(true);
        }
        //internal static IPipeCache GetDefaultCache()
        //{
        //    return new PipeCacheObject();
        //}

        T IPipeCache.Get<T>(object key)
        {
            if (_internalCache.HasValue)
            {
                if (_internalCache.Value.TryGetValue(key, out var value))
                    return (T)value;
            }
            return default(T);
        }
        void IPipeCache.Set<T>(object key, T value)
        {
            if (_internalCache.HasValue)
            {
                _internalCache.Value.Add(key, value);
            }
        }
        void IPipeCache.Delete(object key)
        {
            if (_internalCache.HasValue)
            {
                _internalCache.Value.Remove(key);
            }
        }
    }
}
