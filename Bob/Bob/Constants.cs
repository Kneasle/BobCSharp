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
		/// The string used to compress small integers into a 1 char storage value.
		/// </summary>
		public const string int_value_lookup = @"0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!£$%^&*()-=_+[]{};'#:@~,./<>?\|`¬";

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
		/// <summary>
		/// An even number of called changes are needed to get from rounds to this change.
		/// </summary>
		Even = 0,
		/// <summary>
		/// An odd number of called changes are needed to get from rounds to this change.
		/// </summary>
		Odd = 1
	}

	/// <summary>
	/// An enum to represent a stage.  They also return the correct number when cast to an int.
	/// </summary>
	public enum Stage {
		/// <summary>
		/// A method on 3 bells.
		/// </summary>
		Singles = 3,
		/// <summary>
		/// A method on 4 bells.
		/// </summary>
		Minimus = 4,
		/// <summary>
		/// A method on 5 bells (usually on 6 bells with a tenor cover).
		/// </summary>
		Doubles = 5,
		/// <summary>
		/// A method on 6 bells.
		/// </summary>
		Minor = 6,
		/// <summary>
		/// A method on 7 bells (usually on 8 bells with a tenor cover).
		/// </summary>
		Triples = 7,
		/// <summary>
		/// A method on 8 bells.
		/// </summary>
		Major = 8,
		/// <summary>
		/// A method on 9 bells (usually on 10 bells with a tenor cover).
		/// </summary>
		Caters = 9,
		/// <summary>
		/// A method on 10 bells.
		/// </summary>
		Royal = 10,
		/// <summary>
		/// A method on 11 bells (usually on 12 bells with a tenor cover).
		/// </summary>
		Cinques = 11,
		/// <summary>
		/// A method on 12 bells.
		/// </summary>
		Maximus = 12,
		/// <summary>
		/// A method on 13 bells (usually on 14 bells with a tenor cover).
		/// </summary>
		Sextuples = 13,
		/// <summary>
		/// A method on 14 bells.
		/// </summary>
		Fourteen = 14,
		/// <summary>
		/// A method on 15 bells (usually on 16 bells with a tenor cover).
		/// </summary>
		Septuples = 15,
		/// <summary>
		/// A method on 16 bells.
		/// </summary>
		Sixteen = 16,
		/// <summary>
		/// A method on 17 bells (usually on 18 bells with a tenor cover).
		/// </summary>
		Octuples = 17,
		/// <summary>
		/// A method on 18 bells.
		/// </summary>
		Eighteen = 18,
		/// <summary>
		/// A method on 19 bells (usually on 20 bells with a tenor cover).
		/// </summary>
		Nonuples = 19,
		/// <summary>
		/// A method on 20 bells.
		/// </summary>
		Twenty = 20,
		/// <summary>
		/// A method on 21 bells (usually on 22 bells with a tenor cover).
		/// </summary>
		Decuples = 21,
		/// <summary>
		/// A method on 22 bells.
		/// </summary>
		TwentyTwo = 22
	}

	/// <summary>
	/// An enum to store classifications of methods.
	/// </summary>
	public enum Classification {
		/// <summary>
		/// A method with a plain hunting treble, where the working bells dodge/make points.
		/// </summary>
		Bob,
		/// <summary>
		/// A method with a plain hunting treble, where the working bells never dodge/make points.
		/// </summary>
		Place,
		/// <summary>
		/// A method with a plain hunting treble, and a second hunt bell which makes 2nds over the lead end.
		/// </summary>
		SlowCourse,
		/// <summary>
		/// A method with no hunt bells (all bells are working bells).
		/// </summary>
		Principle,
		/// <summary>
		/// A treble-dodge method where no internal places are made as the treble hunts between dodges.
		/// </summary>
		TrebleBob,
		/// <summary>
		/// A treble-dodge method where internal places are always made as the treble hunts between dodges.
		/// </summary>
		Surprise,
		/// <summary>
		/// A treble-dodge method where some internal places are made as the treble hunts between changes.
		/// </summary>
		Delight,
		/// <summary>
		/// A treble-hunting method where the treble's path is symmetric and spends an equal number of blows in every place it visits.
		/// </summary>
		TreblePlace,
		/// <summary>
		/// A treble-hunting method where the treble's path is symmetric and spends an non-equal number of blows in every place it visits.
		/// </summary>
		Alliance,
		/// <summary>
		/// A treble-hunting method where the treble's path is asymmetric.
		/// </summary>
		Hybrid,
		/// <summary>
		/// A method with no hunt bells, but multiple sets of working bells doing different work.
		/// </summary>
		Differential,
		/// <summary>
		/// An unclassifiable method.  This should never be used.
		/// </summary>
		Unclassified
	}
}
