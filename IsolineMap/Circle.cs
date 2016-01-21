namespace IsolineMap
{
	public class Circle
	{
		/// <summary>
		/// 指定した中心点と半径から円を定義します。
		/// </summary>
		/// <param name="center">中心点</param>
		/// <param name="radius">半径</param>
		public Circle(Vector center, float radius)
		{
			Center = center;
			Radius = radius;
		}

		/// <summary>
		/// 中心点
		/// </summary>
		public Vector Center { get; private set; }

		/// <summary>
		/// 半径
		/// </summary>
		public float Radius { get; private set; }

		/// <summary>
		/// 指定した<see cref="Vector"/>がこの円の内側にあるか判定します。
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public bool Includes(Vector point)
		{
			return (Center - point).Length() < Radius;
		}

		public override string ToString()
		{
			return string.Format("(C:{0} R:{1})", Center, Radius);
		}
	}
}