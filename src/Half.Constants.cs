namespace System
{
    public readonly partial struct Half
    {
        private const ushort c_negZero = c_signMask,
                             c_zero = 0x0000,
                             c_negInf = c_biasedExponentMask | c_signMask,
                             c_posInf = c_biasedExponentMask,
                             c_quiteNaN = c_biasedOrSignificantMask,
                             c_signalingNan = c_biasedOrSignificantMask | c_mantissaMask,
                             c_smallestPosNrm = 0x0400,
                             c_minValueNrm = 0xFBFF,
                             c_maxValueNrm = 0x7BFF,
                             c_smallestPosSubnrm = 0x0001,
                             c_largestPosNrm = 0x03ff,
                             c_largestLessOne = 0x3bff,
                             c_smallestMoreOne = 0x3C01,
                             c_one = 0x3C00,
                             c_negOne = 0xBC00,
                             c_ten = 0x4900,
                             c_half = 0x0600,
                             c_third = 0x3555,
                             c_epsilon = 0x3C00, // ONE - SMALLEST_MORE_ONE
                             c_e = 0x402E,
                             c_pi = 0x4049,
                             c_reciPi = 0x3EA3,  // reciprocal
                             c_reciSqrt2 = 0x3F35,
                             c_frac2Pi = 0x3F23,
                             c_frac2SqrtPi = 0x3F90,
                             c_fracPi2 = 0x3FC9,
                             c_fracPi3 = 0x3F86,
                             c_ln10 = 0x4013,
                             c_ln2 = 0x3F31,
                             c_log10_E = 0x3EDE,
                             c_log10_2 = 0x3E9A,
                             c_log2_E = 0x3FB9,
                             c_log2_10 = 0x4055,
                             c_sqrt2 = 0x3FB5;
        public static readonly Half NegZero = new Half(c_negZero),
                                    Zero = new Half(c_zero),
                                    NegInfinity = new Half(c_negInf),
                                    Infinity = new Half(c_posInf),
                                    NaN = new Half(c_quiteNaN),
                                    SignalingNaN = new Half(c_signalingNan),
                                    SmallestPosNormal = new Half(c_smallestPosNrm),
                                    Minimum = new Half(c_minValueNrm),
                                    Maximum = new Half(c_maxValueNrm),
                                    // Other special values
                                    SmallestPosSubnormal = new Half(c_smallestPosSubnrm),
                                    LargestSubnormal = new Half(c_largestPosNrm),
                                    LargestLessThenOne = new Half(c_largestLessOne),
                                    SmallestLargerThenOne = new Half(c_smallestMoreOne),
                                    // Numeric values
                                    One = new Half(c_one),
                                    NegOne = new Half(c_negOne),
                                    Ten = new Half(c_ten),
                                    OneHalf = new Half(c_half),
                                    Thrid = new Half(c_third),
                                    Epsilon = new Half(c_epsilon),
                                    E = new Half(c_e),
                                    PI = new Half(c_pi),
                                    ReciPi = new Half(c_reciPi),
                                    ReciSqrt2 = new Half(c_reciSqrt2),
                                    Frac2_Pi = new Half(c_frac2Pi),
                                    Frac2_SqrtPi = new Half(c_frac2SqrtPi),
                                    FracPi_2 = new Half(c_fracPi2),
                                    FracPi_3 = new Half(c_fracPi3),
                                    Ln10 = new Half(c_ln10),
                                    Ln2 = new Half(c_ln2),
                                    Log10_E = new Half(c_log10_E),
                                    Log10_2 = new Half(c_log10_2),
                                    Log2_E = new Half(c_log2_E),
                                    Log2_10 = new Half(c_log2_10),
                                    Sqrt2 = new Half(c_sqrt2);
        public const float EpsilonS = 0.0009765625f;
        public const double EpsilonD = 0.0009765625;

        public const int MaxBase10Exponent = 38,
                         MinBase10Exponent = -37,
                         AvgSignificantBase10Digits = 2;
    }
}