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
using System.Linq;

namespace FixedPointy {
	public static partial class FixMath {
		public static readonly Fix PI;
		public static readonly Fix E;
		static Fix _log2_E;
		static Fix _log2_10;
		static Fix _ln2;
		static Fix _log10_2;
		static Fix[] _quarterSine;
		static Fix[] _cordicAngles;
		static Fix[] _cordicGains;

		static FixMath () {
			if (_quarterSineResPower >= Fix.FractionalBits)
				throw new Exception("_quarterSineResPower must be less than Fix.FractionalBits.");
			if (_quarterSineConsts.Length !=  90 * (1 << _quarterSineResPower) + 1)
				throw new Exception("_quarterSineConst.Length must be 90 * 2^(_quarterSineResPower) + 1."); 

			PI = _piConst;
			E = _eConst;
			_log2_E = _log2_EConst;
			_log2_10 = _log2_10Const;
			_ln2 = _ln2Const;
			_log10_2 = _log10_2Const;

			_quarterSine = Array.ConvertAll(_quarterSineConsts, c => (Fix)c);
			_cordicAngles = Array.ConvertAll(_cordicAngleConsts, c => (Fix)c);
			_cordicGains = Array.ConvertAll(_cordicGainConsts, c => (Fix)c);
		}

		public static Fix Abs (Fix value) {
			return value.Raw < 0 ? new Fix(-value.Raw) : value;
		}

		public static Fix Sign (Fix value) {
			if (value < 0)
				return -1;
			else if (value > 0)
				return 1;
			else
				return 0;
		}

		public static Fix Ceiling (Fix value) {
			return new Fix((value.Raw + Fix.FractionMask) & Fix.IntegerMask);
		}

		public static Fix Floor (Fix value) {
			return new Fix(value.Raw & Fix.IntegerMask);
		}

		public static Fix Truncate (Fix value) {
			if (value < 0)
				return new Fix((value.Raw + Fix.FractionRange) & Fix.IntegerMask);
			else
				return new Fix(value.Raw & Fix.IntegerMask);
		}

		public static Fix Round (Fix value) {
			return new Fix((value.Raw + (Fix.FractionRange >> 1)) & ~Fix.FractionMask);
		}

		public static Fix Min (Fix v1, Fix v2) {
			return v1 < v2 ? v1 : v2;
		}

		public static Fix Max (Fix v1, Fix v2) {
			return v1 > v2 ? v1 : v2;
		}

		public static Fix Sqrt (Fix value) {
			if (value.Raw < 0)
				throw new ArgumentOutOfRangeException("value", "Value must be non-negative.");
			if (value.Raw == 0)
				return 0;

			return new Fix((SqrtULong((ulong)value.Raw << (Fix.FractionalBits + 2)) + 1) >> 1);;
		}

		static int SqrtULong (ulong N) {
			ulong x = 1L << ((31 + (Fix.FractionalBits + 2) + 1) / 2);
			while (true) {
				ulong y = (x + N / x) >> 1;
				if (y >= x)
					return (int)x;
				x = y;
			}
		}

		public static Fix Sin (Fix degrees) {
			return CosRaw(degrees.Raw - (90 << Fix.FractionalBits));
		}

		public static Fix Cos (Fix degrees) {
			return CosRaw(degrees.Raw);
		}

		static Fix CosRaw (int raw) {
			raw = raw < 0 ? -raw : raw;
			int t = raw & ((1 << (Fix.FractionalBits - _quarterSineResPower)) - 1);
			raw = (raw >> (Fix.FractionalBits - _quarterSineResPower));

			if (t == 0)
				return CosRawLookup(raw);

			Fix v1 = CosRawLookup(raw);
			Fix v2 = CosRawLookup(raw + 1);

			return new Fix(
				(int)(
					(
						(long)v1.Raw * ((1 << (Fix.FractionalBits - _quarterSineResPower)) - t)
						+ (long)v2.Raw * t
						+ (1 << (Fix.FractionalBits - _quarterSineResPower - 1))
					)
					>> (Fix.FractionalBits - _quarterSineResPower)
				)
			);
		}

		static Fix CosRawLookup (int raw) {
			raw %= 360 * (1 << _quarterSineResPower);

			if (raw < 90 * (1 << _quarterSineResPower)) {
				return _quarterSine[90 * (1 << _quarterSineResPower) - raw];
			} else if (raw < 180 * (1 << _quarterSineResPower)) {
				raw -= 90 * (1 << _quarterSineResPower);
				return -_quarterSine[raw];
			} else if (raw < 270 * (1 << _quarterSineResPower)) {
				raw -= 180 * (1 << _quarterSineResPower);
				return -_quarterSine[90 * (1 << _quarterSineResPower) - raw];
			} else {
				raw -= 270 * (1 << _quarterSineResPower);
				return _quarterSine[raw];
			}
		}

