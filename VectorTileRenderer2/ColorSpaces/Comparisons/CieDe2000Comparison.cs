using System;
using ColorMine.ColorSpaces.Utility;

namespace ColorMine.ColorSpaces.Comparisons
{
	public class CieDe2000Comparison : IColorSpaceComparison
	{
		public double Compare(IColorSpace c1, IColorSpace c2)
		{
			double num = 1.0;
			double num2 = 1.0;
			double num3 = 1.0;
			Lab lab = c1.To<Lab>();
			Lab lab2 = c2.To<Lab>();
			double num4 = Math.Sqrt(lab.A * lab.A + lab.B * lab.B);
			double num5 = Math.Sqrt(lab2.A * lab2.A + lab2.B * lab2.B);
			double num6 = (num4 + num5) / 2.0;
			double num7 = num6 * num6 * num6;
			num7 *= num7 * num6;
			double num8 = 0.5 * (1.0 - Math.Sqrt(num7 / (num7 + 6103515625.0)));
			double num9 = (1.0 + num8) * lab.A;
			double num10 = (1.0 + num8) * lab2.A;
			double num11 = Math.Sqrt(num9 * num9 + lab.B * lab.B);
			double num12 = Math.Sqrt(num10 * num10 + lab2.B * lab2.B);
			double num13 = (MathUtils.RadToDeg(Math.Atan2(lab.B, num9)) + 360.0) % 360.0;
			double num14 = (MathUtils.RadToDeg(Math.Atan2(lab2.B, num10)) + 360.0) % 360.0;
			double num15 = lab2.L - lab.L;
			double num16 = num12 - num11;
			double num17 = Math.Abs(num13 - num14);
			double num18 = ((num11 * num12 == 0.0) ? 0.0 : ((num17 <= 180.0) ? (num14 - num13) : ((!(num17 > 180.0) || !(num14 <= num13)) ? (num14 - num13 - 360.0) : (num14 - num13 + 360.0))));
			double num19 = 2.0 * Math.Sqrt(num11 * num12) * Math.Sin(MathUtils.DegToRad(num18 / 2.0));
			double num20 = (lab.L + lab2.L) / 2.0;
			double num21 = (num11 + num12) / 2.0;
			double num22 = ((num11 * num12 == 0.0) ? 0.0 : ((num17 <= 180.0) ? ((num13 + num14) / 2.0) : ((!(num17 > 180.0) || !(num13 + num14 < 360.0)) ? ((num13 + num14 - 360.0) / 2.0) : ((num13 + num14 + 360.0) / 2.0))));
			double num23 = num20 - 50.0;
			num23 *= num23;
			double num24 = 1.0 + 0.015 * num23 / Math.Sqrt(20.0 + num23);
			double num25 = 1.0 + 0.045 * num21;
			double num26 = 1.0 - 0.17 * Math.Cos(MathUtils.DegToRad(num22 - 30.0)) + 0.24 * Math.Cos(MathUtils.DegToRad(num22 * 2.0)) + 0.32 * Math.Cos(MathUtils.DegToRad(num22 * 3.0 + 6.0)) - 0.2 * Math.Cos(MathUtils.DegToRad(num22 * 4.0 - 63.0));
			double num27 = 1.0 + 0.015 * num26 * num21;
			double num28 = (num22 - 275.0) / 25.0;
			num28 *= num28;
			double num29 = 30.0 * Math.Exp(0.0 - num28);
			double num30 = num21 * num21 * num21;
			num30 *= num30 * num21;
			double num31 = 2.0 * Math.Sqrt(num30 / (num30 + 6103515625.0));
			double num32 = (0.0 - Math.Sin(MathUtils.DegToRad(2.0 * num29))) * num31;
			double num33 = num15 / (num24 * num);
			double num34 = num16 / (num25 * num2);
			double num35 = num19 / (num27 * num3);
			return Math.Sqrt(num33 * num33 + num34 * num34 + num35 * num35 + num32 * num34 * num35);
		}
	}
}
