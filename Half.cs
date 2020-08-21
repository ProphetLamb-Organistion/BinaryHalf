using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 2)]
    public readonly partial struct Half :
        IComparable, IConvertible, IFormattable,
        IComparable<Half>, IComparable<float>, IComparable<double>,
        IEquatable<Half>, IEquatable<float>, IEquatable<double>
    {
        [FieldOffset(0)]
        private readonly ushort _storage;

        private Half(ushort binaryData) => _storage = binaryData;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half FromBits(ushort littleEdian)
        {
            return new Half(littleEdian);
        }

        public static Half FromBigEdianBits(ushort bigEdian)
        {
            byte[] bytes = BitConverter.GetBytes(bigEdian);
            Array.Reverse(bytes);
            return new Half(BitConverter.ToUInt16(bytes));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half CreateSignalingNan(byte payload)
        {
            return new Half((ushort)(SIGNALING_NAN | payload));
        }

        #region Static members
        public enum NumericClass
        {
            SignalingNaN,
            QuiteNaN,
            NegativeInfinity,
            NegativeNormal,
            NegativeSubnormal,
            NegativeZero,
            PositiveZero,
            PositiveSubnormal,
            PositiveNormal,
            PositiveInfinity
        };

        public enum RadixMode : byte
        {
            Two =  2,
            Ten = 10
        }

        /// <summary>
        /// Returns the numeric class of the value.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "enum class(source)".</remarks>
        public static NumericClass GetClass(in Half value)
        {
            if (IsNaN(value))
            {
                if (IsSignaling(value))
                    return NumericClass.SignalingNaN;
                return NumericClass.QuiteNaN;
            }
            if (IsPositive(value))
            {
                if (IsInfinity(value))
                    return NumericClass.PositiveInfinity;
                if (IsSubnormal(value))
                    return NumericClass.PositiveSubnormal;
                if (IsZero(value))
                    return NumericClass.PositiveZero;
                return NumericClass.PositiveNormal;
            }
            else
            {
                if (IsInfinity(value))
                    return NumericClass.NegativeInfinity;
                if (IsSubnormal(value))
                    return NumericClass.NegativeSubnormal;
                if (IsZero(value))
                    return NumericClass.NegativeZero;
                return NumericClass.NegativeNormal;
            }
        }

        /// <summary>
        /// Returns whether the value of negative.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "boolean isSignMinus(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(in Half value) => (value._storage & SIGN_MASK) == SIGN_MASK;

        /// <summary>
        /// Returns whether the value of positive.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive(in Half value) => (value._storage & SIGN_MASK) == 0;

        /// <summary>
        /// Return a 32-bit signed integer indicating the sign of the <see cref="Half"/>.
        /// </summary>
        /// <returns>
        /// 1 if the value is positive, +0.0 or positve infinity;
        /// -1 if the value is negative, -0.0 or negative infinity;
        /// otherise, 0.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSign(in Half value) => IsNaN(value) ? 0 : (value._storage & SIGN_MASK) != 0 ? -1 : 1;

        /// <summary>
        /// Returns whether the value is subnormal.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "boolean isSubnormal(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Biased Exponent is zero and significant bit is set.
        public static bool IsSubnormal(in Half value) => (value._storage & BIASED_OR_SIGNIFICANT_MASK) == SIGNIFICANT_BIT_FLAG;

        /// <summary>
        /// Returns whether the value is normal (not zero, subnormal, infinite, or NaN).
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "boolean isNormal(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNormal(in Half value)
        {
            ushort biasedExp = (ushort)(value._storage & BIASED_EXPONENT_MASK);
            return biasedExp != 0 && biasedExp != BIASED_EXPONENT_MASK;
        }

        /// <summary>
        /// Returns whether the value is zero, subnormal or normal (not infinite or NaN).
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "boolean isFinite(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFinite(in Half value)
        {
            return IsNormal(value) || (IsSubnormal(value) && !IsNaN(value) && !IsInfinity(value));
        }

        /// <summary>
        /// Returns whether the value is negative or positve zero.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "boolean isZero(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Biased Exponent is zero and significant bit is unset.
        public static bool IsZero(in Half value) => (value._storage & BIASED_OR_SIGNIFICANT_MASK) == 0;

        /// <summary>
        /// Returns whether the value is positive or negative infinity.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "boolean isInfinity(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Biased Exponent is max and NAN_FLAG is not set.
        public static bool IsInfinity(in Half value) => (value._storage & BIASED_OR_SIGNIFICANT_MASK) == BIASED_EXPONENT_MASK;

        /// <summary>
        /// Returns whether the value is positive infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositiveInfinity(in Half value) => (value._storage & BIASED_OR_SIGNIFICANT_MASK) == BIASED_OR_SIGNIFICANT_MASK && (value._storage & SIGN_MASK) == 0;

        /// <summary>
        /// Returns whether the value is negative infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegativeInfinity(in Half value) => (value._storage & NEG_INF) == NEG_INF;

        /// <summary>
        /// Returns whether the value is quite or signaling NaN.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "boolean isNaN(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Biased Exponent is max and NAN_FLAG is set.
        public static bool IsNaN(in Half value) => (value._storage & (BIASED_EXPONENT_MASK | SIGNIFICANT_BIT_FLAG)) == (BIASED_EXPONENT_MASK | SIGNIFICANT_BIT_FLAG);

        /// <summary>
        /// Returns whether the NaN value is signaling.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "boolean isSignaling(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Signaling nan flag bit - 2nd mantissa bit - is set.
        public static bool IsSignaling(in Half value) => (value._storage & SIGNALING_NAN_FLAG) != 0;

        /// <summary>
        /// Returns the payload of a signaling NaN.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Payload mask is 0x00FF - the 2nd byte of the _storage. It can be obtained by left shifting 8 bits.
        public static byte GetSignalingNaNPayload(in Half value) => (byte)(value._storage << 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetBase2Exponent(in Half value) => ((value._storage & BIASED_EXPONENT_MASK) >> 10) - EXPONENT_BIAS;

        /// <summary>
        /// Returns the radix mode (2) of <see cref="Half"/>.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "enum radix(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RadixMode GetRadix(in Half value) => RadixMode.Two;

        /// <summary>
        /// Returns the toral order of x relative to y.
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "boolean totalOrder(source, source)".</remarks>
        public static bool GetTotalOrder(in Half x, in Half y)
        {
            if (IsNaN(x) && IsNaN(y))
                return IsPositive(x) && IsNegative(y);
            if (IsNaN(x) || IsNaN(y))
            {
                if (IsNaN(x))
                    return IsNegative(x);
                if (IsNaN(y))
                    return IsPositive(y);
            }
            if (x.CompareTo(y) < 0) return true;
            if (x.CompareTo(y) > 0) return true;
            if (x == NegZero && y == Zero)
                return true;
            if (x == Zero && y == NegZero)
                return false;
            if (IsNegative(x) || IsNegative(y))
                return GetBase2Exponent(x) >= GetBase2Exponent(y);
            else
                return GetBase2Exponent(x) <= GetBase2Exponent(y);
        }

        /// <summary>
        /// Returns the toral order of abs(x) relative to abs(y).
        /// </summary>
        /// <remarks>IEEE 754-2019 complicant implementation of "boolean totalOrderMag(source, source)".</remarks>
        public static bool GetTotalOrderMag(in Half x, in Half y) => GetTotalOrder(Abs(x), Abs(y));

        /// <summary>
        /// Returns whether the 16bit floating point number is infinite.
        /// If sign is negative, then true if the value is negative infinity.
        /// If sign is positive, then true if the value is positive infinity.
        /// If sign is 0, then true if the value is posivie or negative infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinite(in Half value, int sign)
        {
            return IsInfinity(value) && (sign > 0 ? !IsNegative(value) : (sign <= 0 || IsNegative(value)));
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort GetBits() => _storage;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider) => ((float)this).ToString(format, formatProvider);
    }
}
