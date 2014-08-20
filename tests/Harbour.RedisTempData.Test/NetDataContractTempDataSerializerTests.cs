using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Should;
using StackExchange.Redis;

namespace Harbour.RedisTempData.Test
{
    public class NetDataContractTempDataSerializerTests
    {
        [Fact]
        void can_serialize_an_object()
        {
            var sut = new NetDataContractTempDataSerializer();

            var value = sut.Serialize(new FakeItem() { Name = "John Doe" });
            var result = sut.Deserialize(value) as FakeItem;

            result.ShouldNotBeNull();
            result.Name.ShouldEqual("John Doe");
        }

        [Fact]
        void can_serialize_a_string()
        {
            var sut = new NetDataContractTempDataSerializer();

            var serialized = sut.Serialize("Hello World");
            var deserialized = sut.Deserialize(serialized) as string;

            deserialized.ShouldEqual("Hello World");
        }

        [Fact]
        void deserializing_null_input_returns_null()
        {
            var sut = new NetDataContractTempDataSerializer();
            string value = null;

            var result = sut.Deserialize(value);

            result.ShouldBeNull();
        }
    }
}
