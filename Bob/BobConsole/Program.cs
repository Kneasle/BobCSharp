﻿using System;
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

		static void GenerateAnExtentOfLetsRingDelightMinor () {
			Method lets_ring = new Method ("56x56.14x56x16x12x16,12", "Let's Ring is a", Stage.Minor);

			string [] extents = lets_ring.GenerateExtents ("MB", 10);

			foreach (string s in extents) {
				Console.WriteLine (s);
			}
		}

		static void DemonstrateSpeedBreakdown () {
			SpeedProfiler profiler = new SpeedProfiler (4);

			int n = 20;

			for (int x = 0; x < n; x++) {
				Touch t = Method.plain_bob_triples.TouchFromCallingPositions ("OHHH sWHHH WFHHH IH");
				// new Method ("56x56.14x56x16x12x16,12", "Let's Ring is a", Stage.Minor).TouchFromCallingPositions ("WHW");

				profiler.Profile ();

				Change [] c = t.changes;

				profiler.Profile ();

				Dictionary <int, int> d = t.change_repeat_frequencies;

				profiler.Profile ();

				string s = t.ToString ();

				profiler.Profile ();

				Console.Write (".");

				if ((x + 1) % 20 == 0) {
					Console.Write ("\n");

					if ((x + 1) % 100 == 0) {
						Console.Write ("\n");
					}
				}
			}
			
			profiler.Print (new string [] {
				"1. Creating Method and Touch objects",
				"2. Computing changes in the touch",
				"3. Running truth check",
				"4. Constructing string representation"
			}, ":\n >> ");
		}

		static void PrintTheCoursingOrderOfPB8 () {
			Console.WriteLine (Method.GetMethod ("Plain Bob Major").GetCoursingOrderString ());
		}

		static void Main (string [] args) {
			GenerateAnExtentOfLetsRingDelightMinor ();

			Console.ReadLine ();
		}
	}
}
