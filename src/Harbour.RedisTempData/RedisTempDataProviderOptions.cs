using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harbour.RedisTempData
{
    public class RedisTempDataProviderOptions
    {
        /// <summary>
        /// Gets or sets the prefix used for keys in Redis. The default value
        /// is &quot;TempData&quot;.
        /// </summary>
        public string KeyPrefix { get; set; }

        /// <summary>
        /// Gets or sets the separator used for keys in Redis. The default
        /// value is &quot;:&quot;.
        /// </summary>
        public string KeySeparator { get; set; }

        /// <summary>
        /// Resolve a unique key for the current user.
        /// </summary>
        public ITempDataUserProvider UserProvider { get; set; }

        /// <summary>
        /// Serialize and deserialize values for the provider.
        /// </summary>
        public ITempDataSerializer Serializer { get; set; }

        public RedisTempDataProviderOptions()
        {
            KeyPrefix = "TempData";
            KeySeparator = ":";
            UserProvider = new DefaultTempDataUserProvider();
            Serializer = new NetDataContractTempDataSerializer();
        }

        // Copy constructor.
        internal RedisTempDataProviderOptions(RedisTempDataProviderOptions other)
        {
            KeyPrefix = other.KeyPrefix;
            KeySeparator = other.KeySeparator;
            UserProvider = other.UserProvider;
            Serializer = other.Serializer;
        }
    }
}
