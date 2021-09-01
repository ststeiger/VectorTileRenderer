using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class Rgb : ColorSpace, IRgb, IColorSpace
	{
		public double R { get; set; }

		public double G { get; set; }

		public double B { get; set; }

		public sealed override double[] Ordinals
		{
			get
			{
				return new double[3] { R, G, B };
			}
			set
			{
				R = value[0];
				G = value[1];
				B = value[2];
			}
		}

		public Rgb()
		{
		}

		public Rgb(double r, double g, double b)
		{
			R = r;
			G = g;
			B = b;
		}

		public Rgb(IColorSpace color)
		{
			Ordinals = color.To<Rgb>().Ordinals;
		}

		public Rgb(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			RgbConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "R: " + R, "G: " + G, "B: " + B);
		}

		public override IRgb ToRgb()
		{
			return RgbConverter.ToColor(this);
		}
	}
}
