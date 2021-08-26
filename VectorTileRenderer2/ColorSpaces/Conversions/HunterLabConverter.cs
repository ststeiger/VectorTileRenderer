using System;

namespace ColorMine.ColorSpaces.Conversions
{
	internal static class HunterLabConverter
	{
		internal static void ToColorSpace(IRgb color, IHunterLab item)
		{
			Xyz xyz = color.To<Xyz>();
			item.L = 10.0 * Math.Sqrt(xyz.Y);
			item.A = ((xyz.Y != 0.0) ? (17.5 * ((1.02 * xyz.X - xyz.Y) / Math.Sqrt(xyz.Y))) : 0.0);
			item.B = ((xyz.Y != 0.0) ? (7.0 * ((xyz.Y - 0.847 * xyz.Z) / Math.Sqrt(xyz.Y))) : 0.0);
		}

		internal static IRgb ToColor(IHunterLab item)
		{
			double num = item.A / 17.5 * (item.L / 10.0);
			double num2 = item.L / 10.0;
			double num3 = num2 * num2;
			double num4 = item.B / 7.0 * item.L / 10.0;
			return new Xyz
			{
				X = (num + num3) / 1.02,
				Y = num3,
				Z = (0.0 - (num4 - num3)) / 0.847
			}.To<Rgb>();
		}
	}
}
