using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A class to run profiling tests on C# functions.  Used for development purposes only.
	/// </summary>
	public class SpeedProfiler {
		/// <summary>
		/// The number of different functions to profile independantly.
		/// </summary>
		public int length;

		private readonly float [] time_totals;

		private int index = 0;
		private int runs = 1;
		private Stopwatch stop_watch;

		/// <summary>
		/// Starts the profiling session.
		/// </summary>
		public void Start () {
			stop_watch.Start ();
		}

		/// <summary>
		/// Records the time since Profile () or Start () were last called.
		/// </summary>
		public void Profile () {
			time_totals [index] += (float)stop_watch.ElapsedTicks / Stopwatch.Frequency;

			index += 1;

			if (index == length) {
				index = 0;

				runs += 1;
			}

			stop_watch.Reset ();
			stop_watch.Start ();
		}

		/// <summary>
		/// Prints a string of the profile with optional formatting.
		/// </summary>
		/// <param name="names">A list of names of each function.</param>
		/// <param name="delimiter">A delimiter which goes between each name and its average time.</param>
		public void Print (string [] names = null, string delimiter = ":\t") {
			if (names == null) {
				for (int i = 0; i < length; i++) {
					Console.WriteLine (i + delimiter + time_totals [i] / runs);
				}
			} else {
				for (int i = 0; i < length; i++) {
					Console.WriteLine (names [i] + delimiter + time_totals [i] / runs);
				}
			}
		}

		/// <summary>
		/// Creates a new SpeedProfiler.
		/// </summary>
		/// <param name="length">The number of functions to profile.</param>
		public SpeedProfiler (int length) {
			this.length = length;

			time_totals = new float [length];

			stop_watch = new Stopwatch ();
		}
	}
}
