using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;
using Should;
using System.Runtime.Serialization;

namespace Harbour.RedisTempData.Test
{
    public class PerformanceTests : RedisTest
    {
        [Fact]
        async Task concurrent_providers()
        {
            const int iterations = 100000;

            var tasks = Enumerable.Range(0, iterations).Select(i =>
            {
                return Task.Run(() =>
                {
                    var options = new RedisTempDataProviderOptions()
                    {
                        UserProvider = new FakeTempDataUserProvider("user" + i + "@example.com"),
                        Serializer = new NetDataContractTempDataSerializer()
                    };
                    var tdp = new RedisTempDataProvider(options, GetDatabase());
                    var context = new ControllerContext();
                    var tempData = tdp.LoadTempData(context);
                    tempData["id"] = 123;
                    tempData["user"] = new FakeItem() { Name = "User" + i };
                    tdp.SaveTempData(context, tempData);

                    tempData = tdp.LoadTempData(context);
                    tempData["id"].ShouldEqual(123);
                    tempData["user"].ShouldEqual(new FakeItem() { Name = "User" + i });
                });
            });

            await Task.WhenAll(tasks);
        }
    }
}
