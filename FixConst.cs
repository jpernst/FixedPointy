using System;
using System.Text;

namespace FixedPointy {
	public struct FixConst {
		public static explicit operator double (FixConst f) {
			return (double)(f._raw >> 32) + ((uint)(f._raw) / (uint.MaxValue + 1.0));
		}

		public static implicit operator FixConst (double value) {
			if (value < int.MinValue || value >= int.MaxValue + 1L)
				throw new OverflowException();

			var floor = Math.Floor(value);
			return new FixConst(((long)floor << 32) + (long)((value - floor) * (uint.MaxValue + 1.0) + 0.5));
		}

		public static implicit operator Fix (FixConst value) {
			return new Fix((int)((value.Raw + (1 << (32 - Fix.FractionalBits - 1))) >> (32 - Fix.FractionalBits)));
		}

		public static explicit operator int (FixConst value) {
			if (value._raw > 0)
				return (int)(value._raw >> 32);
			else
				return (int)((value._raw + uint.MaxValue) >> 32);
		}

		public static implicit operator FixConst (int value) {
			return new FixConst(value << 32);
		}

		public static bool operator == (FixConst lhs, FixConst rhs) {
			return lhs._raw == rhs._raw;
		}

		public static bool operator != (FixConst lhs, FixConst rhs) {
			return lhs._raw != rhs._raw;
		}

		public static bool operator > (FixConst lhs, FixConst rhs) {
			return lhs._raw > rhs._raw;
		}

		public static bool operator >= (FixConst lhs, FixConst rhs) {
			return lhs._raw >= rhs._raw;
		}

		public static bool operator < (FixConst lhs, FixConst rhs) {
			return lhs._raw < rhs._raw;
		}

		public static bool operator <= (FixConst lhs, FixConst rhs) {
			return lhs._raw <= rhs._raw;
		}

		public static FixConst operator + (FixConst value) {
			return value;
		}

		public static FixConst operator - (FixConst value) {
			return new FixConst(-value._raw);
		}

		long _raw;

		public FixConst (long raw) {
			_raw = raw;
		}

		public long Raw { get { return _raw; } }

		public override bool Equals (object obj) {
			return (obj is FixConst && ((FixConst)obj) == this);
		}

		public override int GetHashCode () {
			return Raw.GetHashCode();
		}

		public override string ToString () {
			var sb = new StringBuilder();
			if (_raw < 0)
				sb.Append(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NegativeSign);
			long abs = (int)this;
			abs = abs < 0 ? -abs : abs;
			sb.Append(abs.ToString());
			ulong fraction = (ulong)(_raw & uint.MaxValue);
			if (fraction == 0)
				return sb.ToString();

			fraction = _raw < 0 ? (uint.MaxValue + 1L) - fraction : fraction;
			fraction *= 1000000000L;
			fraction += (uint.MaxValue + 1L) >> 1;
			fraction >>= 32;

			sb.Append(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			sb.Append(fraction.ToString("D9").TrimEnd('0'));
			return sb.ToString();
		}
	}
}
