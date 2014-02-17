using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harbour.RedisTempData
{
    internal class RedisTempDataItem
    {
        // The full name isn't used so that we can use as few characters as 
        // possible.
        public object V { get; private set; }

        public RedisTempDataItem(object value)
        {
            this.V = value;
        }
    }
}
