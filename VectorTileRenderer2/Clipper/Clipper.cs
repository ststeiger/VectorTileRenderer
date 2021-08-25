using System;
using System.Collections.Generic;

namespace ClipperLib
{
	public class Clipper : ClipperBase
	{
		internal class DoublePoint
		{
			public double X { get; set; }

			public double Y { get; set; }

			public DoublePoint(double x = 0.0, double y = 0.0)
			{
				X = x;
				Y = y;
			}
		}

		private class PolyOffsetBuilder
		{
			private const int buffLength = 128;

			private List<List<IntPoint>> pts;

			private List<IntPoint> currentPoly;

			private List<DoublePoint> normals;

			private double delta;

			private double m_R;

			private int m_i;

			private int m_j;

			private int m_k;

			public PolyOffsetBuilder(List<List<IntPoint>> pts, List<List<IntPoint>> solution, double delta, JoinType jointype, double MiterLimit = 2.0, bool AutoFix = true)
			{
				if (delta == 0.0)
				{
					solution = pts;
					return;
				}
				this.pts = pts;
				this.delta = delta;
				if (AutoFix)
				{
					int count = pts.Count;
					int i;
					for (i = 0; i < count && pts[i].Count == 0; i++)
					{
					}
					if (i == count)
					{
						return;
					}
					IntPoint botPt = pts[i][0];
					for (int j = i; j < count; j++)
					{
						if (pts[j].Count == 0)
						{
							continue;
						}
						if (UpdateBotPt(pts[j][0], ref botPt))
						{
							i = j;
						}
						for (int num = pts[j].Count - 1; num > 0; num--)
						{
							if (ClipperBase.PointsEqual(pts[j][num], pts[j][num - 1]))
							{
								pts[j].RemoveAt(num);
							}
							else if (UpdateBotPt(pts[j][num], ref botPt))
							{
								i = j;
							}
						}
					}
					if (!Orientation(pts[i]))
					{
						ReversePolygons(pts);
					}
				}
				if (MiterLimit <= 1.0)
				{
					MiterLimit = 1.0;
				}
				double num2 = 2.0 / (MiterLimit * MiterLimit);
				normals = new List<DoublePoint>();
				solution.Clear();
				solution.Capacity = pts.Count;
				for (m_i = 0; m_i < pts.Count; m_i++)
				{
					int num3 = pts[m_i].Count;
					if (num3 > 1 && pts[m_i][0].X == pts[m_i][num3 - 1].X && pts[m_i][0].Y == pts[m_i][num3 - 1].Y)
					{
						num3--;
					}
					if (num3 != 0 && (num3 >= 3 || !(delta <= 0.0)))
					{
						if (num3 == 1)
						{
							List<IntPoint> item = BuildArc(pts[m_i][num3 - 1], 0.0, Math.PI * 2.0, delta);
							solution.Add(item);
						}
						else
						{
							normals.Clear();
							normals.Capacity = num3;
							for (int k = 0; k < num3 - 1; k++)
							{
								normals.Add(GetUnitNormal(pts[m_i][k], pts[m_i][k + 1]));
							}
							normals.Add(GetUnitNormal(pts[m_i][num3 - 1], pts[m_i][0]));
							currentPoly = new List<IntPoint>();
							m_k = num3 - 1;
							for (m_j = 0; m_j < num3; m_j++)
							{
								switch (jointype)
								{
								case JoinType.jtMiter:
									m_R = 1.0 + (normals[m_j].X * normals[m_k].X + normals[m_j].Y * normals[m_k].Y);
									if (m_R >= num2)
									{
										DoMiter();
									}
									else
									{
										DoSquare(MiterLimit);
									}
									break;
								case JoinType.jtRound:
									DoRound();
									break;
								case JoinType.jtSquare:
									DoSquare(1.0);
									break;
								}
								m_k = m_j;
							}
							solution.Add(currentPoly);
						}
					}
				}
				Clipper clipper = new Clipper();
				clipper.AddPolygons(solution, PolyType.ptSubject);
				if (delta > 0.0)
				{
					clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftPositive);
					return;
				}
				IntRect bounds = clipper.GetBounds();
				clipper.AddPolygon(new List<IntPoint>(4)
				{
					new IntPoint(bounds.left - 10, bounds.bottom + 10),
					new IntPoint(bounds.right + 10, bounds.bottom + 10),
					new IntPoint(bounds.right + 10, bounds.top - 10),
					new IntPoint(bounds.left - 10, bounds.top - 10)
				}, PolyType.ptSubject);
				clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftNegative, PolyFillType.pftNegative);
				if (solution.Count > 0)
				{
					solution.RemoveAt(0);
					for (int l = 0; l < solution.Count; l++)
					{
						solution[l].Reverse();
					}
				}
			}

			internal bool UpdateBotPt(IntPoint pt, ref IntPoint botPt)
			{
				if (pt.Y > botPt.Y || (pt.Y == botPt.Y && pt.X < botPt.X))
				{
					botPt = pt;
					return true;
				}
				return false;
			}

			internal void AddPoint(IntPoint pt)
			{
				int count = currentPoly.Count;
				if (count == currentPoly.Capacity)
				{
					currentPoly.Capacity = count + 128;
				}
				currentPoly.Add(pt);
			}

			internal void DoSquare(double mul)
			{
				IntPoint pt = new IntPoint(Round((double)pts[m_i][m_j].X + normals[m_k].X * delta), Round((double)pts[m_i][m_j].Y + normals[m_k].Y * delta));
				IntPoint pt2 = new IntPoint(Round((double)pts[m_i][m_j].X + normals[m_j].X * delta), Round((double)pts[m_i][m_j].Y + normals[m_j].Y * delta));
				if ((normals[m_k].X * normals[m_j].Y - normals[m_j].X * normals[m_k].Y) * delta >= 0.0)
				{
					double num = Math.Atan2(normals[m_k].Y, normals[m_k].X);
					double num2 = Math.Atan2(0.0 - normals[m_j].Y, 0.0 - normals[m_j].X);
					num = Math.Abs(num2 - num);
					if (num > Math.PI)
					{
						num = Math.PI * 2.0 - num;
					}
					double num3 = Math.Tan((Math.PI - num) / 4.0) * Math.Abs(delta * mul);
					pt = new IntPoint((long)((double)pt.X - normals[m_k].Y * num3), (long)((double)pt.Y + normals[m_k].X * num3));
					AddPoint(pt);
					pt2 = new IntPoint((long)((double)pt2.X + normals[m_j].Y * num3), (long)((double)pt2.Y - normals[m_j].X * num3));
					AddPoint(pt2);
				}
				else
				{
					AddPoint(pt);
					AddPoint(pts[m_i][m_j]);
					AddPoint(pt2);
				}
			}

			internal void DoMiter()
			{
				if ((normals[m_k].X * normals[m_j].Y - normals[m_j].X * normals[m_k].Y) * delta >= 0.0)
				{
					double num = delta / m_R;
					AddPoint(new IntPoint(Round((double)pts[m_i][m_j].X + (normals[m_k].X + normals[m_j].X) * num), Round((double)pts[m_i][m_j].Y + (normals[m_k].Y + normals[m_j].Y) * num)));
					return;
				}
				IntPoint pt = new IntPoint(Round((double)pts[m_i][m_j].X + normals[m_k].X * delta), Round((double)pts[m_i][m_j].Y + normals[m_k].Y * delta));
				IntPoint pt2 = new IntPoint(Round((double)pts[m_i][m_j].X + normals[m_j].X * delta), Round((double)pts[m_i][m_j].Y + normals[m_j].Y * delta));
				AddPoint(pt);
				AddPoint(pts[m_i][m_j]);
				AddPoint(pt2);
			}

