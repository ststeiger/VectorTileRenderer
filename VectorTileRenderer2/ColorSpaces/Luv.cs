using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class Luv : ColorSpace, ILuv, IColorSpace
	{
		public double L { get; set; }

		public double U { get; set; }

		public double V { get; set; }

		public sealed override double[] Ordinals
		{
			get
			{
				return new double[3] { L, U, V };
			}
			set
			{
				L = value[0];
				U = value[1];
				V = value[2];
			}
		}

		public Luv()
		{
		}

		public Luv(double l, double u, double v)
		{
			L = l;
			U = u;
			V = v;
		}

		public Luv(IColorSpace color)
		{
			Ordinals = color.To<Luv>().Ordinals;
		}

		public Luv(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			LuvConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "L: " + L, "U: " + U, "V: " + V);
		}

		public override IRgb ToRgb()
		{
			return LuvConverter.ToColor(this);
		}
	}
}
