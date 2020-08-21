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
         * Ceiling, floor and round should use System.MathF implementations, sice they would require to add or subtract one form the Half
         * which would cast to a float anyways.
         */
        public static Half Truncate(in Half value)
        {
            int unbiasedExponent = GetBase2Exponent(value);
            // Preserves subnormal: NaN, pos and neg infinity.
            if (unbiasedExponent == MAX_BASE2_EXPONENT_VALUE)
                return value;
            // Decimal values smaller then one, round to zero
            if (unbiasedExponent < -1)
                return Zero;
            if (unbiasedExponent == -1)
                return IsNegative(value) ? NegOne : One;
            // Values greater or equal to 2048, cannot contain decimal digits in base 10.
            // log_2(2048) = 11
            if (unbiasedExponent >= 11)
                return value;
            return new Half((ushort)(
                // Copy but the mantissa
                (value._storage & ~MANTISSA_MASK) |
                // Erase significant decimal digit and lower from mantissa:
                // Mask storage with mantissa bits to keep.
                (value._storage & (MANTISSA_MASK & ~(0xFFFF << (11 - unbiasedExponent))))));
        }
    }
}