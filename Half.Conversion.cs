using System.Net.Http.Headers;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public readonly partial struct Half
    {
        private const uint F32_SIGN_MASK = 0x8000_0000,
                           F32_BIASED_EXPONENT_MASK = 0x7f80_0000,
                           F32_MANSTISSA_MASK = 0x007_fffff,
                           F32_SIGNIFICANT_BIT_FLAG = 0x0040_0000;
        private const ulong F64_SIGN_MASK = 0x8000_0000_0000_0000,
                            F64_BIASED_EXPONENT_MASK = 0x7FF0_0000_0000_0000,
                            F64_MANTISSA_MASK = 0x000F_FFFF_FFFF_FFFF,
                            F64_SIGNIFICANT_BIT_FLAG = 0x0008_0000_0000_0000;

        private static unsafe float HalfToSingle(Half value)
        {
            uint f32Bits = HalfBitsToSingle(value._storage);
            return *(float*)&f32Bits;
        }

        private static unsafe Half FloatToHalf(float value)
        {
            uint storage = *(uint*)&value;
            return new Half(SingleBitsToHalf(storage));
        }

        /*
         * Original implementation of conversion functions
         * by github.com/starkat99 in repository half-rs
         */

        private static uint HalfBitsToSingle(ushort storage)
        {
            uint sign = (uint)(storage & SIGN_MASK) << 16,
                 bexp = (uint)(storage & BIASED_EXPONENT_MASK) >> 10,
                 mant = (uint)(storage & MANTISSA_MASK) << 13;
            if (bexp == MAX_BIASED_EXPONENT_VALUE)
            {
                // Returns pos or neg infinity.
                if (mant == 0)
                    return sign | F32_BIASED_EXPONENT_MASK | mant;
                // Return sNan or qNan preserving the payload.
                return sign | F32_BIASED_EXPONENT_MASK | F32_SIGNIFICANT_BIT_FLAG | mant;
            }
            if (bexp == 0)
            {
                // Returns pos or neg zero.
                if (mant == 0)
                    return sign;
                // Normalize the subnormal flaot 16
                bexp++;
                while ((mant & F32_BIASED_EXPONENT_MASK) == 0)
                {
                    // Shift mantissa value.
                    mant <<= 1;
                    // Decrement exponent.
                    bexp--;
                }
                // Eliminate stray bits introduced by normalization from mantissa.
                mant &= F32_MANSTISSA_MASK;
            }
            return sign | ((bexp + (0x7F - 0xF)) << 23) | mant;
        }

        private static ulong HalfBitsToDouble(ushort storage)
        {
            if ((storage & BIASED_EXPONENT_MASK) == 0)
                return (ulong)storage << 48;
            ulong sign = (ulong)(storage & SIGN_MASK) << 48,
                  bexp = (ulong)(storage & BIASED_EXPONENT_MASK) >> 7,
                  mant = (ulong)(storage & MANTISSA_MASK) << 45;
            if (bexp == BIASED_EXPONENT_MASK)
            {
                // Returns pos or neg infinity.
                if (mant == 0)
                    return sign | F64_BIASED_EXPONENT_MASK;
                // Return sNan or qNan preserving the payload.
                return sign | F64_BIASED_EXPONENT_MASK | F64_SIGNIFICANT_BIT_FLAG | mant;
            }
            if (bexp == 0 && mant == 0)
                return sign == F64_SIGN_MASK ? NEG_ZERO : ZERO;
            
        }

        private static ushort SingleBitsToHalf(uint storage)
        {
            unchecked
            {
                uint sign = storage & F32_SIGN_MASK,
                     bexp = storage & F32_BIASED_EXPONENT_MASK,
                     mant = storage & F32_MANSTISSA_MASK;
                // qNan, sNan, or +-Inf
                if (bexp == F32_BIASED_EXPONENT_MASK)
                {
                    if (mant == 0)
                        return sign == 0 ? POS_INF : NEG_INF;
                    // Treat sNaN as qNan, because accurate conversion of the payload cannot be ensured.
                    return QUITE_NAN;
                }
                if (bexp == 0 && mant == 0)
                    return sign == F32_SIGN_MASK ? NEG_ZERO : ZERO;
                uint f16Sign = sign >> 16;
                // Right align 8bit exponent, unbias 8bit exponent, bias 5bit exponent.
                int f16ExpValue = (int)(bexp >> 23 - 0x7F) + 0xF;
                // Returns pos or neg infinity, because the exponent is to large for 5bit.
                if (f16ExpValue >= MAX_BIASED_EXPONENT_VALUE)
                    return (ushort)(f16Sign | BIASED_EXPONENT_MASK);
                uint f16Mant,
                     hint;
                // Returns a subnormal value.
                if (f16ExpValue <= 0x00)
                {
                    // Return pos or neg zero if the difference between the exponents cant be compensated
                    // for by shifting the mantissa, i.e. the mantissa would jield zero anyways.
                    if (14 - f16ExpValue > 24)
                        return (ushort)f16Sign;
                    uint normMant = mant | F32_SIGN_MASK;
                    // Shift mantissa to compensate for exponent.
                    f16Mant = mant >> (14 - f16ExpValue);
                    // Extract round hint from not normalized biased exponent.
                    hint = (uint)1 << (13 - f16ExpValue);
                    if ((normMant & hint) != 0 && (normMant & (3 * hint - 1)) != 0)
                        f16Mant++;
                    return (ushort)(f16Sign | f16Mant);
                }
                // Returns a normal value.
                uint f16Bexp = (uint)(f16ExpValue << 10);
                f16Mant = mant >> 13;
                hint = 0x00001000;
                if ((mant & hint) != 0 && (mant & (3 * hint - 1)) != 0)
                    return (ushort)((f16Sign | f16Bexp | f16Mant) + 1);
                return (ushort)(f16Sign | f16Bexp | f16Mant);
            }
        }

        #region String parse
        /*
         * Implemented using Single.
         */
        public static Half Parse(string s) => (Half)Single.Parse(s);

        public static Half Parse(string s, NumberStyles style) => (Half)Single.Parse(s, style);

        public static Half Parse(string s, IFormatProvider provider) => (Half)Single.Parse(s, provider);

        public static Half Parse(string s, NumberStyles style, IFormatProvider provider) => (Half)Single.Parse(s, style, provider);

        public static bool TryParse(string s, out Half result)
        {
            return TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.CurrentInfo, out result);
        }

        public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out Half result)
        {
            float res = Single.NaN;
            bool success = Single.TryParse(s, style, provider, out res);
            result = (Half)res;
            return success;
        }

        private static bool TryParse(string s, NumberStyles style, NumberFormatInfo info, out Half result)
        {
            float res = Single.NaN;
            bool success = Single.TryParse(s, style, info, out res);
            result = (Half)res;
            return success;
        }
        #endregion

        #region IConvertible members
        /*
         * Implemented using Single.
         */
        TypeCode IConvertible.GetTypeCode() => TypeCode.Single;

        bool IConvertible.ToBoolean(IFormatProvider provider) => _storage == ZERO;

        byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte(this, provider);

        char IConvertible.ToChar(IFormatProvider provider) => Convert.ToChar(this, provider);

        DateTime IConvertible.ToDateTime(IFormatProvider provider) => Convert.ToDateTime(this, provider);

        decimal IConvertible.ToDecimal(IFormatProvider provider) => Convert.ToDecimal(this, provider);

        double IConvertible.ToDouble(IFormatProvider provider) => Convert.ToDouble(this, provider);

        short IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16(this, provider);

        int IConvertible.ToInt32(IFormatProvider provider) => Convert.ToInt32(this, provider);

        long IConvertible.ToInt64(IFormatProvider provider) => Convert.ToInt64(this, provider);

        sbyte IConvertible.ToSByte(IFormatProvider provider) => Convert.ToSByte(this, provider);

        float IConvertible.ToSingle(IFormatProvider provider) => Convert.ToSingle(this, provider);

        string IConvertible.ToString(IFormatProvider provider) => Convert.ToString(this, provider);

        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => Convert.ChangeType((float)this, conversionType, provider);

        ushort IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16(this);

        uint IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32(this);

        ulong IConvertible.ToUInt64(IFormatProvider provider) => Convert.ToUInt64(this);
        #endregion
    }
}