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
	public struct FixVec3 {
		public static readonly FixVec3 Zero = new FixVec3();
		public static readonly FixVec3 One = new FixVec3(1, 1, 1);
		public static readonly FixVec3 UnitX = new FixVec3(1, 0, 0);
		public static readonly FixVec3 UnitY = new FixVec3(0, 1, 0);
		public static readonly FixVec3 UnitZ = new FixVec3(0, 0, 1);

		public static implicit operator FixVec3 (FixVec2 value) {
			return new FixVec3(value.X, value.Y, 0);
		}

		public static FixVec3 operator + (FixVec3 rhs) {
			return rhs;
		}
		public static FixVec3 operator - (FixVec3 rhs) {
			return new FixVec3(-rhs._x, -rhs._y, -rhs._z);
		}

		public static FixVec3 operator + (FixVec3 lhs, FixVec3 rhs) {
			return new FixVec3(lhs._x + rhs._x, lhs._y + rhs._y, lhs._z + rhs._z);
		}
		public static FixVec3 operator - (FixVec3 lhs, FixVec3 rhs) {
			return new FixVec3(lhs._x - rhs._x, lhs._y - rhs._y, lhs._z - rhs._z);
		}

		public static FixVec3 operator + (FixVec3 lhs, Fix rhs) {
			return lhs.ScalarAdd(rhs);
		}
		public static FixVec3 operator + (Fix lhs, FixVec3 rhs) {
			return rhs.ScalarAdd(lhs);
		}
		public static FixVec3 operator - (FixVec3 lhs, Fix rhs) {
			return new FixVec3(lhs._x - rhs, lhs._y - rhs, lhs._z - rhs);
		}
		public static FixVec3 operator * (FixVec3 lhs, Fix rhs) {
			return lhs.ScalarMultiply(rhs);
		}
		public static FixVec3 operator * (Fix lhs, FixVec3 rhs) {
			return rhs.ScalarMultiply(lhs);
		}
		public static FixVec3 operator / (FixVec3 lhs, Fix rhs) {
			return new FixVec3(lhs._x / rhs, lhs._y / rhs, lhs._z / rhs);
		}

		Fix _x, _y, _z;

		public FixVec3 (Fix x, Fix y, Fix z) {
			_x = x;
			_y = y;
			_z = z;
		}

		public Fix X { get { return _x; } }
		public Fix Y { get { return _y; } }
		public Fix Z { get { return _z; } }

		public Fix Dot (FixVec3 rhs) {
			return _x * rhs._x + _y * rhs._y + _z * rhs._z;
		}

		public FixVec3 Cross (FixVec3 rhs) {
			return new FixVec3(
				_y * rhs._z - _z * rhs._y,
				_z * rhs._x - _x * rhs._z,
				_x * rhs._y - _y * rhs._x
			);
		}

		FixVec3 ScalarAdd (Fix value) {
			return new FixVec3(_x + value, _y + value, _z + value);
		}
		FixVec3 ScalarMultiply (Fix value) {
			return new FixVec3(_x * value, _y * value, _z * value);
		}

		public Fix GetMagnitude () {
			ulong N = (ulong)((long)_x.Raw * (long)_x.Raw + (long)_y.Raw * (long)_y.Raw + (long)_z.Raw * (long)_z.Raw);

			return new Fix((int)(FixMath.SqrtULong(N << 2) + 1) >> 1);
		}

		public FixVec3 Normalize () {
			if (_x == 0 && _y == 0 && _z == 0)
				return FixVec3.Zero;

			var m = GetMagnitude();
			return new FixVec3(_x / m, _y / m, _z / m);
		}

		public override string ToString () {
			return string.Format("({0}, {1}, {2})", _x, _y, _z);
		}
	}
}
