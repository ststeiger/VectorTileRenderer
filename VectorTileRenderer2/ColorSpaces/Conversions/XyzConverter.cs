using System;

namespace ColorMine.ColorSpaces.Conversions
{
	internal static class XyzConverter
	{
		internal const double Epsilon = 0.008856;

		internal const double Kappa = 903.3;

		internal static IXyz WhiteReference { get; private set; }

		static XyzConverter()
		{
			WhiteReference = new Xyz
			{
				X = 95.047,
				Y = 100.0,
				Z = 108.883
			};
		}

		internal static double CubicRoot(double n)
		{
			return Math.Pow(n, 0.33333333333333331);
		}

		internal static void ToColorSpace(IRgb color, IXyz item)
		{
			double num = PivotRgb(color.R / 255.0);
			double num2 = PivotRgb(color.G / 255.0);
			double num3 = PivotRgb(color.B / 255.0);
			item.X = num * 0.4124 + num2 * 0.3576 + num3 * 0.1805;
			item.Y = num * 0.2126 + num2 * 0.7152 + num3 * 0.0722;
			item.Z = num * 0.0193 + num2 * 0.1192 + num3 * 0.9505;
		}

		internal static IRgb ToColor(IXyz item)
		{
			double num = item.X / 100.0;
			double num2 = item.Y / 100.0;
			double num3 = item.Z / 100.0;
			double num4 = num * 3.2406 + num2 * -1.5372 + num3 * -0.4986;
			double num5 = num * -0.9689 + num2 * 1.8758 + num3 * 0.0415;
			double num6 = num * 0.0557 + num2 * -0.204 + num3 * 1.057;
			num4 = ((num4 > 0.0031308) ? (1.055 * Math.Pow(num4, 5.0 / 12.0) - 0.055) : (12.92 * num4));
			num5 = ((num5 > 0.0031308) ? (1.055 * Math.Pow(num5, 5.0 / 12.0) - 0.055) : (12.92 * num5));
			num6 = ((num6 > 0.0031308) ? (1.055 * Math.Pow(num6, 5.0 / 12.0) - 0.055) : (12.92 * num6));
			return new Rgb
			{
				R = ToRgb(num4),
				G = ToRgb(num5),
				B = ToRgb(num6)
			};
		}

		private static double ToRgb(double n)
		{
			double num = 255.0 * n;
			if (num < 0.0)
			{
				return 0.0;
			}
			if (num > 255.0)
			{
				return 255.0;
			}
			return num;
		}

		private static double PivotRgb(double n)
		{
			return ((n > 0.04045) ? Math.Pow((n + 0.055) / 1.055, 2.4) : (n / 12.92)) * 100.0;
		}
	}
}
