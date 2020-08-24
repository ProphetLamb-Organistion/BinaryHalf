using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public readonly unsafe partial struct Half
    {
        // IEEE 754 16bit binary floating point masking and flag constants.
        private const ushort c_signMask = 0x8000,
                             c_biasedExponentMask = 0x7C00,
                             c_mantissaMask = 0x03FF,
                             c_exponentMask = 0xF,
                             c_significantBitFlag = 0x0200,
                             c_signalingNanFlag = 0x0100,
                             c_biasedOrSignificantMask = c_biasedExponentMask | c_significantBitFlag,
                             c_maxBiasedExponentMask = 0x1F;
        private const sbyte c_minBase2ExponentValue = -14,
                            c_maxBase2ExponentValue = 15; // = EXPONENT_BIAS
        /*
         * Conversion using lookup tables, ported from C++ as implemented in http://www.fox-toolkit.org/ftp/fasthalffloatconversion.pdf.
         * We are using pointers for our lookup tables to avoid nasty range checks and validations, that come with the benefits of managed arrays.
         */
        private static readonly uint* s_mantissaTable, s_exponentTable;
        private static readonly ushort* s_offsetTable, s_baseTable, s_shiftTable;

        static Half()
        {
            // Initialize mantissa table.
            s_mantissaTable = (uint*)Marshal.AllocHGlobal(sizeof(int) * 0x800);
            for (uint i = 1; i != 0x400; i++)
                s_mantissaTable[i] = ConvertMantissa(i);
            for (uint i = 0x400; i != 0x800; i++)
                s_mantissaTable[i] = 0x3800_0000u + ((i - 0x400) << 13);
            s_mantissaTable[0] = 0;

            // Initialize exponent table
            s_exponentTable = (uint*)Marshal.AllocHGlobal(sizeof(int) * 64);
            for (uint i = 1; i != 31; i++)
                s_exponentTable[i] = i << 23;
            for (uint i = 33; i != 63; i++)
                s_exponentTable[i] = 0x8000_0000u + ((i - 0x20) << 23);
            s_exponentTable[0] = 0;
            s_exponentTable[32] = 0x8000_0000u;
            s_exponentTable[31] = 0x4780_0000u;
            s_exponentTable[63] = 0xC780_0000u;

            // Initialize offset table
            s_offsetTable = (ushort*)Marshal.AllocHGlobal(sizeof(short) * 64);
            for (uint i = 1; i != 64; i++)
                s_offsetTable[i] = 0x0400;
            s_offsetTable[0] = 0;
            s_offsetTable[32] = 0;

            // Initialize base & shift table
            s_baseTable = (ushort*)Marshal.AllocHGlobal(sizeof(short) * 256);
            s_shiftTable = (ushort*)Marshal.AllocHGlobal(sizeof(short) * 512);
            for (int i = 0; i != 0x100; i++)
            {
                int e = i - 0x7F;
                if (e < -24)
                // Very small numbers map to zero
                {
                    s_baseTable[i | 0x000] = 0x0000;
                    s_baseTable[i | 0x100] = c_signMask;
                    s_shiftTable[i | 0x000] = 24;
                    s_shiftTable[i | 0x100] = 24;
                }
                else if (e < -14)
                // Small numbers mmap to denorms
                {
                    s_baseTable[i | 0x000] = (ushort)(0x0400 >> (-e + c_minBase2ExponentValue));
                    s_baseTable[i | 0x100] = (ushort)(0x0400 >> (-e + c_minBase2ExponentValue) | c_signMask);
                    s_shiftTable[i | 0x000] = (ushort)(-e - 1);
                    s_shiftTable[i | 0x100] = (ushort)(-e - 1);
                }
                else if (e <= 15)
                // Normal numbers just loose precision
                {
                    s_baseTable[i | 0x000] = (ushort)((e + c_maxBase2ExponentValue) << 10);
                    s_baseTable[i | 0x100] = (ushort)(((e + c_maxBase2ExponentValue) << 10) | c_signMask);
                    s_shiftTable[i | 0x000] = 13;
                    s_shiftTable[i | 0x100] = 13;
                }
                else if (e < 128)
                // Large numbers map to infinity
                {
                    s_baseTable[i | 0x000] = c_biasedExponentMask;
                    s_baseTable[i | 0x100] = 0xFC00;
                    s_shiftTable[i | 0x000] = 24;
                    s_shiftTable[i | 0x100] = 24;
                }
                else
                // Infinity and NaN
                {
                    s_baseTable[i | 0x000] = c_biasedExponentMask;
                    s_baseTable[i | 0x100] = 0xFC00;
                    s_shiftTable[i | 0x000] = 13;
                    s_shiftTable[i | 0x100] = 13;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ConvertMantissa(in uint i)
        {
            unchecked
            {
                uint m = i << 13; // Zero pad mantissaa
                uint e = 0; // Zero exponent
                while ((m & 0x0080_0000u) == 0) // While not normalized
                {
                    e -= 0x0080_0000u; // Decrement exponent (1 << 23)
                    m <<= 1; // Shift mantissa
                }
                m &= ~0x0080_0000u; // Clear leading bit
                e += 0x3880_0000u; // Adjust bias ((0x7F - 14) << 23)
                return m | e;
            }
        }

        private static float HalfToSingle(in Half value)
        {
            uint hBits = HalfBitsToSingle(value._storage);
            return *(float*)&hBits;
        }

        private static Half FloatToHalf(float value)
        {
            uint storage = *(uint*)&value;
            return new Half(SingleBitsToHalf(storage));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint HalfBitsToSingle(in ushort h)
        {
            //Debug
            int sigExp = h >> 10,
                mant = h & 0x03FF;
            uint fBExp = s_exponentTable[sigExp],
                 fMant = s_mantissaTable[s_offsetTable[sigExp] + (h & 0x03FF)];
            return fMant + fBExp;
            //return s_mantissaTable[s_offsetTable[h >> 10] + (h & 0x03FF)] + s_exponentTable[h >> 10];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort SingleBitsToHalf(in uint s)
        {
            return (ushort)(s_baseTable[(s >> 23) & 0x01FF] + ((s & 0x007F_FFFF) >> s_shiftTable[(s >> 23) & 0x01FF]));
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
            bool success = Single.TryParse(s, style, provider, out var res);
            result = (Half)res;
            return success;
        }

        private static bool TryParse(string s, NumberStyles style, NumberFormatInfo info, out Half result)
        {
            bool success = Single.TryParse(s, style, info, out var res);
            result = (Half)res;
            return success;
        }
        #endregion

        #region IConvertible members
        /*
         * Implemented using Single.
         */
        TypeCode IConvertible.GetTypeCode() => TypeCode.Single;

        bool IConvertible.ToBoolean(IFormatProvider provider) => _storage == c_zero;

        byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte((float)this, provider);

        char IConvertible.ToChar(IFormatProvider provider) => Convert.ToChar((float)this, provider);

        DateTime IConvertible.ToDateTime(IFormatProvider provider) => Convert.ToDateTime((float)this, provider);

        decimal IConvertible.ToDecimal(IFormatProvider provider) => Convert.ToDecimal((float)this, provider);

        double IConvertible.ToDouble(IFormatProvider provider) => Convert.ToDouble((float)this, provider);

        short IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16((float)this, provider);

        int IConvertible.ToInt32(IFormatProvider provider) => Convert.ToInt32((float)this, provider);

        long IConvertible.ToInt64(IFormatProvider provider) => Convert.ToInt64((float)this, provider);

        sbyte IConvertible.ToSByte(IFormatProvider provider) => Convert.ToSByte((float)this, provider);

        float IConvertible.ToSingle(IFormatProvider provider) => Convert.ToSingle((float)this, provider);

        string IConvertible.ToString(IFormatProvider provider) => Convert.ToString((float)this, provider);

        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => Convert.ChangeType((float)(float)this, conversionType, provider);

        ushort IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16((float)this);

        uint IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32((float)this);

        ulong IConvertible.ToUInt64(IFormatProvider provider) => Convert.ToUInt64((float)this);
        #endregion
    }
}