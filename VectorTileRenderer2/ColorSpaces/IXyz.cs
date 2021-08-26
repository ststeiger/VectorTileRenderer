namespace ColorMine.ColorSpaces
{
	public interface IXyz : IColorSpace
	{
		double X { get; set; }

		double Y { get; set; }

		double Z { get; set; }
	}
}
