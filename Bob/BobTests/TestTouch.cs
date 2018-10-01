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

			Assert.AreEqual (40, touch.Length);
			Assert.AreEqual (false, touch.is_extent);
			Assert.AreEqual (false, touch.is_multiple_extent);
			Assert.AreEqual (true, touch.is_true);
			Assert.AreEqual (true, touch.is_quarter_peal_true);
		}

		[TestMethod]
		public void Touch_BobCourseOfBobDoubles () {
			Call bob = Call.LeadEndBob (Method.plain_bob_doubles, "145");

			Touch touch = new Touch (
				Method.plain_bob_doubles,
				new BasicCall [] {
					new BasicCall (bob, new Touch.CallLocationList ())
				}
			);
			
			Assert.AreEqual (20, touch.Length);
			Assert.AreEqual (false, touch.is_extent);
			Assert.AreEqual (false, touch.is_multiple_extent);
			Assert.AreEqual (true, touch.is_true);
			Assert.AreEqual (true, touch.is_quarter_peal_true);
		}

		[TestMethod]
		public void Touch_120OfBobDoubles () {
			Touch touch = Method.plain_bob_doubles.TouchFromCallList ("PPBP");

			Assert.AreEqual (120, touch.Length);
			Assert.AreEqual (true, touch.is_extent);
			Assert.AreEqual (true, touch.is_multiple_extent);
			Assert.AreEqual (true, touch.is_true);
			Assert.AreEqual (true, touch.is_quarter_peal_true);
		}

		[TestMethod]
		public void Touch_120OfGrandsire () {
			Touch touch = Method.grandsire_doubles.TouchFromCallList ("SBSP");

			Assert.AreEqual (120, touch.Length);
			Assert.AreEqual (true, touch.is_extent);
			Assert.AreEqual (true, touch.is_multiple_extent);
			Assert.AreEqual (true, touch.is_true);
			Assert.AreEqual (true, touch.is_quarter_peal_true);
		}

		[TestMethod]
		public void Touch_180OfBobDoubles () {
			Touch touch = Method.plain_bob_doubles.TouchFromCallList ("PPPBPS");

			Assert.AreEqual (180, touch.Length);
			Assert.AreEqual (false, touch.is_extent);
			Assert.AreEqual (false, touch.is_multiple_extent);
			Assert.AreEqual (false, touch.is_true);
			Assert.AreEqual (true, touch.is_quarter_peal_true);
		}

		[TestMethod]
		public void Touch_FullPeal () {
			Touch touch = Method.plain_bob_triples.TouchFromCallingPositions ("OHHH sWHHH WFHHH IH");

			Assert.AreEqual (5040, touch.Length);
			Assert.AreEqual (true, touch.is_extent);
			Assert.AreEqual (true, touch.is_true);
		}

		[TestMethod]
		public void Touch_Splicing () {
			Touch plain_bob_splicing = new Touch (
				new Method [] { Method.plain_bob_doubles, Method.plain_bob_minor }
			);

			Assert.AreEqual (132, plain_bob_splicing.Length);

			Touch half_course_of_cambridge = new Touch (
				Method.cambridge_major,
				new MethodCall [] {new MethodCall (
					new Method ("X18", "Original", Stage.Major),
					new Touch.CallLocationCountDown (3),
					-2, 0
				)}
			);

			Assert.AreEqual (128, half_course_of_cambridge.Length);
		}

		[TestMethod]
		public void Touch_CalledChanges () {
			CalledChangeTouch touch = new CalledChangeTouch (
				Stage.Doubles,
				new CalledChange [] {
					CalledChange.ByPlaceCalledUp (1),
					CalledChange.ByPlaceCalledUp (3),
					CalledChange.ByPlaceCalledUp (2)
				}
			);

			Assert.AreEqual (12, touch.Length);
			Assert.AreEqual (false, touch.is_extent);
			Assert.AreEqual (true, touch.is_true);
		}

		[TestMethod]
		public void Touch_DoesntComeRound () {
			Touch touch = Method.GetMethod ("Plain Bob Minor").TouchFromCallingPositions ("I");

			Assert.AreEqual (false, touch.comes_round);
		}
	}
}
