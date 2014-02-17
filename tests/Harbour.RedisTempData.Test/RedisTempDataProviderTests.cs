using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;
using Should;

namespace Harbour.RedisTempData.Test
{
    public class RedisTempDataProviderTests
    {
        private const string prefix = "TempData";
        private const string separator = ":";
        private const string user = "john.doe";
        private static readonly string key = prefix + separator + user;
        private static readonly RedisTempDataProviderOptions options;
        private static readonly ControllerContext context;

        static RedisTempDataProviderTests()
        {
            options = new RedisTempDataProviderOptions()
            {
                KeyPrefix = prefix,
                KeySeparator = separator,
                UserProvider = new FakeTempDataUserProvider(user),
                Serializer = new FakeTempDataSerializer()
            };

            context = new ControllerContext();
        }

        public class Saving_temp_data : RedisTest
        {
            [Fact]
            void deletes_the_old_value()
            {
                var sut = new RedisTempDataProvider(options, Redis);
                Redis.Set(key, "old-value");

                sut.SaveTempData(context, new Dictionary<string, object>());

                var result = Redis.Get<string>(key);
                result.ShouldBeNull();
            }

            [Fact]
            void adds_each_serialized_item_to_the_hash()
            {
                var sut = new RedisTempDataProvider(options, Redis);

                sut.SaveTempData(context, new Dictionary<string, object>()
                {
                    { "A", 1 },
                    { "B", "2" },
                    { "C", new FakeItem() { Name = "Three" } }
                });

                Redis.Hashes[key]["A"].ShouldEqual("1");
                Redis.Hashes[key]["B"].ShouldEqual("\"2\"");
                Redis.Hashes[key]["C"].ShouldEqual("{\"__type\":\"Harbour.RedisTempData.Test.FakeItem, Harbour.RedisTempData.Test\",\"Name\":\"Three\"}");
            }
        }

        public class Loading_temp_data : RedisTest
        {
            [Fact]
            void deletes_the_old_value()
            {
                var sut = new RedisTempDataProvider(options, Redis);
                Redis.Hashes[key].Add("old", "value");

                var tempData = sut.LoadTempData(context);

                Redis.Hashes[key].ShouldBeEmpty();
            }

            [Fact]
            void gets_each_item_deserialized()
            {
                var sut = new RedisTempDataProvider(options, Redis);
                
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
