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
		public void Change_RotationSets () {
			Assert.AreEqual ("[0][1][2 4 3][5 6]", new Change ("1245376").rotating_sets_as_string);
		}

		[TestMethod]
		public void Change_Equality () {
			// Assert that `==` and `!=` work as expected
			Assert.AreEqual (true, Change.Rounds (Stage.Major) == Change.Rounds (Stage.Major));
			Assert.AreEqual (false, Change.Rounds (Stage.Major) != Change.Rounds (Stage.Major));
			Assert.AreEqual (false, Change.Rounds (Stage.Minor) == Change.Rounds (Stage.Major));
			Assert.AreEqual (true, Change.Rounds (Stage.Minor) == new Change (new int [] { 0, 1, 2, 3, 4, 5 }));
			Assert.AreEqual (false, Change.Rounds (Stage.Minor) == new Change (new int [] { 0, 1, 4, 3, 2, 5 }));

			// Assert that `Change.Equals (other)` works as expected
			Assert.AreEqual (Change.Rounds (Stage.Royal), Change.Rounds (Stage.Royal));
			Assert.AreNotEqual (Change.Rounds (Stage.Minor), new Change (new int [] { 0, 1, 2, 4, 3, 5 }));
			Assert.AreEqual (Change.Rounds (Stage.Singles), new Change (new int [] { 0, 1, 2 }));
			Assert.AreEqual (new Change (new int [] { 0, 3, 2, 1, 4 }), new Change (new int [] { 0, 3, 2, 1, 4 }));
		}

		[TestMethod]
		public void Change_StringConversions () {
			// Change to string
			Assert.AreEqual ("12345678", Change.Rounds (Stage.Major).ToString ());
			Assert.AreEqual ("1326547", new Change (new int [] { 0, 2, 1, 5, 4, 3, 6 }).ToString ());

			// string to Change
			Assert.AreEqual (Change.Rounds (Stage.Maximus), new Change ("1234567890ET"));
			Assert.AreEqual (new Change (new int [] { 5, 4, 2, 3, 1, 0 }), new Change ("653421"));
		}
	}
}
