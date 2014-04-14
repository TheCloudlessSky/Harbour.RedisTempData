using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Harbour.RedisTempData
{
    public interface ITempDataSerializer
    {
        /// <summary>
        /// Serialize an object to be stored in Redis.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        RedisValue Serialize(object value);

        /// <summary>
        /// Deserialize an object that was stored in Redis.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        object Deserialize(RedisValue value);
    }
}
