using System;
using FixedPointy;

namespace FixedPointy {
	public static class Program {
		public static void Main (string[] args) {
			Console.WriteLine((Fix)(FixConst)Math.Pow(10, -2.1));
			Console.WriteLine(FixMath.Pow(10, Fix.Mix(-2, 10, 100)));
		}
	}
}
