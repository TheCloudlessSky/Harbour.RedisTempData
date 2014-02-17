using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;

namespace Harbour.RedisTempData.Test
{
    public abstract class RedisTest : IDisposable
    {
        // Use a different DB than development.
        private const int testDb = 15;
        private readonly string testHost = "127.0.0.1:6379";

        public IRedisClientsManager ClientManager { get; private set; }
        public IRedisClient Redis { get; private set; }

        protected RedisTest()
        {
            ClientManager = new BasicRedisClientManager(testDb, testHost);
            Redis = GetRedisClient();
}

        protected virtual IRedisClient GetRedisClient()
        {
            var client = ClientManager.GetClient();
            client.FlushDb();
            return client;
        }

        public virtual void Dispose()
        {
            Redis.Dispose();
            ClientManager.Dispose();
        }
    }
}
