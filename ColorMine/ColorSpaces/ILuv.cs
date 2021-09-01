namespace ColorMine.ColorSpaces
{
	public interface ILuv : IColorSpace
	{
		double L { get; set; }

		double U { get; set; }

		double V { get; set; }
	}
}
