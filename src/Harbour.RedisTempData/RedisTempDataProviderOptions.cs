using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harbour.RedisTempData
{
    public class RedisTempDataProviderOptions
    {
        public string KeyPrefix { get; set; }
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
            Serializer = new DefaultTempDataSerializer();
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
