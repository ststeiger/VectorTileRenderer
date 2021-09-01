using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class Lch : ColorSpace, ILch, IColorSpace
	{
		public double L { get; set; }

		public double C { get; set; }

		public double H { get; set; }

		public sealed override double[] Ordinals
		{
			get
			{
				return new double[3] { L, C, H };
			}
			set
			{
				L = value[0];
				C = value[1];
				H = value[2];
			}
		}

		public Lch()
		{
		}

		public Lch(double l, double c, double h)
		{
			L = l;
			C = c;
			H = h;
		}

		public Lch(IColorSpace color)
		{
			Ordinals = color.To<Lch>().Ordinals;
		}

		public Lch(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			LchConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "L: " + L, "C: " + C, "H: " + H);
		}

		public override IRgb ToRgb()
		{
			return LchConverter.ToColor(this);
		}
	}
}
