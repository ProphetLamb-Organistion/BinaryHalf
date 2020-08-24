using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 2)]
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public readonly partial struct Half :
        IComparable, IConvertible, IFormattable,
        IComparable<Half>, IComparable<float>, IComparable<double>, IComparable<object?>,
        IEquatable<Half>, IEquatable<float>, IEquatable<double>, IEquatable<object?>
    {
        [FieldOffset(0)]
        private readonly ushort _storage;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Half(ushort binaryData) => _storage = binaryData;

        #region Constructors
        /// <summary>
        /// Returns a new instance of Half with the specified storage bits.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half FromBits(in ushort littleEdian) => new Half(littleEdian);

        /// <summary>
        /// Returns a new signaling NaN Half with the specified payload.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Half CreateNan(byte payload) => new Half((ushort)(c_signalingNan | payload));
        #endregion

        #region Static members
        public enum NumericClass : sbyte
        {
            SignalingNaN =     -17,
            QuiteNaN =         -16,
            NegativeInfinity =  -4,
            NegativeNormal =    -3,
            NegativeSubnormal = -2,
            NegativeZero =      -1,
            PositiveZero =       1,
            PositiveSubnormal =  2,
            PositiveNormal =     3,
            PositiveInfinity =   4
        };

        public enum RadixMode : byte
        {
            Two = 0x02,
            Ten = 0x0A
        }

        /// <summary>
        /// Returns the numeric class of the value.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "enum class(source)".</remarks>
        public static NumericClass GetClass(in Half value)
        {
            if (IsNaN(value))
            {
                if (IsSignaling(value))
                    return NumericClass.SignalingNaN;
                return NumericClass.QuiteNaN;
            }
            if (!IsSigned(value))
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
        /// Indicates whether the sign bit is set and the value is negative.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "boolean isSignMinus(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSigned(in Half value) => (value._storage & c_signMask) == c_signMask;

        /// <summary>
        /// Return a 32-bit signed integer indicating the sign of the <see cref="Half"/>.
        /// </summary>
        /// <returns>
        /// 1 if the value is positive, +0.0 or positve infinity;
        /// -1 if the value is negative, -0.0 or negative infinity;
        /// otherise, 0.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSign(in Half value) => IsNaN(value) ? 0 : (value._storage & c_signMask) != 0 ? -1 : 1;

        /// <summary>
        /// Indicates whether the value is subnormal.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "boolean isSubnormal(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Biased Exponent is zero or max
        public static bool IsSubnormal(in Half value) => (value._storage & c_biasedExponentMask) == c_biasedExponentMask || (value._storage & c_biasedExponentMask) == 0;

        /// <summary>
        /// Indicates whether the value is normal (not zero, subnormal, infinite, or NaN).
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "boolean isNormal(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNormal(in Half value)
        {
            ushort biasedExp = (ushort)(value._storage & c_biasedExponentMask);
            return biasedExp != 0 && biasedExp != c_biasedExponentMask;
        }

        /// <summary>
        /// Indicates whether the value is zero, subnormal or normal (not infinite or NaN).
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "boolean isFinite(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFinite(in Half value) => (value._storage & c_biasedExponentMask) != c_biasedExponentMask;

        /// <summary>
        /// Indicates whether the value is negative or positve zero.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "boolean isZero(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Biased Exponent is zero and significant bit is unset.
        public static bool IsZero(in Half value) => (value._storage & (c_biasedExponentMask | c_mantissaMask)) == 0;

        /// <summary>
        /// Indicates whether the value is positive or negative infinity.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "boolean isInfinity(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Biased Exponent is max and NAN_FLAG is not set.
        public static bool IsInfinity(in Half value) => (value._storage & c_biasedOrSignificantMask) == c_biasedExponentMask;

        /// <summary>
        /// Indicates whether the value is positive infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositiveInfinity(in Half value) => (value._storage & c_biasedOrSignificantMask) == c_biasedOrSignificantMask && (value._storage & c_signMask) == 0;

        /// <summary>
        /// Indicates whether the value is negative infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegativeInfinity(in Half value) => (value._storage & c_negInf) == c_negInf;

        /// <summary>
        /// Indicates whether the value is quite or signaling NaN.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "boolean isNaN(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Biased Exponent is max and NAN_FLAG is set.
        public static bool IsNaN(in Half value) => (value._storage & (c_biasedExponentMask | c_significantBitFlag)) == (c_biasedExponentMask | c_significantBitFlag);

        /// <summary>
        /// Indicates whether the NaN value is signaling.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "boolean isSignaling(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Signaling nan flag bit - 2nd mantissa bit - is set.
        public static bool IsSignaling(in Half value) => (value._storage & c_signalingNanFlag) != 0;

        /// <summary>
        /// Returns the payload of a signaling NaN.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Payload mask is 0x00FF - the 2nd byte of the _storage. It can be obtained by left shifting 8 bits.
        public static byte GetSignalingNaNPayload(in Half value) => (byte)(value._storage << 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetBase2Exponent(in Half value) => ((value._storage & c_biasedExponentMask) >> 10) - c_exponentMask;

        /// <summary>
        /// Returns the radix mode of <see cref="Half"/>. The enum value returned is equivalent to 0x02
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "enum radix(source)".</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RadixMode GetRadix(in Half value) => RadixMode.Two;

        /// <summary>
        /// Returns the toral order of x relative to y.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "boolean totalOrder(source, source)".</remarks>
        public static bool GetTotalOrder(in Half x, in Half y)
        {
            if (IsNaN(x) && IsNaN(y))
                return !IsSigned(x) && IsSigned(y);
            if (IsNaN(x))
                return IsSigned(x);
            if (IsNaN(y))
                return !IsSigned(y);
            if (x.CompareTo(y) < 0)
                return true;
            if (x.CompareTo(y) > 0)
                return true;
            if (x == NegZero && y == Zero)
                return true;
            if (x == Zero && y == NegZero)
                return false;
            if (IsSigned(x) || IsSigned(y))
                return GetBase2Exponent(x) >= GetBase2Exponent(y);
            else
                return GetBase2Exponent(x) <= GetBase2Exponent(y);
        }

        /// <summary>
        /// Returns the toral order of abs(x) relative to abs(y).
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "boolean totalOrderMag(source, source)".</remarks>
        public static bool GetTotalOrderMag(in Half x, in Half y) => GetTotalOrder(Abs(x), Abs(y));

        /// <summary>
        /// Indicates whether the 16bit floating point number is infinite.
        /// If sign is negative, then true if the value is negative infinity.
        /// If sign is positive, then true if the value is positive infinity.
        /// If sign is 0, then true if the value is posivie or negative infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinite(in Half value, int sign) => IsInfinity(value) && (sign > 0 ? !IsSigned(value) : (sign <= 0 || IsSigned(value)));
        #endregion

        public string GetDebuggerDisplay() => String.Format("[Value = {0} Sign = {1}, BinExp = {2}, Mant = {3}]", ((float)this).ToString(), IsSigned(this) ? 1 : 0, GetBase2Exponent(this), (_storage & c_mantissaMask));

        /// <summary>
        /// Represents the storage bits of the <see cref="Half"/>.
        /// </summary>
        /// <returns></returns>
        public ref readonly ushort GetBits(in Half value) { return ref value._storage; }

        public override string ToString() => ((float)this).ToString();

        public override int GetHashCode() => _storage;

        public string ToString(string format, IFormatProvider formatProvider) => ((float)this).ToString(format, formatProvider);
    }
}
