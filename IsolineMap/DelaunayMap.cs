using System;
using System.Collections.Generic;
using System.Linq;

namespace IsolineMap
{
	/// <summary>
	/// ドロネー三角形分割を行うクラス
	/// </summary>
	public class DelaunayMap
	{
		/// <summary>
		/// ドロネー三角図の三角形データ。
		/// </summary>
		public HashSet<Triangle> Triangles { get; } = new HashSet<Triangle>();

		/// <summary>
		/// 標点マップ。
		/// </summary>
		public HeightMap HeightMap { get; private set; }

		/// <summary>
		/// ドロネー三角形分割を行うクラスをインスタンス化します。
		/// </summary>
		/// <param name="heightMap"></param>
		public DelaunayMap(HeightMap heightMap)
		{
			HeightMap = heightMap;
		}

		/// <summary>
		/// 標点データからドロネー三角図を作成します。
		/// </summary>
		public void Create()
		{
			var hugeTriangle = Triangle.GetHugeTriangle(
				new Vector(HeightMap.Marks.Max(a => a.X), HeightMap.Marks.Max(a => a.Y)),
				new Vector(HeightMap.Marks.Min(a => a.X), HeightMap.Marks.Min(a => a.Y)));
			Triangles.Add(hugeTriangle);

			foreach (var point in HeightMap.Marks)
			{
				var edgeStack = new Stack<Edge>();

				// 追加する点が含まれる三角形を積む
				// 点で三角形を分割し、元の三角形を削除する
				var tmpTriangle = Triangles.First(t => t.IsIncluding(point));
				foreach (var a in tmpTriangle.Edges)
				{
					edgeStack.Push(a);
					AddNewTriangle(point, a);
				}
				Triangles.Remove(tmpTriangle);

				while (edgeStack.Any())
				{
					Edge edge = edgeStack.Pop();

					var commonEdgeTriangles = Triangles.Where(a => a.Edges.Any(b => b.Equals(edge))).ToArray();

					if (commonEdgeTriangles.Length == 2)
					{
						Triangle triangle_ABC = commonEdgeTriangles[0];
						Triangle triangle_ABD = commonEdgeTriangles[1];

						if (triangle_ABC.Equals(triangle_ABD))
						{
							Triangles.Remove(triangle_ABC);
							Triangles.Remove(triangle_ABD);
							continue;
						}

						Vector point_A = edge.Point1;
						Vector point_B = edge.Point2;
						Vector point_C = triangle_ABC.GetOppositeVertex(edge);
						Vector point_D = triangle_ABD.GetOppositeVertex(edge);

						// 外接円内の三角形の場合は共有する辺を入れ替え
						if (triangle_ABC.GetCircumscribedCircle().Includes(point_D))
						{
							Triangles.Remove(triangle_ABC);
							Triangles.Remove(triangle_ABD);

							var newEdge = new Edge(point_C, point_D);

							Triangles.Add(new Triangle(newEdge, triangle_ABC.GetOppositeEdge(point_B), triangle_ABD.GetOppositeEdge(point_B)));
							Triangles.Add(new Triangle(newEdge, triangle_ABC.GetOppositeEdge(point_A), triangle_ABD.GetOppositeEdge(point_A)));

							// 上記三角形の辺をedge stackに追加
							foreach (Edge tmpEdge in commonEdgeTriangles.SelectMany(a => a.Edges).Where(a => !a.Equals(edge)))
								edgeStack.Push(tmpEdge);
						}
					}
				}
			}

			// 外郭三角形の各辺を含む三角形を削除
			foreach (var triangle in Triangles.Where(t => t.HasCommonPoints(hugeTriangle)).ToArray())
			{
				Triangles.Remove(triangle);
			}
		}

		/// <summary>
		/// 指定された<see cref="Vector"/>と<see cref="Edge"/>から<see cref="Triangle"/>を生成します。
		/// </summary>
		/// <param name="point"></param>
		/// <param name="edge"></param>
		private void AddNewTriangle(Vector point, Edge edge)
		{
			// pointがedgeの直線上にないことをチェック
			if (Vector.Cross(edge.Point1 - point, edge.Point2 - point) == 0) return;

			var edge1 = new Edge(edge.Point2, point);
			var edge2 = new Edge(point, edge.Point1);

			Triangles.Add(new Triangle(edge, edge1, edge2));
		}
	}
}