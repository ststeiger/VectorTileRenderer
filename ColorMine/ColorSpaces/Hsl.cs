using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class Hsl : ColorSpace, IHsl, IColorSpace
	{
		public double H { get; set; }

		public double S { get; set; }

		public double L { get; set; }

		public sealed override double[] Ordinals
		{
			get
			{
				return new double[3] { H, S, L };
			}
			set
			{
				H = value[0];
				S = value[1];
				L = value[2];
			}
		}

		public Hsl()
		{
		}

		public Hsl(double h, double s, double l)
		{
			H = h;
			S = s;
			L = l;
		}

		public Hsl(IColorSpace color)
		{
			Ordinals = color.To<Hsl>().Ordinals;
		}

		public Hsl(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			HslConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "H: " + H, "S: " + S, "L: " + L);
		}

		public override IRgb ToRgb()
		{
			return HslConverter.ToColor(this);
		}
	}
}
