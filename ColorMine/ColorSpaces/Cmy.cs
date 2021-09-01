using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class Cmy : ColorSpace, ICmy, IColorSpace
	{
		public double C { get; set; }

		public double M { get; set; }

		public double Y { get; set; }

		public sealed override double[] Ordinals
		{
			get
			{
				return new double[3] { C, M, Y };
			}
			set
			{
				C = value[0];
				M = value[1];
				Y = value[2];
			}
		}

		public Cmy()
		{
		}

		public Cmy(double c, double m, double y)
		{
			C = c;
			M = m;
			Y = y;
		}

		public Cmy(IColorSpace color)
		{
			Ordinals = color.To<Cmy>().Ordinals;
		}

		public Cmy(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			CmyConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "C: " + C, "M: " + M, "Y: " + Y);
		}

		public override IRgb ToRgb()
		{
			return CmyConverter.ToColor(this);
		}
	}
}
