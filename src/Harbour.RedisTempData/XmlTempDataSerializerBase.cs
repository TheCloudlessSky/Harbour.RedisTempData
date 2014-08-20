using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using StackExchange.Redis;

namespace Harbour.RedisTempData
{
    public abstract class XmlTempDataSerializerBase : ITempDataSerializer
    {
        public RedisValue Serialize(object value)
        {
            var serializer = CreateSerializer();
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                serializer.WriteObject(stream, value);
                stream.Position = 0;
                var result = reader.ReadToEnd();
                return result;
            }
        }

        public object Deserialize(RedisValue value)
        {
            if (value.IsNull) return null;

            var serializer = CreateSerializer();
            using (var stream = new MemoryStream())
            {
                byte[] data = value;
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                var result = serializer.ReadObject(stream);
                return result;
            }
        }

        protected abstract XmlObjectSerializer CreateSerializer();
    }
}
