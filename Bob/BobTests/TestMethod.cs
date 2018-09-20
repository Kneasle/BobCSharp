using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bob;

namespace BobTests {
	[TestClass]
	public class TestMethod {
		[TestMethod]
		public void Method_NamingConventions () {
			Assert.AreEqual ("Cambridge Surprise Major", new Method ("X", "Cambridge", Classification.Surprise, Stage.Major).title);
			Assert.AreEqual ("Stedman Triples", new Method ("3", "Stedman", Classification.Principle, Stage.Triples).title);
			Assert.AreEqual ("Kent Treble Bob Twenty-two", new Method ("X", "Kent", Classification.TrebleBob, Stage.TwentyTwo).title);
			Assert.AreEqual ("Grandsire Triples", Method.grandsire_triples.title);
		}

		[TestMethod]
		public void Method_HuntBells () {
			Method p5 = Method.plain_bob_doubles;
			Method p6 = Method.plain_bob_minor;
			Method c8 = Method.cambridge_major;
			Method g5 = Method.grandsire_doubles;

			Assert.AreEqual (true, p5.is_treble_hunting);
			Assert.AreEqual (1, p6.hunt_bells.Length);
			Assert.AreEqual (true, p6.main_hunt_bell.is_plain_hunting);
			Assert.AreEqual (false, p6.main_hunt_bell.is_little);
			Assert.AreEqual (false, p6.main_hunt_bell.is_treble_dodging);

			Assert.AreEqual (true, c8.is_treble_hunting);
			Assert.AreEqual (1, c8.hunt_bells.Length);
			Assert.AreEqual (false, c8.main_hunt_bell.is_plain_hunting);
			Assert.AreEqual (false, c8.main_hunt_bell.is_little);
			Assert.AreEqual (true, c8.main_hunt_bell.is_treble_dodging);
			
			Assert.AreEqual (true, g5.is_treble_hunting);
			Assert.AreEqual (2, g5.hunt_bells.Length);
			Assert.AreEqual (true, g5.hunt_bells [1].is_plain_hunting);
			Assert.AreEqual (true, g5.main_hunt_bell.is_plain_hunting);
			Assert.AreEqual (false, g5.main_hunt_bell.is_little);
			Assert.AreEqual (false, g5.main_hunt_bell.is_treble_dodging);
		}

		[TestMethod]
		public void Method_Symmetry () {
			Assert.AreEqual (Method.SymmetryType.PlainBobLike, Method.plain_bob_doubles.symmetry_type);
			Assert.AreEqual (Method.SymmetryType.GrandsireLike, Method.grandsire_triples.symmetry_type);
		}

		[TestMethod]
		public void Method_PathFinder () {
			CollectionAssert.AreEqual (
				new int [] {
					0, 0, 1, 2, 3, 4, 4, 3, 2, 3,
					2, 1, 0, 0, 1, 2, 3, 4, 4, 4,
					4, 3, 2, 1, 0, 0, 1, 2, 3, 2,
					3, 4, 4, 3, 2, 1, 0, 0, 1, 1
				},
				Method.plain_bob_doubles.GetPathOfBell (1)
			);

			CollectionAssert.AreEqual (
				new int [] { 1, 2, 3, 4, 4, 3, 2, 1, 0, 0 },
				Method.plain_bob_doubles.GetPathOfBell (0)
			);
		}

		[TestMethod]
		public void Method_ClassificationAndNaming () {
			Assert.AreEqual ("Original Minimus", new Method ("X14", "Original", Stage.Minimus).title);
			Assert.AreEqual ("Upham Differential Triples", new Method ("5.3.7.5.147.5.3.145.3.5.127.5.3.7.3.5.1.7.3.5.1.5.3.1.3.5.1.5.3.7.5.1.5.1.7,147", "Upham", Stage.Triples).title);
			Assert.AreEqual ("St Lawrence Little Bob Minor", new Method ("36.14x14,12", "St Lawrence", Stage.Minor).title);
		}

		[TestMethod]
		public void Method_Misc () {
			Assert.AreEqual (70, Method.grandsire_triples.plain_course_length);
		}
	}
}
