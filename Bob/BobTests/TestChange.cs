using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bob;

namespace BobTests {
	[TestClass]
	public class TestChange {
		[TestMethod]
		public void Change_Parity () {
			Assert.AreEqual (Parity.Even, Change.Rounds (Stage.Eighteen).parity);
			Assert.AreEqual (Parity.Odd, new Change (new int [] { 0, 2, 1, 3 }).parity);
		}

		[TestMethod]
		public void Change_Order () {
			Assert.AreEqual (1, Change.Rounds (Stage.Major).order);
			Assert.AreEqual (2, new Change (new int [] { 0, 2, 1, 3 }).order);
			Assert.AreEqual (3, new Change (new int [] { 0, 2, 3, 1, 4, 5 }).order);
			Assert.AreEqual (6, new Change (new int [] { 0, 2, 3, 1, 5, 4 }).order);
		}

		[TestMethod]
		public void Change_Equality () {
			Assert.AreEqual (true, Change.Rounds (Stage.Major) == Change.Rounds (Stage.Major));
			Assert.AreEqual (false, Change.Rounds (Stage.Major) != Change.Rounds (Stage.Major));
			Assert.AreEqual (false, Change.Rounds (Stage.Minor) == Change.Rounds (Stage.Major));
			Assert.AreEqual (true, Change.Rounds (Stage.Minor) == new Change (new int [] { 0, 1, 2, 3, 4, 5 }));
			Assert.AreEqual (false, Change.Rounds (Stage.Minor) == new Change (new int [] { 0, 1, 4, 3, 2, 5 }));

			Assert.AreEqual (Change.Rounds (Stage.Royal), Change.Rounds (Stage.Royal));
			Assert.AreNotEqual (Change.Rounds (Stage.Minor), new Change (new int [] { 0, 1, 2, 4, 3, 5 }));
			Assert.AreEqual (Change.Rounds (Stage.Singles), new Change (new int [] { 0, 1, 2 }));
		}
	}
}
