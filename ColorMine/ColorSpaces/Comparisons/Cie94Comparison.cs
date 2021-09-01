using System;

namespace ColorMine.ColorSpaces.Comparisons
{
	public class Cie94Comparison : IColorSpaceComparison
	{
		public enum Application
		{
			GraphicArts,
			Textiles
		}

		internal class ApplicationConstants
		{
			internal double Kl { get; private set; }

			internal double K1 { get; private set; }

			internal double K2 { get; private set; }

			public ApplicationConstants(Application application)
			{
				switch (application)
				{
				case Application.GraphicArts:
					Kl = 1.0;
					K1 = 0.045;
					K2 = 0.015;
					break;
				case Application.Textiles:
					Kl = 2.0;
					K1 = 0.048;
					K2 = 0.014;
					break;
				}
			}
		}

		internal ApplicationConstants Constants { get; private set; }

		public Cie94Comparison()
		{
			Constants = new ApplicationConstants(Application.GraphicArts);
		}

		public Cie94Comparison(Application application)
		{
			Constants = new ApplicationConstants(application);
		}

		public double Compare(IColorSpace a, IColorSpace b)
		{
			Lab lab = a.To<Lab>();
			Lab lab2 = b.To<Lab>();
			double num = lab.L - lab2.L;
			double num2 = lab.A - lab2.A;
			double num3 = lab.B - lab2.B;
			double num4 = Math.Sqrt(lab.A * lab.A + lab.B * lab.B);
			double num5 = Math.Sqrt(lab2.A * lab2.A + lab2.B * lab2.B);
			double num6 = num4 - num5;
			double num7 = num2 * num2 + num3 * num3 - num6 * num6;
			num7 = ((num7 < 0.0) ? 0.0 : Math.Sqrt(num7));
			double num8 = 1.0 + Constants.K1 * num4;
			double num9 = 1.0 + Constants.K2 * num4;
			double num10 = num / (Constants.Kl * 1.0);
			double num11 = num6 / (1.0 * num8);
			double num12 = num7 / (1.0 * num9);
			double num13 = num10 * num10 + num11 * num11 + num12 * num12;
			if (!(num13 < 0.0))
			{
				return Math.Sqrt(num13);
			}
			return 0.0;
		}
	}
}
