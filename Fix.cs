using System;
using System.Text;

namespace FixedPointy {
	public struct Fix {
		public const int FractionalBits = 12;

		public const int IntegerBits = sizeof(int) * 8 - FractionalBits;
		public const int FractionMask = (int)(uint.MaxValue >> IntegerBits);
		public const int IntegerMask = (int)(-1 & ~FractionMask);
		public const int FractionRange = FractionMask + 1;
		public const int MinInteger = int.MinValue >> FractionalBits;
		public const int MaxInteger = int.MaxValue >> FractionalBits;

		public static readonly Fix Zero = new Fix(0);
		public static readonly Fix One = new Fix(FractionRange);
		public static readonly Fix MinValue = new Fix(int.MinValue);
		public static readonly Fix MaxValue = new Fix(int.MaxValue);
		public static readonly Fix Epsilon = new Fix(1);

		static Fix () {
			if (FractionalBits < 8)
				throw new Exception("Fix must have at least 8 fractional bits.");
			if (IntegerBits < 10)
				throw new Exception("Fix must have at least 10 integer bits.");
			if (FractionalBits % 2 == 1)
				throw new Exception("Fix must have an even number of fractional and integer bits.");
		}

		public static Fix Mix (int integer, int numerator, int denominator) {
			if (numerator < 0 || denominator < 0)
				throw new ArgumentException("Ratio must be positive.");

			int fraction = ((int)((long)FractionRange * numerator / denominator) & FractionMask);
			fraction = integer < 0 ? -fraction : fraction;

			return new Fix((integer << FractionalBits) + fraction);
		}

		public static explicit operator double (Fix value) {
			return (double)(value._raw >> FractionalBits) + (value._raw & FractionMask) / (double)FractionRange;
		}

		public static explicit operator float (Fix value) {
			return (float)(double)value;
		}

		public static explicit operator int (Fix value) {
			if (value._raw > 0)
				return value._raw >> FractionalBits;
			else
				return (value._raw + FractionMask) >> FractionalBits;
		}

		public static implicit operator Fix (int value) {
			return new Fix(value << FractionalBits);
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
			return new Fix((int)(((long)lhs._raw * (long)rhs._raw + (FractionRange >> 1)) >> FractionalBits));
		}

		public static Fix operator / (Fix lhs, Fix rhs) {
			return new Fix((int)((((long)lhs._raw << (FractionalBits + 1)) / (long)rhs._raw + 1) >> 1));
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
			ulong fraction = (ulong)(_raw & FractionMask);
			if (fraction == 0)
				return sb.ToString();

			fraction = _raw < 0 ? FractionRange - fraction : fraction;
			fraction *= 1000000L;
			fraction += FractionRange >> 1;
			fraction >>= FractionalBits;

			sb.Append(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			sb.Append(fraction.ToString("D6").TrimEnd('0'));
			return sb.ToString();
		}
	}
}
