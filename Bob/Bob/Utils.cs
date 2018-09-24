using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A class to store utility functions.
	/// </summary>
	public static class Utils {
		/// <summary>
		/// Converts a <see cref="Stage"/> to a string.
		/// </summary>
		/// <param name="stage">The <see cref="Stage"/> who's string representation is needed.</param>
		/// <returns>A string representing the given stage.</returns>
		public static string StageToString (Stage stage) {
			if (stage == Stage.TwentyTwo) {
				return "Twenty-two";
			}

			return stage.ToString ();
		}

		/// <summary>
		/// Converts a <see cref="Classification"/> to a string.
		/// </summary>
		/// <param name="classification">The <see cref="Classification"/> who's string representation is needed.</param>
		/// <returns>A string representing the given Classification</returns>
		public static string ClassificationToString (Classification classification) {
			if (classification == Classification.SlowCourse) {
				return "Slow Course";
			}

			if (classification == Classification.TreblePlace) {
				return "Treble Place";
			}

			if (classification == Classification.TrebleBob) {
				return "Treble Bob";
			}

			return classification.ToString ();
		}

		/// <summary>
		/// Computes the factorial of an integer `n`.  Please do NOT put large numbers into this function, or your computer will crash.
		/// </summary>
		/// <param name="n">The integer who's factorial is required.</param>
		/// <returns>The factoral of that integer.</returns>
		public static int Factorial (int n) {
			int result = 1;

			for (int i = 2; i <= n; i++) {
				result *= i;
			}

			return result;
		}
	}
}
