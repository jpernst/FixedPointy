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
	public struct FixTrans3 {
		public static readonly FixTrans3 Identity = new FixTrans3(
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0
		);

		public static FixTrans3 operator * (FixTrans3 lhs, FixTrans3 rhs) {
			return new FixTrans3(
				lhs._m11 * rhs._m11 + lhs._m12 * rhs._m21 + lhs._m13 * rhs._m31,
				lhs._m11 * rhs._m12 + lhs._m12 * rhs._m22 + lhs._m13 * rhs._m32,
				lhs._m11 * rhs._m13 + lhs._m12 * rhs._m23 + lhs._m13 * rhs._m33,
				lhs._m11 * rhs._m14 + lhs._m12 * rhs._m24 + lhs._m13 * rhs._m34 + lhs._m14,
				lhs._m21 * rhs._m11 + lhs._m22 * rhs._m21 + lhs._m23 * rhs._m31,
				lhs._m21 * rhs._m12 + lhs._m22 * rhs._m22 + lhs._m23 * rhs._m32,
				lhs._m21 * rhs._m13 + lhs._m22 * rhs._m23 + lhs._m23 * rhs._m33,
				lhs._m21 * rhs._m14 + lhs._m22 * rhs._m24 + lhs._m23 * rhs._m34 + lhs._m24,
				lhs._m31 * rhs._m11 + lhs._m32 * rhs._m21 + lhs._m33 * rhs._m31,
				lhs._m31 * rhs._m12 + lhs._m32 * rhs._m22 + lhs._m33 * rhs._m32,
				lhs._m31 * rhs._m13 + lhs._m32 * rhs._m23 + lhs._m33 * rhs._m33,
				lhs._m31 * rhs._m14 + lhs._m32 * rhs._m24 + lhs._m33 * rhs._m34 + lhs._m34
			);
		}

		public static FixVec3 operator * (FixTrans3 lhs, FixVec3 rhs) {
			return new FixVec3(
				lhs._m11 * rhs.X + lhs._m12 * rhs.Y + lhs._m13 * rhs.Z + lhs._m14,
				lhs._m21 * rhs.X + lhs._m22 * rhs.Y + lhs._m23 * rhs.Z + lhs._m24,
				lhs._m31 * rhs.X + lhs._m32 * rhs.Y + lhs._m33 * rhs.Z + lhs._m34
			);
		}

		public static FixTrans3 MakeRotationZ (Fix degrees) {
			Fix cos = FixMath.Cos(degrees);
			Fix sin = FixMath.Sin(degrees);
			return new FixTrans3(
				cos, -sin, 0, 0,
				sin, cos, 0, 0,
				0, 0, 1, 0
			);
		}

		public static FixTrans3 MakeRotationY (Fix degrees) {
			Fix cos = FixMath.Cos(degrees);
			Fix sin = FixMath.Sin(degrees);
			return new FixTrans3(
				cos, 0, sin, 0,
				0, 1, 0, 0,
				-sin, 0, cos, 0
			);
		}

		public static FixTrans3 MakeRotationX (Fix degrees) {
			Fix cos = FixMath.Cos(degrees);
			Fix sin = FixMath.Sin(degrees);
			return new FixTrans3(
				1, 0, 0, 0,
				0, cos, -sin, 0,
				0, sin, cos, 0
			);
		}

		public static FixTrans3 MakeRotation (FixVec3 degrees) {
			return MakeRotationX(degrees.X)
				.RotateY(degrees.Y)
				.RotateZ(degrees.Z);
		}

		public static FixTrans3 MakeScale (FixVec3 scale) {
			return new FixTrans3(
				scale.X, 0, 0, 0,
				0, scale.Y, 0, 0,
				0, 0, scale.Z, 0
			);
		}

		public static FixTrans3 MakeTranslation (FixVec3 delta) {
			return new FixTrans3(
				1, 0, 0, delta.X,
				0, 1, 0, delta.Y,
				0, 0, 1, delta.Z
			);
		}

		Fix _m11, _m21, _m31, _m12, _m22, _m32, _m13, _m23, _m33, _m14, _m24, _m34;

		public FixTrans3 (
			Fix m11, Fix m12, Fix m13, Fix m14,
			Fix m21, Fix m22, Fix m23, Fix m24,
			Fix m31, Fix m32, Fix m33, Fix m34
		) {
			_m11 = m11; _m12 = m12; _m13 = m13; _m14 = m14;
			_m21 = m11; _m22 = m12; _m23 = m13; _m24 = m14;
			_m31 = m11; _m32 = m12; _m33 = m13; _m34 = m14;
		}

		public FixTrans3 (FixVec3 position, FixVec3 scale, FixVec3 rotation) {
			this = MakeRotationX(rotation.X)
				.RotateY(rotation.Y)
				.RotateZ(rotation.Z)
				.Scale(scale)
				.Translate(position);
		}

		public Fix M11 { get { return _m11; } }
		public Fix M12 { get { return _m12; } }
		public Fix M13 { get { return _m13; } }
		public Fix M14 { get { return _m14; } }
		public Fix M21 { get { return _m21; } }
		public Fix M22 { get { return _m22; } }
		public Fix M23 { get { return _m23; } }
		public Fix M24 { get { return _m24; } }
		public Fix M31 { get { return _m31; } }
		public Fix M32 { get { return _m32; } }
		public Fix M33 { get { return _m33; } }
		public Fix M34 { get { return _m34; } }

		public FixTrans3 RotateZ (Fix degrees) {
			return MakeRotationZ(degrees) * this;
		}

		public FixTrans3 RotateY (Fix degrees) {
			return MakeRotationY(degrees) * this;
		}

		public FixTrans3 RotateX (Fix degrees) {
			return MakeRotationX(degrees) * this;
		}

		public FixTrans3 Rotate (FixVec3 degrees) {
			return MakeRotation(degrees);
		}

		public FixTrans3 Scale (FixVec3 scale) {
			return new FixTrans3(
				_m11 * scale.X, _m12 * scale.X, _m13 * scale.X, _m14 * scale.X,
				_m21 * scale.Y, _m22 * scale.Y, _m23 * scale.Y, _m24 * scale.Y,
				_m31 * scale.Z, _m32 * scale.Z, _m33 * scale.Z, _m34 * scale.Z
			);
		}

		public FixTrans3 Translate (FixVec3 delta) {
			return new FixTrans3(
				_m11, _m12, _m13, _m14 + delta.X,
				_m21, _m22, _m23, _m24 + delta.Y,
				_m31, _m32, _m33, _m34 + delta.Z
			);
		}

		public FixVec3 Apply (FixVec3 vec) {
			return this * vec;
		}

		public override string ToString () {
			return string.Format ("[[{0}, {1}, {2}], [{3}, {4}, {5}]]", _m11, _m12, _m13, _m21, _m22, _m23);
		}
	}
}
