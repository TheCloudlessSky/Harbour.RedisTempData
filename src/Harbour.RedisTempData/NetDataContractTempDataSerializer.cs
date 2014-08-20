using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Harbour.RedisTempData
{
    public class NetDataContractTempDataSerializer : XmlTempDataSerializerBase
    {
        protected override XmlObjectSerializer CreateSerializer()
        {
            var serializer = new NetDataContractSerializer();
            return serializer;
        }
    }
}
