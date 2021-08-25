using System;
using System.Collections.Generic;

namespace ClipperLib
{
	public class ClipperBase
	{
		protected const double horizontal = -3.4E+38;

		internal const long loRange = 1073741823L;

		internal const long hiRange = 4611686018427387903L;

		internal LocalMinima m_MinimaList;

		internal LocalMinima m_CurrentLM;

		internal List<List<TEdge>> m_edges = new List<List<TEdge>>();

		internal bool m_UseFullRange;

		protected static bool PointsEqual(IntPoint pt1, IntPoint pt2)
		{
			if (pt1.X == pt2.X)
			{
				return pt1.Y == pt2.Y;
			}
			return false;
		}

		internal bool PointIsVertex(IntPoint pt, OutPt pp)
		{
			OutPt outPt = pp;
			do
			{
				if (PointsEqual(outPt.pt, pt))
				{
					return true;
				}
				outPt = outPt.next;
			}
			while (outPt != pp);
			return false;
		}

		internal bool PointInPolygon(IntPoint pt, OutPt pp, bool UseFulllongRange)
		{
			OutPt outPt = pp;
			bool flag = false;
			if (UseFulllongRange)
			{
				do
				{
					if (((outPt.pt.Y <= pt.Y && pt.Y < outPt.prev.pt.Y) || (outPt.prev.pt.Y <= pt.Y && pt.Y < outPt.pt.Y)) && new Int128(pt.X - outPt.pt.X) < Int128.Int128Mul(outPt.prev.pt.X - outPt.pt.X, pt.Y - outPt.pt.Y) / new Int128(outPt.prev.pt.Y - outPt.pt.Y))
					{
						flag = !flag;
					}
					outPt = outPt.next;
				}
				while (outPt != pp);
			}
			else
			{
				do
				{
					if (((outPt.pt.Y <= pt.Y && pt.Y < outPt.prev.pt.Y) || (outPt.prev.pt.Y <= pt.Y && pt.Y < outPt.pt.Y)) && pt.X - outPt.pt.X < (outPt.prev.pt.X - outPt.pt.X) * (pt.Y - outPt.pt.Y) / (outPt.prev.pt.Y - outPt.pt.Y))
					{
						flag = !flag;
					}
					outPt = outPt.next;
				}
				while (outPt != pp);
			}
			return flag;
		}

		internal bool SlopesEqual(TEdge e1, TEdge e2, bool UseFullRange)
		{
			if (UseFullRange)
			{
				return Int128.Int128Mul(e1.deltaY, e2.deltaX) == Int128.Int128Mul(e1.deltaX, e2.deltaY);
			}
			return e1.deltaY * e2.deltaX == e1.deltaX * e2.deltaY;
		}

		protected bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, bool UseFullRange)
		{
			if (UseFullRange)
			{
				return Int128.Int128Mul(pt1.Y - pt2.Y, pt2.X - pt3.X) == Int128.Int128Mul(pt1.X - pt2.X, pt2.Y - pt3.Y);
			}
			return (pt1.Y - pt2.Y) * (pt2.X - pt3.X) - (pt1.X - pt2.X) * (pt2.Y - pt3.Y) == 0;
		}

