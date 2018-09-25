using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bob;

namespace BobTests {
	[TestClass]
	public class TestCalledChanges {
		[TestMethod]
		public void CallChanges_ByPlaceCalledUpInt () {
			Assert.AreEqual (
				new Change ("13245"), 
				Change.Rounds (Stage.Doubles) * 
				CalledChange.ByPlaceCalledUp (1)
			);
			Assert.AreEqual (
				new Change ("1235647"), 
				Change.Rounds (Stage.Triples) * 
				CalledChange.ByPlaceCalledUp (3) * 
				CalledChange.ByPlaceCalledUp (4)
			);
		}

		[TestMethod]
		public void CallChanges_ByPlaceCalledUpStr () {
			Assert.AreEqual (
				new Change ("13245"),
				Change.Rounds (Stage.Doubles) *
				CalledChange.ByPlaceCalledUp ('2')
			);
			Assert.AreEqual (
				new Change ("1235647"),
				Change.Rounds (Stage.Triples) *
				CalledChange.ByPlaceCalledUp ('4') *
				CalledChange.ByPlaceCalledUp ('5')
			);
		}

		[TestMethod]
		public void CallChanges_ByBellCalledUpInt () {
			Assert.AreEqual (
				new Change ("135246"), // Queens
				Change.Rounds (Stage.Minor) * // Start at rounds.
				CalledChange.ByBellCalledUp (1) * // "2 to x" (2 to 3)
				CalledChange.ByBellCalledUp (3) * // "4 to x" (4 to 5)
				CalledChange.ByBellCalledUp (1)   // "2 to x" (2 to 5)
			);
		}

		[TestMethod]
		public void CallChanges_ByBellCalledUpStr () {
			Assert.AreEqual (
				new Change ("135246"), // Queens
				Change.Rounds (Stage.Minor) * // Start at rounds.
				CalledChange.ByBellCalledUp ('2') * // "2 to x" (2 to 3)
				CalledChange.ByBellCalledUp ('4') * // "4 to x" (4 to 5)
				CalledChange.ByBellCalledUp ('2')   // "2 to x" (2 to 5)
			);
		}

		[TestMethod]
		public void CallChanges_ByBellCalledDownInt () {
			Assert.AreEqual (
				new Change ("135246"), // Queens
				Change.Rounds (Stage.Minor) * // Start at rounds.
				CalledChange.ByBellCalledDown (2) * // "x to 3" (2 to 3)
				CalledChange.ByBellCalledDown (4) * // "x to 5" (4 to 5)
				CalledChange.ByBellCalledDown (4)   // "x to 5" (2 to 5)
			);
		}
		
		[TestMethod]
		public void CallChanges_ByBellCalledDownStr () {
			Assert.AreEqual (
				new Change ("135246"), // Queens
				Change.Rounds (Stage.Minor) * // Start at rounds.
				CalledChange.ByBellCalledDown ('3') * // "x to 3" (2 to 3)
				CalledChange.ByBellCalledDown ('5') * // "x to 5" (4 to 5)
				CalledChange.ByBellCalledDown ('5')   // "x to 5" (2 to 5)
			);
		}
	}
}
