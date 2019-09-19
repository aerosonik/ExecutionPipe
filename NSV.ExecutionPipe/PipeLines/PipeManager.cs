using System.Collections.Concurrent;
using System.Threading;

namespace NSV.ExecutionPipe.PipeLines
{
    public static class PipeManager
    {
        private static Optional<ConcurrentDictionary<string, SemaphoreSlim>> _semaphores
            = Optional<ConcurrentDictionary<string, SemaphoreSlim>>.Default;
        private static readonly object _lock = new object();

        public static void SetSemaphore(int min, int max, string key)
        {
            if (!_semaphores.HasValue)
                _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

            lock (_lock)
            {
                if (_semaphores.Value.ContainsKey(key))
                    return;
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
            lock (_lock)
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

    public static class PipeBuilder
    {
        public static void Pipe<M, R>()
        {
        }

        public static void Pipe<M>()
        {
        }

        public static void Pipe()
        {
        }
    }

}
