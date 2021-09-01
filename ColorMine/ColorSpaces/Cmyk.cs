using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class Cmyk : ColorSpace, ICmyk, IColorSpace
	{
		public double C { get; set; }

		public double M { get; set; }

		public double Y { get; set; }

		public double K { get; set; }

		public sealed override double[] Ordinals
		{
			get
			{
				return new double[4] { C, M, Y, K };
			}
			set
			{
				C = value[0];
				M = value[1];
				Y = value[2];
				K = value[3];
			}
		}

		public Cmyk()
		{
		}

		public Cmyk(double c, double m, double y, double k)
		{
			C = c;
			M = m;
			Y = y;
			K = k;
		}

		public Cmyk(IColorSpace color)
		{
			Ordinals = color.To<Cmyk>().Ordinals;
		}

		public Cmyk(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			CmykConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "C: " + C, "M: " + M, "Y: " + Y, "K: " + K);
		}

		public override IRgb ToRgb()
		{
			return CmykConverter.ToColor(this);
		}
	}
}
