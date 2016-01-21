using System;
using System.Collections.Generic;

namespace IsolineMap
{
	/// <summary>
	///     2次元ベクタを表す構造体です。
	/// </summary>
	public struct Vector : IEquatable<Vector>
	{
		/// <summary>
		///     2次元ベクタを指定した値で初期化します。値を指定しない場合、ゼロベクタとなります。
		/// </summary>
		/// <param name="x">3DベクタのX軸方向の数値。</param>
		/// <param name="y">3DベクタのY軸方向の数値。</param>
		public Vector(float x = 0.0f, float y = 0.0f) : this()
		{
			X = x;
			Y = y;
		}

		/// <summary>
		///     2次元ベクタのX座標値を取得します。
		/// </summary>
		public float X { get; private set; }

		/// <summary>
		///     2次元ベクタのY座標値を取得します。
		/// </summary>
		public float Y { get; private set; }

		/// <summary>
		///     この2次元ベクタと指定した2次元ベクタを比較します。
		/// </summary>
		/// <param name="other">比較する2次元ベクタ。</param>
		/// <returns>比較結果の論理値を返します。</returns>
		public bool Equals(Vector other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
		}

		/// <summary>
		///     2次元ベクタの大きさを返します。
		/// </summary>
		/// <returns></returns>
		public float Length()
		{
			return (float) Math.Sqrt(X*X + Y*Y);
		}

		/// <summary>
		///     2次元ベクタの大きさの二乗を返します。
		/// </summary>
		/// <returns></returns>
		public float LengthSq()
		{
			return X*X + Y*Y;
		}

		/// <summary>
		///     2次元ベクタを文字列形式で出力します。形は(x,y,z)です。
		/// </summary>
		/// <returns>2次元ベクタの文字列。</returns>
		public override string ToString()
		{
			return string.Format("({0:E},{1:E})", X, Y);
		}

		/// <summary>
		///     指定された書式で2次元ベクタを文字列形式で出力します。形は(x,y,z)です。
		/// </summary>
		/// <param name="format">書式指定文字列。XYZ全ての値に同様に適用されます。</param>
		/// <returns>指定した書式の2次元ベクタ文字列。</returns>
		public string ToString(string format)
		{
			return "(" + X.ToString(format) + ", " + Y.ToString(format) + ")";
		}

		/// <summary>
		///     オブジェクトとの比較を行います。
		/// </summary>
		/// <param name="obj">比較するオブジェクト。</param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj != null && obj is Vector)
			{
				return Equals((Vector)obj);
			}
			return false;
		}

