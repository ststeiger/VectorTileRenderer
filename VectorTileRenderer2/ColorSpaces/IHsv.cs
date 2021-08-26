namespace ColorMine.ColorSpaces
{
	public interface IHsv : IColorSpace
	{
		double H { get; set; }

		double S { get; set; }

		double V { get; set; }
	}
}
