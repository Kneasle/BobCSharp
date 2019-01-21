using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bob;

namespace BobTests {
	[TestClass]
	public class TestMethodLibrary {
		[TestMethod]
		public void Library_PlaceNotation () {
			Assert.AreEqual ("Plain Bob Minimus", MethodLibrary.GetMethodByPlaceNotation ("x14x14,12", Stage.Minimus).title);
			Assert.IsNull (MethodLibrary.GetMethodByPlaceNotation ("x", Stage.Maximus));

			Assert.AreEqual ("Plain Bob Doubles", Method.GetMethod ("5.1.5.1.5,2", Stage.Doubles).title);
		}
	}
}