		public static Fix Tan (Fix degrees) {
			return Sin(degrees) / Cos(degrees);
		}

		public static Fix Asin (Fix value) {
			return Atan2(value, Sqrt((1 + value) * (1 - value)));
		}

		public static Fix Acos (Fix value) {
			return Atan2(Sqrt((1 + value) * (1 - value)), value);
		}

		public static Fix Atan (Fix value) {
			return Atan2(value, 1);
		}

		public static Fix Atan2 (Fix y, Fix x) {
			if (x == 0 && y == 0)
				throw new ArgumentOutOfRangeException("y and x cannot both be 0.");

			Fix angle = 0;
			Fix xNew, yNew;

			if (x < 0) {
				if (y < 0) {
					xNew = -y;
					yNew = x;
					angle = -90;
				} else if (y > 0) {
					xNew = y;
					yNew = -x;
					angle = 90;
				} else {
					xNew = x;
					yNew = y;
					angle = 180;
				}
				x = xNew;
				y = yNew;
			}

			for (int i = 0; i < Fix.FractionalBits + 2; i++) {
				if (y > 0) {
					xNew = x + (y >> i);
					yNew = y - (x >> i);
					angle += _cordicAngles[i];
				} else if (y < 0) {
					xNew = x - (y >> i);
					yNew = y + (x >> i);
					angle -= _cordicAngles[i];
				} else
					break;

				x = xNew;
				y = yNew;
			}

			return angle;
		}

		public static Fix Exp (Fix value) {
			return Exp2(value, _log2_E);
		}

		public static Fix Pow (Fix b, Fix exp) {
			if (b == 2)
				return Exp2(exp, Fix.One);
			else if (b == E)
				return Exp(exp);
			else if (b == 10)
				return Exp2(exp, _log2_10);
			else
				return Exp2(exp, Log2(b));
		}

		static Fix Exp2 (Fix value, Fix scale) {
			if (value == 0)
				return 1;

			long v = (long)value.Raw * scale.Raw;
			if (v == (long)Fix.FractionRange << Fix.FractionalBits)
				return 2;
			if (Fix.FractionalBits * 2 > 32) {
				v = (v + (1 << ((Fix.FractionalBits * 2) - 32 - 1))) >> ((Fix.FractionalBits * 2) - 32);
			} else if (Fix.FractionalBits * 2 < 32)
				v <<= 32 - (Fix.FractionalBits * 2);

			int intPow = (int)((v + (1L << (32 - 1))) >> 32);
			Fix intFactor = intPow < 0 ? Fix.One >> -intPow : Fix.One << intPow;

			long x = (
					((v - ((long)intPow << 32)) * _ln2Const.Raw)
					+ (1L << (32 - 1))
				) >> 32;
			if (x == 0)
				return intFactor;

			long fracFactor = x;
			long xp = x;
			for (int i = 2; i < _invFactConsts.Length; i++) {
				if (xp == 0)
					break;
				xp *= x;
				xp += (1L << (32 - 1));
				xp >>= 32;
				long p = xp * _invFactConsts[i].Raw;
				p += (1L << (32 - 1));
				p >>= 32;
				fracFactor += p;
			}

			return new Fix((int)((((long)intFactor.Raw * fracFactor + (1L << (32 - 1))) >> 32) + intFactor.Raw));
		}

		public static Fix Log (Fix value) {
			return Log2(value) * _ln2;
		}

		public static Fix Log (Fix value, Fix b) {
			if (b == 2)
				return Log2(value);
			else if (b == E)
				return Log(value);
			else if (b == 10)
				return Log10(value);
			else
				return Log2(value) / Log2(b);
		}

		public static Fix Log10 (Fix value) {
			return Log2(value) * _log10_2;
		}

		static Fix Log2 (Fix value) {
			if (value <= 0)
				throw new ArgumentOutOfRangeException("value", "Value must be positive.");

			uint x = (uint)value.Raw;
			uint b = 1U << (Fix.FractionalBits - 1);
			uint y = 0;

			while (x < 1U << Fix.FractionalBits) {
				x <<= 1;
				y -= 1U << Fix.FractionalBits;
			}

			while (x >= 2U << Fix.FractionalBits) {
				x >>= 1;
				y += 1U << Fix.FractionalBits;
			}

			ulong z = x;

			for (int i = 0; i < Fix.FractionalBits; i++) {
				z = z * z >> Fix.FractionalBits;
				if (z >= 2U << Fix.FractionalBits) {
					z >>= 1;
					y += b;
				}
				b >>= 1;
			}

			return new Fix((int)y);
		}
	}
}
