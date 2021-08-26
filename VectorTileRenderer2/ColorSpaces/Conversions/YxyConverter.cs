using ColorMine.ColorSpaces.Utility;

namespace ColorMine.ColorSpaces.Conversions
{
	internal static class YxyConverter
	{
		internal static void ToColorSpace(IRgb color, IYxy item)
		{
			Xyz xyz = new Xyz();
			xyz.Initialize(color);
			item.Y1 = xyz.Y;
			double num = xyz.X + xyz.Y + xyz.Z;
			item.X = (num.BasicallyEqualTo(0.0) ? 0.0 : (xyz.X / num));
			double a = xyz.X + xyz.Y + xyz.Z;
			item.Y2 = (a.BasicallyEqualTo(0.0) ? 0.0 : (xyz.Y / (xyz.X + xyz.Y + xyz.Z)));
		}

		internal static IRgb ToColor(IYxy item)
		{
			return new Xyz
			{
				X = item.X * (item.Y1 / item.Y2),
				Y = item.Y1,
				Z = (1.0 - item.X - item.Y2) * (item.Y1 / item.Y2)
			}.ToRgb();
		}
	}
}
