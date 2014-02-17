using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace Harbour.RedisTempData
{
    public class RedisTempDataProvider : ITempDataProvider
    {
        private readonly RedisTempDataProviderOptions options;
        private readonly IRedisClient redis;

        public RedisTempDataProvider(IRedisClient redis)
            : this(new RedisTempDataProviderOptions(), redis)
        {

        }

        public RedisTempDataProvider(RedisTempDataProviderOptions options, IRedisClient redis)
        {
            this.options = options;
            this.redis = redis;
        }

        private string GetKey(ControllerContext controllerContext)
        {
            var user = options.UserProvider.GetUser(controllerContext);
            return options.KeyPrefix + options.KeySeparator + user;
        }

        public IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
        {
            var hashKey = GetKey(controllerContext);
            var result = new Dictionary<string, object>();

            using (var transaction = redis.CreateTransaction())
            {
                // HACK: ServiceStack.Redis doesn't support dictionaries inside
                // of a transaction. Therefore, we use the native client and
                // do the conversion ourselves.
                transaction.QueueCommand(r => ((IRedisNativeClient)r).HGetAll(hashKey), multiDataList =>
                {
                    for (var i = 0; i < multiDataList.Length; i += 2)
                    {
                        var key = multiDataList[i].FromUtf8Bytes();
                        var value = multiDataList[i + 1].FromUtf8Bytes();
                        result[key] = Deserialize(value);
                    }
                });

                // Remove *after* reading the items since we're done with them.
                transaction.QueueCommand(r => r.Remove(hashKey));

                transaction.Commit();
            }

            return result;
        }

        public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values)
        {
            var hashKey = GetKey(controllerContext);

            using (var transaction = redis.CreateTransaction())
            {
                var hash = redis.Hashes[hashKey];

                // Clear the hash since we don't want old items.
                transaction.QueueCommand(r => r.Remove(hashKey));

                if (values != null && values.Count > 0)
                {
                    foreach (var kvp in values)
                    {
                        transaction.QueueCommand(r => hash.Add(kvp.Key, Serialize(kvp.Value)));
                    }
                }

                transaction.Commit();
            }
        }

        private object Deserialize(string value)
        {
            if (value == null) return null;

            return options.Serializer.Deserialize(value);
        }

        private string Serialize(object value)
        {
            return options.Serializer.Serialize(value);
        }
    }
}
