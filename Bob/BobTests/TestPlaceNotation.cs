
using System;
using System.Diagnostics;
using Bob;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BobTests {
	[TestClass]
	public class TestPlaceNotation {
		[TestMethod]
		[ExpectedException (typeof (Bob.XNotationWithTenorCoverException))]
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
		public void PlaceNot_ImplicitPlaces () {
			HandyStuff.AssertArraysEqual (new int [] { 0, 1, 3, 2, 4 }, new PlaceNotation ("2", Stage.Doubles).array);
			HandyStuff.AssertArraysEqual (new int [] { 0, 2, 1, 3, 5, 4, 7, 6, 9, 8 }, new PlaceNotation ("4", Stage.Royal).array);
		}

		[TestMethod]
		public void PlaceNot_DecodeFullNotation () {
			PlaceNotation[] notations = PlaceNotation.DecodeFullNotation ("5.1.5.1.5,2", Stage.Doubles);

			Change lead_end = HandyStuff.GetLeadEnd (notations);
		}
	}
}
