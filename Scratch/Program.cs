using System;
using FixedPointy;

namespace FixedPointy {
	public static class Program {
		public static void Main (string[] args) {
			Console.WriteLine((Fix)(FixConst)Math.Pow(2, 13.007));
			Console.WriteLine(FixMath.Pow(2, Fix.Mix(13, 7, 1000)));
		}
	}
}
