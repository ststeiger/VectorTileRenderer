namespace ClipperLib
{
	internal class TEdge
	{
		public long xbot;

		public long ybot;

		public long xcurr;

		public long ycurr;

		public long xtop;

		public long ytop;

		public double dx;

		public long deltaX;

		public long deltaY;

		public long tmpX;

		public PolyType polyType;

		public EdgeSide side;

		public int windDelta;

		public int windCnt;

		public int windCnt2;

		public int outIdx;

		public TEdge next;

		public TEdge prev;

		public TEdge nextInLML;

		public TEdge nextInAEL;

		public TEdge prevInAEL;

		public TEdge nextInSEL;

		public TEdge prevInSEL;
	}
}
