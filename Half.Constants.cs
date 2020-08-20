using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public readonly partial struct Half
    {
        private const ushort NEG_ZERO = SIGN_MASK,
                             ZERO = 0x0000,
                             NEG_INF = BIASED_EXPONENT_MASK | SIGN_MASK,
                             POS_INF = BIASED_EXPONENT_MASK,
                             QUITE_NAN = BIASED_OR_SIGNIFICANT_MASK,
                             SIGNALING_NAN = BIASED_OR_SIGNIFICANT_MASK | MANTISSA_MASK,
                             SMALLEST_POS_NRM = 0x0400,
                             MIN_VALUE_NRM = 0xFBFF,
                             MAX_VALUE_NRM = 0x7BFF,
                             SMALLEST_POS_SUB = 0x0001,
                             LARGEST_POS_SUB = 0x03ff,
                             LARGEST_LESS_ONE = 0x3bff,
                             SMALLEST_MORE_ONE = 0x3C01,
                             HALF = 0x0600,
                             THIRD = 0x3555,
                             EPSILON = 0x3C00, // ONE - SMALLEST_MORE_ONE
                             _E = 0x402E,
                             _PI = 0x4049,
                             RECI_PI = 0x3EA3,  // reciprocal
                             RECI_SQRT_2 = 0x3F35,
                             FRAC_2_PI = 0x3F23,
                             FRAC_2_SQRT_PI = 0x3F90,
                             FRAC_PI_2 = 0x3FC9,
                             FRAC_PI_3 = 0x3F86,
                             LN_10 = 0x4013,
                             LN_2 = 0x3F31,
                             LOG10_E = 0x3EDE,
                             LOG10_2 = 0x3E9A,
                             LOG2_E = 0x3FB9,
                             LOG2_10 = 0x4055,
                             SQRT_2 = 0x3FB5;
        public static readonly Half NegZero = new Half(NEG_ZERO),
                                    Zero = new Half(ZERO),
                                    NegInfinity = new Half(NEG_INF),
                                    Infinity = new Half(POS_INF),
                                    NaN = new Half(QUITE_NAN),
                                    SignalingNaN = new Half(SIGNALING_NAN),
                                    SmallestPosNormal = new Half(SMALLEST_POS_NRM),
                                    Minimum = new Half(MIN_VALUE_NRM),
                                    Maximum = new Half(MAX_VALUE_NRM),
                                    // Other special values
                                    SmallestPosSubnormal = new Half(SMALLEST_POS_SUB),
                                    LargestSubnormal = new Half(LARGEST_POS_SUB),
                                    LargestLessThenOne = new Half(LARGEST_LESS_ONE),
                                    SmallestLargerThenOne = new Half(SMALLEST_MORE_ONE),
                                    // Numeric values
                                    OneHalf = new Half(HALF),
                                    Thrid = new Half(THIRD),
                                    Epsilon = new Half(EPSILON),
                                    E = new Half(_E),
                                    PI = new Half(_PI),
                                    ReciPi = new Half(RECI_PI),
                                    ReciSqrt2 = new Half(RECI_SQRT_2),
                                    Frac2_Pi = new Half(FRAC_2_PI),
                                    Frac2_SqrtPi = new Half(FRAC_2_SQRT_PI),
                                    FracPi_2 = new Half(FRAC_PI_2),
                                    FracPi_3 = new Half(FRAC_PI_3),
                                    Ln10 = new Half(LN_10),
                                    Ln2 = new Half(LN_2),
                                    Log10_E = new Half(LOG10_E),
                                    Log10_2 = new Half(LOG10_2),
                                    Log2_E = new Half(LOG2_E),
                                    Log2_10 = new Half(LOG2_10),
                                    Sqrt2 = new Half(SQRT_2);
        internal const float EpsilonS = 0.0009765625f;
        internal const double EpsilonD = 0.0009765625;

        public const int MaximumBase10Exponent = 38,
                         MinimumBase10Exponent = -37,
                         SignificantBase10Digits = 2;
    }
}