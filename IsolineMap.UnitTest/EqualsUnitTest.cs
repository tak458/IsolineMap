using IsolineMap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsolineMap.UnitTest
{
	[TestClass]
	public class EqualsUnitTest
	{
		[TestMethod]
		public void VectorEqualsTest()
		{
			var x = new Vector(1, 2);
			var y = new Vector(1, 2);
			var z = new Vector(1, 2);

			Assert.IsTrue(x.Equals(x));

			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));

			Assert.IsTrue(y.Equals(z));
			Assert.IsTrue(x.Equals(z));
		}

		[TestMethod]
		public void EdgeEqualsTest()
		{
			var x = new Edge(new Vector(1, 2), new Vector(3, 4));
			var y = new Edge(new Vector(3, 4), new Vector(1, 2));
			var z = new Edge(new Vector(1, 2), new Vector(3, 4));

			Assert.IsTrue(x.Equals(x));

			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));

			Assert.IsTrue(y.Equals(z));
			Assert.IsTrue(x.Equals(z));
		}

		[TestMethod]
		public void EdgeGetHashCodeTest()
		{
			var x = new Edge(new Vector(1, 2), new Vector(3, 4));
			var y = new Edge(new Vector(3, 4), new Vector(1, 2));
			var z = new Edge(new Vector(1, 2), new Vector(3, 4));

			Console.WriteLine("x:" + x.GetHashCode());
			Console.WriteLine("x1:{0}, x2:{1}", x.Point1.GetHashCode(), x.Point2.GetHashCode());
			Console.WriteLine("y:" + y.GetHashCode());
			Console.WriteLine("y1:{0}, y2:{1}", y.Point1.GetHashCode(), y.Point2.GetHashCode());
			Console.WriteLine("z:" + z.GetHashCode());
			Console.WriteLine("z1:{0}, z2:{1}", z.Point1.GetHashCode(), z.Point2.GetHashCode());

			Assert.IsTrue(x.GetHashCode().Equals(x.GetHashCode()));

			Assert.IsTrue(x.GetHashCode().Equals(y.GetHashCode()));
			Assert.IsTrue(y.GetHashCode().Equals(x.GetHashCode()));

			Assert.IsTrue(y.GetHashCode().Equals(z.GetHashCode()));
			Assert.IsTrue(x.GetHashCode().Equals(z.GetHashCode()));
		}

		[TestMethod]
		public void TriangleEqualsTest()
		{
			var edge1 = new Edge(new Vector(1, 1), new Vector(5, 2));
			var edge2 = new Edge(new Vector(4, 5), new Vector(1, 1));
			var edge3 = new Edge(new Vector(5, 2), new Vector(4, 5));

			var w = new Triangle(edge3, edge1, edge2);
			var x = new Triangle(edge1, edge2, edge3);
			var y = new Triangle(edge2, edge3, edge1);
			var z = new Triangle(edge2, edge1, edge3);

			Assert.IsTrue(x.Equals(x));

			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));

			Assert.IsTrue(y.Equals(z));
			Assert.IsTrue(x.Equals(z));
			Assert.IsTrue(x.Equals(w));
		}
	}
}
