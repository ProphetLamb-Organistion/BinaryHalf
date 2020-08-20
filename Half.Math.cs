using System;
using System.Runtime.CompilerServices;

namespace System
{
    public readonly partial struct Half
    {
        public static Half Negate(in Half value) => FromBits((ushort)(value._storage ^ SIGN_MASK));

        public static Half Abs(in Half value) => FromBits((ushort)(value._storage & 0x7FFF));

        public static Half Max(in Half value1, in Half value2) => value1.CompareTo(value2) == 1 ? value1 : value2;

        public static Half Min(in Half value1, in Half value2) => value1.CompareTo(value2) == -1 ? value2 : value1;

        /*
         * http://pages.cs.wisc.edu/~markhill/cs354/Fall2008/notes/flpt.apprec.html
         * 
         *    1.XXXXXXXXXXXXXXXXXXXXXXX   0   0   0
         *
         *    ^         ^                 ^   ^   ^
         *    |         |                 |   |   |
         *    |         |                 |   |   -  sticky bit (s)
         *    |         |                 |   -  round bit (r)
         *    |         |                 -  guard bit (g)
         *    |         -  23 bit mantissa from a representation
         *    -  hidden bit
         */
        public static Half Truncate(in Half value)
        {
        }

        public static Half Ceil(in Half value)
        {

        }

        public static Half Floor(in Half value)
        {

        }

        public static Half Round(in Half value)
        {
        }
    }
}