using System;
using System.Diagnostics;
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
			Method lets_ring = new Method ("56x56.14x56x16x12x16,12", "Let's Ring is a", Stage.Minor);

			string [] extents = lets_ring.GenerateExtents ("MB", 10);

			foreach (string s in extents) {
				Console.WriteLine (s);
			}
		}

		static void DemonstrateSpeedBreakdown () {
			Stopwatch stop_watch = Stopwatch.StartNew ();

			long total_object_creation_time = 0;
			long total_change_computation_time = 0;
			long total_truth_check_time = 0;
			long total_string_conversion_time = 0;

			int n = 100;

			for (int x = 0; x < n; x++) {
				Touch t = new Method ("56x56.14x56x16x12x16,12", "Let's Ring is a", Stage.Minor).TouchFromCallingPositions ("WHW");

				total_object_creation_time += stop_watch.ElapsedMilliseconds;

				stop_watch.Reset ();
				stop_watch.Start ();

				Change [] c = t.changes;

				total_change_computation_time += stop_watch.ElapsedMilliseconds;

				stop_watch.Reset ();
				stop_watch.Start ();

				string s = t.ToString ();

				total_truth_check_time += stop_watch.ElapsedMilliseconds;

				stop_watch.Reset ();
				stop_watch.Start ();
			}

			stop_watch.Stop ();

			Console.WriteLine ("1. Creating Method and Touch objects:");
			Console.WriteLine (" >> {0} ms", total_object_creation_time / n);
			Console.WriteLine ("2. Computing changes in the touch:");
			Console.WriteLine (" >> {0} ms", total_change_computation_time / n);
			Console.WriteLine ("3. Running truth check and string conversion.");
			Console.WriteLine (" >> {0} ms", total_truth_check_time / n);
			Console.WriteLine ("4. Constructing string representation.");
			Console.WriteLine (" >> {0} ms", total_string_conversion_time / n);
		}

		static void PrintTheCoursingOrderOfPB8 () {
			Console.WriteLine (Method.GetMethod ("Plain Bob Major").GetCoursingOrderString ());
		}

		static void Main (string [] args) {
			DemonstrateSpeedBreakdown ();

			Console.ReadLine ();
		}
	}
}
