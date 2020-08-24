using System.Runtime.CompilerServices;

namespace System
{
    public readonly partial struct Half
    {
        #region Cast
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
        public static explicit operator Half(in float value)
        {
            return FloatToHalf(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Half(in double value)
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
        public static bool operator <(in Half left, in Half right) => left.CompareTo(right) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(in Half left, in Half right) => left.CompareTo(right) > 0;

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
        #endregion
    }
}