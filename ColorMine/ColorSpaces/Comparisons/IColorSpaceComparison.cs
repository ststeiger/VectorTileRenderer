namespace ColorMine.ColorSpaces.Comparisons
{
	public interface IColorSpaceComparison
	{
		double Compare(IColorSpace a, IColorSpace b);
	}
}
