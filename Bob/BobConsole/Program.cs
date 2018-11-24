using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bob;

namespace BobConsole {
	class Program {
		static void PrintGrandsireTriplesPlainCourse () {
			Console.WriteLine (Method.grandsire_triples.plain_course);
		}

		static void Print120OfPlainBobDoubles () {
			Console.WriteLine (Method.plain_bob_doubles.TouchFromCallList ("PPPBPS"));
		}

		static void PrintSpacer () {
			Console.WriteLine ("\n\n\n\n\n");
		}

		static void Print720OfBobMinor () {
			Console.WriteLine (Method.plain_bob_minor.TouchFromCallingPositions ("WsWWsWH"));
		}

		static void PrintHalfACourseOfCambridgeMajor () {
			Console.WriteLine (new Touch (
				Method.cambridge_major,
				new MethodCall [] {new MethodCall (
					new Method ("X18", "Original", Stage.Major),
					new Touch.CallLocationCountDown (3),
					-2
				)}
			));
		}

		static void PrintSomeBobMinorAndDoublesSpliced () {
			Console.WriteLine (new Touch (
				new Method [] { Method.GetMethod ("Plain Bob Doubles"), Method.GetMethod ("Plain Bob Minor") }
			));
		}
		
		static void PrintACalledChangeTouch () {
			Console.WriteLine (new CalledChangeTouch (
				Stage.Doubles,
				new CalledChange [] {
					CalledChange.ByPlaceCalledUp (1),
					CalledChange.ByPlaceCalledUp (3),
					CalledChange.ByPlaceCalledUp (2)
				}
			));
		}

		static void PrintATouchWhichDoesntComeRound () {
			Console.WriteLine (Method.GetMethod ("Plain Bob Minor").TouchFromCallingPositions ("I"));
		}

		static void ComputeAnExtentOfPlainBobDoubles () {
			string [] extents = Method.GetMethod ("St Remigius Bob Doubles").GenerateExtents ("MB");

			foreach (string s in extents) {
				Console.WriteLine (s);
			}
		}

		static void ComputeAnExtentOfLetsRingDelightMinor () {
			// Draws a breakdown of how long function calls take.
			Method lets_ring = new Method ("56x56.14x56x16x12x16,12", "Let's Ring is a", Stage.Minor);

			string [] extents = lets_ring.GenerateExtents ("MB", 10);

			foreach (string s in extents) {
				Console.WriteLine (s);
			}
		}

		static void DemonstrateSpeedBreakdown () {
			Console.WriteLine ("1");

			Touch t = new Method ("56x56.14x56x16x12x16,12", "Let's Ring is a", Stage.Minor).TouchFromCallingPositions ("WHW");

			Console.WriteLine ("2");

			Change[] c = t.changes;

			Console.WriteLine ("3");

			string s = t.ToString ();

			Console.WriteLine ("4");

			Console.WriteLine (s);
		}

		static void PrintTheCoursingOrderOfPB8 () {
			Console.WriteLine (Method.GetMethod ("Plain Bob Major").GetCoursingOrderString ());
		}

		static void Main (string [] args) {
			ComputeAnExtentOfLetsRingDelightMinor ();

			Console.ReadLine ();
		}
	}
}
