using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryHalf.UnitTest
{
    public class UnitTest
    {
        [SetUp]
        public void SetUp()
        { }

        const ushort c_maxMantissa = 0x03FF,
                     c_minExponent = 0x0400,
                     c_maxExponent = 0x7C00,
                     c_sign = 0x8000;
        ushort _mant = 0, _bexp = 0, _sign = 0;

        Half GenerateNextHalf()
        {
            // Maximum value
            if (_mant >= c_maxMantissa && _bexp >= c_maxExponent && _sign != 0)
                Assert.Pass();
            // Toggle sign bit
            if (_sign == 0)
                return Half.FromBits((ushort)(_mant | _bexp | (_sign = c_sign)));
            else
                _sign = 0;
            if (_mant >= c_maxMantissa)
            {
                // Increment exponent, reset mantissa
                _mant = 0;
                _bexp += c_minExponent;
                return Half.FromBits((ushort)(_mant | _bexp | _sign));
            }
            // Increment mantissa
            _mant++;
            var f = Half.FromBits((ushort)(_mant | _bexp | _sign));
            return f;
        }

        [Test]
        public void ConversionTest()
        {
            for (int iter = 0; ; iter++)
            {
                // Generate next half asserts Pass when all combinations are tested.
                Half origin = GenerateNextHalf();
                float lossless = origin;
                Half fromFloat = (Half)lossless;
                Assert.IsTrue(origin.EpsilonEquals(fromFloat) || Half.IsNaN(origin)); // NaN =|= NaN
                Assert.IsTrue(origin.Equals(fromFloat) || Half.IsNaN(origin));
            }
        }

        [Test]
        public void EqualsTest()
        {
            Assert.AreEqual(Half.E, Half.E);
            // Zero ignores sign
            Assert.IsTrue(Half.Zero.Equals(Half.NegZero));
            // Any NaN is equal
            Assert.IsTrue(Half.NaN.Equals(Half.CreateSignalingNan(0xFF)));
            Assert.Pass();
        }
    }
}
