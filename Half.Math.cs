using System.Diagnostics;
using System;
using System.Runtime.CompilerServices;

namespace System
{
    public readonly partial struct Half
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Negate(in Half value) => FromBits((ushort)(value._storage ^ SIGN_MASK));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Abs(in Half value) => FromBits((ushort)(value._storage & 0x7FFF));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Max(in Half value1, in Half value2) => value1.CompareTo(value2) == 1 ? value1 : value2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Min(in Half value1, in Half value2) => value1.CompareTo(value2) == -1 ? value2 : value1;

        /// <summary>
        /// Returns the value of y with the sign of x.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "sourceFormat copySign(source, source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half CopySign(Half x, Half y)
        {
            return new Half((ushort)((y._storage & SIGN_MASK) | (x._storage & (BIASED_EXPONENT_MASK | MANTISSA_MASK))));
        }

        /*
         * Ceiling, floor and round should use System.MathF implementations, sice they would require to add or subtract one form the Half
         * which would cast to a float anyways.
         */
        /// <summary>
        /// Returns the 
        /// </summary>
        /// <remarks></remarks>
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

        /// <summary>
        /// Returns the remainder of x divided by y.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "sourceFormat remainder(source, source)".<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Remainder(in Half x, in Half y)
        {
            // Preserve NaN
            if (IsNaN(x)) return x;
            if (IsNaN(y)) return y;
            if (IsInfinity(y))
                return x;
            return x - y * Truncate(x / y);
        }

        /// <summary>
        /// Returns the number greater then the value, by the smallest possible margin.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "sourceFormat nextUp(source)".</remarks>
        public static Half NextUp(in Half value)
        {
            if (IsNegativeInfinity(value))
                return Minimum;
            if (IsPositiveInfinity(value))
                return value;
            if (IsZero(value) || IsSubnormal(value))
                return SmallestPosNormal;
            ushort mant = (ushort)((value._storage & MANTISSA_MASK) + 1);
            // Mantissa is at max.
            if (mant >= MANTISSA_MASK)
            {
                // Increment exponent.
                ushort bexp = (ushort)((value._storage & BIASED_EXPONENT_MASK) + (1 << 10));
                // Exponent is at max.
                if (bexp >= BIASED_EXPONENT_MASK)
                    return IsNegative(value) ? NegInfinity : Infinity;
                return new Half((ushort)((value._storage & ~BIASED_EXPONENT_MASK) | (bexp & BIASED_EXPONENT_MASK)));
            }
            return new Half((ushort)((value._storage & ~MANTISSA_MASK) | (mant & MANTISSA_MASK)));
        }

        /// <summary>
        /// Returns the number smaller then the value, by the smallest possible margin.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "sourceFormat nextDown(source)".</remarks>
        public static Half NextDown(in Half value)
        {
            if (IsPositiveInfinity(value))
                return Maximum;
            if (IsNegativeInfinity(value))
                return value;
            if (IsZero(value) || IsSubnormal(value))
                return -SmallestPosNormal;
            ushort mant = (ushort)(value._storage & MANTISSA_MASK);
            // Mantissa is at min.
            if (mant == 0)
            {
                // Decrement exponent.
                ushort bexp = (ushort)((value._storage & BIASED_EXPONENT_MASK) - (1 << 10));
                // Exponent is at min.
                // Mantissa mask + 1 = min exponent.
                if (bexp <= MANTISSA_MASK + 1)
                    return IsNegative(value) ? NegZero : Zero;
                return new Half((ushort)((value._storage & ~BIASED_EXPONENT_MASK) | (bexp & BIASED_EXPONENT_MASK)));
            }
            return new Half((ushort)((value._storage & ~MANTISSA_MASK) | ((mant - 1) & MANTISSA_MASK)));
        }

        /// <summary>
        /// Returns a value with the numerical value x and the exponent y.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "sourceFormat quantize(source, source)".</remarks>
        public static Half Quantize(in Half x, in Half y)
        {
            if (IsNaN(x))
                return x;
            if (IsNaN(y))
                return y;
            if (IsInfinity(x))
                return x;
            // Preserve sign of x
            if (IsInfinity(y))
                return new Half((ushort)((y._storage & (BIASED_EXPONENT_MASK | MANTISSA_MASK)) | (x._storage & SIGN_MASK)));
            return x ^ y;
        }

        /// <summary>
        /// Returns the exponent of the value.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "sourceFormat quantum(source, source)".</remarks>
        public static Half GetExponent(in Half value)
        {
            if (IsInfinity(value))
                return Infinity;
            return GetBase2Exponent(value);
        }

        /// <summary>
        /// Returns the value x * (2 ^ y).
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "sourceFormat ScaleB(source, logBFormat)".</remarks>
        public static Half Scale2(Half x, int y)
        {
            if (IsInfinity(x) || IsNaN(x))
                return x;
            return x * (2 ^ y);
        }
    }
}