using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BobTests {
	public class HandyStuff {
		public static void AssertArraysEqual<T> (T [] array1, T [] array2) {
			for (int i = 0; i < Math.Max (array1.Length, array2.Length); i++) {
				Assert.AreEqual (array1 [i], array2 [i]);
			}
		}
	}
}
