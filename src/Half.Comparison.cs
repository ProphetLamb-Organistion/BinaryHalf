using System.Runtime.CompilerServices;

namespace System
{
    public readonly partial struct Half
    {
        #region IEquatable members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj)
        {
            return obj is Half h && Equals(h)
                || obj is float f && Equals(f)
                || obj is double d && Equals(d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(double other) => Equals((Half)other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(float other) => Equals((Half)other);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <remarks>IEEE 754-2019 comform implementation of "boolean compareQuiteEqual(source, source)".</remarks>
        public bool Equals(Half other)
        {
            if (IsSubnormal(this) || IsSubnormal(other))
            {
                // Both are NaN; ignore signaling and payload
                if ((_storage & other._storage & c_quiteNaN) == c_quiteNaN)
                    return true;
                // Both are Zero; ignore sign
                if (IsZero(this) && IsZero(other))
                    return true;
                // Infinity of the same sign
                if (IsInfinity(this) && IsInfinity(other))
                    return IsSigned(this) == IsSigned(other);
            }
            return _storage == other._storage;
        }

        /// <summary>
        /// Indicates whether the current object is equal, or widthin margin of error to another object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EpsilonEquals(double other) => Equals(other) || Math.Abs(this - other) <= EpsilonD;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EpsilonEquals(float other) => Equals(other) || MathF.Abs(this - other) <= EpsilonS;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EpsilonEquals(Half other) => Equals(other) || MathF.Abs(this - other) <= EpsilonS;
        #endregion

        #region IComparable members
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object obj)
        {
            return obj switch
            {
                Half f16 => CompareTo(f16),
                float f32 => CompareTo(f32),
                double f64 => CompareTo(f64),
                _ => throw new ArgumentException("Object must be a Half, Single, or Double.")
            };
        }

        public int CompareTo(Half other)
        {
            if (_storage == other._storage)
                return 0;
            // Not a number cannot be compared to a number; ignore NaN payload.
            if (IsNaN(this) || IsNaN(other))
                return 0;
            if (IsSubnormal(this) && IsSubnormal(other))
            {
                if (IsPositiveInfinity(this))
                    return 1;
                if (IsNegativeInfinity(this))
                    return -1;
                // Subnormal values are equal.
                return 0;
            }
            // Compare sign
            if (IsSigned(this) != IsSigned(other))
                return IsSigned(this) ? -1 : 1;
            // Compare exponent
            int expDelta = (_storage & c_biasedExponentMask) - (other._storage & c_biasedExponentMask);
            if (expDelta < 0)
                return -1;
            if (expDelta > 0)
                return 1;
            // Compare mantissa
            return (_storage & c_mantissaMask).CompareTo(other._storage & c_mantissaMask);
        }

        public int CompareTo(float other)
        {
            // Not a number cannot be compared to a number; ignore NaN payload.
            if (IsNaN(this) || Single.IsNaN(other))
                return 0;
            if (IsSubnormal(this) && Single.IsSubnormal(other))
            {
                if (IsPositiveInfinity(this))
                    return 1;
                if (IsNegativeInfinity(this))
                    return -1;
                // Convert subnormal values to Single
            }
            return CompareTo((Half)other);
        }

        public int CompareTo(double other)
        {
            // Not a number cannot be compared to a number; ignore NaN payload.
            if (IsNaN(this) || Double.IsNaN(other))
                return 0;
            if (IsSubnormal(this) && Double.IsSubnormal(other))
            {
                if (IsPositiveInfinity(this))
                    return 1;
                if (IsNegativeInfinity(this))
                    return -1;
                // Convert subnormal values to Double
            }
            return CompareTo((Half)other);
        }
        #endregion
    }
}