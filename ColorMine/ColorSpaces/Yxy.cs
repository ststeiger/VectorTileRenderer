using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class Yxy : ColorSpace, IYxy, IColorSpace
	{
		public double Y1 { get; set; }

		public double X { get; set; }

		public double Y2 { get; set; }

		public sealed override double[] Ordinals
		{
			get
			{
				return new double[3] { Y1, X, Y2 };
			}
			set
			{
				Y1 = value[0];
				X = value[1];
				Y2 = value[2];
			}
		}

		public Yxy()
		{
		}

		public Yxy(double y1, double x, double y2)
		{
			Y1 = y1;
			X = x;
			Y2 = y2;
		}

		public Yxy(IColorSpace color)
		{
			Ordinals = color.To<Yxy>().Ordinals;
		}

		public Yxy(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			YxyConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "Y1: " + Y1, "X: " + X, "Y2: " + Y2);
		}

		public override IRgb ToRgb()
		{
			return YxyConverter.ToColor(this);
		}
	}
}
