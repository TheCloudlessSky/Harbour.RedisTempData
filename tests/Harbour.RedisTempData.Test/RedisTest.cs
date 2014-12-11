using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Redis;

namespace Harbour.RedisTempData.Test
{
    public abstract class RedisTest : IDisposable
    {
        // Use a different DB than development.
        private const int testDb = 15;
        private readonly string testHost = "localhost";
        private readonly int testPort = 6379;

        public ConnectionMultiplexer Multiplexer { get; private set; }
        public IDatabase Redis { get; private set; }

        protected RedisTest()
        {
            var config = new ConfigurationOptions() { AllowAdmin = true };
            config.EndPoints.Add(testHost, testPort);
            Multiplexer = ConnectionMultiplexer.Connect(config);
            Redis = GetFlushedRedis();
        }

        private IDatabase GetFlushedRedis()
        {
            var redis = GetDatabase();
            var server = Multiplexer.GetServer(testHost + ":" + testPort);
            server.FlushDatabase(testDb);
            return redis;
        }

        protected IDatabase GetDatabase()
        {
            return Multiplexer.GetDatabase(testDb);
        }

        public virtual void Dispose()
        {
            Multiplexer.Dispose();
        }
    }
}
