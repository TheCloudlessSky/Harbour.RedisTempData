using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harbour.RedisTempData
{
    public interface ITempDataSerializer
    {
        string Serialize(object value);
        object Deserialize(string value);
    }
}
