namespace ColorMine.ColorSpaces
{
	public interface IRgb : IColorSpace
	{
		double R { get; set; }

		double G { get; set; }

		double B { get; set; }
	}
}
