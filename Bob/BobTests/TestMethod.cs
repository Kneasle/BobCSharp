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
		public void Method_Misc () {
			Assert.AreEqual (70, Method.grandsire_triples.plain_course_length);
		}
	}
}
