using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;

namespace Harbour.RedisTempData.Test
{
    public class FakeTempDataSerializer : ITempDataSerializer
    {
        public string Serialize(object value)
        {
            using (JsConfig.With(includeTypeInfo: true))
            {
                var result = value.ToJson();
                return result;
            }
        }

        public object Deserialize(string value)
        {
            using (JsConfig.With(includeTypeInfo: true))
            {
                var result = value.FromJson<object>();
                return result;
            }
        }
    }
}
