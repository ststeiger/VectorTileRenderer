using System;

namespace ColorMine.ColorSpaces.Conversions
{
	internal static class LabConverter
	{
		internal static void ToColorSpace(IRgb color, ILab item)
		{
			Xyz xyz = new Xyz();
			xyz.Initialize(color);
			IXyz whiteReference = XyzConverter.WhiteReference;
			double num = PivotXyz(xyz.X / whiteReference.X);
			double num2 = PivotXyz(xyz.Y / whiteReference.Y);
			double num3 = PivotXyz(xyz.Z / whiteReference.Z);
			item.L = Math.Max(0.0, 116.0 * num2 - 16.0);
			item.A = 500.0 * (num - num2);
			item.B = 200.0 * (num2 - num3);
		}

		internal static IRgb ToColor(ILab item)
		{
			double num = (item.L + 16.0) / 116.0;
			double num2 = item.A / 500.0 + num;
			double num3 = num - item.B / 200.0;
			IXyz whiteReference = XyzConverter.WhiteReference;
			double num4 = num2 * num2 * num2;
			double num5 = num3 * num3 * num3;
			return new Xyz
			{
				X = whiteReference.X * ((num4 > 0.008856) ? num4 : ((num2 - 0.13793103448275862) / 7.787)),
				Y = whiteReference.Y * ((item.L > 7.9996247999999985) ? Math.Pow((item.L + 16.0) / 116.0, 3.0) : (item.L / 903.3)),
				Z = whiteReference.Z * ((num5 > 0.008856) ? num5 : ((num3 - 0.13793103448275862) / 7.787))
			}.ToRgb();
		}

		private static double PivotXyz(double n)
		{
			if (!(n > 0.008856))
			{
				return (903.3 * n + 16.0) / 116.0;
			}
			return CubicRoot(n);
		}

		private static double CubicRoot(double n)
		{
			return Math.Pow(n, 0.33333333333333331);
		}
	}
}
