using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.PipeLines
{
    public static class PipeManager
    {
        private static Optional<ConcurrentDictionary<string,SemaphoreSlim>> _semaphores 
            = Optional<ConcurrentDictionary<string, SemaphoreSlim>>.Default;
        private static readonly object _lock = new object();

        public static void SetSemaphore(int min, int max, string key)
        {
            if (!_semaphores.HasValue)
                _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

            if (_semaphores.Value.ContainsKey(key))
                return;

            lock(_lock)
            {
                var semaphoreSlim = new SemaphoreSlim(min, max);
                _semaphores.Value.TryAdd(key, semaphoreSlim);
            }
        }

        public static void SetSemaphore(int max, string key)
        {
            SetSemaphore(0, max, key);
        }

        public static SemaphoreSlim GetSemaphore(string key)
        {
            if (!_semaphores.Value.ContainsKey(key))
                return null;

            var resut = _semaphores.Value
                .TryGetValue(key, out var semaphoreSlim);
            return resut 
                ? semaphoreSlim 
                : null;
        }

    }
}
