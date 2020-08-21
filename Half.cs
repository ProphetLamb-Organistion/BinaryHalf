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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(in Half value) => (value._storage & SIGN_MASK) == SIGN_MASK;

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

        // Biased Exponent is zero and significant bit is set.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSubnormal(in Half value) => (value._storage & BIASED_OR_SIGNIFICANT_MASK) == SIGNIFICANT_BIT_FLAG;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNormal(in Half value)
        {
            ushort biasedExp = (ushort)(value._storage & BIASED_EXPONENT_MASK);
            return biasedExp != 0 && biasedExp != BIASED_EXPONENT_MASK;
        }

        // Biased Exponent is zero and significant bit is unset.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(in Half value) => (value._storage & BIASED_OR_SIGNIFICANT_MASK) == 0;

        // Biased Exponent is max and NAN_FLAG is not set.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinity(in Half value) => (value._storage & BIASED_OR_SIGNIFICANT_MASK) == BIASED_EXPONENT_MASK;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositiveInfinity(in Half value) => (value._storage & BIASED_OR_SIGNIFICANT_MASK) == BIASED_OR_SIGNIFICANT_MASK && (value._storage & SIGN_MASK) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegativeInfinity(in Half value) => (value._storage & NEG_INF) == NEG_INF;

        // Biased Exponent is max and NAN_FLAG is set.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(in Half value) => (value._storage & (BIASED_EXPONENT_MASK | SIGNIFICANT_BIT_FLAG)) == (BIASED_EXPONENT_MASK | SIGNIFICANT_BIT_FLAG);

        // Signaling nan flag bit - 2nd mantissa bit - is set.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSignaling(in Half value) => (value._storage & SIGNALING_NAN_FLAG) != 0;

        // Payload mask is 0x00FF - the 2nd byte of the _storage. It can be obtained by left shifting 8 bits.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetSignalingNaNPayload(in Half value) => (byte)(value._storage << 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetBase2Exponent(in Half value) => ((value._storage & BIASED_EXPONENT_MASK) >> 10) - EXPONENT_BIAS;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort GetBits() => _storage;

        /// <summary>
        /// Returns whether the 16bit floating point number is infinite.
        /// If sign is negative, then true if the value is negative infinity.
        /// If sign is positive, then true if the value is positive infinity.
        /// If sign is 0, then true if the value is posivie or negative infinity.
        /// </summary>
        /// <param name="sign">The sign value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinite(in Half value, int sign)
        {
            return IsInfinity(value) && (sign > 0 ? !IsNegative(value) : (sign <= 0 || IsNegative(value)));
        }
        #endregion

        public string ToString(string format, IFormatProvider formatProvider) => ((float)this).ToString(format, formatProvider);
    }
}
