using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Harbour.RedisTempData.Test
{
    public class FakeItem : IEquatable<FakeItem>
    {
        public string Name { get; set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FakeItem);
        }

        public bool Equals(FakeItem other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }
    }
}
