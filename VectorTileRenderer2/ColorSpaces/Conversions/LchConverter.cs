using System;
using ColorMine.ColorSpaces.Utility;

namespace ColorMine.ColorSpaces.Conversions
{
	internal static class LchConverter
	{
		internal static void ToColorSpace(IRgb color, ILch item)
		{
			Lab lab = color.To<Lab>();
			double num = MathUtils.RadToDeg(Math.Atan2(lab.B, lab.A));
			if (num < 0.0)
			{
				num += 360.0;
			}
			else if (num >= 360.0)
			{
				num -= 360.0;
			}
			item.L = lab.L;
			item.C = Math.Sqrt(lab.A * lab.A + lab.B * lab.B);
			item.H = num;
		}

		internal static IRgb ToColor(ILch item)
		{
			double num = MathUtils.DegToRad(item.H);
			return new Lab
			{
				L = item.L,
				A = Math.Cos(num) * item.C,
				B = Math.Sin(num) * item.C
			}.To<Rgb>();
		}
	}
}
