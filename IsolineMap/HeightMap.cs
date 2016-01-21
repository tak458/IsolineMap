using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IsolineMap
{
	/// <summary>
	/// 標点データとその高さを収めたクラス
	/// </summary>
	public class HeightMap : IDisposable
	{
		/// <summary>
		/// 標点データと標点の高さを収めたマップ
		/// </summary>
		public IDictionary<Vector, float> Heights { get; } = new Dictionary<Vector, float>();

		/// <summary>
		/// 標点データのみ
		/// </summary>
		public IEnumerable<Vector> Marks
		{
			get { return Heights.Keys; }
		}

		/// <summary>
		/// 標高の最底点
		/// </summary>
		public float MinHeight { get { return Heights.Values.Min(); } }

		/// <summary>
		/// 標高の最高点
		/// </summary>
		public float MaxHeight { get { return Heights.Values.Max(); } }

		public HeightMap() { }

		/// <summary>
		/// ファイルから標点データを読み込みます。
		/// </summary>
		/// <remarks>
		/// ファイル形式はCSVとし、囲み文字は使用しないでください。
		/// <list type="bullet">
		/// <item>
		/// <term>1列目</term>
		/// <description>X軸の数値（幅）</description>
		/// </item>
		/// <item>
		/// <term>2列目</term>
		/// <description>Y軸の数値（奥行き）</description>
		/// </item>
		/// <item>
		/// <term>3列目</term>
		/// <description>Z軸の数値（標高）</description>
		/// </item>
		/// </list>
		/// </remarks>
		/// <param name="filePath">ファイルパス。</param>
		public void LoadFile(string filePath)
		{
			Heights.Clear();

			using (var sr = new StreamReader(filePath))
			{
				while (!sr.EndOfStream)
				{
					var data = sr.ReadLine().Split(',').Select(a => float.Parse(a)).ToArray();
					Heights.Add(new Vector(data[0], data[1]), data[2]);
				}
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					Heights.Clear();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~HeightMap() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

	}
}
