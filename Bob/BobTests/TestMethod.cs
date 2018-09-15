using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bob;

namespace BobTests {
	[TestClass]
	public class TestMethod {
		[TestMethod]
		public void Method_NamingConventions () {
			Assert.AreEqual ("Cambridge Surprise Major", new Method ("X", "Cambridge", Catagory.Surprise, Stage.Major).title);
			//Assert.AreEqual ("Stedman Triples", new Method ("3", "Stedman", Catagory.Principle, Stage.Triples).title);
			//Assert.AreEqual ("Kent Treble Bob Twenty-two", new Method ("X", "kent", Catagory.TrebleBob, Stage.TwentyTwo).title);
		}
	}
}
