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
		/// Converts a string to its respective <see cref="Stage"/>.
		/// </summary>
		/// <param name="name">The name of the stage as a string.</param>
		/// <returns>The <see cref="Stage"/> who's name is `name`.</returns>
		/// <exception cref="ArgumentException">Thrown if the stage can't be found.</exception>
		public static Stage StringToStage (string name) {
			foreach (Stage s in (Stage [])Enum.GetValues (typeof (Stage))) {
				if (StageToString (s) == name) {
					return s;
				}
			}

			throw new ArgumentException ("A stage of name \"" + name + "\" cannot be found.");
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
		/// Converts a string to its respective <see cref="Classification"/>.
		/// </summary>
		/// <param name="name">The name of the classification as a string.</param>
		/// <returns>The <see cref="Classification"/> who's name is `name`.</returns>
		public static Classification StringToClassification (string name) {
			foreach (Classification c in (Classification [])Enum.GetValues (typeof (Classification))) {
				if (ClassificationToString (c) == name) {
					return c;
				}
			}

			throw new ArgumentException ("A classification of name \"" + name + "\" cannot be found.");
		}

		/// <summary>
		/// Gets the stage of a method, given its title as a string.
		/// </summary>
		/// <param name="title">The title of the method whos name you want.</param>
		/// <returns>The stage of the method, or throws an <see cref="ArgumentException"/> exception.</returns>
		public static Stage GetStageOfMethodFromMethodTitle (string title) {
			return StringToStage (title.Substring (title.LastIndexOf (' ') + 1));
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
