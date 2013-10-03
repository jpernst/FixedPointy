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
	public struct FixTrans2 {
		public static readonly FixTrans2 Identity = new FixTrans2(
			1, 0, 0,
			0, 1, 0
		);

		public static FixTrans2 operator * (FixTrans2 lhs, FixTrans2 rhs) {
			return new FixTrans2(
				lhs._m11 * rhs._m11 + lhs._m12 * rhs._m21,
				lhs._m11 * rhs._m12 + lhs._m12 * rhs._m22,
				lhs._m11 * rhs._m13 + lhs._m12 * rhs._m23 + lhs._m13,
				lhs._m21 * rhs._m11 + lhs._m22 * rhs._m21,
				lhs._m21 * rhs._m12 + lhs._m22 * rhs._m22,
				lhs._m21 * rhs._m13 + lhs._m22 * rhs._m23 + lhs._m23
			);
		}

		public static FixVec2 operator * (FixTrans2 lhs, FixVec2 rhs) {
			return new FixVec2(
				lhs._m11 * rhs.X + lhs._m12 * rhs.Y + lhs._m13,
				lhs._m21 * rhs.X + lhs._m22 * rhs.Y + lhs._m23
			);
		}

		public static FixTrans2 MakeRotation (Fix degrees) {
			Fix cos = FixMath.Cos(degrees);
			Fix sin = FixMath.Sin(degrees);
			return new FixTrans2(
				cos, -sin, 0,
				sin, cos, 0
			);
		}

		public static FixTrans2 MakeScale (FixVec2 scale) {
			return new FixTrans2(
				scale.X, 0, 0,
				0, scale.Y, 0
			);
		}

		public static FixTrans2 MakeTranslation (FixVec2 delta) {
			return new FixTrans2(
				1, 0, delta.X,
				0, 1, delta.Y
			);
		}

		Fix _m11, _m21, _m12, _m22, _m13, _m23;

		public FixTrans2 (
			Fix m11, Fix m12, Fix m13,
			Fix m21, Fix m22, Fix m23
		) {
			_m11 = m11; _m12 = m12; _m13 = m13;
			_m21 = m21; _m22 = m22; _m23 = m23;
		}

		public FixTrans2 (FixVec2 position, FixVec2 scale, Fix rotation) {
			Fix cos = FixMath.Cos(rotation);
			Fix sin = FixMath.Sin(rotation);

			_m11 = cos * scale.X; _m12 = -sin * scale.X; _m13 = position.X;
			_m21 = sin * scale.Y; _m22 = cos * scale.Y; _m23 = position.Y;
		}

		public Fix M11 { get { return _m11; } }
		public Fix M12 { get { return _m12; } }
		public Fix M13 { get { return _m13; } }
		public Fix M21 { get { return _m21; } }
		public Fix M22 { get { return _m22; } }
		public Fix M23 { get { return _m23; } }

		public FixTrans2 Rotate (Fix degrees) {
			return MakeRotation(degrees) * this;
		}

		public FixTrans2 Scale (FixVec2 scale) {
			return new FixTrans2(
				_m11 * scale.X, _m12 * scale.X, _m13 * scale.X,
				_m21 * scale.Y, _m22 * scale.Y, _m23 * scale.Y
			);
		}

		public FixTrans2 Translate (FixVec2 delta) {
			return new FixTrans2(
				_m11, _m12, _m13 + delta.X,
				_m21, _m22, _m23 + delta.Y
			);
		}

		public FixVec2 Apply (FixVec2 vec) {
			return this * vec;
		}

		public override string ToString () {
			return string.Format ("[[{0}, {1}, {2}], [{3}, {4}, {5}]]", _m11, _m12, _m13, _m21, _m22, _m23);
		}
	}
}
