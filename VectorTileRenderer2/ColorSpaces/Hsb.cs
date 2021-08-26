using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class Hsb : ColorSpace, IHsb, IColorSpace
	{
		public double H { get; set; }

		public double S { get; set; }

		public double B { get; set; }

		public sealed override double[] Ordinals
		{
			get
			{
				return new double[3] { H, S, B };
			}
			set
			{
				H = value[0];
				S = value[1];
				B = value[2];
			}
		}

		public Hsb()
		{
		}

		public Hsb(double h, double s, double b)
		{
			H = h;
			S = s;
			B = b;
		}

		public Hsb(IColorSpace color)
		{
			Ordinals = color.To<Hsb>().Ordinals;
		}

		public Hsb(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			HsbConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "H: " + H, "S: " + S, "B: " + B);
		}

		public override IRgb ToRgb()
		{
			return HsbConverter.ToColor(this);
		}
	}
}
