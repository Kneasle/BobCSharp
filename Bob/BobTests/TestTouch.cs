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
		}

		[TestMethod]
		public void Touch_120OfBobDoubles () {
			Method method = Method.plain_bob_doubles;

			Call bob = Call.LeadEndBob (method, "145");
			Call plain = Call.LeadEndPlain (method);

			Touch touch = new Touch (
				method,
				new BasicCall [] {
					new BasicCall (plain, new CallLocationList ()),
					new BasicCall (plain, new CallLocationList ()),
					new BasicCall (plain, new CallLocationList ()),
					new BasicCall (bob, new CallLocationList ())
				}
			);

			Assert.AreEqual (120, touch.Length);
		}
	}
}
