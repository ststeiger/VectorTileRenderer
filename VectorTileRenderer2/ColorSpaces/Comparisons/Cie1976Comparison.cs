using System;

namespace ColorMine.ColorSpaces.Comparisons
{
	public class Cie1976Comparison : IColorSpaceComparison
	{
		public double Compare(IColorSpace colorA, IColorSpace colorB)
		{
			Lab lab = colorA.To<Lab>();
			Lab lab2 = colorB.To<Lab>();
			return Math.Sqrt(Distance(lab.L, lab2.L) + Distance(lab.A, lab2.A) + Distance(lab.B, lab2.B));
		}

		private static double Distance(double a, double b)
		{
			return (a - b) * (a - b);
		}
	}
}
