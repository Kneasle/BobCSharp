
using System;
using System.Diagnostics;
using Bob;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BobTests {
	[TestClass]
	public class TestPlaceNotation {
		[TestMethod]
		[ExpectedException (typeof (XNotationWithTenorCoverException))]
		public void PlaceNot_ExceptionIfXNotationUsedWithTenorCover () {
			new PlaceNotation ("X", Stage.Doubles);
		}

		[TestMethod]
		public void PlaceNot_XNotation () {
			HandyStuff.AssertArraysEqual (new int [] { 1, 0, 3, 2, 5, 4, 7, 6 }, new PlaceNotation ("X", Stage.Major).array);
		}

		[TestMethod]
		public void PlaceNot_BasicNotations () {
			HandyStuff.AssertArraysEqual (new int [] { 0, 1, 2, 3, 5, 4 }, new PlaceNotation ("1234", Stage.Minor).array);
			HandyStuff.AssertArraysEqual (new int [] { 0, 2, 1, 3, 5, 4, 6 }, new PlaceNotation ("147", Stage.Triples).array);
			HandyStuff.AssertArraysEqual (new int [] { 1, 0, 2, 4, 3 }, new PlaceNotation ("3", Stage.Doubles).array);
		}

		[TestMethod]
		[ExpectedException (typeof (ArgumentException))]
		public void PlaceNot_EmptyString () {
			new PlaceNotation ("", Stage.Doubles);
		}

		[TestMethod]
		public void PlaceNot_ImplicitPlaces () {
			HandyStuff.AssertArraysEqual (new int [] { 0, 1, 3, 2, 4 }, new PlaceNotation ("2", Stage.Doubles).array);
			HandyStuff.AssertArraysEqual (new int [] { 0, 2, 1, 3, 5, 4, 7, 6, 9, 8 }, new PlaceNotation ("4", Stage.Royal).array);
		}

		[TestMethod]
		public void PlaceNot_DecodeFullNotation () {
			Assert.AreEqual (PlaceNotation.CombinePlaceNotations (PlaceNotation.DecodeFullNotation ("5.1.5.1.5,2", Stage.Doubles)), new Change ("13524"));
			Assert.AreEqual (PlaceNotation.CombinePlaceNotations (PlaceNotation.DecodeFullNotation ("X18", Stage.Major)), new Change ("24163857"));
		}

		[TestMethod]
		public void PlaceNot_Equality () {
			Assert.AreEqual (new PlaceNotation ("12", Stage.Doubles), new PlaceNotation ("125", Stage.Doubles));
			Assert.AreEqual (false, new PlaceNotation ("12", Stage.Minor) == new PlaceNotation ("12", Stage.Doubles));
			Assert.AreEqual (true, new PlaceNotation ("12", Stage.Doubles) == new PlaceNotation ("125", Stage.Doubles));
			Assert.AreEqual (false, new PlaceNotation ("12", Stage.Doubles) != new PlaceNotation ("125", Stage.Doubles));
		}

		[TestMethod]
		public void PlaceNot_InternalPlaces () {
			Assert.AreEqual (false, new PlaceNotation ("X", Stage.Minor).has_internal_places);
			Assert.AreEqual (false, new PlaceNotation ("18", Stage.Major).has_internal_places);
			Assert.AreEqual (true, new PlaceNotation ("16", Stage.Major).has_internal_places);
			Assert.AreEqual (true, new PlaceNotation ("36", Stage.Minor).has_internal_places);
		}

		[TestMethod]
		public void PlaceNot_Is12Or1n () {
			Assert.AreEqual (true, new PlaceNotation ("12", Stage.Major).is_12);
			Assert.AreEqual (false, new PlaceNotation ("12", Stage.Major).is_1n);

			Assert.AreEqual (false, new PlaceNotation ("1", Stage.Doubles).is_1n);

			Assert.AreEqual (true, new PlaceNotation ("125", Stage.Doubles).is_12);
		}
	}
}
