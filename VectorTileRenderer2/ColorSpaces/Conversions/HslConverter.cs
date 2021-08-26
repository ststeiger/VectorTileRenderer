using System;

namespace ColorMine.ColorSpaces.Conversions
{
	internal static class HslConverter
	{
		public static IHsl ToColorSpace(double R, double G, double B)
		{
			double num = Math.Min(R, Math.Min(G, B));
			double num2 = Math.Max(R, Math.Max(G, B));
			double num3 = num2 - num;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			num6 = (num2 + num) / 2.0;
			if (num3 == 0.0)
			{
				num4 = 0.0;
				num5 = 0.0;
			}
			else
			{
				num5 = ((!(num6 <= 0.5)) ? (num3 / (2.0 - 2.0 * num6)) : (num3 / (2.0 * num6)));
				if (R == num2)
				{
					num4 = (G - B) / num3;
					if (G < B)
					{
						num4 += 6.0;
					}
				}
				else if (B == num2)
				{
					num4 = 4.0 + (R - G) / num3;
				}
				else if (G == num2)
				{
					num4 = 2.0 + (B - R) / num3;
				}
				num4 *= 60.0;
			}
			return new Hsl(num4, num5, num6);
		}

		internal static void ToColorSpace(IRgb color, IHsl item)
		{
			IHsl hsl = ToColorSpace(color.R / 255.0, color.G / 255.0, color.B / 255.0);
			item.H = hsl.H;
			item.S = hsl.S;
			item.L = hsl.L;
			item.S = Math.Round(item.S * 100.0, 3);
			item.L = Math.Round(item.L * 100.0, 3);
		}

		private static IRgb Rotate(double h, double s, ref double l)
		{
			if (h == 0.0)
			{
				h = 1E-05;
			}
			double num = (1.0 - Math.Abs(2.0 * l - 1.0)) * s;
			double num2 = num * (1.0 - Math.Abs(h % 2.0 - 1.0));
			l -= 0.5 * num;
			double obj = Math.Ceiling(h);
			if (!1.0.Equals(obj))
			{
				if (!2.0.Equals(obj))
				{
					if (!3.0.Equals(obj))
					{
						if (!4.0.Equals(obj))
						{
							if (!5.0.Equals(obj))
							{
								if (6.0.Equals(obj))
								{
									return new Rgb(num, 0.0, num2);
								}
								return new Rgb(0.0, 0.0, 0.0);
							}
							return new Rgb(num2, 0.0, num);
						}
						return new Rgb(0.0, num2, num);
					}
					return new Rgb(0.0, num, num2);
				}
				return new Rgb(num2, num, 0.0);
			}
			return new Rgb(num, num2, 0.0);
		}

		internal static IRgb ToColor(IHsl item)
		{
			double h = item.H / 60.0;
			double num = item.S / 100.0;
			double l = item.L / 100.0;
			if (num > 0.0)
			{
				IRgb rgb = Rotate(h, num, ref l);
				return new Rgb
				{
					R = (rgb.R + l) * 255.0,
					G = (rgb.G + l) * 255.0,
					B = (rgb.B + l) * 255.0
				};
			}
			return new Rgb
			{
				R = l * 255.0,
				G = l * 255.0,
				B = l * 255.0
			};
		}

		private static double GetColorComponent(double temp1, double temp2, double temp3)
		{
			temp3 = MoveIntoRange(temp3);
			if (temp3 < 0.16666666666666666)
			{
				return temp1 + (temp2 - temp1) * 6.0 * temp3;
			}
			if (temp3 < 0.5)
			{
				return temp2;
			}
			if (temp3 < 2.0 / 3.0)
			{
				return temp1 + (temp2 - temp1) * (2.0 / 3.0 - temp3) * 6.0;
			}
			return temp1;
		}

		private static double MoveIntoRange(double temp3)
		{
			if (temp3 < 0.0)
			{
				return temp3 + 1.0;
			}
			if (temp3 > 1.0)
			{
				return temp3 - 1.0;
			}
			return temp3;
		}
	}
}
