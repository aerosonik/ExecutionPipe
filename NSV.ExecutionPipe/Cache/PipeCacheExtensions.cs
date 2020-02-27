using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Cache
{
    public static class PipeCacheExtensions
    {
        public static T GetSafely<T>(this IPipeCache cache, object key)
        {
            if (cache != null && key != null)
                return cache.Get<T>(key);

            return default;
        }

        public static void SetSafely<T>(this IPipeCache cache, object key, T value)
        {
            if (cache != null && key != null)
                cache.Set<T>(key, value);
        }

        public static void SetOrUpdateSafely<T>(this IPipeCache cache, object key, T value)
        {
            if (cache != null && key != null)
                cache.SetOrUpdate<T>(key, value);
        }

        public static void DelSafely<T>(this IPipeCache cache, object key)
        {
            if (cache != null && key != null)
                cache.Delete(key);
        }

        public static void ClearSafely(this IPipeCache cache)
        {
            if (cache != null)
                cache.Clear();
        }
    }
}
