namespace ColorMine.ColorSpaces.Conversions
{
	internal static class LuvConverter
	{
		internal static void ToColorSpace(IRgb color, ILuv item)
		{
			Xyz xyz = new Xyz();
			IXyz whiteReference = XyzConverter.WhiteReference;
			xyz.Initialize(color);
			double num = xyz.Y / XyzConverter.WhiteReference.Y;
			item.L = ((num > 0.008856) ? (116.0 * XyzConverter.CubicRoot(num) - 16.0) : (903.3 * num));
			double denominator = GetDenominator(xyz);
			double denominator2 = GetDenominator(whiteReference);
			double num2 = ((denominator == 0.0) ? 0.0 : (4.0 * xyz.X / denominator - 4.0 * whiteReference.X / denominator2));
			double num3 = ((denominator == 0.0) ? 0.0 : (9.0 * xyz.Y / denominator - 9.0 * whiteReference.Y / denominator2));
			item.U = 13.0 * item.L * num2;
			item.V = 13.0 * item.L * num3;
		}

		internal static IRgb ToColor(ILuv item)
		{
			IXyz whiteReference = XyzConverter.WhiteReference;
			double num = 4.0 * whiteReference.X / GetDenominator(whiteReference);
			double num2 = 9.0 * whiteReference.Y / GetDenominator(whiteReference);
			double num3 = 0.33333333333333331 * (52.0 * item.L / (item.U + 13.0 * item.L * num) - 1.0);
			double num4 = (item.L + 16.0) / 116.0;
			double num5 = ((item.L > 7.9996247999999985) ? (num4 * num4 * num4) : (item.L / 903.3));
			double num6 = -5.0 * num5;
			double num7 = (num5 * (39.0 * item.L / (item.V + 13.0 * item.L * num2) - 5.0) - num6) / (num3 - -0.33333333333333331);
			double num8 = num7 * num3 + num6;
			return new Xyz
			{
				X = 100.0 * num7,
				Y = 100.0 * num5,
				Z = 100.0 * num8
			}.ToRgb();
		}

		private static double GetDenominator(IXyz xyz)
		{
			return xyz.X + 15.0 * xyz.Y + 3.0 * xyz.Z;
		}
	}
}
