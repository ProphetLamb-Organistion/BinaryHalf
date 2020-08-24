# BinaryHalf

A paritally IEEE 754 conform 16bit binary floating point implementation written in C#.

## IEEE 754-2019 conformaty

All implemented methods are IEEE 754-2019 conform, but only general operations (5.7.2), comparison (5.6, 5.11), and equality comparison operation (5.11) are implemented. For all other operations and functionally please refer the the `System.MathF` libary. The main reason for not implementing own arithmetic operations is that, the implementation for the Single (IEEE 754 32bit binary floating point) datatype are supported by intrinsic methods, and thus immensely faster then any software implementation. Taking into account, that casting to Single is backed by branchless unmanged LUT lookups (which results in very quick conversions), there is no reason to write software code for such operations.

Furthermore this valuetype does not implement any signaling comparison operations, i.e. no operation on sNaN or qNaN will result in an `InvalidOperationException` to be thrown, instead the NaN value with payload is preserved or silenced.

## Constructors

Half has no publicly exposed constructor methods, but static constructors.

* `FromBits(ushort)`: Returns a new instance of Half with the specified storage bits.
* `CreateNan(byte)`: Returns a new signaling NaN Half with the specified payload.

## General operations

Entires maked with [Non IEEE] are extra functionallity not definded in the IEEE 754-2019 specification.

* `Boolean IsSigned(Half)`: Indicates whether the sign bit is set and value is negative.
* `Int32 GetSign(Half)`: [Non IEEE] Return a 32-bit signed integer indicating the sign of the Half.
  if the value is positive, 0 or positve infinity, 1;
  if the value is negative, -0 or negative infinity, -1;
  otherwise, 0.
* `Boolean IsSubnormal(Half)`: Indicates whether the value is subnormal: the exponent is eiter 0x00 or 0x1F.
* `Boolean IsNormal(Half)`: Indicates whether the value is normal (not zero, subnormal, infinite, or NaN).
* `Boolean IsFinite(Half)`: Indicates whether the value is zero, subnormal or normal (not infinite or NaN).
* `Boolean IsZero(Half)`: Indicates whether the value is negative or positve zero.
* `Boolean IsInfinity(Half)`: Indicates whether the value is positive or negative infinity.
* `Boolean IsPositiveInfinity(Half)`: [Non IEEE] Indicates whether the value is positive infinity.
* `Boolean IsNegativeInfinity(Half)`: [Non IEEE] Indicates whether the value is negative infinity.
* `Boolean IsNan(Half)`: Indicates whether the value is quite or signaling NaN.
* `Boolean IsSignaling(Half)`: Indicates whether the NaN value is signaling.
* `byte GetSignalingNaNPayload(Half)`: [Non IEEE] Returns the payload of a signaling NaN.
* `RadixMode GetRadix(Half)`: Returns the radix mode of Half. Where `RadixMode` represents the radix of the floating point number according to IEEE 754-2019 5.7.2:
  * Two = 0x02
  * Ten = 0x0A
  The enum value returned is equivalent to 0x02.
* `NumericClass GetClass(Half)`: Returns the numeric class of the value. Where `NumericClass` represents the class of the floating point number according to IEEE 754-2019 5.7.2:
  * SignalingNaN
  * QuiteNaN
  * NegativeInfinity
  * NegativeNormal
  * NegativeSubnormal
  * NegativeZero
  * PositiveZero
  * PositiveSubnormal
  * PositiveNormal
  * PositiveInfinity
* `Boolean GetTotalOrder(Half, Half)`: Returns the toral order of x relative to y.
* `Boolean GetTotalOrderMag(Half, Half)`: Returns the toral order of abs(x) relative to abs(y)
* `Boolean IsInfinite(Half, Int32)`: [Non IEEE] Indicates whether the 16bit floating point number is infinite.
  If sign is negative, true if and only if the value is negative infinity.
  If sign is positive, true if and only if the value is positive infinity.
  If sign is 0, true if and only ifthe value is posivie or negative infinity.
* `UInt16 GetBits()`: [Non IEEE] Represents the storage bits of the Half.

## Comparison operations

* `Boolean Equals(Half|Single|Double)`: Indicates whether the current object is equal to another object.
* `Boolean EpsilonEquals(Half|Single|Double)`: Indicates whether the current object is equal, or widthin margin of error to another object.
* `Int32 CompareTo(Half|Single|Double)`: Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.

* `==`: Inline call to `Equals(Half|Single|Double)`
* `!=`: Inline call to `!Equals(Half|Single|Double)`
* `<`: Inline call to `CompareTo(Half|Single|Double) < 0`
* `>`: Inline call to `CompareTo(Half|Single|Double) > 0`
* `<=`: Inline call to `CompareTo(Half|Single|Double) <= 0`
* `>=`: Inline call to `CompareTo(Half|Single|Double) >= 0`

## Cast operators

* `implicit operator Single(Half)`: Casts the 16bit to a 32bit IEEE 754 binary floating point number.
* `explicit operator Half(Single)`: Casts the 32bit to a 16bit IEEE 754 binary floating point number.
* `implicit operator Double(Half)`: Casts the 16bit to a 64it IEEE 754 binary floating point number.
* `explicit operator Half(Double)`: Casts the 64bit to a 16bit IEEE 754 binary floating point number.

## Arithmetic operators

Following arithmetic operations on Half cast the Half to Single, then execute the same operation on two 32bit binary floating point values and yield such.

* `+`
* `-`
* `*`
* `/`
* `^`: Inline call to `MathF.Pow(Single, Single)`
* `%`

## Constants

* NegInfinity: -∞
* Infinity: ∞
* NaN: Quite NaN with empty payload
* SignalingNaN: Signaling NaN with empty payload
* Minimum: -6.5504E-4
* Maximum: 6.5504E+4
* SmallestPosSubnormal: 5.9604645E-8
* LargestSubnormal: 6.097552E-5
* LargestLessThenOne: 9.9951172E-1
* SmallestLargerThenOne: 1.00097656
* SmallestPosNormal: 6.1035156E-5
* NegZero: -0
* Zero: 0
* One: 1
* NegOne: -1
* Ten: 1E+1
* OneHalf: 5E-1
* Thrid: 3.33333333E-1
* Epsilon: 9.7656E-4
* E: 2.71828
* PI: π ~= 3.14159
* ReciPi: 1/π
* ReciSqrt2: 1/√2
* Frac2_Pi: 2/π
* Frac2_SqrtPi: 2/√π
* FracPi_2: π/2
* FracPi_3: π/3
* Ln10: ln(10)
* Ln2: ln(2)
* Log10_E: log10(e)
* Log10_2: log10(2)
* Log2_E: log2(e)
* Log2_10: log2(10)
* Sqrt2: √2