		/// <summary>
		///     このインスタンスのハッシュコードを返します。
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				// Suitable nullity checks etc, of course :)
				hash = hash * 23 + BitConverter.ToInt32(BitConverter.GetBytes(X), 0);
				hash = hash * 23 + BitConverter.ToInt32(BitConverter.GetBytes(Y), 0);
				return hash;
			}
		}

		#region 演算メソッド

		/// <summary>
		///     この2次元ベクタの単位ベクトルを取得します。
		/// </summary>
		/// <returns></returns>
		public Vector Normalize()
		{
			return this*(1.0f/Length());
		}

		/// <summary>
		///     指定した2次元ベクタの単位ベクトルを取得します。
		/// </summary>
		public static Vector Normalize(Vector value)
		{
			return value*(1.0f/value.Length());
		}

		/// <summary>
		///     この2次元ベクタに指定した数値を乗算した値を返します。
		/// </summary>
		/// <param name="value">指定するスカラー値。</param>
		/// <returns></returns>
		public Vector Multiply(float value)
		{
			return new Vector(X*value, Y*value);
		}

		/// <summary>
		///     指定した2次元ベクタと数値を乗算した2次元ベクタを返します。
		/// </summary>
		/// <param name="left">指定する2次元ベクタ。</param>
		/// <param name="right">指定するスカラー値。</param>
		/// <returns>乗算した2次元ベクタ。</returns>
		public static Vector Multiply(Vector left, float right)
		{
			return new Vector(left.X*right, left.Y*right);
		}

		/// <summary>
		///     指定した2次元ベクタと数値を乗算した2次元ベクタを返します。
		/// </summary>
		/// <param name="left">指定するスカラー値。</param>
		/// <param name="right">指定する2次元ベクタ。</param>
		/// <returns>乗算した2次元ベクタ。</returns>
		public static Vector Multiply(float left, Vector right)
		{
			return new Vector(right.X*left, right.Y*left);
		}

		/// <summary>
		///     指定した二つの2次元ベクタの内積を返します。
		/// </summary>
		/// <param name="left">指定する2次元ベクタ。</param>
		/// <param name="right">指定する2次元ベクタ。</param>
		/// <returns>内積したスカラー値。</returns>
		public static float Dot(Vector left, Vector right)
		{
			return left.X*right.X + left.Y*right.Y;
		}

		/// <summary>
		///     指定した二つの2次元ベクタの外積を返します。
		/// </summary>
		/// <param name="left">指定する2次元ベクタ。</param>
		/// <param name="right">指定する2次元ベクタ。</param>
		/// <returns>外積した2次元ベクタ。</returns>
		public static float Cross(Vector left, Vector right)
		{
			return left.X*right.Y - left.Y*right.X;
		}

		///// <summary>
		///// 指定した二つの2次元ベクタのなす角度を返します。
		///// </summary>
		///// <param name="left">定する2次元ベクタ。</param>
		///// <param name="right">定する2次元ベクタ。</param>
		///// <returns></returns>
		//public static float GetInterAngle(Vector left, Vector right)
		//{
		//    if (left.LengthSq() > 0 && right.LengthSq() > 0)
		//        return Math.Acos(Vector.Dot(left, right) / (left.Length() * right.Length()));
		//    return 0;
		//}

		#endregion

		#region 演算子

		/// <summary>
		///     2次元ベクタの加算を行います。
		/// </summary>
		/// <param name="left">2次元ベクタ。</param>
		/// <param name="right">2次元ベクタ。</param>
		/// <returns>加算後の2次元ベクタを返します。</returns>
		public static Vector operator +(Vector left, Vector right)
		{
			return new Vector(left.X + right.X, left.Y + right.Y);
		}

		/// <summary>
		///     2次元ベクタの減算を行います。
		/// </summary>
		/// <param name="left">2次元ベクタ。</param>
		/// <param name="right">2次元ベクタ。</param>
		/// <returns>減算後の2次元ベクタを返します。</returns>
		public static Vector operator -(Vector left, Vector right)
		{
			return new Vector(left.X - right.X, left.Y - right.Y);
		}

		/// <summary>
		///     2次元ベクタの逆ベクタを返します。
		/// </summary>
		/// <param name="value">2次元ベクタ。</param>
		/// <returns>逆ベクタを返します。</returns>
		public static Vector operator -(Vector value)
		{
			return new Vector(-value.X, -value.Y);
		}

		/// <summary>
		///     2次元ベクタと実数の乗算を行います。
		/// </summary>
		/// <param name="left">2次元ベクタ。</param>
		/// <param name="right">実数値。</param>
		/// <returns>実数倍された2次元ベクタを返します。</returns>
		public static Vector operator *(Vector left, float right)
		{
			return new Vector(left.X*right, left.Y*right);
		}

		/// <summary>
		///     実数と2次元ベクタの乗算を行います。
		/// </summary>
		/// <param name="left">実数値。</param>
		/// <param name="right">2次元ベクタ。</param>
		/// <returns>実数倍された2次元ベクタを返します。</returns>
		public static Vector operator *(float left, Vector right)
		{
			return new Vector(right.X*left, right.Y*left);
		}

		/// <summary>
		///     2次元ベクタ同士の比較を行います。
		/// </summary>
		/// <param name="left">2次元ベクタ。</param>
		/// <param name="right">2次元ベクタ。</param>
		/// <returns>比較結果の論理値を返します。</returns>
		public static bool operator ==(Vector left, Vector right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     2次元ベクタ同士の比較を行います。
		/// </summary>
		/// <param name="left">2次元ベクタ。</param>
		/// <param name="right">2次元ベクタ。</param>
		/// <returns>比較結果の論理値を返します。</returns>
		public static bool operator !=(Vector left, Vector right)
		{
			return !left.Equals(right);
		}

		#endregion
	}

	public class VectorEqualityComparer : IEqualityComparer<Vector>
	{
		public bool Equals(Vector x, Vector y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(Vector obj)
		{
			return obj.GetHashCode();
		}
	}
}