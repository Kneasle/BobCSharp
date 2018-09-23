using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A static class to store the generic constants for BobC#
	/// </summary>
	public static class Constants {
		/// <summary>
		/// A string of all bell names (basically, rounds will always be a substring of this string).
		/// </summary>
		public static string bell_names = "1234567890ETABCDFGHIJKLMNOPQRSUVWYZ";

		/// <summary>
		/// An integer to represent the heaviest bell (used when specifying which bell is conducting).
		/// </summary>
		public const int tenor = -1;
		/// <summary>
		/// A string of every lowercase character.
		/// </summary>
		public const string alpha = "abcdefghijklmnopqrstuvwxyz";
		/// <summary>
		/// A string of every uppercase character.
		/// </summary>
		public const string ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		/// <summary>
		/// Gets the bell name representing bell `n` where the treble is bell #0.
		/// </summary>
		/// <param name="n">The requested bell's index.</param>
		/// <returns>The character representing bell `n`.</returns>
		public static char GetBellNameIndexingFromZero (int n) => bell_names [n];

		/// <summary>
		/// Gets the bell name representing bell `n` where the treble is bell #1.
		/// </summary>
		/// <param name="n">The requested bell's index.</param>
		/// <returns>The character representing bell `n`.</returns>
		public static Char GetBellNameIndexingFromOne (int n) => bell_names [n - 1];

		/// <summary>
		/// Gets the index of a character representing a bell, where the treble will return 0.
		/// </summary>
		/// <param name="value">The character representing a bell.</param>
		/// <returns>The index (from 0) of the bell.</returns>
		public static int GetBellIndexFromZero (char value) => bell_names.IndexOf (value);

		/// <summary>
		/// Gets the index of a character representing a bell, where the treble will return 1.
		/// </summary>
		/// <param name="value">The character representing a bell.</param>
		/// <returns>The index (from 1) of the bell.</returns>
		public static int GetBellIndexFromOne (char value) => bell_names.IndexOf (value) + 1;
	}

	/// <summary>
	/// An enum which represents parity (oddness/evenness) of changes.
	/// </summary>
	public enum Parity {
		Even = 0,
		Odd = 1
	}

	/// <summary>
	/// An enum to represent a stage.  They also return the correct number when cast to an int.
	/// </summary>
	public enum Stage {
		Singles = 3,
		Minimus = 4,
		Doubles = 5,
		Minor = 6,
		Triples = 7,
		Major = 8,
		Caters = 9,
		Royal = 10,
		Cinques = 11,
		Maximus = 12,
		Sextuples = 13,
		Fourteen = 14,
		Septuples = 15,
		Sixteen = 16,
		Octuples = 17,
		Eighteen = 18,
		Nonuples = 19,
		Twenty = 20,
		Decuples = 21,
		TwentyTwo = 22
	}

	/// <summary>
	/// An enum to store classifications of methods.
	/// </summary>
	public enum Classification {
		Bob,
		Place,
		SlowCourse,
		Principle,
		TrebleBob,
		Surprise,
		Delight,
		TreblePlace,
		Alliance,
		Hybrid,
		Differential,
		Unclassified
	}
}
