namespace ColorMine.ColorSpaces.Conversions
{
	internal static class HsbConverter
	{
		internal static void ToColorSpace(IRgb color, IHsb item)
		{
			Hsv hsv = new Hsv();
			HsvConverter.ToColorSpace(color, hsv);
			item.H = hsv.H;
			item.S = hsv.S;
			item.B = hsv.V;
		}

		internal static IRgb ToColor(IHsb item)
		{
			return HsvConverter.ToColor(new Hsv
			{
				H = item.H,
				S = item.S,
				V = item.B
			});
		}
	}
}