			internal void DoRound()
			{
				IntPoint pt = new IntPoint(Round((double)pts[m_i][m_j].X + normals[m_k].X * delta), Round((double)pts[m_i][m_j].Y + normals[m_k].Y * delta));
				IntPoint pt2 = new IntPoint(Round((double)pts[m_i][m_j].X + normals[m_j].X * delta), Round((double)pts[m_i][m_j].Y + normals[m_j].Y * delta));
				AddPoint(pt);
				if ((normals[m_k].X * normals[m_j].Y - normals[m_j].X * normals[m_k].Y) * delta >= 0.0)
				{
					if (normals[m_j].X * normals[m_k].X + normals[m_j].Y * normals[m_k].Y < 0.985)
					{
						double num = Math.Atan2(normals[m_k].Y, normals[m_k].X);
						double num2 = Math.Atan2(normals[m_j].Y, normals[m_j].X);
						if (delta > 0.0 && num2 < num)
						{
							num2 += Math.PI * 2.0;
						}
						else if (delta < 0.0 && num2 > num)
						{
							num2 -= Math.PI * 2.0;
						}
						List<IntPoint> list = BuildArc(pts[m_i][m_j], num, num2, delta);
						for (int i = 0; i < list.Count; i++)
						{
							AddPoint(list[i]);
						}
					}
				}
				else
				{
					AddPoint(pts[m_i][m_j]);
				}
				AddPoint(pt2);
			}
		}

		private List<OutRec> m_PolyOuts;

		private ClipType m_ClipType;

		private Scanbeam m_Scanbeam;

		private TEdge m_ActiveEdges;

		private TEdge m_SortedEdges;

		private IntersectNode m_IntersectNodes;

		private bool m_ExecuteLocked;

		private PolyFillType m_ClipFillType;

		private PolyFillType m_SubjFillType;

		private List<JoinRec> m_Joins;

		private List<HorzJoinRec> m_HorizJoins;

		private bool m_ReverseOutput;

		private bool m_UsingPolyTree;

		public bool ReverseSolution
		{
			get
			{
				return m_ReverseOutput;
			}
			set
			{
				m_ReverseOutput = value;
			}
		}

		public Clipper()
		{
			m_Scanbeam = null;
			m_ActiveEdges = null;
			m_SortedEdges = null;
			m_IntersectNodes = null;
			m_ExecuteLocked = false;
			m_UsingPolyTree = false;
			m_PolyOuts = new List<OutRec>();
			m_Joins = new List<JoinRec>();
			m_HorizJoins = new List<HorzJoinRec>();
			m_ReverseOutput = false;
		}

		public override void Clear()
		{
			if (m_edges.Count != 0)
			{
				DisposeAllPolyPts();
				base.Clear();
			}
		}

		private void DisposeScanbeamList()
		{
			while (m_Scanbeam != null)
			{
				Scanbeam next = m_Scanbeam.next;
				m_Scanbeam = null;
				m_Scanbeam = next;
			}
		}

		protected override void Reset()
		{
			base.Reset();
			m_Scanbeam = null;
			m_ActiveEdges = null;
			m_SortedEdges = null;
			DisposeAllPolyPts();
			for (LocalMinima localMinima = m_MinimaList; localMinima != null; localMinima = localMinima.next)
			{
				InsertScanbeam(localMinima.Y);
				InsertScanbeam(localMinima.leftBound.ytop);
			}
		}

		private void InsertScanbeam(long Y)
		{
			if (m_Scanbeam == null)
			{
				m_Scanbeam = new Scanbeam();
				m_Scanbeam.next = null;
				m_Scanbeam.Y = Y;
				return;
			}
			if (Y > m_Scanbeam.Y)
			{
				Scanbeam scanbeam = new Scanbeam();
				scanbeam.Y = Y;
				scanbeam.next = m_Scanbeam;
				m_Scanbeam = scanbeam;
				return;
			}
			Scanbeam scanbeam2 = m_Scanbeam;
			while (scanbeam2.next != null && Y <= scanbeam2.next.Y)
			{
				scanbeam2 = scanbeam2.next;
			}
			if (Y != scanbeam2.Y)
			{
				Scanbeam scanbeam3 = new Scanbeam();
				scanbeam3.Y = Y;
				scanbeam3.next = scanbeam2.next;
				scanbeam2.next = scanbeam3;
			}
		}

		public bool Execute(ClipType clipType, List<List<IntPoint>> solution, PolyFillType subjFillType, PolyFillType clipFillType)
		{
			if (m_ExecuteLocked)
			{
				return false;
			}
			m_ExecuteLocked = true;
			solution.Clear();
			m_SubjFillType = subjFillType;
			m_ClipFillType = clipFillType;
			m_ClipType = clipType;
			m_UsingPolyTree = false;
			bool flag = ExecuteInternal();
			if (flag)
			{
				BuildResult(solution);
			}
			m_ExecuteLocked = false;
			return flag;
		}

		public bool Execute(ClipType clipType, PolyTree polytree, PolyFillType subjFillType, PolyFillType clipFillType)
		{
			if (m_ExecuteLocked)
			{
				return false;
			}
			m_ExecuteLocked = true;
			m_SubjFillType = subjFillType;
			m_ClipFillType = clipFillType;
			m_ClipType = clipType;
			m_UsingPolyTree = true;
			bool flag = ExecuteInternal();
			if (flag)
			{
				BuildResult2(polytree);
			}
			m_ExecuteLocked = false;
			return flag;
		}

		public bool Execute(ClipType clipType, List<List<IntPoint>> solution)
		{
			return Execute(clipType, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
		}

		public bool Execute(ClipType clipType, PolyTree polytree)
		{
			return Execute(clipType, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
		}

		internal void FixHoleLinkage(OutRec outRec)
		{
			if (outRec.FirstLeft != null && (outRec.isHole == outRec.FirstLeft.isHole || outRec.FirstLeft.pts == null))
			{
				OutRec firstLeft = outRec.FirstLeft;
				while (firstLeft != null && (firstLeft.isHole == outRec.isHole || firstLeft.pts == null))
				{
					firstLeft = firstLeft.FirstLeft;
				}
				outRec.FirstLeft = firstLeft;
			}
		}

		private bool ExecuteInternal()
		{
			bool flag;
			try
			{
				Reset();
				if (m_CurrentLM == null)
				{
					return true;
				}
				long botY = PopScanbeam();
				do
				{
					InsertLocalMinimaIntoAEL(botY);
					m_HorizJoins.Clear();
					ProcessHorizontals();
					long num = PopScanbeam();
					flag = ProcessIntersections(botY, num);
					if (flag)
					{
						ProcessEdgesAtTopOfScanbeam(num);
						botY = num;
						continue;
					}
					break;
				}
				while (m_Scanbeam != null);
			}
			catch
			{
				flag = false;
			}
			if (flag)
			{
				for (int i = 0; i < m_PolyOuts.Count; i++)
				{
					OutRec outRec = m_PolyOuts[i];
					if (outRec.pts != null)
					{
						FixupOutPolygon(outRec);
						if (outRec.pts != null && (outRec.isHole ^ m_ReverseOutput) == Area(outRec, m_UseFullRange) > 0.0)
						{
							ReversePolyPtLinks(outRec.pts);
						}
					}
				}
				JoinCommonEdges();
			}
			m_Joins.Clear();
			m_HorizJoins.Clear();
			return flag;
		}

		private long PopScanbeam()
		{
			long y = m_Scanbeam.Y;
			m_Scanbeam = m_Scanbeam.next;
			return y;
		}

		private void DisposeAllPolyPts()
		{
			for (int i = 0; i < m_PolyOuts.Count; i++)
			{
				DisposeOutRec(i);
			}
			m_PolyOuts.Clear();
		}

		private void DisposeOutRec(int index)
		{
			OutRec outRec = m_PolyOuts[index];
			if (outRec.pts != null)
			{
				DisposeOutPts(outRec.pts);
			}
			outRec = null;
			m_PolyOuts[index] = null;
		}

		private void DisposeOutPts(OutPt pp)
		{
			if (pp != null)
			{
				pp.prev.next = null;
				while (pp != null)
				{
					pp = pp.next;
				}
			}
		}

		private void AddJoin(TEdge e1, TEdge e2, int e1OutIdx, int e2OutIdx)
		{
			JoinRec joinRec = new JoinRec();
			if (e1OutIdx >= 0)
			{
				joinRec.poly1Idx = e1OutIdx;
			}
			else
			{
				joinRec.poly1Idx = e1.outIdx;
			}
			joinRec.pt1a = new IntPoint(e1.xcurr, e1.ycurr);
			joinRec.pt1b = new IntPoint(e1.xtop, e1.ytop);
			if (e2OutIdx >= 0)
			{
				joinRec.poly2Idx = e2OutIdx;
			}
			else
			{
				joinRec.poly2Idx = e2.outIdx;
			}
			joinRec.pt2a = new IntPoint(e2.xcurr, e2.ycurr);
			joinRec.pt2b = new IntPoint(e2.xtop, e2.ytop);
			m_Joins.Add(joinRec);
		}

		private void AddHorzJoin(TEdge e, int idx)
		{
			HorzJoinRec horzJoinRec = new HorzJoinRec();
			horzJoinRec.edge = e;
			horzJoinRec.savedIdx = idx;
			m_HorizJoins.Add(horzJoinRec);
		}

		private void InsertLocalMinimaIntoAEL(long botY)
		{
			while (m_CurrentLM != null && m_CurrentLM.Y == botY)
			{
				TEdge leftBound = m_CurrentLM.leftBound;
				TEdge rightBound = m_CurrentLM.rightBound;
				InsertEdgeIntoAEL(leftBound);
				InsertScanbeam(leftBound.ytop);
				InsertEdgeIntoAEL(rightBound);
				if (IsEvenOddFillType(leftBound))
				{
					leftBound.windDelta = 1;
					rightBound.windDelta = 1;
				}
				else
				{
					rightBound.windDelta = -leftBound.windDelta;
				}
				SetWindingCount(leftBound);
				rightBound.windCnt = leftBound.windCnt;
				rightBound.windCnt2 = leftBound.windCnt2;
				if (rightBound.dx == -3.4E+38)
				{
					AddEdgeToSEL(rightBound);
					InsertScanbeam(rightBound.nextInLML.ytop);
				}
				else
				{
					InsertScanbeam(rightBound.ytop);
				}
				if (IsContributing(leftBound))
				{
					AddLocalMinPoly(leftBound, rightBound, new IntPoint(leftBound.xcurr, m_CurrentLM.Y));
				}
				if (rightBound.outIdx >= 0 && rightBound.dx == -3.4E+38)
				{
					for (int i = 0; i < m_HorizJoins.Count; i++)
					{
						IntPoint pt = default(IntPoint);
						IntPoint pt2 = default(IntPoint);
						HorzJoinRec horzJoinRec = m_HorizJoins[i];
						if (GetOverlapSegment(new IntPoint(horzJoinRec.edge.xbot, horzJoinRec.edge.ybot), new IntPoint(horzJoinRec.edge.xtop, horzJoinRec.edge.ytop), new IntPoint(rightBound.xbot, rightBound.ybot), new IntPoint(rightBound.xtop, rightBound.ytop), ref pt, ref pt2))
						{
							AddJoin(horzJoinRec.edge, rightBound, horzJoinRec.savedIdx, -1);
						}
					}
				}
				if (leftBound.nextInAEL != rightBound)
				{
					if (rightBound.outIdx >= 0 && rightBound.prevInAEL.outIdx >= 0 && SlopesEqual(rightBound.prevInAEL, rightBound, m_UseFullRange))
					{
						AddJoin(rightBound, rightBound.prevInAEL, -1, -1);
					}
					TEdge nextInAEL = leftBound.nextInAEL;
					IntPoint pt3 = new IntPoint(leftBound.xcurr, leftBound.ycurr);
					while (nextInAEL != rightBound)
					{
						if (nextInAEL == null)
						{
							throw new ClipperException("InsertLocalMinimaIntoAEL: missing rightbound!");
						}
						IntersectEdges(rightBound, nextInAEL, pt3, Protects.ipNone);
						nextInAEL = nextInAEL.nextInAEL;
					}
				}
				PopLocalMinima();
			}
		}

		private void InsertEdgeIntoAEL(TEdge edge)
		{
			edge.prevInAEL = null;
			edge.nextInAEL = null;
			if (m_ActiveEdges == null)
			{
				m_ActiveEdges = edge;
				return;
			}
			if (E2InsertsBeforeE1(m_ActiveEdges, edge))
			{
				edge.nextInAEL = m_ActiveEdges;
				m_ActiveEdges.prevInAEL = edge;
				m_ActiveEdges = edge;
				return;
			}
			TEdge tEdge = m_ActiveEdges;
			while (tEdge.nextInAEL != null && !E2InsertsBeforeE1(tEdge.nextInAEL, edge))
			{
				tEdge = tEdge.nextInAEL;
			}
			edge.nextInAEL = tEdge.nextInAEL;
			if (tEdge.nextInAEL != null)
			{
				tEdge.nextInAEL.prevInAEL = edge;
			}
			edge.prevInAEL = tEdge;
			tEdge.nextInAEL = edge;
		}

		private bool E2InsertsBeforeE1(TEdge e1, TEdge e2)
		{
			if (e2.xcurr != e1.xcurr)
			{
				return e2.xcurr < e1.xcurr;
			}
			return e2.dx > e1.dx;
		}

		private bool IsEvenOddFillType(TEdge edge)
		{
			if (edge.polyType == PolyType.ptSubject)
			{
				return m_SubjFillType == PolyFillType.pftEvenOdd;
			}
			return m_ClipFillType == PolyFillType.pftEvenOdd;
		}

		private bool IsEvenOddAltFillType(TEdge edge)
		{
			if (edge.polyType == PolyType.ptSubject)
			{
				return m_ClipFillType == PolyFillType.pftEvenOdd;
			}
			return m_SubjFillType == PolyFillType.pftEvenOdd;
		}

		private bool IsContributing(TEdge edge)
		{
			PolyFillType polyFillType;
			PolyFillType polyFillType2;
			if (edge.polyType == PolyType.ptSubject)
			{
				polyFillType = m_SubjFillType;
				polyFillType2 = m_ClipFillType;
			}
			else
			{
				polyFillType = m_ClipFillType;
				polyFillType2 = m_SubjFillType;
			}
			switch (polyFillType)
			{
			case PolyFillType.pftEvenOdd:
			case PolyFillType.pftNonZero:
				if (Math.Abs(edge.windCnt) != 1)
				{
					return false;
				}
				break;
			case PolyFillType.pftPositive:
				if (edge.windCnt != 1)
				{
					return false;
				}
				break;
			default:
				if (edge.windCnt != -1)
				{
					return false;
				}
				break;
			}
			switch (m_ClipType)
			{
			case ClipType.ctIntersection:
				switch (polyFillType2)
				{
				case PolyFillType.pftEvenOdd:
				case PolyFillType.pftNonZero:
					return edge.windCnt2 != 0;
				case PolyFillType.pftPositive:
					return edge.windCnt2 > 0;
				default:
					return edge.windCnt2 < 0;
				}
			case ClipType.ctUnion:
				switch (polyFillType2)
				{
				case PolyFillType.pftEvenOdd:
				case PolyFillType.pftNonZero:
					return edge.windCnt2 == 0;
				case PolyFillType.pftPositive:
					return edge.windCnt2 <= 0;
				default:
					return edge.windCnt2 >= 0;
				}
			case ClipType.ctDifference:
				if (edge.polyType == PolyType.ptSubject)
				{
					switch (polyFillType2)
					{
					case PolyFillType.pftEvenOdd:
					case PolyFillType.pftNonZero:
						return edge.windCnt2 == 0;
					case PolyFillType.pftPositive:
						return edge.windCnt2 <= 0;
					default:
						return edge.windCnt2 >= 0;
					}
				}
				switch (polyFillType2)
				{
				case PolyFillType.pftEvenOdd:
				case PolyFillType.pftNonZero:
					return edge.windCnt2 != 0;
				case PolyFillType.pftPositive:
					return edge.windCnt2 > 0;
				default:
					return edge.windCnt2 < 0;
				}
			default:
				return true;
			}
		}

		private void SetWindingCount(TEdge edge)
		{
			TEdge prevInAEL = edge.prevInAEL;
			while (prevInAEL != null && prevInAEL.polyType != edge.polyType)
			{
				prevInAEL = prevInAEL.prevInAEL;
			}
			if (prevInAEL == null)
			{
				edge.windCnt = edge.windDelta;
				edge.windCnt2 = 0;
				prevInAEL = m_ActiveEdges;
			}
			else if (IsEvenOddFillType(edge))
			{
				edge.windCnt = 1;
				edge.windCnt2 = prevInAEL.windCnt2;
				prevInAEL = prevInAEL.nextInAEL;
			}
			else
			{
				if (prevInAEL.windCnt * prevInAEL.windDelta < 0)
				{
					if (Math.Abs(prevInAEL.windCnt) > 1)
					{
						if (prevInAEL.windDelta * edge.windDelta < 0)
						{
							edge.windCnt = prevInAEL.windCnt;
						}
						else
						{
							edge.windCnt = prevInAEL.windCnt + edge.windDelta;
						}
					}
					else
					{
						edge.windCnt = prevInAEL.windCnt + prevInAEL.windDelta + edge.windDelta;
					}
				}
				else if (Math.Abs(prevInAEL.windCnt) > 1 && prevInAEL.windDelta * edge.windDelta < 0)
				{
					edge.windCnt = prevInAEL.windCnt;
				}
				else if (prevInAEL.windCnt + edge.windDelta == 0)
				{
					edge.windCnt = prevInAEL.windCnt;
				}
				else
				{
					edge.windCnt = prevInAEL.windCnt + edge.windDelta;
				}
				edge.windCnt2 = prevInAEL.windCnt2;
				prevInAEL = prevInAEL.nextInAEL;
			}
			if (IsEvenOddAltFillType(edge))
			{
				while (prevInAEL != edge)
				{
					edge.windCnt2 = ((edge.windCnt2 == 0) ? 1 : 0);
					prevInAEL = prevInAEL.nextInAEL;
				}
			}
			else
			{
				while (prevInAEL != edge)
				{
					edge.windCnt2 += prevInAEL.windDelta;
					prevInAEL = prevInAEL.nextInAEL;
				}
			}
		}

		private void AddEdgeToSEL(TEdge edge)
		{
			if (m_SortedEdges == null)
			{
				m_SortedEdges = edge;
				edge.prevInSEL = null;
				edge.nextInSEL = null;
			}
			else
			{
				edge.nextInSEL = m_SortedEdges;
				edge.prevInSEL = null;
				m_SortedEdges.prevInSEL = edge;
				m_SortedEdges = edge;
			}
		}

		private void CopyAELToSEL()
		{
			TEdge tEdge = (m_SortedEdges = m_ActiveEdges);
			if (m_ActiveEdges != null)
			{
				m_SortedEdges.prevInSEL = null;
				for (tEdge = tEdge.nextInAEL; tEdge != null; tEdge = tEdge.nextInAEL)
				{
					tEdge.prevInSEL = tEdge.prevInAEL;
					tEdge.prevInSEL.nextInSEL = tEdge;
					tEdge.nextInSEL = null;
				}
			}
		}

		private void SwapPositionsInAEL(TEdge edge1, TEdge edge2)
		{
			if (edge1.nextInAEL == edge2)
			{
				TEdge nextInAEL = edge2.nextInAEL;
				if (nextInAEL != null)
				{
					nextInAEL.prevInAEL = edge1;
				}
				TEdge prevInAEL = edge1.prevInAEL;
				if (prevInAEL != null)
				{
					prevInAEL.nextInAEL = edge2;
				}
				edge2.prevInAEL = prevInAEL;
				edge2.nextInAEL = edge1;
				edge1.prevInAEL = edge2;
				edge1.nextInAEL = nextInAEL;
			}
			else if (edge2.nextInAEL == edge1)
			{
				TEdge nextInAEL2 = edge1.nextInAEL;
				if (nextInAEL2 != null)
				{
					nextInAEL2.prevInAEL = edge2;
				}
				TEdge prevInAEL2 = edge2.prevInAEL;
				if (prevInAEL2 != null)
				{
					prevInAEL2.nextInAEL = edge1;
				}
				edge1.prevInAEL = prevInAEL2;
				edge1.nextInAEL = edge2;
				edge2.prevInAEL = edge1;
				edge2.nextInAEL = nextInAEL2;
			}
			else
			{
				TEdge nextInAEL3 = edge1.nextInAEL;
				TEdge prevInAEL3 = edge1.prevInAEL;
				edge1.nextInAEL = edge2.nextInAEL;
				if (edge1.nextInAEL != null)
				{
					edge1.nextInAEL.prevInAEL = edge1;
				}
				edge1.prevInAEL = edge2.prevInAEL;
				if (edge1.prevInAEL != null)
				{
					edge1.prevInAEL.nextInAEL = edge1;
				}
				edge2.nextInAEL = nextInAEL3;
				if (edge2.nextInAEL != null)
				{
					edge2.nextInAEL.prevInAEL = edge2;
				}
				edge2.prevInAEL = prevInAEL3;
				if (edge2.prevInAEL != null)
				{
					edge2.prevInAEL.nextInAEL = edge2;
				}
			}
			if (edge1.prevInAEL == null)
			{
				m_ActiveEdges = edge1;
			}
			else if (edge2.prevInAEL == null)
			{
				m_ActiveEdges = edge2;
			}
		}

		private void SwapPositionsInSEL(TEdge edge1, TEdge edge2)
		{
			if ((edge1.nextInSEL == null && edge1.prevInSEL == null) || (edge2.nextInSEL == null && edge2.prevInSEL == null))
			{
				return;
			}
			if (edge1.nextInSEL == edge2)
			{
				TEdge nextInSEL = edge2.nextInSEL;
				if (nextInSEL != null)
				{
					nextInSEL.prevInSEL = edge1;
				}
				TEdge prevInSEL = edge1.prevInSEL;
				if (prevInSEL != null)
				{
					prevInSEL.nextInSEL = edge2;
				}
				edge2.prevInSEL = prevInSEL;
				edge2.nextInSEL = edge1;
				edge1.prevInSEL = edge2;
				edge1.nextInSEL = nextInSEL;
			}
			else if (edge2.nextInSEL == edge1)
			{
				TEdge nextInSEL2 = edge1.nextInSEL;
				if (nextInSEL2 != null)
				{
					nextInSEL2.prevInSEL = edge2;
				}
				TEdge prevInSEL2 = edge2.prevInSEL;
				if (prevInSEL2 != null)
				{
					prevInSEL2.nextInSEL = edge1;
				}
				edge1.prevInSEL = prevInSEL2;
				edge1.nextInSEL = edge2;
				edge2.prevInSEL = edge1;
				edge2.nextInSEL = nextInSEL2;
			}
			else
			{
				TEdge nextInSEL3 = edge1.nextInSEL;
				TEdge prevInSEL3 = edge1.prevInSEL;
				edge1.nextInSEL = edge2.nextInSEL;
				if (edge1.nextInSEL != null)
				{
					edge1.nextInSEL.prevInSEL = edge1;
				}
				edge1.prevInSEL = edge2.prevInSEL;
				if (edge1.prevInSEL != null)
				{
					edge1.prevInSEL.nextInSEL = edge1;
				}
				edge2.nextInSEL = nextInSEL3;
				if (edge2.nextInSEL != null)
				{
					edge2.nextInSEL.prevInSEL = edge2;
				}
				edge2.prevInSEL = prevInSEL3;
				if (edge2.prevInSEL != null)
				{
					edge2.prevInSEL.nextInSEL = edge2;
				}
			}
			if (edge1.prevInSEL == null)
			{
				m_SortedEdges = edge1;
			}
			else if (edge2.prevInSEL == null)
			{
				m_SortedEdges = edge2;
			}
		}

		private void AddLocalMaxPoly(TEdge e1, TEdge e2, IntPoint pt)
		{
			AddOutPt(e1, pt);
			if (e1.outIdx == e2.outIdx)
			{
				e1.outIdx = -1;
				e2.outIdx = -1;
			}
			else if (e1.outIdx < e2.outIdx)
			{
				AppendPolygon(e1, e2);
			}
			else
			{
				AppendPolygon(e2, e1);
			}
		}

		private void AddLocalMinPoly(TEdge e1, TEdge e2, IntPoint pt)
		{
			TEdge tEdge;
			TEdge tEdge2;
			if (e2.dx == -3.4E+38 || e1.dx > e2.dx)
			{
				AddOutPt(e1, pt);
				e2.outIdx = e1.outIdx;
				e1.side = EdgeSide.esLeft;
				e2.side = EdgeSide.esRight;
				tEdge = e1;
				tEdge2 = ((tEdge.prevInAEL != e2) ? tEdge.prevInAEL : e2.prevInAEL);
			}
			else
			{
				AddOutPt(e2, pt);
				e1.outIdx = e2.outIdx;
				e1.side = EdgeSide.esRight;
				e2.side = EdgeSide.esLeft;
				tEdge = e2;
				tEdge2 = ((tEdge.prevInAEL != e1) ? tEdge.prevInAEL : e1.prevInAEL);
			}
			if (tEdge2 != null && tEdge2.outIdx >= 0 && TopX(tEdge2, pt.Y) == TopX(tEdge, pt.Y) && SlopesEqual(tEdge, tEdge2, m_UseFullRange))
			{
				AddJoin(tEdge, tEdge2, -1, -1);
			}
		}

		private OutRec CreateOutRec()
		{
			OutRec outRec = new OutRec();
			outRec.idx = -1;
			outRec.isHole = false;
			outRec.FirstLeft = null;
			outRec.pts = null;
			outRec.bottomPt = null;
			outRec.polyNode = null;
			return outRec;
		}

		private void AddOutPt(TEdge e, IntPoint pt)
		{
			bool flag = e.side == EdgeSide.esLeft;
			if (e.outIdx < 0)
			{
				OutRec outRec = CreateOutRec();
				m_PolyOuts.Add(outRec);
				outRec.idx = m_PolyOuts.Count - 1;
				e.outIdx = outRec.idx;
				OutPt outPt = (outRec.bottomPt = (outRec.pts = new OutPt()));
				outPt.pt = pt;
				outPt.idx = outRec.idx;
				outPt.next = outPt;
				outPt.prev = outPt;
				SetHoleState(e, outRec);
				return;
			}
			OutRec outRec2 = m_PolyOuts[e.outIdx];
			OutPt pts = outRec2.pts;
			if ((!flag || !ClipperBase.PointsEqual(pt, pts.pt)) && (flag || !ClipperBase.PointsEqual(pt, pts.prev.pt)))
			{
				OutPt outPt2 = new OutPt();
				outPt2.pt = pt;
				outPt2.idx = outRec2.idx;
				if (outPt2.pt.Y == outRec2.bottomPt.pt.Y && outPt2.pt.X < outRec2.bottomPt.pt.X)
				{
					outRec2.bottomPt = outPt2;
				}
				outPt2.next = pts;
				outPt2.prev = pts.prev;
				outPt2.prev.next = outPt2;
				pts.prev = outPt2;
				if (flag)
				{
					outRec2.pts = outPt2;
				}
			}
		}

		internal void SwapPoints(ref IntPoint pt1, ref IntPoint pt2)
		{
			IntPoint intPoint = pt1;
			pt1 = pt2;
			pt2 = intPoint;
		}

		private bool GetOverlapSegment(IntPoint pt1a, IntPoint pt1b, IntPoint pt2a, IntPoint pt2b, ref IntPoint pt1, ref IntPoint pt2)
		{
			if (Math.Abs(pt1a.X - pt1b.X) > Math.Abs(pt1a.Y - pt1b.Y))
			{
				if (pt1a.X > pt1b.X)
				{
					SwapPoints(ref pt1a, ref pt1b);
				}
				if (pt2a.X > pt2b.X)
				{
					SwapPoints(ref pt2a, ref pt2b);
				}
				if (pt1a.X > pt2a.X)
				{
					pt1 = pt1a;
				}
				else
				{
					pt1 = pt2a;
				}
				if (pt1b.X < pt2b.X)
				{
					pt2 = pt1b;
				}
				else
				{
					pt2 = pt2b;
				}
				return pt1.X < pt2.X;
			}
			if (pt1a.Y < pt1b.Y)
			{
				SwapPoints(ref pt1a, ref pt1b);
			}
			if (pt2a.Y < pt2b.Y)
			{
				SwapPoints(ref pt2a, ref pt2b);
			}
			if (pt1a.Y < pt2a.Y)
			{
				pt1 = pt1a;
			}
			else
			{
				pt1 = pt2a;
			}
			if (pt1b.Y > pt2b.Y)
			{
				pt2 = pt1b;
			}
			else
			{
				pt2 = pt2b;
			}
			return pt1.Y > pt2.Y;
		}

		private bool FindSegment(ref OutPt pp, ref IntPoint pt1, ref IntPoint pt2)
		{
			if (pp == null)
			{
				return false;
			}
			OutPt outPt = pp;
			IntPoint intPoint = new IntPoint(pt1);
			IntPoint intPoint2 = new IntPoint(pt2);
			do
			{
				if (SlopesEqual(intPoint, intPoint2, pp.pt, pp.prev.pt, UseFullRange: true) && SlopesEqual(intPoint, intPoint2, pp.pt, UseFullRange: true) && GetOverlapSegment(intPoint, intPoint2, pp.pt, pp.prev.pt, ref pt1, ref pt2))
				{
					return true;
				}
				pp = pp.next;
			}
			while (pp != outPt);
			return false;
		}

		internal bool Pt3IsBetweenPt1AndPt2(IntPoint pt1, IntPoint pt2, IntPoint pt3)
		{
			if (ClipperBase.PointsEqual(pt1, pt3) || ClipperBase.PointsEqual(pt2, pt3))
			{
				return true;
			}
			if (pt1.X != pt2.X)
			{
				return pt1.X < pt3.X == pt3.X < pt2.X;
			}
			return pt1.Y < pt3.Y == pt3.Y < pt2.Y;
		}

		private OutPt InsertPolyPtBetween(OutPt p1, OutPt p2, IntPoint pt)
		{
			OutPt outPt = new OutPt();
			outPt.pt = pt;
			if (p2 == p1.next)
			{
				p1.next = outPt;
				p2.prev = outPt;
				outPt.next = p2;
				outPt.prev = p1;
			}
			else
			{
				p2.next = outPt;
				p1.prev = outPt;
				outPt.next = p1;
				outPt.prev = p2;
			}
			return outPt;
		}

		private void SetHoleState(TEdge e, OutRec outRec)
		{
			bool flag = false;
			for (TEdge prevInAEL = e.prevInAEL; prevInAEL != null; prevInAEL = prevInAEL.prevInAEL)
			{
				if (prevInAEL.outIdx >= 0)
				{
					flag = !flag;
					if (outRec.FirstLeft == null)
					{
						outRec.FirstLeft = m_PolyOuts[prevInAEL.outIdx];
					}
				}
			}
			if (flag)
			{
				outRec.isHole = true;
			}
		}

		private double GetDx(IntPoint pt1, IntPoint pt2)
		{
			if (pt1.Y == pt2.Y)
			{
				return -3.4E+38;
			}
			return (double)(pt2.X - pt1.X) / (double)(pt2.Y - pt1.Y);
		}

		private bool FirstIsBottomPt(OutPt btmPt1, OutPt btmPt2)
		{
			OutPt prev = btmPt1.prev;
			while (ClipperBase.PointsEqual(prev.pt, btmPt1.pt) && prev != btmPt1)
			{
				prev = prev.prev;
			}
			double num = Math.Abs(GetDx(btmPt1.pt, prev.pt));
			prev = btmPt1.next;
			while (ClipperBase.PointsEqual(prev.pt, btmPt1.pt) && prev != btmPt1)
			{
				prev = prev.next;
			}
			double num2 = Math.Abs(GetDx(btmPt1.pt, prev.pt));
			prev = btmPt2.prev;
			while (ClipperBase.PointsEqual(prev.pt, btmPt2.pt) && prev != btmPt2)
			{
				prev = prev.prev;
			}
			double num3 = Math.Abs(GetDx(btmPt2.pt, prev.pt));
			prev = btmPt2.next;
			while (ClipperBase.PointsEqual(prev.pt, btmPt2.pt) && prev != btmPt2)
			{
				prev = prev.next;
			}
			double num4 = Math.Abs(GetDx(btmPt2.pt, prev.pt));
			if (!(num >= num3) || !(num >= num4))
			{
				if (num2 >= num3)
				{
					return num2 >= num4;
				}
				return false;
			}
			return true;
		}

		private OutPt GetBottomPt(OutPt pp)
		{
			OutPt outPt = null;
			OutPt next;
			for (next = pp.next; next != pp; next = next.next)
			{
				if (next.pt.Y > pp.pt.Y)
				{
					pp = next;
					outPt = null;
				}
				else if (next.pt.Y == pp.pt.Y && next.pt.X <= pp.pt.X)
				{
					if (next.pt.X < pp.pt.X)
					{
						outPt = null;
						pp = next;
					}
					else if (next.next != pp && next.prev != pp)
					{
						outPt = next;
					}
				}
			}
			if (outPt != null)
			{
				while (outPt != next)
				{
					if (!FirstIsBottomPt(next, outPt))
					{
						pp = outPt;
					}
					outPt = outPt.next;
					while (!ClipperBase.PointsEqual(outPt.pt, pp.pt))
					{
						outPt = outPt.next;
					}
				}
			}
			return pp;
		}

		private OutRec GetLowermostRec(OutRec outRec1, OutRec outRec2)
		{
			OutPt bottomPt = outRec1.bottomPt;
			OutPt bottomPt2 = outRec2.bottomPt;
			if (bottomPt.pt.Y > bottomPt2.pt.Y)
			{
				return outRec1;
			}
			if (bottomPt.pt.Y < bottomPt2.pt.Y)
			{
				return outRec2;
			}
			if (bottomPt.pt.X < bottomPt2.pt.X)
			{
				return outRec1;
			}
			if (bottomPt.pt.X > bottomPt2.pt.X)
			{
				return outRec2;
			}
			if (bottomPt.next == bottomPt)
			{
				return outRec2;
			}
			if (bottomPt2.next == bottomPt2)
			{
				return outRec1;
			}
			if (FirstIsBottomPt(bottomPt, bottomPt2))
			{
				return outRec1;
			}
			return outRec2;
		}

		private bool Param1RightOfParam2(OutRec outRec1, OutRec outRec2)
		{
			do
			{
				outRec1 = outRec1.FirstLeft;
				if (outRec1 == outRec2)
				{
					return true;
				}
			}
			while (outRec1 != null);
			return false;
		}

		private void AppendPolygon(TEdge e1, TEdge e2)
		{
			OutRec outRec = m_PolyOuts[e1.outIdx];
			OutRec outRec2 = m_PolyOuts[e2.outIdx];
			OutRec outRec3 = (Param1RightOfParam2(outRec, outRec2) ? outRec2 : ((!Param1RightOfParam2(outRec2, outRec)) ? GetLowermostRec(outRec, outRec2) : outRec));
			OutPt pts = outRec.pts;
			OutPt prev = pts.prev;
			OutPt pts2 = outRec2.pts;
			OutPt prev2 = pts2.prev;
			EdgeSide side;
			if (e1.side == EdgeSide.esLeft)
			{
				if (e2.side == EdgeSide.esLeft)
				{
					ReversePolyPtLinks(pts2);
					pts2.next = pts;
					pts.prev = pts2;
					prev.next = prev2;
					prev2.prev = prev;
					outRec.pts = prev2;
				}
				else
				{
					prev2.next = pts;
					pts.prev = prev2;
					pts2.prev = prev;
					prev.next = pts2;
					outRec.pts = pts2;
				}
				side = EdgeSide.esLeft;
			}
			else
			{
				if (e2.side == EdgeSide.esRight)
				{
					ReversePolyPtLinks(pts2);
					prev.next = prev2;
					prev2.prev = prev;
					pts2.next = pts;
					pts.prev = pts2;
				}
				else
				{
					prev.next = pts2;
					pts2.prev = prev;
					pts.prev = prev2;
					prev2.next = pts;
				}
				side = EdgeSide.esRight;
			}
			if (outRec3 == outRec2)
			{
				outRec.bottomPt = outRec2.bottomPt;
				outRec.bottomPt.idx = outRec.idx;
				if (outRec2.FirstLeft != outRec)
				{
					outRec.FirstLeft = outRec2.FirstLeft;
				}
				outRec.isHole = outRec2.isHole;
			}
			outRec2.pts = null;
			outRec2.bottomPt = null;
			outRec2.FirstLeft = outRec;
			int outIdx = e1.outIdx;
			int outIdx2 = e2.outIdx;
			e1.outIdx = -1;
			e2.outIdx = -1;
			for (TEdge tEdge = m_ActiveEdges; tEdge != null; tEdge = tEdge.nextInAEL)
			{
				if (tEdge.outIdx == outIdx2)
				{
					tEdge.outIdx = outIdx;
					tEdge.side = side;
					break;
				}
			}
			for (int i = 0; i < m_Joins.Count; i++)
			{
				if (m_Joins[i].poly1Idx == outIdx2)
				{
					m_Joins[i].poly1Idx = outIdx;
				}
				if (m_Joins[i].poly2Idx == outIdx2)
				{
					m_Joins[i].poly2Idx = outIdx;
				}
			}
			for (int j = 0; j < m_HorizJoins.Count; j++)
			{
				if (m_HorizJoins[j].savedIdx == outIdx2)
				{
					m_HorizJoins[j].savedIdx = outIdx;
				}
			}
		}

		private void ReversePolyPtLinks(OutPt pp)
		{
			if (pp != null)
			{
				OutPt outPt = pp;
				do
				{
					OutPt next = outPt.next;
					outPt.next = outPt.prev;
					outPt.prev = next;
					outPt = next;
				}
				while (outPt != pp);
			}
		}

		private static void SwapSides(TEdge edge1, TEdge edge2)
		{
			EdgeSide side = edge1.side;
			edge1.side = edge2.side;
			edge2.side = side;
		}

		private static void SwapPolyIndexes(TEdge edge1, TEdge edge2)
		{
			int outIdx = edge1.outIdx;
			edge1.outIdx = edge2.outIdx;
			edge2.outIdx = outIdx;
		}

		private void DoEdge1(TEdge edge1, TEdge edge2, IntPoint pt)
		{
			AddOutPt(edge1, pt);
			SwapSides(edge1, edge2);
			SwapPolyIndexes(edge1, edge2);
		}

		private void DoEdge2(TEdge edge1, TEdge edge2, IntPoint pt)
		{
			AddOutPt(edge2, pt);
			SwapSides(edge1, edge2);
			SwapPolyIndexes(edge1, edge2);
		}

		private void DoBothEdges(TEdge edge1, TEdge edge2, IntPoint pt)
		{
			AddOutPt(edge1, pt);
			AddOutPt(edge2, pt);
			SwapSides(edge1, edge2);
			SwapPolyIndexes(edge1, edge2);
		}

		private void IntersectEdges(TEdge e1, TEdge e2, IntPoint pt, Protects protects)
		{
			bool flag = (Protects.ipLeft & protects) == 0 && e1.nextInLML == null && e1.xtop == pt.X && e1.ytop == pt.Y;
			bool flag2 = (Protects.ipRight & protects) == 0 && e2.nextInLML == null && e2.xtop == pt.X && e2.ytop == pt.Y;
			bool flag3 = e1.outIdx >= 0;
			bool flag4 = e2.outIdx >= 0;
			if (e1.polyType == e2.polyType)
			{
				if (IsEvenOddFillType(e1))
				{
					int windCnt = e1.windCnt;
					e1.windCnt = e2.windCnt;
					e2.windCnt = windCnt;
				}
				else
				{
					if (e1.windCnt + e2.windDelta == 0)
					{
						e1.windCnt = -e1.windCnt;
					}
					else
					{
						e1.windCnt += e2.windDelta;
					}
					if (e2.windCnt - e1.windDelta == 0)
					{
						e2.windCnt = -e2.windCnt;
					}
					else
					{
						e2.windCnt -= e1.windDelta;
					}
				}
			}
			else
			{
				if (!IsEvenOddFillType(e2))
				{
					e1.windCnt2 += e2.windDelta;
				}
				else
				{
					e1.windCnt2 = ((e1.windCnt2 == 0) ? 1 : 0);
				}
				if (!IsEvenOddFillType(e1))
				{
					e2.windCnt2 -= e1.windDelta;
				}
				else
				{
					e2.windCnt2 = ((e2.windCnt2 == 0) ? 1 : 0);
				}
			}
			PolyFillType polyFillType;
			PolyFillType polyFillType2;
			if (e1.polyType == PolyType.ptSubject)
			{
				polyFillType = m_SubjFillType;
				polyFillType2 = m_ClipFillType;
			}
			else
			{
				polyFillType = m_ClipFillType;
				polyFillType2 = m_SubjFillType;
			}
			PolyFillType polyFillType3;
			PolyFillType polyFillType4;
			if (e2.polyType == PolyType.ptSubject)
			{
				polyFillType3 = m_SubjFillType;
				polyFillType4 = m_ClipFillType;
			}
			else
			{
				polyFillType3 = m_ClipFillType;
				polyFillType4 = m_SubjFillType;
			}
			int num;
			switch (polyFillType)
			{
			case PolyFillType.pftPositive:
				num = e1.windCnt;
				break;
			case PolyFillType.pftNegative:
				num = -e1.windCnt;
				break;
			default:
				num = Math.Abs(e1.windCnt);
				break;
			}
			int num2;
			switch (polyFillType3)
			{
			case PolyFillType.pftPositive:
				num2 = e2.windCnt;
				break;
			case PolyFillType.pftNegative:
				num2 = -e2.windCnt;
				break;
			default:
				num2 = Math.Abs(e2.windCnt);
				break;
			}
			if (flag3 && flag4)
			{
				if (flag || flag2 || (num != 0 && num != 1) || (num2 != 0 && num2 != 1) || (e1.polyType != e2.polyType && m_ClipType != ClipType.ctXor))
				{
					AddLocalMaxPoly(e1, e2, pt);
				}
				else
				{
					DoBothEdges(e1, e2, pt);
				}
			}
			else if (flag3)
			{
				if ((num2 == 0 || num2 == 1) && (m_ClipType != 0 || e2.polyType == PolyType.ptSubject || e2.windCnt2 != 0))
				{
					DoEdge1(e1, e2, pt);
				}
			}
			else if (flag4)
			{
				if ((num == 0 || num == 1) && (m_ClipType != 0 || e1.polyType == PolyType.ptSubject || e1.windCnt2 != 0))
				{
					DoEdge2(e1, e2, pt);
				}
			}
			else if ((num == 0 || num == 1) && (num2 == 0 || num2 == 1) && !flag && !flag2)
			{
				long num3;
				switch (polyFillType2)
				{
				case PolyFillType.pftPositive:
					num3 = e1.windCnt2;
					break;
				case PolyFillType.pftNegative:
					num3 = -e1.windCnt2;
					break;
				default:
					num3 = Math.Abs(e1.windCnt2);
					break;
				}
				long num4;
				switch (polyFillType4)
				{
				case PolyFillType.pftPositive:
					num4 = e2.windCnt2;
					break;
				case PolyFillType.pftNegative:
					num4 = -e2.windCnt2;
					break;
				default:
					num4 = Math.Abs(e2.windCnt2);
					break;
				}
				if (e1.polyType != e2.polyType)
				{
					AddLocalMinPoly(e1, e2, pt);
				}
				else if (num == 1 && num2 == 1)
				{
					switch (m_ClipType)
					{
					case ClipType.ctIntersection:
						if (num3 > 0 && num4 > 0)
						{
							AddLocalMinPoly(e1, e2, pt);
						}
						break;
					case ClipType.ctUnion:
						if (num3 <= 0 && num4 <= 0)
						{
							AddLocalMinPoly(e1, e2, pt);
						}
						break;
					case ClipType.ctDifference:
						if ((e1.polyType == PolyType.ptClip && num3 > 0 && num4 > 0) || (e1.polyType == PolyType.ptSubject && num3 <= 0 && num4 <= 0))
						{
							AddLocalMinPoly(e1, e2, pt);
						}
						break;
					case ClipType.ctXor:
						AddLocalMinPoly(e1, e2, pt);
						break;
					}
				}
				else
				{
					SwapSides(e1, e2);
				}
			}
			if (flag != flag2 && ((flag && e1.outIdx >= 0) || (flag2 && e2.outIdx >= 0)))
			{
				SwapSides(e1, e2);
				SwapPolyIndexes(e1, e2);
			}
			if (flag)
			{
				DeleteFromAEL(e1);
			}
			if (flag2)
			{
				DeleteFromAEL(e2);
			}
		}

		private void DeleteFromAEL(TEdge e)
		{
			TEdge prevInAEL = e.prevInAEL;
			TEdge nextInAEL = e.nextInAEL;
			if (prevInAEL != null || nextInAEL != null || e == m_ActiveEdges)
			{
				if (prevInAEL != null)
				{
					prevInAEL.nextInAEL = nextInAEL;
				}
				else
				{
					m_ActiveEdges = nextInAEL;
				}
				if (nextInAEL != null)
				{
					nextInAEL.prevInAEL = prevInAEL;
				}
				e.nextInAEL = null;
				e.prevInAEL = null;
			}
		}

		private void DeleteFromSEL(TEdge e)
		{
			TEdge prevInSEL = e.prevInSEL;
			TEdge nextInSEL = e.nextInSEL;
			if (prevInSEL != null || nextInSEL != null || e == m_SortedEdges)
			{
				if (prevInSEL != null)
				{
					prevInSEL.nextInSEL = nextInSEL;
				}
				else
				{
					m_SortedEdges = nextInSEL;
				}
				if (nextInSEL != null)
				{
					nextInSEL.prevInSEL = prevInSEL;
				}
				e.nextInSEL = null;
				e.prevInSEL = null;
			}
		}

		private void UpdateEdgeIntoAEL(ref TEdge e)
		{
			if (e.nextInLML == null)
			{
				throw new ClipperException("UpdateEdgeIntoAEL: invalid call");
			}
			TEdge prevInAEL = e.prevInAEL;
			TEdge nextInAEL = e.nextInAEL;
			e.nextInLML.outIdx = e.outIdx;
			if (prevInAEL != null)
			{
				prevInAEL.nextInAEL = e.nextInLML;
			}
			else
			{
				m_ActiveEdges = e.nextInLML;
			}
			if (nextInAEL != null)
			{
				nextInAEL.prevInAEL = e.nextInLML;
			}
			e.nextInLML.side = e.side;
			e.nextInLML.windDelta = e.windDelta;
			e.nextInLML.windCnt = e.windCnt;
			e.nextInLML.windCnt2 = e.windCnt2;
			e = e.nextInLML;
			e.prevInAEL = prevInAEL;
			e.nextInAEL = nextInAEL;
			if (e.dx != -3.4E+38)
			{
				InsertScanbeam(e.ytop);
			}
		}

		private void ProcessHorizontals()
		{
			for (TEdge sortedEdges = m_SortedEdges; sortedEdges != null; sortedEdges = m_SortedEdges)
			{
				DeleteFromSEL(sortedEdges);
				ProcessHorizontal(sortedEdges);
			}
		}

		private void ProcessHorizontal(TEdge horzEdge)
		{
			long num;
			long num2;
			Direction direction;
			if (horzEdge.xcurr < horzEdge.xtop)
			{
				num = horzEdge.xcurr;
				num2 = horzEdge.xtop;
				direction = Direction.dLeftToRight;
			}
			else
			{
				num = horzEdge.xtop;
				num2 = horzEdge.xcurr;
				direction = Direction.dRightToLeft;
			}
			TEdge tEdge = ((horzEdge.nextInLML == null) ? GetMaximaPair(horzEdge) : null);
			TEdge tEdge2 = GetNextInAEL(horzEdge, direction);
			while (tEdge2 != null)
			{
				TEdge nextInAEL = GetNextInAEL(tEdge2, direction);
				if (tEdge != null || (direction == Direction.dLeftToRight && tEdge2.xcurr <= num2) || (direction == Direction.dRightToLeft && tEdge2.xcurr >= num))
				{
					if (tEdge2.xcurr == horzEdge.xtop && tEdge == null)
					{
						if (SlopesEqual(tEdge2, horzEdge.nextInLML, m_UseFullRange))
						{
							if (horzEdge.outIdx >= 0 && tEdge2.outIdx >= 0)
							{
								AddJoin(horzEdge.nextInLML, tEdge2, horzEdge.outIdx, -1);
							}
							break;
						}
						if (tEdge2.dx < horzEdge.nextInLML.dx)
						{
							break;
						}
					}
					if (tEdge2 == tEdge)
					{
						if (direction == Direction.dLeftToRight)
						{
							IntersectEdges(horzEdge, tEdge2, new IntPoint(tEdge2.xcurr, horzEdge.ycurr), Protects.ipNone);
						}
						else
						{
							IntersectEdges(tEdge2, horzEdge, new IntPoint(tEdge2.xcurr, horzEdge.ycurr), Protects.ipNone);
						}
						if (tEdge.outIdx >= 0)
						{
							throw new ClipperException("ProcessHorizontal error");
						}
						return;
					}
					if (tEdge2.dx == -3.4E+38 && !IsMinima(tEdge2) && tEdge2.xcurr <= tEdge2.xtop)
					{
						if (direction == Direction.dLeftToRight)
						{
							IntersectEdges(horzEdge, tEdge2, new IntPoint(tEdge2.xcurr, horzEdge.ycurr), IsTopHorz(horzEdge, tEdge2.xcurr) ? Protects.ipLeft : Protects.ipBoth);
						}
						else
						{
							IntersectEdges(tEdge2, horzEdge, new IntPoint(tEdge2.xcurr, horzEdge.ycurr), IsTopHorz(horzEdge, tEdge2.xcurr) ? Protects.ipRight : Protects.ipBoth);
						}
					}
					else if (direction == Direction.dLeftToRight)
					{
						IntersectEdges(horzEdge, tEdge2, new IntPoint(tEdge2.xcurr, horzEdge.ycurr), IsTopHorz(horzEdge, tEdge2.xcurr) ? Protects.ipLeft : Protects.ipBoth);
					}
					else
					{
						IntersectEdges(tEdge2, horzEdge, new IntPoint(tEdge2.xcurr, horzEdge.ycurr), IsTopHorz(horzEdge, tEdge2.xcurr) ? Protects.ipRight : Protects.ipBoth);
					}
					SwapPositionsInAEL(horzEdge, tEdge2);
				}
				else if ((direction == Direction.dLeftToRight && tEdge2.xcurr > num2 && horzEdge.nextInSEL == null) || (direction == Direction.dRightToLeft && tEdge2.xcurr < num && horzEdge.nextInSEL == null))
				{
					break;
				}
				tEdge2 = nextInAEL;
			}
			if (horzEdge.nextInLML != null)
			{
				if (horzEdge.outIdx >= 0)
				{
					AddOutPt(horzEdge, new IntPoint(horzEdge.xtop, horzEdge.ytop));
				}
				UpdateEdgeIntoAEL(ref horzEdge);
				return;
			}
			if (horzEdge.outIdx >= 0)
			{
				IntersectEdges(horzEdge, tEdge, new IntPoint(horzEdge.xtop, horzEdge.ycurr), Protects.ipBoth);
			}
			DeleteFromAEL(tEdge);
			DeleteFromAEL(horzEdge);
		}

		private bool IsTopHorz(TEdge horzEdge, double XPos)
		{
			for (TEdge tEdge = m_SortedEdges; tEdge != null; tEdge = tEdge.nextInSEL)
			{
				if (XPos >= (double)Math.Min(tEdge.xcurr, tEdge.xtop) && XPos <= (double)Math.Max(tEdge.xcurr, tEdge.xtop))
				{
					return false;
				}
			}
			return true;
		}

		private TEdge GetNextInAEL(TEdge e, Direction Direction)
		{
			if (Direction != Direction.dLeftToRight)
			{
				return e.prevInAEL;
			}
			return e.nextInAEL;
		}

		private bool IsMinima(TEdge e)
		{
			if (e != null && e.prev.nextInLML != e)
			{
				return e.next.nextInLML != e;
			}
			return false;
		}

		private bool IsMaxima(TEdge e, double Y)
		{
			if (e != null && (double)e.ytop == Y)
			{
				return e.nextInLML == null;
			}
			return false;
		}

		private bool IsIntermediate(TEdge e, double Y)
		{
			if ((double)e.ytop == Y)
			{
				return e.nextInLML != null;
			}
			return false;
		}

		private TEdge GetMaximaPair(TEdge e)
		{
			if (!IsMaxima(e.next, e.ytop) || e.next.xtop != e.xtop)
			{
				return e.prev;
			}
			return e.next;
		}

		private bool ProcessIntersections(long botY, long topY)
		{
			if (m_ActiveEdges == null)
			{
				return true;
			}
			try
			{
				BuildIntersectList(botY, topY);
				if (m_IntersectNodes == null)
				{
					return true;
				}
				if (!FixupIntersections())
				{
					return false;
				}
				ProcessIntersectList();
			}
			catch
			{
				m_SortedEdges = null;
				DisposeIntersectNodes();
				throw new ClipperException("ProcessIntersections error");
			}
			return true;
		}

		private void BuildIntersectList(long botY, long topY)
		{
			if (m_ActiveEdges == null)
			{
				return;
			}
			for (TEdge tEdge = (m_SortedEdges = m_ActiveEdges); tEdge != null; tEdge = tEdge.nextInAEL)
			{
				tEdge.prevInSEL = tEdge.prevInAEL;
				tEdge.nextInSEL = tEdge.nextInAEL;
				tEdge.tmpX = TopX(tEdge, topY);
			}
			bool flag = true;
			while (flag && m_SortedEdges != null)
			{
				flag = false;
				TEdge tEdge = m_SortedEdges;
				while (tEdge.nextInSEL != null)
				{
					TEdge nextInSEL = tEdge.nextInSEL;
					IntPoint ip = default(IntPoint);
					if (tEdge.tmpX > nextInSEL.tmpX && IntersectPoint(tEdge, nextInSEL, ref ip))
					{
						if (ip.Y > botY)
						{
							ip.Y = botY;
							ip.X = TopX(tEdge, ip.Y);
						}
						AddIntersectNode(tEdge, nextInSEL, ip);
						SwapPositionsInSEL(tEdge, nextInSEL);
						flag = true;
					}
					else
					{
						tEdge = nextInSEL;
					}
				}
				if (tEdge.prevInSEL == null)
				{
					break;
				}
				tEdge.prevInSEL.nextInSEL = null;
			}
			m_SortedEdges = null;
		}

		private bool FixupIntersections()
		{
			if (m_IntersectNodes.next == null)
			{
				return true;
			}
			CopyAELToSEL();
			IntersectNode intersectNode = m_IntersectNodes;
			for (IntersectNode next = m_IntersectNodes.next; next != null; next = intersectNode.next)
			{
				TEdge edge = intersectNode.edge1;
				TEdge edge2;
				if (edge.prevInSEL == intersectNode.edge2)
				{
					edge2 = edge.prevInSEL;
				}
				else if (edge.nextInSEL == intersectNode.edge2)
				{
					edge2 = edge.nextInSEL;
				}
				else
				{
					while (next != null && next.edge1.nextInSEL != next.edge2 && next.edge1.prevInSEL != next.edge2)
					{
						next = next.next;
					}
					if (next == null)
					{
						return false;
					}
					SwapIntersectNodes(intersectNode, next);
					edge = intersectNode.edge1;
					edge2 = intersectNode.edge2;
				}
				SwapPositionsInSEL(edge, edge2);
				intersectNode = intersectNode.next;
			}
			m_SortedEdges = null;
			if (intersectNode.edge1.prevInSEL != intersectNode.edge2)
			{
				return intersectNode.edge1.nextInSEL == intersectNode.edge2;
			}
			return true;
		}

		private void ProcessIntersectList()
		{
			while (m_IntersectNodes != null)
			{
				IntersectNode next = m_IntersectNodes.next;
				IntersectEdges(m_IntersectNodes.edge1, m_IntersectNodes.edge2, m_IntersectNodes.pt, Protects.ipBoth);
				SwapPositionsInAEL(m_IntersectNodes.edge1, m_IntersectNodes.edge2);
				m_IntersectNodes = null;
				m_IntersectNodes = next;
			}
		}

		private static long Round(double value)
		{
			if (!(value < 0.0))
			{
				return (long)(value + 0.5);
			}
			return (long)(value - 0.5);
		}

		private static long TopX(TEdge edge, long currentY)
		{
			if (currentY == edge.ytop)
			{
				return edge.xtop;
			}
			return edge.xbot + Round(edge.dx * (double)(currentY - edge.ybot));
		}

		private void AddIntersectNode(TEdge e1, TEdge e2, IntPoint pt)
		{
			IntersectNode intersectNode = new IntersectNode();
			intersectNode.edge1 = e1;
			intersectNode.edge2 = e2;
			intersectNode.pt = pt;
			intersectNode.next = null;
			if (m_IntersectNodes == null)
			{
				m_IntersectNodes = intersectNode;
				return;
			}
			if (ProcessParam1BeforeParam2(intersectNode, m_IntersectNodes))
			{
				intersectNode.next = m_IntersectNodes;
				m_IntersectNodes = intersectNode;
				return;
			}
			IntersectNode intersectNode2 = m_IntersectNodes;
			while (intersectNode2.next != null && ProcessParam1BeforeParam2(intersectNode2.next, intersectNode))
			{
				intersectNode2 = intersectNode2.next;
			}
			intersectNode.next = intersectNode2.next;
			intersectNode2.next = intersectNode;
		}

		private bool ProcessParam1BeforeParam2(IntersectNode node1, IntersectNode node2)
		{
			if (node1.pt.Y == node2.pt.Y)
			{
				if (node1.edge1 == node2.edge1 || node1.edge2 == node2.edge1)
				{
					bool flag = node2.pt.X > node1.pt.X;
					if (!(node2.edge1.dx > 0.0))
					{
						return flag;
					}
					return !flag;
				}
				if (node1.edge1 == node2.edge2 || node1.edge2 == node2.edge2)
				{
					bool flag = node2.pt.X > node1.pt.X;
					if (!(node2.edge2.dx > 0.0))
					{
						return flag;
					}
					return !flag;
				}
				return node2.pt.X > node1.pt.X;
			}
			return node1.pt.Y > node2.pt.Y;
		}

		private void SwapIntersectNodes(IntersectNode int1, IntersectNode int2)
		{
			TEdge edge = int1.edge1;
			TEdge edge2 = int1.edge2;
			IntPoint pt = int1.pt;
			int1.edge1 = int2.edge1;
			int1.edge2 = int2.edge2;
			int1.pt = int2.pt;
			int2.edge1 = edge;
			int2.edge2 = edge2;
			int2.pt = pt;
		}

		private bool IntersectPoint(TEdge edge1, TEdge edge2, ref IntPoint ip)
		{
			if (SlopesEqual(edge1, edge2, m_UseFullRange))
			{
				return false;
			}
			if (edge1.dx == 0.0)
			{
				ip.X = edge1.xbot;
				if (edge2.dx == -3.4E+38)
				{
					ip.Y = edge2.ybot;
				}
				else
				{
					double num = (double)edge2.ybot - (double)edge2.xbot / edge2.dx;
					ip.Y = Round((double)ip.X / edge2.dx + num);
				}
			}
			else if (edge2.dx == 0.0)
			{
				ip.X = edge2.xbot;
				if (edge1.dx == -3.4E+38)
				{
					ip.Y = edge1.ybot;
				}
				else
				{
					double num2 = (double)edge1.ybot - (double)edge1.xbot / edge1.dx;
					ip.Y = Round((double)ip.X / edge1.dx + num2);
				}
			}
			else
			{
				double num2 = (double)edge1.xbot - (double)edge1.ybot * edge1.dx;
				double num = (double)edge2.xbot - (double)edge2.ybot * edge2.dx;
				double num3 = (num - num2) / (edge1.dx - edge2.dx);
				ip.Y = Round(num3);
				if (Math.Abs(edge1.dx) < Math.Abs(edge2.dx))
				{
					ip.X = Round(edge1.dx * num3 + num2);
				}
				else
				{
					ip.X = Round(edge2.dx * num3 + num);
				}
			}
			if (ip.Y < edge1.ytop || ip.Y < edge2.ytop)
			{
				if (edge1.ytop > edge2.ytop)
				{
					ip.X = edge1.xtop;
					ip.Y = edge1.ytop;
					return TopX(edge2, edge1.ytop) < edge1.xtop;
				}
				ip.X = edge2.xtop;
				ip.Y = edge2.ytop;
				return TopX(edge1, edge2.ytop) > edge2.xtop;
			}
			return true;
		}

		private void DisposeIntersectNodes()
		{
			while (m_IntersectNodes != null)
			{
				IntersectNode next = m_IntersectNodes.next;
				m_IntersectNodes = null;
				m_IntersectNodes = next;
			}
		}

		private void ProcessEdgesAtTopOfScanbeam(long topY)
		{
			TEdge e = m_ActiveEdges;
			while (e != null)
			{
				if (IsMaxima(e, topY) && GetMaximaPair(e).dx != -3.4E+38)
				{
					TEdge prevInAEL = e.prevInAEL;
					DoMaxima(e, topY);
					e = ((prevInAEL != null) ? prevInAEL.nextInAEL : m_ActiveEdges);
					continue;
				}
				if (IsIntermediate(e, topY) && e.nextInLML.dx == -3.4E+38)
				{
					if (e.outIdx >= 0)
					{
						AddOutPt(e, new IntPoint(e.xtop, e.ytop));
						for (int i = 0; i < m_HorizJoins.Count; i++)
						{
							IntPoint pt = default(IntPoint);
							IntPoint pt2 = default(IntPoint);
							HorzJoinRec horzJoinRec = m_HorizJoins[i];
							if (GetOverlapSegment(new IntPoint(horzJoinRec.edge.xbot, horzJoinRec.edge.ybot), new IntPoint(horzJoinRec.edge.xtop, horzJoinRec.edge.ytop), new IntPoint(e.nextInLML.xbot, e.nextInLML.ybot), new IntPoint(e.nextInLML.xtop, e.nextInLML.ytop), ref pt, ref pt2))
							{
								AddJoin(horzJoinRec.edge, e.nextInLML, horzJoinRec.savedIdx, e.outIdx);
							}
						}
						AddHorzJoin(e.nextInLML, e.outIdx);
					}
					UpdateEdgeIntoAEL(ref e);
					AddEdgeToSEL(e);
				}
				else
				{
					e.xcurr = TopX(e, topY);
					e.ycurr = topY;
				}
				e = e.nextInAEL;
			}
			ProcessHorizontals();
			for (e = m_ActiveEdges; e != null; e = e.nextInAEL)
			{
				if (IsIntermediate(e, topY))
				{
					if (e.outIdx >= 0)
					{
						AddOutPt(e, new IntPoint(e.xtop, e.ytop));
					}
					UpdateEdgeIntoAEL(ref e);
					TEdge prevInAEL2 = e.prevInAEL;
					TEdge nextInAEL = e.nextInAEL;
					if (prevInAEL2 != null && prevInAEL2.xcurr == e.xbot && prevInAEL2.ycurr == e.ybot && e.outIdx >= 0 && prevInAEL2.outIdx >= 0 && prevInAEL2.ycurr > prevInAEL2.ytop && SlopesEqual(e, prevInAEL2, m_UseFullRange))
					{
						AddOutPt(prevInAEL2, new IntPoint(e.xbot, e.ybot));
						AddJoin(e, prevInAEL2, -1, -1);
					}
					else if (nextInAEL != null && nextInAEL.xcurr == e.xbot && nextInAEL.ycurr == e.ybot && e.outIdx >= 0 && nextInAEL.outIdx >= 0 && nextInAEL.ycurr > nextInAEL.ytop && SlopesEqual(e, nextInAEL, m_UseFullRange))
					{
						AddOutPt(nextInAEL, new IntPoint(e.xbot, e.ybot));
						AddJoin(e, nextInAEL, -1, -1);
					}
				}
			}
		}

		private void DoMaxima(TEdge e, long topY)
		{
			TEdge maximaPair = GetMaximaPair(e);
			long xtop = e.xtop;
			for (TEdge nextInAEL = e.nextInAEL; nextInAEL != maximaPair; nextInAEL = nextInAEL.nextInAEL)
			{
				if (nextInAEL == null)
				{
					throw new ClipperException("DoMaxima error");
				}
				IntersectEdges(e, nextInAEL, new IntPoint(xtop, topY), Protects.ipBoth);
				SwapPositionsInAEL(e, nextInAEL);
			}
			if (e.outIdx < 0 && maximaPair.outIdx < 0)
			{
				DeleteFromAEL(e);
				DeleteFromAEL(maximaPair);
				return;
			}
			if (e.outIdx >= 0 && maximaPair.outIdx >= 0)
			{
				IntersectEdges(e, maximaPair, new IntPoint(xtop, topY), Protects.ipNone);
				return;
			}
			throw new ClipperException("DoMaxima error");
		}

		public static void ReversePolygons(List<List<IntPoint>> polys)
		{
			polys.ForEach(delegate(List<IntPoint> poly)
			{
				poly.Reverse();
			});
		}

		public static bool Orientation(List<IntPoint> poly)
		{
			return Area(poly) >= 0.0;
		}

		private int PointCount(OutPt pts)
		{
			if (pts == null)
			{
				return 0;
			}
			int num = 0;
			OutPt outPt = pts;
			do
			{
				num++;
				outPt = outPt.next;
			}
			while (outPt != pts);
			return num;
		}

		private void BuildResult(List<List<IntPoint>> polyg)
		{
			polyg.Clear();
			polyg.Capacity = m_PolyOuts.Count;
			for (int i = 0; i < m_PolyOuts.Count; i++)
			{
				OutRec outRec = m_PolyOuts[i];
				if (outRec.pts == null)
				{
					continue;
				}
				OutPt outPt = outRec.pts;
				int num = PointCount(outPt);
				if (num >= 3)
				{
					List<IntPoint> list = new List<IntPoint>(num);
					for (int j = 0; j < num; j++)
					{
						list.Add(outPt.pt);
						outPt = outPt.prev;
					}
					polyg.Add(list);
				}
			}
		}

		private void BuildResult2(PolyTree polytree)
		{
			polytree.Clear();
			polytree.m_AllPolys.Capacity = m_PolyOuts.Count;
			for (int i = 0; i < m_PolyOuts.Count; i++)
			{
				OutRec outRec = m_PolyOuts[i];
				int num = PointCount(outRec.pts);
				if (num >= 3)
				{
					FixHoleLinkage(outRec);
					PolyNode polyNode = new PolyNode();
					polytree.m_AllPolys.Add(polyNode);
					outRec.polyNode = polyNode;
					polyNode.m_polygon.Capacity = num;
					OutPt outPt = outRec.pts;
					for (int j = 0; j < num; j++)
					{
						polyNode.m_polygon.Add(outPt.pt);
						outPt = outPt.prev;
					}
				}
			}
			polytree.m_Childs.Capacity = m_PolyOuts.Count;
			for (int k = 0; k < m_PolyOuts.Count; k++)
			{
				OutRec outRec2 = m_PolyOuts[k];
				if (outRec2.polyNode != null)
				{
					if (outRec2.FirstLeft == null)
					{
						outRec2.polyNode.m_Index = polytree.m_Childs.Count;
						polytree.m_Childs.Add(outRec2.polyNode);
						outRec2.polyNode.m_Parent = polytree;
					}
					else
					{
						outRec2.FirstLeft.polyNode.AddChild(outRec2.polyNode);
					}
				}
			}
		}

		private void FixupOutPolygon(OutRec outRec)
		{
			OutPt outPt = null;
			outRec.pts = outRec.bottomPt;
			OutPt outPt2 = outRec.bottomPt;
			while (true)
			{
				if (outPt2.prev == outPt2 || outPt2.prev == outPt2.next)
				{
					DisposeOutPts(outPt2);
					outRec.pts = null;
					outRec.bottomPt = null;
					return;
				}
				if (ClipperBase.PointsEqual(outPt2.pt, outPt2.next.pt) || SlopesEqual(outPt2.prev.pt, outPt2.pt, outPt2.next.pt, m_UseFullRange))
				{
					outPt = null;
					if (outPt2 == outRec.bottomPt)
					{
						outRec.bottomPt = null;
					}
					outPt2.prev.next = outPt2.next;
					outPt2.next.prev = outPt2.prev;
					outPt2 = outPt2.prev;
				}
				else
				{
					if (outPt2 == outPt)
					{
						break;
					}
					if (outPt == null)
					{
						outPt = outPt2;
					}
					outPt2 = outPt2.next;
				}
			}
			if (outRec.bottomPt == null)
			{
				outRec.bottomPt = GetBottomPt(outPt2);
				outRec.bottomPt.idx = outRec.idx;
				outRec.pts = outRec.bottomPt;
			}
		}

		private bool JoinPoints(JoinRec j, out OutPt p1, out OutPt p2)
		{
			p1 = null;
			p2 = null;
			OutRec outRec = m_PolyOuts[j.poly1Idx];
			OutRec outRec2 = m_PolyOuts[j.poly2Idx];
			if (outRec == null || outRec2 == null)
			{
				return false;
			}
			OutPt pp = outRec.pts;
			OutPt pp2 = outRec2.pts;
			IntPoint pt = j.pt2a;
			IntPoint pt2 = j.pt2b;
			IntPoint pt3 = j.pt1a;
			IntPoint pt4 = j.pt1b;
			if (!FindSegment(ref pp, ref pt, ref pt2))
			{
				return false;
			}
			if (outRec == outRec2)
			{
				pp2 = pp.next;
				if (!FindSegment(ref pp2, ref pt3, ref pt4) || pp2 == pp)
				{
					return false;
				}
			}
			else if (!FindSegment(ref pp2, ref pt3, ref pt4))
			{
				return false;
			}
			if (!GetOverlapSegment(pt, pt2, pt3, pt4, ref pt, ref pt2))
			{
				return false;
			}
			OutPt prev = pp.prev;
			if (ClipperBase.PointsEqual(pp.pt, pt))
			{
				p1 = pp;
			}
			else if (ClipperBase.PointsEqual(prev.pt, pt))
			{
				p1 = prev;
			}
			else
			{
				p1 = InsertPolyPtBetween(pp, prev, pt);
			}
			if (ClipperBase.PointsEqual(pp.pt, pt2))
			{
				p2 = pp;
			}
			else if (ClipperBase.PointsEqual(prev.pt, pt2))
			{
				p2 = prev;
			}
			else if (p1 == pp || p1 == prev)
			{
				p2 = InsertPolyPtBetween(pp, prev, pt2);
			}
			else if (Pt3IsBetweenPt1AndPt2(pp.pt, p1.pt, pt2))
			{
				p2 = InsertPolyPtBetween(pp, p1, pt2);
			}
			else
			{
				p2 = InsertPolyPtBetween(p1, prev, pt2);
			}
			prev = pp2.prev;
			OutPt outPt = (ClipperBase.PointsEqual(pp2.pt, pt) ? pp2 : ((!ClipperBase.PointsEqual(prev.pt, pt)) ? InsertPolyPtBetween(pp2, prev, pt) : prev));
			OutPt outPt2 = (ClipperBase.PointsEqual(pp2.pt, pt2) ? pp2 : (ClipperBase.PointsEqual(prev.pt, pt2) ? prev : ((outPt == pp2 || outPt == prev) ? InsertPolyPtBetween(pp2, prev, pt2) : ((!Pt3IsBetweenPt1AndPt2(pp2.pt, outPt.pt, pt2)) ? InsertPolyPtBetween(outPt, prev, pt2) : InsertPolyPtBetween(pp2, outPt, pt2)))));
			if (p1.next == p2 && outPt.prev == outPt2)
			{
				p1.next = outPt;
				outPt.prev = p1;
				p2.prev = outPt2;
				outPt2.next = p2;
				return true;
			}
			if (p1.prev == p2 && outPt.next == outPt2)
			{
				p1.prev = outPt;
				outPt.next = p1;
				p2.next = outPt2;
				outPt2.prev = p2;
				return true;
			}
			return false;
		}

		private void FixupJoinRecs(JoinRec j, OutPt pt, int startIdx)
		{
			for (int i = startIdx; i < m_Joins.Count; i++)
			{
				JoinRec joinRec = m_Joins[i];
				if (joinRec.poly1Idx == j.poly1Idx && PointIsVertex(joinRec.pt1a, pt))
				{
					joinRec.poly1Idx = j.poly2Idx;
				}
				if (joinRec.poly2Idx == j.poly1Idx && PointIsVertex(joinRec.pt2a, pt))
				{
					joinRec.poly2Idx = j.poly2Idx;
				}
			}
		}

		private bool Poly2ContainsPoly1(OutPt outPt1, OutPt outPt2, bool UseFullInt64Range)
		{
			OutPt outPt3 = outPt1;
			while (PointIsVertex(outPt3.pt, outPt2))
			{
				outPt3 = outPt3.next;
				if (outPt3 == outPt1)
				{
					break;
				}
			}
			bool flag;
			do
			{
				flag = PointInPolygon(outPt3.pt, outPt2, UseFullInt64Range);
				outPt3 = outPt3.next;
			}
			while (flag && outPt3 != outPt1);
			return flag;
		}

		private void FixupFirstLefts1(OutRec OldOutRec, OutRec NewOutRec)
		{
			for (int i = 0; i < m_PolyOuts.Count; i++)
			{
				OutRec outRec = m_PolyOuts[i];
				if (outRec.pts != null && outRec.FirstLeft == OldOutRec && Poly2ContainsPoly1(outRec.pts, NewOutRec.pts, m_UseFullRange))
				{
					outRec.FirstLeft = NewOutRec;
				}
			}
		}

		private void FixupFirstLefts2(OutRec OldOutRec, OutRec NewOutRec)
		{
			foreach (OutRec polyOut in m_PolyOuts)
			{
				if (polyOut.FirstLeft == OldOutRec)
				{
					polyOut.FirstLeft = NewOutRec;
				}
			}
		}

		private void JoinCommonEdges()
		{
			for (int i = 0; i < m_Joins.Count; i++)
			{
				JoinRec joinRec = m_Joins[i];
				OutRec outRec = m_PolyOuts[joinRec.poly1Idx];
				OutRec outRec2 = m_PolyOuts[joinRec.poly2Idx];
				if (outRec.pts == null || outRec2.pts == null)
				{
					continue;
				}
				OutRec outRec3 = ((outRec == outRec2) ? outRec : (Param1RightOfParam2(outRec, outRec2) ? outRec2 : ((!Param1RightOfParam2(outRec2, outRec)) ? GetLowermostRec(outRec, outRec2) : outRec)));
				if (!JoinPoints(joinRec, out var p, out var p2))
				{
					continue;
				}
				if (outRec == outRec2)
				{
					outRec.pts = GetBottomPt(p);
					outRec.bottomPt = outRec.pts;
					outRec.bottomPt.idx = outRec.idx;
					outRec2 = CreateOutRec();
					m_PolyOuts.Add(outRec2);
					outRec2.idx = m_PolyOuts.Count - 1;
					joinRec.poly2Idx = outRec2.idx;
					outRec2.pts = GetBottomPt(p2);
					outRec2.bottomPt = outRec2.pts;
					outRec2.bottomPt.idx = outRec2.idx;
					if (Poly2ContainsPoly1(outRec2.pts, outRec.pts, m_UseFullRange))
					{
						outRec2.isHole = !outRec.isHole;
						outRec2.FirstLeft = outRec;
						FixupJoinRecs(joinRec, p2, i + 1);
						if (m_UsingPolyTree)
						{
							FixupFirstLefts2(outRec2, outRec);
						}
						FixupOutPolygon(outRec);
						FixupOutPolygon(outRec2);
						if ((outRec2.isHole ^ m_ReverseOutput) == Area(outRec2, m_UseFullRange) > 0.0)
						{
							ReversePolyPtLinks(outRec2.pts);
						}
					}
					else if (Poly2ContainsPoly1(outRec.pts, outRec2.pts, m_UseFullRange))
					{
						outRec2.isHole = outRec.isHole;
						outRec.isHole = !outRec2.isHole;
						outRec2.FirstLeft = outRec.FirstLeft;
						outRec.FirstLeft = outRec2;
						FixupJoinRecs(joinRec, p2, i + 1);
						if (m_UsingPolyTree)
						{
							FixupFirstLefts2(outRec, outRec2);
						}
						FixupOutPolygon(outRec);
						FixupOutPolygon(outRec2);
						if ((outRec.isHole ^ m_ReverseOutput) == Area(outRec, m_UseFullRange) > 0.0)
						{
							ReversePolyPtLinks(outRec.pts);
						}
					}
					else
					{
						outRec2.isHole = outRec.isHole;
						outRec2.FirstLeft = outRec.FirstLeft;
						FixupJoinRecs(joinRec, p2, i + 1);
						if (m_UsingPolyTree)
						{
							FixupFirstLefts1(outRec, outRec2);
						}
						FixupOutPolygon(outRec);
						FixupOutPolygon(outRec2);
					}
					continue;
				}
				FixupOutPolygon(outRec);
				int idx = outRec.idx;
				int idx2 = outRec2.idx;
				outRec2.pts = null;
				outRec2.bottomPt = null;
				outRec.isHole = outRec3.isHole;
				if (outRec3 == outRec2)
				{
					outRec.FirstLeft = outRec2.FirstLeft;
				}
				outRec2.FirstLeft = outRec;
				for (int j = i + 1; j < m_Joins.Count; j++)
				{
					JoinRec joinRec2 = m_Joins[j];
					if (joinRec2.poly1Idx == idx2)
					{
						joinRec2.poly1Idx = idx;
					}
					if (joinRec2.poly2Idx == idx2)
					{
						joinRec2.poly2Idx = idx;
					}
				}
				if (m_UsingPolyTree)
				{
					FixupFirstLefts2(outRec2, outRec);
				}
			}
		}

		private static bool FullRangeNeeded(List<IntPoint> pts)
		{
			bool result = false;
			for (int i = 0; i < pts.Count; i++)
			{
				if (Math.Abs(pts[i].X) > 4611686018427387903L || Math.Abs(pts[i].Y) > 4611686018427387903L)
				{
					throw new ClipperException("Coordinate exceeds range bounds.");
				}
				if (Math.Abs(pts[i].X) > 1073741823 || Math.Abs(pts[i].Y) > 1073741823)
				{
					result = true;
				}
			}
			return result;
		}

		public static double Area(List<IntPoint> poly)
		{
			int num = poly.Count - 1;
			if (num < 2)
			{
				return 0.0;
			}
			if (FullRangeNeeded(poly))
			{
				Int128 @int = new Int128(0L);
				@int = Int128.Int128Mul(poly[num].X + poly[0].X, poly[0].Y - poly[num].Y);
				for (int i = 1; i <= num; i++)
				{
					@int += Int128.Int128Mul(poly[i - 1].X + poly[i].X, poly[i].Y - poly[i - 1].Y);
				}
				return @int.ToDouble() / 2.0;
			}
			double num2 = ((double)poly[num].X + (double)poly[0].X) * ((double)poly[0].Y - (double)poly[num].Y);
			for (int j = 1; j <= num; j++)
			{
				num2 += ((double)poly[j - 1].X + (double)poly[j].X) * ((double)poly[j].Y - (double)poly[j - 1].Y);
			}
			return num2 / 2.0;
		}

		private double Area(OutRec outRec, bool UseFull64BitRange)
		{
			OutPt outPt = outRec.pts;
			if (outPt == null)
			{
				return 0.0;
			}
			if (UseFull64BitRange)
			{
				Int128 @int = new Int128(0L);
				do
				{
					@int += Int128.Int128Mul(outPt.pt.X + outPt.prev.pt.X, outPt.prev.pt.Y - outPt.pt.Y);
					outPt = outPt.next;
				}
				while (outPt != outRec.pts);
				return @int.ToDouble() / 2.0;
			}
			double num = 0.0;
			do
			{
				num += (double)((outPt.pt.X + outPt.prev.pt.X) * (outPt.prev.pt.Y - outPt.pt.Y));
				outPt = outPt.next;
			}
			while (outPt != outRec.pts);
			return num / 2.0;
		}

		internal static List<IntPoint> BuildArc(IntPoint pt, double a1, double a2, double r)
		{
			long num = Math.Max(6, (int)(Math.Sqrt(Math.Abs(r)) * Math.Abs(a2 - a1)));
			if (num > 256)
			{
				num = 256L;
			}
			int num2 = (int)num;
			List<IntPoint> list = new List<IntPoint>(num2);
			double num3 = (a2 - a1) / (double)(num2 - 1);
			double num4 = a1;
			for (int i = 0; i < num2; i++)
			{
				list.Add(new IntPoint(pt.X + Round(Math.Cos(num4) * r), pt.Y + Round(Math.Sin(num4) * r)));
				num4 += num3;
			}
			return list;
		}

		internal static DoublePoint GetUnitNormal(IntPoint pt1, IntPoint pt2)
		{
			double num = pt2.X - pt1.X;
			double num2 = pt2.Y - pt1.Y;
			if (num == 0.0 && num2 == 0.0)
			{
				return new DoublePoint();
			}
			double num3 = 1.0 / Math.Sqrt(num * num + num2 * num2);
			num *= num3;
			num2 *= num3;
			return new DoublePoint(num2, 0.0 - num);
		}

		public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> poly, double delta, JoinType jointype, double MiterLimit, bool AutoFix)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>(poly.Count);
			new PolyOffsetBuilder(poly, list, delta, jointype, MiterLimit, AutoFix);
			return list;
		}

		public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> poly, double delta, JoinType jointype, double MiterLimit)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>(poly.Count);
			new PolyOffsetBuilder(poly, list, delta, jointype, MiterLimit);
			return list;
		}

		public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> poly, double delta, JoinType jointype)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>(poly.Count);
			new PolyOffsetBuilder(poly, list, delta, jointype);
			return list;
		}

		public static List<List<IntPoint>> OffsetPolygons(List<List<IntPoint>> poly, double delta)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>(poly.Count);
			new PolyOffsetBuilder(poly, list, delta, JoinType.jtSquare);
			return list;
		}

		public static List<List<IntPoint>> SimplifyPolygon(List<IntPoint> poly, PolyFillType fillType = PolyFillType.pftEvenOdd)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			Clipper clipper = new Clipper();
			clipper.AddPolygon(poly, PolyType.ptSubject);
			clipper.Execute(ClipType.ctUnion, list, fillType, fillType);
			return list;
		}

		public static List<List<IntPoint>> SimplifyPolygons(List<List<IntPoint>> polys, PolyFillType fillType = PolyFillType.pftEvenOdd)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			Clipper clipper = new Clipper();
			clipper.AddPolygons(polys, PolyType.ptSubject);
			clipper.Execute(ClipType.ctUnion, list, fillType, fillType);
			return list;
		}

		public static List<IntPoint> CleanPolygon(List<IntPoint> poly, double delta = 1.415)
		{
			int count = poly.Count;
			if (count < 3)
			{
				return null;
			}
			List<IntPoint> list = new List<IntPoint>(poly);
			int num = (int)(delta * delta);
			IntPoint intPoint = poly[0];
			int num2 = 1;
			for (int i = 1; i < count; i++)
			{
				if ((poly[i].X - intPoint.X) * (poly[i].X - intPoint.X) + (poly[i].Y - intPoint.Y) * (poly[i].Y - intPoint.Y) > num)
				{
					list[num2] = poly[i];
					intPoint = poly[i];
					num2++;
				}
			}
			intPoint = poly[num2 - 1];
			if ((poly[0].X - intPoint.X) * (poly[0].X - intPoint.X) + (poly[0].Y - intPoint.Y) * (poly[0].Y - intPoint.Y) <= num)
			{
				num2--;
			}
			if (num2 < count)
			{
				list.RemoveRange(num2, count - num2);
			}
			return list;
		}

		public static void PolyTreeToPolygons(PolyTree polytree, List<List<IntPoint>> polygons)
		{
			polygons.Clear();
			polygons.Capacity = polytree.Total;
			AddPolyNodeToPolygons(polytree, polygons);
		}

		public static void AddPolyNodeToPolygons(PolyNode polynode, List<List<IntPoint>> polygons)
		{
			if (polynode.Contour.Count > 0)
			{
				polygons.Add(polynode.Contour);
			}
			foreach (PolyNode child in polynode.Childs)
			{
				AddPolyNodeToPolygons(child, polygons);
			}
		}
	}
}
