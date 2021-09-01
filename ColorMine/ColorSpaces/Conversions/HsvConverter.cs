using System;

namespace ColorMine.ColorSpaces.Conversions
{
	internal static class HsvConverter
	{
		public static IHsv ToColorSpace(double R, double G, double B)
		{
			double num = Math.Min(R, Math.Min(G, B));
			double num2 = Math.Max(R, Math.Max(G, B));
			double num3 = num2 - num;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			num6 = num2;
			if (num3 == 0.0)
			{
				num4 = 0.0;
				num5 = 0.0;
			}
			else
			{
				num5 = num3 / num2;
				if (R == num2)
				{
					num4 = (G - B) / num3;
					if (G < B)
					{
						num4 += 6.0;
					}
				}
				else if (G == num2)
				{
					num4 = 2.0 + (B - R) / num3;
				}
				else if (B == num2)
				{
					num4 = 4.0 + (R - G) / num3;
				}
				num4 *= 60.0;
			}
			return new Hsv(num4, num5, num6);
		}

		internal static void ToColorSpace(IRgb color, IHsv item)
		{
			IHsv hsv = ToColorSpace(color.R / 255.0, color.G / 255.0, color.B / 255.0);
			item.H = hsv.H;
			item.S = hsv.S;
			item.V = hsv.V;
		}

		internal static IRgb ToColor(IHsv item)
		{
			int num = Convert.ToInt32(Math.Floor(item.H / 60.0)) % 6;
			double num2 = item.H / 60.0 - Math.Floor(item.H / 60.0);
			double num3 = item.V * 255.0;
			double num4 = num3 * (1.0 - item.S);
			double num5 = num3 * (1.0 - num2 * item.S);
			double num6 = num3 * (1.0 - (1.0 - num2) * item.S);
			switch (num)
			{
			case 0:
				return NewRgb(num3, num6, num4);
			case 1:
				return NewRgb(num5, num3, num4);
			case 2:
				return NewRgb(num4, num3, num6);
			case 3:
				return NewRgb(num4, num5, num3);
			case 4:
				return NewRgb(num6, num4, num3);
			default:
				return NewRgb(num3, num4, num5);
			}
		}

		private static IRgb NewRgb(double r, double g, double b)
		{
			return new Rgb
			{
				R = r,
				G = g,
				B = b
			};
		}
	}
}
