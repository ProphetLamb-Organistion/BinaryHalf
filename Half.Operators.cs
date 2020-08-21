using System;
using System.Runtime.CompilerServices;

namespace System
{
    public readonly partial struct Half
    {
        #region Cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator byte(in Half value)
        {
            return (byte)HalfToSingle(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator char(in Half value)
        {
            return (char)HalfToSingle(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(in Half value)
        {
            return (int)HalfToSingle(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator long(in Half value)
        {
            return (long)HalfToSingle(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float(in Half value)
        {
            return HalfToSingle(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double(in Half value)
        {
            return HalfToSingle(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator decimal(in Half value)
        {
            return (decimal)HalfToSingle(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Half(in byte value)
        {
            return FloatToHalf(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Half(in char value)
        {
            return FloatToHalf(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Half(in int value)
        {
            return FloatToHalf(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Half(in long value)
        {
            return FloatToHalf(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Half(in float value)
        {
            return FloatToHalf(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Half(in double value)
        {
            return FloatToHalf((float)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Half(in decimal value)
        {
            return FloatToHalf((float)value);
        }
        #endregion

        #region Comparison
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Half left, in Half right) => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Half left, in Half right) => !left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator < (in Half left, in Half right) => left.CompareTo(right) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator > (in Half left, in Half right) => left.CompareTo(right) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in Half left, in Half right) => left.CompareTo(right) <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in Half left, in Half right) => left.CompareTo(right) >= 0;
        #endregion

        #region Arithmetic
        /*
         * Half doesnt implement its any operator logic because intrinsic FPArithmetic
         * is just that fast.
         * Every arithmetic operation on a Half value types will yield a Single,
         * because the conversion form Half to Single is rather lightweight.
         */
        // Half to Half
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator +(in Half left, in Half right)
        {
            return HalfToSingle(left) + HalfToSingle(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator -(in Half left, in Half right)
        {
            return HalfToSingle(left) - HalfToSingle(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator *(in Half left, in Half right)
        {
            return HalfToSingle(left) * HalfToSingle(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator /(in Half left, in Half right)
        {
            return HalfToSingle(left) / HalfToSingle(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator ^(in Half left, in Half right)
        {
            return MathF.Pow(HalfToSingle(left), HalfToSingle(right));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator %(in Half left, in Half right)
        {
            return HalfToSingle(left) % HalfToSingle(right);
        }

        // Half to Single
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator +(in Half left, in float right)
        {
            return HalfToSingle(left) + right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator -(in Half left, in float right)
        {
            return HalfToSingle(left) - right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator *(in Half left, in float right)
        {
            return HalfToSingle(left) * right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator /(in Half left, in float right)
        {
            return HalfToSingle(left) / right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator ^(in Half left, in float right)
        {
            return MathF.Pow(HalfToSingle(left), right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator %(in Half left, in float right)
        {
            return HalfToSingle(left) % right;
        }

        // Single to Half
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator +(in float left, in Half right)
        {
            return left + HalfToSingle(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator -(in float left, in Half right)
        {
            return left - HalfToSingle(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator *(in float left, in Half right)
        {
            return left * HalfToSingle(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator /(in float left, in Half right)
        {
            return left / HalfToSingle(right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator ^(in float left, in Half right)
        {
            return MathF.Pow(left, HalfToSingle(right));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float operator %(in float left, in Half right)
        {
            return left % HalfToSingle(right);
        }
        #endregion
    }
}