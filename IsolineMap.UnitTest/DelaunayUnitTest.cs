using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IsolineMap;
using System.Diagnostics;
using System.IO;

namespace IsolineMap.UnitTest
{
	[TestClass]
	public class DelaunayUnitTest
	{
		private readonly string testFilePath = "testdata.csv";

		[TestInitialize]
		public void Initialize()
		{
			// 標点データのサンプルを作成する

			// 設定値：幅(X)、奥行き(Y)、高低差(Z)、評点数
			const int width = 300, depth = 300, heightDiff = 100, ptCount = 1000;

			var rand = new Random();
			var dict = new Dictionary<Vector, float>();

			for(int i=0; i < ptCount; i++)
			{
				// 位置を一様分布で出力
				var x = rand.Next(-width / 2, width / 2) + rand.NextDouble() - 0.5;
				var y = rand.Next(-depth / 2, depth / 2) + rand.NextDouble() - 0.5;

				// 高さを正規分布で出力
				var z = Math.Sqrt(-2 * Math.Log(rand.NextDouble())) * Math.Cos(2 * Math.PI * rand.NextDouble()) * heightDiff;

				var v = new Vector((float)x, (float)y);
				if (dict.ContainsKey(v) || Math.Abs(x) > width / 2 || Math.Abs(y) > depth / 2)
				{
					i--;
				}
				else
				{
					dict.Add(v, (float)z);
				}
			}

			Console.WriteLine("{0}～{1}", dict.Values.Min(), dict.Values.Max());

			using (var sw = new StreamWriter(testFilePath, false))
			{
				foreach (var pair in dict)
				{
					sw.WriteLine($"{pair.Key.X},{pair.Key.Y},{pair.Value}");
				}
			}
		}

		[TestMethod]
		public void DrawImage()
		{
			var heights = new HeightMap();
			var delaunay = new DelaunayMap(heights);
			var contour = new ContourMap(delaunay);

			var timer = new Stopwatch();

			timer.Start();
			heights.LoadFile(testFilePath);
			timer.Stop();
			Console.WriteLine(timer.Elapsed);

			timer.Reset();
			timer.Start();
			delaunay.Create();
			timer.Stop();
			Console.WriteLine(timer.Elapsed);

			timer.Reset();
			timer.Start();
			contour.Create(10);
			timer.Stop();
			Console.WriteLine(timer.Elapsed);

			var margin = 20;
			var img = new Bitmap(1000, 500);
			var ghRect = new RectangleF(margin, margin, img.Width - margin * 2, img.Height - margin * 2);
			var rlRect = new RectangleF(new PointF(heights.Marks.Min(a => a.X), heights.Marks.Min(a => a.Y)), 
				new SizeF(heights.Marks.Max(a => a.X) - heights.Marks.Min(a => a.X), heights.Marks.Max(a => a.Y) - heights.Marks.Min(a => a.Y)));

			using (var g = Graphics.FromImage(img))
			{
				g.FillRectangle(Brushes.White, g.VisibleClipBounds);

				delaunay.Draw(g, rlRect, ghRect);

				contour.Draw(g, rlRect, ghRect);

				heights.Draw(g, rlRect, ghRect);
			}

			img.Save("result.png");
		}
	}

	/// <summary>
	/// イメージ描画用の拡張クラス
	/// </summary>
	public static class MyExtentions
	{
		/// <summary>
		/// <see cref="HeightMap"/>内の標点データをプロットします。
		/// </summary>
		/// <param name="self">標点マップ</param>
		/// <param name="g">描画対象の<see cref="Graphics"/></param>
		/// <param name="realRect">標点データの存在する領域</param>
		/// <param name="drawRect">描画対象となる領域</param>
		public static void Draw(this HeightMap self, Graphics g, RectangleF realRect, RectangleF drawRect)
		{
			foreach (var vec in self.Marks.Select(a=>a.ToPointF().Transform(realRect, drawRect)))
			{
				g.FillEllipse(Brushes.Red, vec.X - 1, vec.Y - 1, 2, 2);
			}
		}

		/// <summary>
		/// <see cref="DelaunayMap"/>内のドロネー三角図を描画します。
		/// </summary>
		/// <param name="self">ドロネー三角図データ</param>
		/// <param name="g">描画対象の<see cref="Graphics"/></param>
		/// <param name="realRect">標点データの存在する領域</param>
		/// <param name="drawRect">描画対象となる領域</param>
		public static void Draw(this DelaunayMap self, Graphics g, RectangleF realRect, RectangleF drawRect)
		{
			foreach (var edge in self.Triangles.SelectMany(a => a.Edges).Distinct())
			{
				g.DrawLine(Pens.LightGray, edge.Point1.ToPointF().Transform(realRect, drawRect), edge.Point2.ToPointF().Transform(realRect, drawRect));
			}
		}

		/// <summary>
		/// <see cref="ContourMap"/>内の等高線データを描画します。
		/// </summary>
		/// <param name="contour">等高線マップ</param>
		/// <param name="g">描画対象の<see cref="Graphics"/></param>
		/// <param name="realRect">標点データの存在する領域</param>
		/// <param name="drawRect">描画対象となる領域</param>
		public static void Draw(this ContourMap contour, Graphics g, RectangleF realRect, RectangleF drawRect)
		{
			if (contour.DataPoints.Count > 0)
			{
				foreach (var triangle in contour.DelaunayMap.Triangles)
				{
					var includeData = contour.DataPoints.Where(a => triangle.Edges.Any(b => a.Key.Equals(b))).SelectMany(a => a.Value);

					foreach (var date in includeData.Select(a => a.Key).Distinct())
					{
						var points2 = includeData.Where(a => a.Key == date).Select(a => a.Value).ToArray();

						if (points2.Length > 1 && date > contour.DelaunayMap.HeightMap.MinHeight)
						{
							var ratio = (date - contour.DelaunayMap.HeightMap.MinHeight) / (contour.DelaunayMap.HeightMap.MaxHeight - contour.DelaunayMap.HeightMap.MinHeight);
							var color = Color.FromArgb((int)(255 * ratio), 0, (int)(255 * (1 - ratio)));

							g.DrawLines(new Pen(new SolidBrush(color)), points2.Select(a => a.ToPointF().Transform(realRect,drawRect)).ToArray());
						}
					}
				}
			}
		}

		/// <summary>
		/// 現実領域の<see cref="PointF"/>から描画領域の<see cref="PointF"/>へ変換します。
		/// </summary>
		/// <param name="realPt">現実領域の標点データの<see cref="PointF"/></param>
		/// <param name="realRect">標点データの存在する領域</param>
		/// <param name="drawRect">描画対象となる領域</param>
		/// <returns>描画領域にプロットする標点データ</returns>
		public static PointF Transform(this PointF realPt, RectangleF realRect, RectangleF drawRect)
		{
			var x = drawRect.Left + (realPt.X - realRect.Location.X) * drawRect.Width / realRect.Width;
			var y = drawRect.Top + (realPt.Y - realRect.Location.Y) * drawRect.Height / realRect.Height;

			return new PointF(x, y);
		}

		/// <summary>
		/// <see cref="Vector"/>を<see cref="PointF"/>に変換します。
		/// </summary>
		/// <param name="vec">変換する<see cref="Vector"/></param>
		/// <returns>変換後の<see cref="PointF"/></returns>
		public static PointF ToPointF(this Vector vec)
		{
			return new PointF(vec.X, vec.Y);
		}
	}
}