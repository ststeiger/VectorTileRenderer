namespace ColorMine.ColorSpaces
{
	public interface IHunterLab : IColorSpace
	{
		double L { get; set; }

		double A { get; set; }

		double B { get; set; }
	}
}
