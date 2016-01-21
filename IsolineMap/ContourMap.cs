using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsolineMap;

namespace IsolineMap
{
	/// <summary>
	/// 等高線データを作成するクラス
	/// </summary>
	public class ContourMap
	{
		/// <summary>
		/// ドロネー三角図
		/// </summary>
		public DelaunayMap DelaunayMap { get; private set; }

		/// <summary>
		/// 等高線データ
		/// </summary>
		public Dictionary<Edge, Dictionary<float, Vector>> DataPoints { get; private set; }

		/// <summary>
		/// <see cref="DelaunayMap"/>から等高線データを作成できます。
		/// </summary>
		/// <param name="delaunayMap">ドロネー三角図</param>
		public ContourMap(DelaunayMap delaunayMap)
		{
			DataPoints = new Dictionary<Edge, Dictionary<float, Vector>>();
			DelaunayMap = delaunayMap;
		}

		/// <summary>
		/// 指定した分割数で等高線データを作成します。
		/// </summary>
		/// <param name="partitionCount">等高線の分割数</param>
		public void Create(int partitionCount)
		{
			var heights = Enumerable.Range(1, partitionCount - 1).Select(a => a * (DelaunayMap.HeightMap.MaxHeight - DelaunayMap.HeightMap.MinHeight) / partitionCount + DelaunayMap.HeightMap.MinHeight);

			foreach (Edge edge in DelaunayMap.Triangles.SelectMany(a => a.Edges).Distinct())
			{
				var tmpDict = new Dictionary<float, Vector>();

				Vector str = edge.Point1;
				Vector end = edge.Point2;
				float strHeight = DelaunayMap.HeightMap.Heights[str];
				float endHeight = DelaunayMap.HeightMap.Heights[end];

				if (strHeight > endHeight)
				{
					ChangeValue(ref strHeight, ref endHeight);
					ChangeValue(ref str, ref end);
				}

				foreach (var height in heights.Where(a => strHeight <= a && a <= endHeight))
				{
					float ratio_str = (height - strHeight) / (endHeight - strHeight);
					float ratio_end = (endHeight - height) / (endHeight - strHeight);

					tmpDict[height] = (ratio_end * str + ratio_str * end);
				}

				DataPoints[edge] = tmpDict;
			}
		}

		private static void ChangeValue<T>(ref T value1, ref T value2)
		{
			T tmp = value1;
			value1 = value2;
			value2 = tmp;
		}
	}
}
