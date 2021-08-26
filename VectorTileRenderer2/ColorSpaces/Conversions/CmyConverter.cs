namespace ColorMine.ColorSpaces.Conversions
{
	internal static class CmyConverter
	{
		internal static void ToColorSpace(IRgb color, ICmy item)
		{
			item.C = 1.0 - color.R / 255.0;
			item.M = 1.0 - color.G / 255.0;
			item.Y = 1.0 - color.B / 255.0;
		}

		internal static IRgb ToColor(ICmy item)
		{
			return new Rgb
			{
				R = (1.0 - item.C) * 255.0,
				G = (1.0 - item.M) * 255.0,
				B = (1.0 - item.Y) * 255.0
			};
		}
	}
}
