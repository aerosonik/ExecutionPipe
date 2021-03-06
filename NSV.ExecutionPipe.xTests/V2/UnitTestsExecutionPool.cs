﻿using Microsoft.Extensions.DependencyInjection;
using NSV.ExecutionPipe.Builders;
using NSV.ExecutionPipe.Models;
using NSV.ExecutionPipe.Pipes;
using NSV.ExecutionPipe.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NSV.ExecutionPipe.xTests.V2
{
    public class UnitTestsExecutionPool
    {
        private IAsyncPipe<int, int> CreateAsyncPipe(IPipeBuilder<int, int> builder)
        {
            return builder.AsyncPipe()
                .Executor(async (model, cache) =>
                {
                    await Task.Delay(1000);
                    return PipeResult<int>
                        .DefaultSuccessful
                        .SetValue(model);
                })
                .Add()
                .Return((model, results) => results[0]);
        }

        [Fact]
        public async Task ExecutionPoolInit10()
        {
            var serviceProvider = new ServiceCollection()
                .AddAsyncExecutionPool<int, int>(CreateAsyncPipe, 10, 100, 2)
                .BuildServiceProvider();
            var pool = serviceProvider
                .GetRequiredService<IAsyncExecutionPool<int, int>>();

            var results = new List<Task<PipeResult<int>>>();
            var counters = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    results.Add(pool.ExecuteAsync(j + i));
                    counters.Add(pool.PoolSize);
                }
                await Task.Delay(10);
            }
            var finish = await Task.WhenAll(results);
            Assert.Equal(1000, finish.Length);
            Assert.Equal(10, counters.Min());
            Assert.Equal(100, counters.Max());
            Assert.True(counters.Average() < 100);
        }

        [Fact]
        public async Task ExecutionPoolInit100()
        {
            var serviceProvider = new ServiceCollection()
                .AddAsyncExecutionPool<int, int>(CreateAsyncPipe, 100, 100, 2)
                .BuildServiceProvider();
            var pool = serviceProvider
                .GetRequiredService<IAsyncExecutionPool<int, int>>();

            var results = new List<Task<PipeResult<int>>>();
            var counters = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    results.Add(pool.ExecuteAsync(j + i));
                    counters.Add(pool.PoolSize);
                }
                await Task.Delay(10);
            }
            var finish = await Task.WhenAll(results);
            Assert.Equal(1000, finish.Length);
            Assert.Equal(100, counters.Min());
            Assert.Equal(100, counters.Max());
            Assert.True(counters.Average() == 100);
        }
    }
}
