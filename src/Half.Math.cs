using System.Runtime.CompilerServices;

namespace System
{
    public readonly partial struct Half
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Negate(in Half value) => FromBits((ushort)(value._storage ^ c_signMask));

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
        public static Half CopySign(in Half x, in Half y) => new Half((ushort)((y._storage & c_signMask) | (x._storage & (c_biasedExponentMask | c_mantissaMask))));

        /// <summary>
        /// Rounds the value towards zero.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat roundToIntegralTowardZero(source)".<remarks>
        public static Half Truncate(in Half value)
        {
            int unbiasedExponent = GetBase2Exponent(value);
            // Preserves subnormal: NaN, pos and neg infinity.
            if (unbiasedExponent == c_maxBase2ExponentValue)
                return value;
            // Decimal values smaller then one, round to zero
            if (unbiasedExponent < -1)
                return Zero;
            if (unbiasedExponent == -1)
                return IsSigned(value) ? NegOne : One;
            // Values greater or equal to 2048, cannot contain decimal digits in base 10.
            // log_2(2048) = 11
            if (unbiasedExponent >= 11)
                return value;
            return new Half((ushort)(
                // Copy but the mantissa
                (value._storage & ~c_mantissaMask) |
                // Erase significant decimal digit and lower from mantissa:
                // Mask storage with mantissa bits to keep.
                (value._storage & (c_mantissaMask & ~(0xFFFF << (11 - unbiasedExponent))))));
        }

        /// <summary>
        /// Rounds the value to an integral value toward positive infinity.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat roundToIntegralTowardPositive(source)". Implemented using the <see cref="MathF"/> libary.<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Ceiling(in Half value) => (Half)MathF.Ceiling(value);

        /// <summary>
        /// Rounds the value to an integral value toward negative infinity.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat roundToIntegralTowardNegative(source)". Implemented using the <see cref="MathF"/> libary.<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Floor(in Half value) => (Half)MathF.Floor(value);

        /// <summary>
        /// Rounds the value to the nearest integer, and uses the specified rounding convention for midpoint values.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat roundToIntegralTiesToEven(source)" and "sourceFormat roundToInegralTiesToAway(source)". Implemented using the <see cref="MathF"/> libary.<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Round(in Half value, MidpointRounding mode) => (Half)MathF.Round(value, mode);

        /// <summary>
        /// Rounds the value to a specifed number of fractional digits, and uses the specified rounding convention for midpoint values.
        /// </summary>
        /// <remarks>Implemented using the <see cref="MathF"/> libary.<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Round(in Half value, int digits, MidpointRounding mode) => (Half)MathF.Round(value, digits, mode);

        /// <summary>
        /// Rounds a single-precision floating-point value to the nearest integral value, and rounds midpoint values to the nearest even number.
        /// </summary>
        /// <remarks>Implemented using the <see cref="MathF"/> libary.<remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half Round(in Half value) => (Half)MathF.Round(value);

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
            return (Half)(x - y * Truncate((Half)(x / y)));
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
            ushort mant = (ushort)((value._storage & c_mantissaMask) + 1);
            // Mantissa is at max.
            if (mant >= c_mantissaMask)
            {
                // Increment exponent.
                ushort bexp = (ushort)((value._storage & c_biasedExponentMask) + (1 << 10));
                // Exponent is at max.
                if (bexp >= c_biasedExponentMask)
                    return IsSigned(value) ? NegInfinity : Infinity;
                return new Half((ushort)((value._storage & ~c_biasedExponentMask) | (bexp & c_biasedExponentMask)));
            }
            return new Half((ushort)((value._storage & ~c_mantissaMask) | (mant & c_mantissaMask)));
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
                return new Half(c_smallestPosNrm | c_signMask);
            ushort mant = (ushort)(value._storage & c_mantissaMask);
            // Mantissa is at min.
            if (mant == 0)
            {
                // Decrement exponent.
                ushort bexp = (ushort)((value._storage & c_biasedExponentMask) - (1 << 10));
                // Exponent is at min.
                // Mantissa mask + 1 = min exponent.
                if (bexp <= c_mantissaMask + 1)
                    return IsSigned(value) ? NegZero : Zero;
                return new Half((ushort)((value._storage & ~c_biasedExponentMask) | (bexp & c_biasedExponentMask)));
            }
            return new Half((ushort)((value._storage & ~c_mantissaMask) | ((mant - 1) & c_mantissaMask)));
        }

        /// <summary>
        /// Returns the exponent of the value.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "sourceFormat quantum(source, source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half GetExponent(in Half value) => IsInfinity(value) ? Infinity : (Half)GetBase2Exponent(value);
    }
}