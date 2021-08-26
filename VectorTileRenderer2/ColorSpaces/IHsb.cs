namespace ColorMine.ColorSpaces
{
	public interface IHsb : IColorSpace
	{
		double H { get; set; }

		double S { get; set; }

		double B { get; set; }
	}
}
