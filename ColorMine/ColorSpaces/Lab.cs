using ColorMine.ColorSpaces.Conversions;

namespace ColorMine.ColorSpaces
{
	public class Lab : ColorSpace, ILab, IColorSpace
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

		public Lab()
		{
		}

		public Lab(double l, double a, double b)
		{
			L = l;
			A = a;
			B = b;
		}

		public Lab(IColorSpace color)
		{
			Ordinals = color.To<Lab>().Ordinals;
		}

		public Lab(double[] ordinals)
		{
			Ordinals = ordinals;
		}

		public override void Initialize(IRgb color)
		{
			LabConverter.ToColorSpace(color, this);
		}

		public override string ToString()
		{
			return string.Join(", ", "L: " + L, "A: " + A, "B: " + B);
		}

		public override IRgb ToRgb()
		{
			return LabConverter.ToColor(this);
		}
	}
}
