using System.Diagnostics;
using System;
using System.Runtime.CompilerServices;
using System.Dynamic;

namespace System
{
    public readonly partial struct Half
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Negate(in Half value) => FromBits((ushort)(value._storage ^ SIGN_MASK));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Abs(in Half value) => FromBits((ushort)(value._storage & 0x7FFF));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Max(in Half x, in Half y) => IsNaN(x) || IsNaN(y) ? NaN : x.CompareTo(y) == 1 ? x : y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Min(in Half x, in Half y) => IsNaN(x) || IsNaN(y) ? NaN : x.CompareTo(y) == -1 ? y : x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half MaxMag(in Half x, in Half y) => Max(Abs(x), Abs(y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half MinMag(in Half x, in Half y) => Min(Abs(x), Abs(y));

        /// <summary>
        /// Returns the value of y with the sign of x.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat copySign(source, source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half CopySign(Half x, Half y) => new Half((ushort)((y._storage & SIGN_MASK) | (x._storage & (BIASED_EXPONENT_MASK | MANTISSA_MASK))));

        /// <summary>
        /// Rounds the value towards zero.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat roundToIntegralTowardZero(source)".<remarks>
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
        /// Rounds the value to an integral value toward positive infinity.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat roundToIntegralTowardPositive(source)". Implemented using the <see cref="MathF"/> libary.<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Ceiling(in Half value) => MathF.Ceiling(value);

        /// <summary>
        /// Rounds the value to an integral value toward negative infinity.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat roundToIntegralTowardNegative(source)". Implemented using the <see cref="MathF"/> libary.<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Floor(in Half value) => MathF.Floor(value);

        /// <summary>
        /// Rounds the value to the nearest integer, and uses the specified rounding convention for midpoint values.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat roundToIntegralTiesToEven(source)" and "sourceFormat roundToInegralTiesToAway(source)". Implemented using the <see cref="MathF"/> libary.<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Round(in Half value, MidpointRounding mode) => MathF.Round(value, mode);

        /// <summary>
        /// Rounds the value to a specifed number of fractional digits, and uses the specified rounding convention for midpoint values.
        /// </summary>
        /// <remarks>Implemented using the <see cref="MathF"/> libary.<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Round(in Half value, int digits, MidpointRounding mode) => MathF.Round(value, digits, mode);

        /// <summary>
        /// Rounds a single-precision floating-point value to the nearest integral value, and rounds midpoint values to the nearest even number.
        /// </summary>
        /// <remarks>Implemented using the <see cref="MathF"/> libary.<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Round(in Half value) => MathF.Round(value);

        /// <summary>
        /// Returns the remainder of x divided by y.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat remainder(source, source)".<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Remainder(in Half x, in Half y)
        {
            if (IsNaN(x))
                return x;
            if (IsNaN(y))
                return y;
            if (IsInfinity(y))
                return x;
            return x - y * Truncate(x / y);
        }

        /// <summary>
        /// Returns the number greater then the value, by the smallest possible margin.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat nextUp(source)".</remarks>
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
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat nextDown(source)".</remarks>
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
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat quantize(source, source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat quantum(source, source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half GetExponent(in Half value) => IsInfinity(value) ? Infinity : GetBase2Exponent(value);

        /// <summary>
        /// Returns the value x * (2 ^ y).
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat ScaleB(source, logBFormat)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Scale2(Half x, int y)
        {
            if (IsNaN(x) || IsPositiveInfinity(x))
                return x;
            return x * (2 ^ y);
        }

        /// <summary>
        /// Returns the natural (base e) logarithm of a specified number.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "logBFormat logB(source)".</remarks>
        // Ported from C to C# source http://freshmeat.sourceforge.net/projects/icsilog
        public static unsafe Half Log(in Half value)
        {
            if (IsNaN(value) || IsPositiveInfinity(value))
                return value;
            if (IsZero(value))
                return NegInfinity;
            if (IsNegative(value))
                return NaN;
            if (value == One)
                return Zero;
            const float log2_4 = 0.69314718055995f;
            int exp = GetBase2Exponent(value);
            int mant = value._storage & MANTISSA_MASK;
            return (exp + s_logTable[mant]) * log2_4;
        }

        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat squareRoot(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Sqrt(in Half value)
        {
            if (IsZero(value) && IsNegative(value))
                return value;
            return MathF.Sqrt(value);
        }
    }
}