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

namespace FixedPointy {
	public struct FixVec2 {
		public static readonly FixVec2 Zero = new FixVec2();
		public static readonly FixVec2 One = new FixVec2(1, 1);
		public static readonly FixVec2 UnitX = new FixVec2(1, 0);
		public static readonly FixVec2 UnitY = new FixVec2(0, 1);

		public static FixVec2 operator + (FixVec2 rhs) {
			return rhs;
		}
		public static FixVec2 operator - (FixVec2 rhs) {
			return new FixVec2(-rhs._x, -rhs._y);
		}

		public static FixVec2 operator + (FixVec2 lhs, FixVec2 rhs) {
			return new FixVec2(lhs._x + rhs._x, lhs._y + rhs._y);
		}
		public static FixVec2 operator - (FixVec2 lhs, FixVec2 rhs) {
			return new FixVec2(lhs._x - rhs._x, lhs._y - rhs._y);
		}

		public static FixVec2 operator + (FixVec2 lhs, Fix rhs) {
			return lhs.ScalarAdd(rhs);
		}
		public static FixVec2 operator + (Fix lhs, FixVec2 rhs) {
			return rhs.ScalarAdd(lhs);
		}
		public static FixVec2 operator - (FixVec2 lhs, Fix rhs) {
			return new FixVec2(lhs._x - rhs, lhs._y - rhs);
		}
		public static FixVec2 operator * (FixVec2 lhs, Fix rhs) {
			return lhs.ScalarMultiply(rhs);
		}
		public static FixVec2 operator * (Fix lhs, FixVec2 rhs) {
			return rhs.ScalarMultiply(lhs);
		}
		public static FixVec2 operator / (FixVec2 lhs, Fix rhs) {
			return new FixVec2(lhs._x / rhs, lhs._y / rhs);
		}

		Fix _x, _y;

		public FixVec2 (Fix x, Fix y) {
			_x = x;
			_y = y;
		}

		public Fix X { get { return _x; } }
		public Fix Y { get { return _y; } }

		public Fix Dot (FixVec2 rhs) {
			return _x * rhs._x + _y * rhs._y;
		}

		public Fix Cross (FixVec2 rhs) {
			return _x * rhs._y - _y * rhs._x;
		}

		FixVec2 ScalarAdd (Fix value) {
			return new FixVec2(_x + value, _y + value);
		}
		FixVec2 ScalarMultiply (Fix value) {
			return new FixVec2(_x * value, _y * value);
		}

		public Fix GetMagnitude () {
			ulong N = (ulong)((long)_x.Raw * (long)_x.Raw + (long)_y.Raw * (long)_y.Raw);

			return new Fix((int)(FixMath.SqrtULong(N << 2) + 1) >> 1);
		}

		public FixVec2 Normalize () {
			if (_x == 0 && _y == 0)
				return FixVec2.Zero;

			var m = GetMagnitude();
			return new FixVec2(_x / m, _y / m);
		}

		public override string ToString () {
			return string.Format("({0}, {1})", _x, _y);
		}
	}
}
