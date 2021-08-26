using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class Xyz : ColorSpace, IXyz, IColorSpace
	{
		public double X { get; set; }

		public double Y { get; set; }

		public double Z { get; set; }

		public sealed override double[] Ordinals
		{
			get
			{
				return new double[3] { X, Y, Z };
			}
			set
			{
				X = value[0];
				Y = value[1];
				Z = value[2];
			}
		}

		public Xyz()
		{
		}

		public Xyz(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Xyz(IColorSpace color)
		{
			Ordinals = color.To<Xyz>().Ordinals;
		}

		public Xyz(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			XyzConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "X: " + X, "Y: " + Y, "Z: " + Z);
		}

		public override IRgb ToRgb()
		{
			return XyzConverter.ToColor(this);
		}
	}
}
