using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class HunterLab : ColorSpace, IHunterLab, IColorSpace
	{
		public double L { get; set; }

		public double A { get; set; }

		public double B { get; set; }

		public sealed override double[] Ordinals
		{
			get
			{
				return new double[3] { L, A, B };
			}
			set
			{
				L = value[0];
				A = value[1];
				B = value[2];
			}
		}

		public HunterLab()
		{
		}

		public HunterLab(double l, double a, double b)
		{
			L = l;
			A = a;
			B = b;
		}

		public HunterLab(IColorSpace color)
		{
			Ordinals = color.To<HunterLab>().Ordinals;
		}

		public HunterLab(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			HunterLabConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "L: " + L, "A: " + A, "B: " + B);
		}

		public override IRgb ToRgb()
		{
			return HunterLabConverter.ToColor(this);
		}
	}
}
