using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class Hsv : ColorSpace, IHsv, IColorSpace
	{
		public double H { get; set; }

		public double S { get; set; }

		public double V { get; set; }

		public sealed override double[] Ordinals
		{
			get
			{
				return new double[3] { H, S, V };
			}
			set
			{
				H = value[0];
				S = value[1];
				V = value[2];
			}
		}

		public Hsv()
		{
		}

		public Hsv(double h, double s, double v)
		{
			H = h;
			S = s;
			V = v;
		}

		public Hsv(IColorSpace color)
		{
			Ordinals = color.To<Hsv>().Ordinals;
		}

		public Hsv(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			HsvConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "H: " + H, "S: " + S, "V: " + V);
		}

		public override IRgb ToRgb()
		{
			return HsvConverter.ToColor(this);
		}
	}
}
