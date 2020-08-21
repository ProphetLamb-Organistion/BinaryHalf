using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public readonly partial struct Half
    {
        #region IEquatable members
        public override bool Equals(object? obj) => obj is Half half && Equals(half);

        public override int GetHashCode() => _storage;

        public bool Equals(float other) => other.Equals(this);

        public bool Equals(Half other) => IsNaN(this) && IsNaN(other) || _storage == other._storage;

        public bool EpsilonEquals(float other) => (this - other) <= EpsilonS;

        public bool EpsilonEquals(Half other) => (this - other) <= EpsilonS;

        public bool Equals(double other) => (this - other) <= EpsilonD;
        #endregion

        #region IComparable members
        public int CompareTo(object obj)
        {
            if (obj is Half f16)
                return CompareTo(f16);
            if (obj is float f32)
                return CompareTo(f32);
            if (obj is double f64)
                return CompareTo(f64);
            throw new ArgumentException("Object must be a Half, Single, or Double.");
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
            if (IsNegative(this) != IsNegative(other))
                return IsNegative(this) ? -1 : 1;
            // Compare exponent
            int expDelta = (_storage & BIASED_EXPONENT_MASK) - (other._storage & BIASED_EXPONENT_MASK);
            if (expDelta < 0)
                return -1;
            if (expDelta > 0)
                return 1;
            // Compare mantissa
            return (_storage & MANTISSA_MASK).CompareTo(other._storage & MANTISSA_MASK);
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
            return ((float)this).CompareTo(other);
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
            return ((double)this).CompareTo(other);
        }
        #endregion
    }
}