namespace ColorMine.ColorSpaces
{
	public interface ILab : IColorSpace
	{
		double L { get; set; }

		double A { get; set; }

		double B { get; set; }
	}
}