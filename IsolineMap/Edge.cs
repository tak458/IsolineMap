using System;
using System.Collections.Generic;

namespace IsolineMap
{
	public class Edge : IEquatable<Edge>, IComparable
	{
		/// <summary>
		/// 指定した点から辺を定義します。
		/// </summary>
		/// <param name="start">点1</param>
		/// <param name="end">点2</param>
		public Edge(Vector start, Vector end)
		{
			if (start.GetHashCode() < end.GetHashCode())
			{
				Point1 = start;
				Point2 = end;
			}
			else
			{
				Point1 = end;
				Point2 = start;
			}
		}

		public Vector Point1 { get; private set; }

		public Vector Point2 { get; private set; }

		public IEnumerable<Vector> Points
		{
			get
			{
				yield return Point1;
				yield return Point2;
			}
		}

		public int CompareTo(object obj)
		{
			return GetHashCode() - obj.GetHashCode();
		}

		public bool Equals(Edge other)
		{
			return Point1.Equals(other.Point1) && Point2.Equals(other.Point2);
		}

		public override bool Equals(object obj)
		{
			if (obj != null || obj is Edge)
			{
				var edge = obj as Edge;
				return Equals(edge);
			}
			return false;
		}

		public override int GetHashCode()
		{
			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				// Suitable nullity checks etc, of course :)
				hash = hash * 23 + Point1.GetHashCode();
				hash = hash * 23 + Point2.GetHashCode();
				return hash;
			}
		}
		public override string ToString()
		{
			return string.Format("(P1:{0} P2:{1})", Point1, Point2);
		}
	}
}