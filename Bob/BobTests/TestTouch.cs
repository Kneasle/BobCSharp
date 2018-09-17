using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bob;

namespace BobTests {
	[TestClass]
	public class TestTouch {
		[TestMethod]
		public void Touch_PlainCouseOfBobDoubles () {
			Touch touch = Method.plain_bob_doubles.plain_course;

			Assert.AreEqual (touch.target_change, Change.Rounds (Stage.Doubles));
			Assert.AreEqual (touch.Length, 40);
			Assert.AreEqual (false, touch.is_extent);
		}

		[TestMethod]
		public void Touch_BobCourseOfBobDoubles () {
			Call bob = Call.LeadEndBob (Method.plain_bob_doubles, "145");

			Touch touch = new Touch (
				Method.plain_bob_doubles,
				new BasicCall [] {
					new BasicCall (bob, new CallLocationList ())
				}
			);
			
			Assert.AreEqual (20, touch.Length);
			Assert.AreEqual (false, touch.is_extent);
		}

		[TestMethod]
		public void Touch_120OfBobDoubles () {
			Touch touch = Method.plain_bob_doubles.TouchFromCallList ("PPBP");

			Assert.AreEqual (120, touch.Length);
			Assert.AreEqual (true, touch.is_extent);
		}
	}
}
