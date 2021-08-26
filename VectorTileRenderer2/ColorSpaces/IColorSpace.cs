using ColorMine.ColorSpaces.Comparisons;

namespace ColorMine.ColorSpaces
{
	public interface IColorSpace
	{
		double[] Ordinals { get; set; }

		void Initialize(IRgb color);

		IRgb ToRgb();

		T To<T>() where T : IColorSpace, new();

		double Compare(IColorSpace compareToValue, IColorSpaceComparison comparer);
	}
}
