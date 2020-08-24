using System.Runtime.CompilerServices;

namespace System
{
    public readonly partial struct Half
    {
        #region Cast
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float(in Half value) => HalfToSingle(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double(in Half value) => HalfToSingle(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator decimal(in Half value) => (decimal)HalfToSingle(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Half(in float value) => FloatToHalf(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Half(in double value) => FloatToHalf((float)value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Half(in decimal value) => FloatToHalf((float)value);
        #endregion

        #region Comparison
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Half left, in Half right) => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Half left, in Half right) => !left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(in Half left, in Half right) => InternalComparison(left, right, 0) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(in Half left, in Half right) => InternalComparison(left, right, 0) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in Half left, in Half right) => InternalComparison(left, right, 1) <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in Half left, in Half right) => InternalComparison(left, right, -1) >= 0;
        #endregion

        #region Arithmetic
        /*
         * Half doesnt implement its any operator logic because intrinsic FPArithmetic
         * is just that fast.
         * Every arithmetic operation on a Half value types will yield a Single,
         * because the conversion form Half to Single is rather lightweight.
         * 
         * The right parameter is a float not a Half, because while there is no effect on the syntax, because of the implicit conversion,
         * we can assume that operations to 32bit floating point numbers will occur with high frequency.
         * Accepting a float as right eliminates the redundant cast to Half and then to float in the operator function.
         */
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
        #endregion
    }
}