using ColorMine.ColorSpaces.Utility;

namespace ColorMine.ColorSpaces.Conversions
{
	public static class CmykConverter
	{
		public static void ToColorSpace(IRgb color, ICmyk item)
		{
			Cmy cmy = new Cmy();
			cmy.Initialize(color);
			double num = 1.0;
			if (cmy.C < num)
			{
				num = cmy.C;
			}
			if (cmy.M < num)
			{
				num = cmy.M;
			}
			if (cmy.Y < num)
			{
				num = cmy.Y;
			}
			item.K = num;
			if (num.BasicallyEqualTo(1.0))
			{
				item.C = 0.0;
				item.M = 0.0;
				item.Y = 0.0;
			}
			else
			{
				item.C = (cmy.C - num) / (1.0 - num);
				item.M = (cmy.M - num) / (1.0 - num);
				item.Y = (cmy.Y - num) / (1.0 - num);
			}
		}

		public static IRgb ToColor(ICmyk item)
		{
			return new Cmy
			{
				C = item.C * (1.0 - item.K) + item.K,
				M = item.M * (1.0 - item.K) + item.K,
				Y = item.Y * (1.0 - item.K) + item.K
			}.ToRgb();
		}
	}
}
