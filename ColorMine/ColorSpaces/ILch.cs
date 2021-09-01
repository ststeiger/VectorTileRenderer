namespace ColorMine.ColorSpaces
{
	public interface ILch : IColorSpace
	{
		double L { get; set; }

		double C { get; set; }

		double H { get; set; }
	}
}
