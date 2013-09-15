/* FixedPointy - A simple fixed-point math library for C#.
 * 
 * Copyright (c) 2013 Jameson Ernst
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Text;

namespace FixedPointy {
	public struct Fix {
		internal const int FRACTIONAL_BITS = 16;

		internal const int INTEGER_BITS = sizeof(int) * 8 - FRACTIONAL_BITS;
		internal const int FRACTION_MASK = (int)(uint.MaxValue >> INTEGER_BITS);
		internal const int INTEGER_MASK = (int)(-1 & ~FRACTION_MASK);
		internal const int FRACTION_RANGE = FRACTION_MASK + 1;
		internal const int MIN_INTEGER = int.MinValue >> FRACTIONAL_BITS;
		internal const int MAX_INTEGER = int.MaxValue >> FRACTIONAL_BITS;

		public static readonly Fix Zero = new Fix(0);
		public static readonly Fix One = new Fix(FRACTION_RANGE);
		public static readonly Fix MinValue = new Fix(int.MinValue);
		public static readonly Fix MaxValue = new Fix(int.MaxValue);
		public static readonly Fix Epsilon = new Fix(1);

		static Fix () {
			if (FRACTIONAL_BITS < 8)
				throw new Exception("Fix must have at least 8 fractional bits.");
			if (INTEGER_BITS < 10)
				throw new Exception("Fix must have at least 10 integer bits.");
			if (FRACTIONAL_BITS % 2 == 1)
				throw new Exception("Fix must have an even number of fractional and integer bits.");
		}

		public static int FractionalBits { get { return FRACTIONAL_BITS; } }
		public static int IntegerBits { get { return INTEGER_BITS; } }
		public static int FractionMask { get { return FRACTION_MASK; } }
		public static int IntegerMask { get { return INTEGER_MASK; } }
		public static int FractionRange { get { return FRACTION_RANGE; } }
		public static int MinInteger { get { return MIN_INTEGER; } }
		public static int MaxInteger { get { return MAX_INTEGER; } }

		public static Fix Mix (int integer, int numerator, int denominator) {
			if (numerator < 0 || denominator < 0)
				throw new ArgumentException("Ratio must be positive.");

			int fraction = ((int)((long)FRACTION_RANGE * numerator / denominator) & FRACTION_MASK);
			fraction = integer < 0 ? -fraction : fraction;

			return new Fix((integer << FRACTIONAL_BITS) + fraction);
		}

		public static Fix Ratio (int numerator, int denominator) {
			return new Fix((int)((((long)numerator << (FRACTIONAL_BITS + 1)) / (long)denominator + 1) >> 1));
		}

		public static explicit operator double (Fix value) {
			return (double)(value._raw >> FRACTIONAL_BITS) + (value._raw & FRACTION_MASK) / (double)FRACTION_RANGE;
		}

		public static explicit operator float (Fix value) {
			return (float)(double)value;
		}

		public static explicit operator int (Fix value) {
			if (value._raw > 0)
				return value._raw >> FRACTIONAL_BITS;
			else
				return (value._raw + FRACTION_MASK) >> FRACTIONAL_BITS;
		}

		public static implicit operator Fix (int value) {
			return new Fix(value << FRACTIONAL_BITS);
		}

		public static bool operator == (Fix lhs, Fix rhs) {
			return lhs._raw == rhs._raw;
		}

		public static bool operator != (Fix lhs, Fix rhs) {
			return lhs._raw != rhs._raw;
		}

		public static bool operator > (Fix lhs, Fix rhs) {
			return lhs._raw > rhs._raw;
		}

		public static bool operator >= (Fix lhs, Fix rhs) {
			return lhs._raw >= rhs._raw;
		}

		public static bool operator < (Fix lhs, Fix rhs) {
			return lhs._raw < rhs._raw;
		}

		public static bool operator <= (Fix lhs, Fix rhs) {
			return lhs._raw <= rhs._raw;
		}

		public static Fix operator + (Fix value) {
			return value;
		}

		public static Fix operator - (Fix value) {
			return new Fix(-value._raw);
		}

		public static Fix operator + (Fix lhs, Fix rhs) {
			return new Fix(lhs._raw + rhs._raw);
		}

		public static Fix operator - (Fix lhs, Fix rhs) {
			return new Fix(lhs._raw - rhs._raw);
		}

		public static Fix operator * (Fix lhs, Fix rhs) {
			return new Fix((int)(((long)lhs._raw * (long)rhs._raw + (FRACTION_RANGE >> 1)) >> FRACTIONAL_BITS));
		}

		public static Fix operator / (Fix lhs, Fix rhs) {
			return new Fix((int)((((long)lhs._raw << (FRACTIONAL_BITS + 1)) / (long)rhs._raw + 1) >> 1));
		}

		public static Fix operator % (Fix lhs, Fix rhs) {
			return new Fix(lhs.Raw % rhs.Raw);
		}

		public static Fix operator << (Fix lhs, int rhs) {
			return new Fix(lhs.Raw << rhs);
		}

		public static Fix operator >> (Fix lhs, int rhs) {
			return new Fix(lhs.Raw >> rhs);
		}

		int _raw;

		public Fix (int raw) {
			_raw = raw;
		}

		public int Raw { get { return _raw; } }

		public override bool Equals (object obj) {
			return (obj is Fix && ((Fix)obj) == this);
		}

		public override int GetHashCode () {
			return Raw.GetHashCode();
		}

		public override string ToString () {
			var sb = new StringBuilder();
			if (_raw < 0)
				sb.Append(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NegativeSign);
			int abs = (int)this;
			abs = abs < 0 ? -abs : abs;
			sb.Append(abs.ToString());
			ulong fraction = (ulong)(_raw & FRACTION_MASK);
			if (fraction == 0)
				return sb.ToString();

			fraction = _raw < 0 ? FRACTION_RANGE - fraction : fraction;
			fraction *= 1000000L;
			fraction += FRACTION_RANGE >> 1;
			fraction >>= FRACTIONAL_BITS;

			sb.Append(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			sb.Append(fraction.ToString("D6").TrimEnd('0'));
			return sb.ToString();
		}
	}
}
