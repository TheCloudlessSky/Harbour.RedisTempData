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

        public ConnectionMultiplexer Multiplexer { get; private set; }
        public IDatabase Redis { get; private set; }

        protected RedisTest()
        {
            var config = new ConfigurationOptions() { AllowAdmin = true };
            config.EndPoints.Add(testHost);
            Multiplexer = ConnectionMultiplexer.Connect(config);
            Redis = GetRedis();
        }

        protected virtual IDatabase GetRedis()
        {
            var client = Multiplexer.GetDatabase(testDb);
            var server = Multiplexer.GetServer(testHost);
            server.FlushDatabase(testDb);
            return client;
        }

        public virtual void Dispose()
        {
            Multiplexer.Dispose();
        }
    }
}
