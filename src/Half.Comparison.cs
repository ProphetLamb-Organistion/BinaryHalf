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
                    return false;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EpsilonEquals(in double other) => Equals(other) || Math.Abs(this - other) <= EpsilonD;

        /// <summary>
        /// Indicates whether the current object is equal, or widthin margin of error to another object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EpsilonEquals(in float other) => Equals(other) || MathF.Abs(this - other) <= EpsilonS;

        /// <summary>
        /// Indicates whether the current object is equal, or widthin margin of error to another object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EpsilonEquals(in Half other) => Equals(other) || MathF.Abs(this - other) <= EpsilonS;
        #endregion

        #region IComparable members
        private static int InternalComparison(in Half left, in Half right, in int falsify)
        {
            /* 
             * In order to comply with the IEEE 754 NaN comparison specification that any comparison with NaN as a parameter will yield false,
             * the additional parameter contains the value to return if so desired is passed.
             */
            if (left._storage == right._storage)
                return 0;
            bool thisSigned = IsSigned(left),
                 otherSigned = IsSigned(right);
            if (IsSubnormal(left) || IsSubnormal(right))
            {
                // Not a number cannot be compared to a number
                if (IsNaN(left) || IsNaN(right))
                    return falsify;
                if (IsInfinity(left))
                    // +inf cmp +inf == 0, -inf cmp -inf == 0
                    return IsInfinity(right) && thisSigned == otherSigned ? 0
                    // otherwise, +inf > other and -inf < other
                         : thisSigned ? 1 : -1;
            }
            // Compare sign
            if (thisSigned != otherSigned)
                return thisSigned ? -1 : 1;
            // Compare exponent
            int bexpDelta = (left._storage & c_biasedExponentMask) - (right._storage & c_biasedExponentMask);
            if (bexpDelta < 0)
                return -1;
            if (bexpDelta > 0)
                return 1;
            // Compare mantissa
            return ((uint)left._storage & c_mantissaMask).CompareTo((uint)right._storage & c_mantissaMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Half other) => InternalComparison(this, other, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(float other) => InternalComparison(this, (Half)other, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(double other) => InternalComparison(this, (Half)other, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object? obj)
        {
            return obj switch
            {
                Half f16 => CompareTo(f16),
                float f32 => CompareTo(f32),
                double f64 => CompareTo(f64),
                _ => throw new ArgumentException("Object must be a Half, Single, or Double.")
            };
        }
        #endregion
    }
}