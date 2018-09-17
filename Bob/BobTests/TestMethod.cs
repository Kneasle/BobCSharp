using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bob;

namespace BobTests {
	[TestClass]
	public class TestMethod {
		[TestMethod]
		public void Method_NamingConventions () {
			Assert.AreEqual ("Cambridge Surprise Major", new Method ("X", "Cambridge", Catagory.Surprise, Stage.Major).title);
			Assert.AreEqual ("Stedman Triples", new Method ("3", "Stedman", Catagory.Principle, Stage.Triples).title);
			Assert.AreEqual ("Kent Treble Bob Twenty-two", new Method ("X", "kenT", Catagory.TrebleBob, Stage.TwentyTwo).title);
			Assert.AreEqual ("Grandsire Triples", Method.grandsire_triples.title);
		}

		[TestMethod]
		public void Method_HuntBells () {
			Assert.AreEqual (true, Method.plain_bob_doubles.is_treble_hunting);
			Assert.AreEqual (1, Method.plain_bob_minor.hunt_bells.Length);
			Assert.AreEqual (true, Method.plain_bob_minor.main_hunt_bell.is_plain_hunting);
			Assert.AreEqual (false, Method.plain_bob_minor.main_hunt_bell.is_little);
			Assert.AreEqual (false, Method.plain_bob_minor.main_hunt_bell.is_treble_dodging);

			Assert.AreEqual (true, Method.cambridge_major.is_treble_hunting);
			Assert.AreEqual (1, Method.cambridge_major.hunt_bells.Length);
			Assert.AreEqual (false, Method.cambridge_major.main_hunt_bell.is_plain_hunting);
			Assert.AreEqual (false, Method.cambridge_major.main_hunt_bell.is_little);
			Assert.AreEqual (true, Method.cambridge_major.main_hunt_bell.is_treble_dodging);
		}

		[TestMethod]
		public void Method_Symmetry () {
			Assert.AreEqual (Method.SymmetryType.PlainBobLike, Method.plain_bob_doubles.symmetry_type);
			Assert.AreEqual (Method.SymmetryType.GrandsireLike, Method.grandsire_triples.symmetry_type);
		}

		[TestMethod]
		public void Method_Misc () {
			Assert.AreEqual (70, Method.grandsire_triples.plain_course_length);
		}
	}
}
