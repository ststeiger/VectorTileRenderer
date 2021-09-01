using System;
using ColorMine.ColorSpaces.Utility;

namespace ColorMine.ColorSpaces.Comparisons
{
	public class CmcComparison : IColorSpaceComparison
	{
		public const double DefaultLightness = 2.0;

		public const double DefaultChroma = 1.0;

		private readonly double _lightness;

		private readonly double _chroma;

		public CmcComparison()
		{
			_lightness = 2.0;
			_chroma = 1.0;
		}

		public CmcComparison(double lightness = 2.0, double chroma = 1.0)
		{
			_lightness = lightness;
			_chroma = chroma;
		}

		public double Compare(IColorSpace colorA, IColorSpace colorB)
		{
			Lab lab = colorA.To<Lab>();
			Lab lab2 = colorB.To<Lab>();
			double a = lab.L - lab2.L;
			double num = MathUtils.RadToDeg(Math.Atan2(lab.B, lab.A));
			double num2 = Math.Sqrt(lab.A * lab.A + lab.B * lab.B);
			double num3 = Math.Sqrt(lab2.A * lab2.A + lab2.B * lab2.B);
			double num4 = num2 - num3;
			double num5 = (lab.A - lab2.A) * (lab.A - lab2.A) + (lab.B - lab2.B) * (lab.B - lab2.B) - num4 * num4;
			double num6 = num2 * num2;
			double num7 = num6 * num6;
			double num8 = ((164.0 <= num && num <= 345.0) ? (0.56 + Math.Abs(0.2 * Math.Cos(MathUtils.DegToRad(num + 168.0)))) : (0.36 + Math.Abs(0.4 * Math.Cos(MathUtils.DegToRad(num + 35.0)))));
			double num9 = Math.Sqrt(num7 / (num7 + 1900.0));
			double num10 = ((lab.L < 16.0) ? 0.511 : (0.040975 * lab.L / (1.0 + 0.01765 * lab.L)));
			double num11 = 0.0638 * num2 / (1.0 + 0.0131 * num2) + 0.638;
			double num12 = num11 * (num9 * num8 + 1.0 - num9);
			return Math.Sqrt(DistanceDivided(a, _lightness * num10) + DistanceDivided(num4, _chroma * num11) + num5 / (num12 * num12));
		}

		private static double DistanceDivided(double a, double dividend)
		{
			double num = a / dividend;
			return num * num;
		}
	}
}
