using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NSV.ExecutionPipe.Helpers
{
    public static class AsyncHelper
    {
        private static readonly TaskFactory _factory = new TaskFactory(
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

        public static TResult RunSync<TResult>(Task<TResult> task)
        {
            return _factory
                .StartNew(() => task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        public static void RunSync(Task task)
        {
            _factory
                .StartNew(() => task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }
    }
}
