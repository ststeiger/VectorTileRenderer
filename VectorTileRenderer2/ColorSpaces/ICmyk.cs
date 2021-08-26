namespace ColorMine.ColorSpaces
{
	public interface ICmyk : IColorSpace
	{
		double C { get; set; }

		double M { get; set; }

		double Y { get; set; }

		double K { get; set; }
	}
}
