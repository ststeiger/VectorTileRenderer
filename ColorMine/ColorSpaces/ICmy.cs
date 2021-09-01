namespace ColorMine.ColorSpaces
{
	public interface ICmy : IColorSpace
	{
		double C { get; set; }

		double M { get; set; }

		double Y { get; set; }
	}
}
