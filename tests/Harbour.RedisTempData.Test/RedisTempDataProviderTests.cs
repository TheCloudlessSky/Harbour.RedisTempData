using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;
using Should;
using StackExchange.Redis;

namespace Harbour.RedisTempData.Test
{
    public class RedisTempDataProviderTests
    {
        private const string prefix = "TempData";
        private const string separator = ":";
        private const string user = "john.doe";
        private static readonly RedisKey key = prefix + separator + user;
        private static readonly RedisTempDataProviderOptions options;
        private static readonly ControllerContext context;

        static RedisTempDataProviderTests()
        {
            options = new RedisTempDataProviderOptions()
            {
                KeyPrefix = prefix,
                KeySeparator = separator,
                UserProvider = new FakeTempDataUserProvider(user),
                Serializer = new XmlObjectSerializerTempDataSerializer()
            };

            context = new ControllerContext();
        }

        public class Saving_temp_data : RedisTest
        {
            [Fact]
            void deletes_the_old_value()
            {
                var sut = new RedisTempDataProvider(options, Redis);
                Redis.StringSet(key, "old-value");

                sut.SaveTempData(context, new Dictionary<string, object>());

                var result = Redis.StringGet(key);
                result.ShouldEqual(RedisValue.Null);
            }

            [Fact]
            void adds_each_serialized_item_to_the_hash()
            {
                var sut = new RedisTempDataProvider(options, Redis);
                Redis.KeyDelete(key);

                sut.SaveTempData(context, new Dictionary<string, object>()
                {
                    { "A", 1 },
                    { "B", "2" },
                    { "C", new FakeItem() { Name = "Three" } }
                });

                Redis.HashGet(key, "A").ShouldEqual((RedisValue)options.Serializer.Serialize(1));
                Redis.HashGet(key, "B").ShouldEqual((RedisValue)options.Serializer.Serialize("2"));
                Redis.HashGet(key, "C").ShouldEqual((RedisValue)options.Serializer.Serialize(new FakeItem() { Name = "Three" }));
            }
        }

        public class Loading_temp_data : RedisTest
        {
            [Fact]
            void deletes_the_old_value()
            {
                var sut = new RedisTempDataProvider(options, Redis);
                Redis.KeyDelete(key);
                Redis.HashSet(key, "old", options.Serializer.Serialize("value"));

                var tempData = sut.LoadTempData(context);

                string result = Redis.StringGet(key);
                result.ShouldBeNull();
            }

            [Fact]
            void gets_each_item_deserialized()
            {
                var sut = new RedisTempDataProvider(options, Redis);
                Redis.KeyDelete(key);

                sut.SaveTempData(context, new Dictionary<string, object>()
                {
                    { "A", "1" },
                    { "B", new FakeItem() { Name = "Three" } }
                });

                var tempData = sut.LoadTempData(context);

                tempData["A"].ShouldEqual("1");
                tempData["B"].ShouldEqual(new FakeItem() { Name = "Three" });
            }
        }
    }
}
