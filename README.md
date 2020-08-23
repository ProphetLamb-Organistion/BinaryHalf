# BinaryHalf
A paritally IEEE 754 conform 16bit binary floating point implementation written in C#.

## IEEE 754-2019 conformaty
All implemented methods are IEEE 754-2019 conform, but only attributes (4.1, 4.2) comparison (5.6, 5.11), and equality comparison (5.11) operation are implemented. For all other operations and functionally please refer the the `System.MathF` libary. The main reason for not implementing own arithmetic is that, the implementation for the Single (IEEE 754 32bit binary floating point) datatype are supported by intrinsic methods, and thus much faster then any software implementation. Taking into account, that casting to Single is backed by branchless unmanged LUT (which results in very quick conversions), there is no reason to write software code that will still perform slower even thought only half of the data compared to Single is processed.

Furthermore this valuetype does not implement any signaling comparison operations or others, i.e. no operation on sNaN or qNaN will result in a `InvalidOperationExcepttion` to be thrown, instead the NaN value with payload is preserved or silenced.

## Attributes
* `Boolean IsSigned(Half)`: Indicates whether the sign bit is set and value is negative.
* `Int32 GetSign(Half)`: Return a 32-bit signed integer indicating the sign of the Half.
  1 if the value is positive, +0.0 or positve infinity;
  -1 if the value is negative, -0.0 or negative infinity;
  otherise, 0.
* `Boolean IsSubnormal(Half)`: Indicates whether the value is subnormal: the exponent is eiter 0x00 or 0x1F.
* `Boolean IsNormal(Half)`: Indicates whether the value is normal (not zero, subnormal, infinite, or NaN).
* `Boolean IsFinite(Half)`: Indicates whether the value is zero, subnormal or normal (not infinite or NaN).
* `Boolean IsZero(Half)`: Indicates whether the value is negative or positve zero.
* `Boolean IsInfinity(Half)`: Indicates whether the value is positive or negative infinity.
* `Boolean IsPositiveInfinity(Half)`: Indicates whether the value is positive infinity.
* `Boolean IsNegativeInfinity(Half)`: Indicates whether the value is negative infinity.
* `Boolean IsNan(Half)`: Indicates whether the value is quite or signaling NaN.
* `Boolean IsSignaling(Half)`: Indicates whether the NaN value is signaling.
* `byte GetSignalingNaNPayload(Half)`: Returns the payload of a signaling NaN.
* `RadixMode GetRadix(Half)`: Returns the radix mode of Half. The enum value returned is equivalent to 0x02.
* `NumericClass GetClass(Half)`: Returns the numeric class of the value.
* `Boolean GetTotalOrder(Half, Half)`: Returns the toral order of x relative to y.
* `Boolean GetTotalOrderMag(Half, Half)`: Returns the toral order of abs(x) relative to abs(y).
* `Boolean IsInfinite(Half, Int32)`: Indicates whether the 16bit floating point number is infinite.
  If sign is negative, true if and only if the value is negative infinity.
  If sign is positive, true if and only if the value is positive infinity.
  If sign is 0, true if and only ifthe value is posivie or negative infinity.
* `UInt16 GetBits()`: Represents the storage bits of the Half.
* `Int32 GetHashCode`: Returns the hash code for this instance.
* `String ToString(String, IFormatProvider)`: Formats the value of the current instance using the specified format.

## Comparisons
* `Boolean Equals(Half|Single|Double)`: Indicates whether the current object is equal to another object.
* `Boolean EpsilonEquals(Half|Single|Double)`: Indicates whether the current object is equal, or widthin margin of error to another object.
* `Int32 CompareTo(Half|Single|Double)`: Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.

## Operators
### Casts
* `implicit operator Single(Half)`: Casts the 16bit to a 32bit IEEE 754 binary floating point number.
* `explicit operator Half(Single)`: Casts the 32bit to a 16bit IEEE 754 binary floating point number.
### Comparisons
* `==`
* `!=`
* `<`
* `>`
* `<=`
* `>=`
### Arithmetic
Following arithmetic operations on Half cast the Half to Single, then execute the same operation on two 32bit binary floating point values and yield such.
* `+`
* `-`
* `*`
* `/`
* `^`: Calls `MathF.Pow(Single, Single)`
* `%`
