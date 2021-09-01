namespace ColorMine.ColorSpaces
{
	public interface IYxy : IColorSpace
	{
		double Y1 { get; set; }

		double X { get; set; }

		double Y2 { get; set; }
	}
}
