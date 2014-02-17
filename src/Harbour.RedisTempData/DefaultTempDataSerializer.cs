using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace Harbour.RedisTempData
{
    public class DefaultTempDataSerializer : ITempDataSerializer
    {
        public DefaultTempDataSerializer()
        {
        }
        
        // NOTE: A wrapper item is used so that the type info is *always* 
        // included when serializing.

        public string Serialize(object value)
        {
            using (var scope = CreateJsConfigScope())
            {
                var result = new RedisTempDataItem(value).ToJson();
                return result;
            }
        }

        public object Deserialize(string value)
        {
            using (var scope = CreateJsConfigScope())
            {
                var result = value.FromJson<RedisTempDataItem>();
                return result != null ? result.V : null;
            }
        }

        private JsConfigScope CreateJsConfigScope()
        {
            var scope = JsConfig.With(
                // Always include the type of objects so that they can be
                // deserialized to their original types.
                includeTypeInfo: true
            );

            return scope;
        }
    }
}
