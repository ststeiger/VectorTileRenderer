namespace ClipperLib
{
	public struct IntPoint
	{
		public long X;

		public long Y;

		public IntPoint(long X, long Y)
		{
			this.X = X;
			this.Y = Y;
		}

		public IntPoint(IntPoint pt)
		{
			X = pt.X;
			Y = pt.Y;
		}
	}
}
