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
		public int length;

		public float [] time_totals;

		private int index = 0;
		private int runs = 1;
		private Stopwatch stop_watch;

		public void Start () {
			stop_watch.Start ();
		}

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

		public SpeedProfiler (int length) {
			this.length = length;

			time_totals = new float [length];

			stop_watch = new Stopwatch ();
		}
	}
}
