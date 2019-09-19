using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NSV.ExecutionPipe.Cache
{
    public class PipeCacheObject : IPipeCache
    {
        private readonly Optional<IDictionary<object, object>> _internalCache;

        private PipeCacheObject(bool useThreadSafe)
        {
            if (useThreadSafe)
                _internalCache = new ConcurrentDictionary<object, object>();
            else
                _internalCache = new Dictionary<object, object>();
        }
        private PipeCacheObject()
        {
            _internalCache = Optional<IDictionary<object, object>>.Default;
        }

        public static IPipeCache GetCache()
        {
            return new PipeCacheObject(false);
        }
        public static IPipeCache GetThreadSafeCache()
        {
            return new PipeCacheObject(true);
        }
        public static IPipeCache GetDefaultCache()
        {
            return new PipeCacheObject();
        }

        public T Get<T>(object key)
        {
            if (_internalCache.HasValue)
            {
                if (_internalCache.Value.TryGetValue(key, out var value))
                    return (T)value;
            }
            return default(T);
        }
        public void Set<T>(object key, T value)
        {
            if (_internalCache.HasValue)
            {
                _internalCache.Value.Add(key, value);
            }
        }
        public void Delete(object key)
        {
            if (_internalCache.HasValue)
            {
                _internalCache.Value.Remove(key);
            }
        }
    }
}
