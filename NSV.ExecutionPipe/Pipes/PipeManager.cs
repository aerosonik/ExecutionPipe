﻿using System.Collections.Concurrent;
using System.Threading;

namespace NSV.ExecutionPipe.Pipes
{
    public static class PipeManager
    {
        private static Optional<ConcurrentDictionary<string, SemaphoreSlim>> _semaphores
            = new ConcurrentDictionary<string, SemaphoreSlim>();

        public static void SetSemaphore(int initialCount, string key)
        {

            if (_semaphores.Value.ContainsKey(key))
                return;
            var semaphoreSlim = new SemaphoreSlim(initialCount);
            _semaphores.Value.TryAdd(key, semaphoreSlim);
        }

        public static SemaphoreSlim GetSemaphore(string key)
        {
            var result = _semaphores.Value
            .TryGetValue(key, out var semaphoreSlim);

            return result
               ? semaphoreSlim
               : null;
        }

    }
}
