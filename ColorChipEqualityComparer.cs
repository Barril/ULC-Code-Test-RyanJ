using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipSecuritySystem
{
    internal class ColorChipEqualityComparer : IEqualityComparer<ColorChip>
    {
        public bool Equals(ColorChip x, ColorChip y)
        {
            return x.StartColor == y.StartColor && x.EndColor == y.EndColor;
        }
        public int GetHashCode(ColorChip obj)
        {
            return obj.StartColor.GetHashCode() ^ obj.EndColor.GetHashCode();
        }
    }
}