		protected bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, IntPoint pt4, bool UseFullRange)
		{
			if (UseFullRange)
			{
				return Int128.Int128Mul(pt1.Y - pt2.Y, pt3.X - pt4.X) == Int128.Int128Mul(pt1.X - pt2.X, pt3.Y - pt4.Y);
			}
			return (pt1.Y - pt2.Y) * (pt3.X - pt4.X) - (pt1.X - pt2.X) * (pt3.Y - pt4.Y) == 0;
		}

		internal ClipperBase()
		{
			m_MinimaList = null;
			m_CurrentLM = null;
			m_UseFullRange = false;
		}

		public virtual void Clear()
		{
			DisposeLocalMinimaList();
			for (int i = 0; i < m_edges.Count; i++)
			{
				for (int j = 0; j < m_edges[i].Count; j++)
				{
					m_edges[i][j] = null;
				}
				m_edges[i].Clear();
			}
			m_edges.Clear();
			m_UseFullRange = false;
		}

		private void DisposeLocalMinimaList()
		{
			while (m_MinimaList != null)
			{
				LocalMinima next = m_MinimaList.next;
				m_MinimaList = null;
				m_MinimaList = next;
			}
			m_CurrentLM = null;
		}

		public bool AddPolygons(List<List<IntPoint>> ppg, PolyType polyType)
		{
			bool result = false;
			for (int i = 0; i < ppg.Count; i++)
			{
				if (AddPolygon(ppg[i], polyType))
				{
					result = true;
				}
			}
			return result;
		}

		public bool AddPolygon(List<IntPoint> pg, PolyType polyType)
		{
			int count = pg.Count;
			if (count < 3)
			{
				return false;
			}
			List<IntPoint> list = new List<IntPoint>(count);
			list.Add(new IntPoint(pg[0].X, pg[0].Y));
			int num = 0;
			for (int i = 1; i < count; i++)
			{
				long num2 = ((!m_UseFullRange) ? 1073741823 : 4611686018427387903L);
				if (Math.Abs(pg[i].X) > num2 || Math.Abs(pg[i].Y) > num2)
				{
					if (Math.Abs(pg[i].X) > 4611686018427387903L || Math.Abs(pg[i].Y) > 4611686018427387903L)
					{
						throw new ClipperException("Coordinate exceeds range bounds");
					}
					num2 = 4611686018427387903L;
					m_UseFullRange = true;
				}
				if (PointsEqual(list[num], pg[i]))
				{
					continue;
				}
				if (num > 0 && SlopesEqual(list[num - 1], list[num], pg[i], m_UseFullRange))
				{
					if (PointsEqual(list[num - 1], pg[i]))
					{
						num--;
					}
				}
				else
				{
					num++;
				}
				if (num < list.Count)
				{
					list[num] = pg[i];
				}
				else
				{
					list.Add(new IntPoint(pg[i].X, pg[i].Y));
				}
			}
			if (num < 2)
			{
				return false;
			}
			for (count = num + 1; count > 2; count--)
			{
				if (PointsEqual(list[num], list[0]))
				{
					num--;
				}
				else if (PointsEqual(list[0], list[1]) || SlopesEqual(list[num], list[0], list[1], m_UseFullRange))
				{
					list[0] = list[num--];
				}
				else if (SlopesEqual(list[num - 1], list[num], list[0], m_UseFullRange))
				{
					num--;
				}
				else
				{
					if (!SlopesEqual(list[0], list[1], list[2], m_UseFullRange))
					{
						break;
					}
					for (int j = 2; j <= num; j++)
					{
						list[j - 1] = list[j];
					}
					num--;
				}
			}
			if (count < 3)
			{
				return false;
			}
			List<TEdge> list2 = new List<TEdge>(count);
			for (int k = 0; k < count; k++)
			{
				list2.Add(new TEdge());
			}
			m_edges.Add(list2);
			list2[0].xcurr = list[0].X;
			list2[0].ycurr = list[0].Y;
			InitEdge(list2[count - 1], list2[0], list2[count - 2], list[count - 1], polyType);
			for (int num3 = count - 2; num3 > 0; num3--)
			{
				InitEdge(list2[num3], list2[num3 + 1], list2[num3 - 1], list[num3], polyType);
			}
			InitEdge(list2[0], list2[1], list2[count - 1], list[0], polyType);
			TEdge tEdge = list2[0];
			TEdge tEdge2 = tEdge;
			do
			{
				tEdge.xcurr = tEdge.xbot;
				tEdge.ycurr = tEdge.ybot;
				if (tEdge.ytop < tEdge2.ytop)
				{
					tEdge2 = tEdge;
				}
				tEdge = tEdge.next;
			}
			while (tEdge != list2[0]);
			if (tEdge2.windDelta > 0)
			{
				tEdge2 = tEdge2.next;
			}
			if (tEdge2.dx == -3.4E+38)
			{
				tEdge2 = tEdge2.next;
			}
			tEdge = tEdge2;
			do
			{
				tEdge = AddBoundsToLML(tEdge);
			}
			while (tEdge != tEdge2);
			return true;
		}

		private void InitEdge(TEdge e, TEdge eNext, TEdge ePrev, IntPoint pt, PolyType polyType)
		{
			e.next = eNext;
			e.prev = ePrev;
			e.xcurr = pt.X;
			e.ycurr = pt.Y;
			if (e.ycurr >= e.next.ycurr)
			{
				e.xbot = e.xcurr;
				e.ybot = e.ycurr;
				e.xtop = e.next.xcurr;
				e.ytop = e.next.ycurr;
				e.windDelta = 1;
			}
			else
			{
				e.xtop = e.xcurr;
				e.ytop = e.ycurr;
				e.xbot = e.next.xcurr;
				e.ybot = e.next.ycurr;
				e.windDelta = -1;
			}
			SetDx(e);
			e.polyType = polyType;
			e.outIdx = -1;
		}

		private void SetDx(TEdge e)
		{
			e.deltaX = e.xtop - e.xbot;
			e.deltaY = e.ytop - e.ybot;
			if (e.deltaY == 0)
			{
				e.dx = -3.4E+38;
			}
			else
			{
				e.dx = (double)e.deltaX / (double)e.deltaY;
			}
		}

		private TEdge AddBoundsToLML(TEdge e)
		{
			e.nextInLML = null;
			e = e.next;
			while (true)
			{
				if (e.dx == -3.4E+38)
				{
					if (e.next.ytop < e.ytop && e.next.xbot > e.prev.xbot)
					{
						break;
					}
					if (e.xtop != e.prev.xbot)
					{
						SwapX(e);
					}
					e.nextInLML = e.prev;
				}
				else
				{
					if (e.ycurr == e.prev.ycurr)
					{
						break;
					}
					e.nextInLML = e.prev;
				}
				e = e.next;
			}
			LocalMinima localMinima = new LocalMinima();
			localMinima.next = null;
			localMinima.Y = e.prev.ybot;
			if (e.dx == -3.4E+38)
			{
				if (e.xbot != e.prev.xbot)
				{
					SwapX(e);
				}
				localMinima.leftBound = e.prev;
				localMinima.rightBound = e;
			}
			else if (e.dx < e.prev.dx)
			{
				localMinima.leftBound = e.prev;
				localMinima.rightBound = e;
			}
			else
			{
				localMinima.leftBound = e;
				localMinima.rightBound = e.prev;
			}
			localMinima.leftBound.side = EdgeSide.esLeft;
			localMinima.rightBound.side = EdgeSide.esRight;
			InsertLocalMinima(localMinima);
			while (e.next.ytop != e.ytop || e.next.dx == -3.4E+38)
			{
				e.nextInLML = e.next;
				e = e.next;
				if (e.dx == -3.4E+38 && e.xbot != e.prev.xtop)
				{
					SwapX(e);
				}
			}
			return e.next;
		}

		private void InsertLocalMinima(LocalMinima newLm)
		{
			if (m_MinimaList == null)
			{
				m_MinimaList = newLm;
				return;
			}
			if (newLm.Y >= m_MinimaList.Y)
			{
				newLm.next = m_MinimaList;
				m_MinimaList = newLm;
				return;
			}
			LocalMinima localMinima = m_MinimaList;
			while (localMinima.next != null && newLm.Y < localMinima.next.Y)
			{
				localMinima = localMinima.next;
			}
			newLm.next = localMinima.next;
			localMinima.next = newLm;
		}

		protected void PopLocalMinima()
		{
			if (m_CurrentLM != null)
			{
				m_CurrentLM = m_CurrentLM.next;
			}
		}

		private void SwapX(TEdge e)
		{
			e.xcurr = e.xtop;
			e.xtop = e.xbot;
			e.xbot = e.xcurr;
		}

		protected virtual void Reset()
		{
			m_CurrentLM = m_MinimaList;
			for (LocalMinima localMinima = m_MinimaList; localMinima != null; localMinima = localMinima.next)
			{
				for (TEdge tEdge = localMinima.leftBound; tEdge != null; tEdge = tEdge.nextInLML)
				{
					tEdge.xcurr = tEdge.xbot;
					tEdge.ycurr = tEdge.ybot;
					tEdge.side = EdgeSide.esLeft;
					tEdge.outIdx = -1;
				}
				for (TEdge tEdge = localMinima.rightBound; tEdge != null; tEdge = tEdge.nextInLML)
				{
					tEdge.xcurr = tEdge.xbot;
					tEdge.ycurr = tEdge.ybot;
					tEdge.side = EdgeSide.esRight;
					tEdge.outIdx = -1;
				}
			}
		}

		public IntRect GetBounds()
		{
			IntRect result = default(IntRect);
			LocalMinima localMinima = m_MinimaList;
			if (localMinima == null)
			{
				return result;
			}
			result.left = localMinima.leftBound.xbot;
			result.top = localMinima.leftBound.ybot;
			result.right = localMinima.leftBound.xbot;
			result.bottom = localMinima.leftBound.ybot;
			while (localMinima != null)
			{
				if (localMinima.leftBound.ybot > result.bottom)
				{
					result.bottom = localMinima.leftBound.ybot;
				}
				TEdge tEdge = localMinima.leftBound;
				while (true)
				{
					TEdge tEdge2 = tEdge;
					while (tEdge.nextInLML != null)
					{
						if (tEdge.xbot < result.left)
						{
							result.left = tEdge.xbot;
						}
						if (tEdge.xbot > result.right)
						{
							result.right = tEdge.xbot;
						}
						tEdge = tEdge.nextInLML;
					}
					if (tEdge.xbot < result.left)
					{
						result.left = tEdge.xbot;
					}
					if (tEdge.xbot > result.right)
					{
						result.right = tEdge.xbot;
					}
					if (tEdge.xtop < result.left)
					{
						result.left = tEdge.xtop;
					}
					if (tEdge.xtop > result.right)
					{
						result.right = tEdge.xtop;
					}
					if (tEdge.ytop < result.top)
					{
						result.top = tEdge.ytop;
					}
					if (tEdge2 != localMinima.leftBound)
					{
						break;
					}
					tEdge = localMinima.rightBound;
				}
				localMinima = localMinima.next;
			}
			return result;
		}
	}
}
