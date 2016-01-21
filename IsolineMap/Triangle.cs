using System;
using System.Collections.Generic;
using System.Linq;

namespace IsolineMap
{
	/// <summary>
	/// 三角形
	/// </summary>
	public class Triangle : IEquatable<Triangle>, IDisposable
	{
		/// <summary>
		/// 指定した３つの辺から三角形データを生成します。
		/// </summary>
		/// <param name="e1">辺1</param>
		/// <param name="e2">辺2</param>
		/// <param name="e3">辺3</param>
		public Triangle(Edge e1, Edge e2, Edge e3)
		{
			var ary = new List<Edge>() { e1, e2, e3 };
			ary.Sort();

			Edge1 = ary[0];
			Edge2 = ary[1];
			Edge3 = ary[2];

			var vetices = Edges.SelectMany(a => a.Points).Distinct().ToArray();

			if (vetices.Length != 3)
				throw new ArgumentException();

			// 対辺・対頂点は添字が同じもの
			Vertex1 = vetices.First(a => !Edge1.Points.Any(a.Equals));
			Vertex2 = vetices.First(a => !Edge2.Points.Any(a.Equals));
			Vertex3 = vetices.First(a => !Edge3.Points.Any(a.Equals));
		}

		public Vector Vertex1 { get; private set; }
		public Vector Vertex2 { get; private set; }
		public Vector Vertex3 { get; private set; }

		public Edge Edge1 { get; private set; }
		public Edge Edge2 { get; private set; }
		public Edge Edge3 { get; private set; }

		public IEnumerable<Vector> Vertices
		{
			get
			{
				yield return Vertex1;
				yield return Vertex2;
				yield return Vertex3;
			}
		}

		public IEnumerable<Edge> Edges
		{
			get
			{
				yield return Edge1;
				yield return Edge2;
				yield return Edge3;
			}
		}

		public bool Equals(Triangle other)
		{
			return Vertex1.Equals(other.Vertex1) && Vertex2.Equals(other.Vertex2) && Vertex3.Equals(other.Vertex3);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Triangle))
				return false;

			return Equals(obj as Triangle);
		}

		public override int GetHashCode()
		{
			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				// Suitable nullity checks etc, of course :)
				hash = hash * 23 + Edge1.GetHashCode();
				hash = hash * 23 + Edge2.GetHashCode();
				hash = hash * 23 + Edge3.GetHashCode();
				return hash;
			}
		}

		/// <summary>
		/// 指定した<see cref="Triangle"/>が共有する頂点を持つか判定します。
		/// </summary>
		/// <param name="t">チェックする三角形</param>
		/// <returns></returns>
		public bool HasCommonPoints(Triangle t)
		{
			return Vertex1.Equals(t.Vertex1) || Vertex1.Equals(t.Vertex2) || Vertex1.Equals(t.Vertex3) ||
				   Vertex2.Equals(t.Vertex1) || Vertex2.Equals(t.Vertex2) || Vertex2.Equals(t.Vertex3) ||
				   Vertex3.Equals(t.Vertex1) || Vertex3.Equals(t.Vertex2) || Vertex3.Equals(t.Vertex3);
		}

		/// <summary>
		/// 外接円を取得します。
		/// </summary>
		/// <returns></returns>
		public Circle GetCircumscribedCircle()
		{
			var a2 = (Vertex2 - Vertex3).LengthSq();
			var b2 = (Vertex3 - Vertex1).LengthSq();
			var c2 = (Vertex1 - Vertex2).LengthSq();
			var a = a2 * (b2 + c2 - a2);
			var b = b2 * (c2 + a2 - b2);
			var c = c2 * (a2 + b2 - c2);

			Vector center = (a * Vertex1 + b * Vertex2 + c * Vertex3) * (1.0f / (a + b + c));

			return new Circle(center, (center - Vertex1).Length());
		}

		/// <summary>
		/// 対頂点を取得します。
		/// </summary>
		/// <param name="edge">この三角形にある辺。</param>
		/// <returns></returns>
		public Vector GetOppositeVertex(Edge edge)
		{
			if (Edge1.Equals(edge)) return Vertex1;
			if (Edge2.Equals(edge)) return Vertex2;
			if (Edge3.Equals(edge)) return Vertex3;

			throw new ArgumentException();
		}

		/// <summary>
		/// 対辺を取得します。
		/// </summary>
		/// <param name="edge">この三角形にある点。</param>
		/// <returns></returns>
		public Edge GetOppositeEdge(Vector point)
		{
			if (Vertex1.Equals(point)) return Edge1;
			if (Vertex2.Equals(point)) return Edge2;
			if (Vertex3.Equals(point)) return Edge3;

			throw new ArgumentException();
		}

		/// <summary>
		/// 指定した<see cref="Vector"/>がこの三角形内にあるか判定します。
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public bool IsIncluding(Vector point)
		{
			float c1 = Vector.Cross(Vertex2 - Vertex1, point - Vertex1);
			float c2 = Vector.Cross(Vertex3 - Vertex2, point - Vertex2);
			float c3 = Vector.Cross(Vertex1 - Vertex3, point - Vertex3);

			return (c1 >= 0 && c2 >= 0 && c3 >= 0) || (c1 <= 0 && c2 <= 0 && c3 <= 0);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Triangle()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Edge1 = null;
				Edge2 = null;
				Edge3 = null;
			}
		}

		/// <summary>
		///     任意の矩形を包含する正三角形を求める
		/// </summary>
		/// <param name="start">矩形の左上座標</param>
		/// <param name="end">矩形の右下座標</param>
		/// <returns></returns>
		public static Triangle GetHugeTriangle(Vector start, Vector end)
		{
			// 1) 与えられた矩形を包含する円を求める  
			//      円の中心 c = 矩形の中心  
			//      円の半径 r = |p - c| + ρ  
			//    ただし、pは与えられた矩形の任意の頂点  
			//    ρは任意の正数  
			Vector center = (start + end) * 0.5f;
			float radius = (center - start).Length() + 1.0f;

			// 2) その円に外接する正三角形を求める  
			//    重心は、円の中心に等しい  
			//    一辺の長さは 2√3･r  
			Vector vt1 = center + new Vector(+(float)Math.Sqrt(3), -1) * radius;
			Vector vt2 = center + new Vector(-(float)Math.Sqrt(3), -1) * radius;
			Vector vt3 = center + new Vector(0, 2 * radius);

			return new Triangle(new Edge(vt1, vt2), new Edge(vt2, vt3), new Edge(vt3, vt1));
		}
	}
}