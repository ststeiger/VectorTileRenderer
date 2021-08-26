using ColorMine.ColorSpaces.Comparisons;

namespace ColorMine.ColorSpaces
{
	public abstract class ColorSpace : IColorSpace
	{
		public abstract double[] Ordinals { get; set; }

		public abstract void Initialize(IRgb color);

		public abstract IRgb ToRgb();

		public double Compare(IColorSpace compareToValue, IColorSpaceComparison comparer)
		{
			return comparer.Compare(this, compareToValue);
		}

		public T To<T>() where T : IColorSpace, new()
		{
			if (typeof(T) == GetType())
			{
				return (T)MemberwiseClone();
			}
			T result = new T();
			result.Initialize(ToRgb());
			return result;
		}
	}
}
