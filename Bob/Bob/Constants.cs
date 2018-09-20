using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public static class Constants {
		public static string bell_names = "1234567890ETABCDFGHIJKLMNOPQRSUVWYZ";

		public const int tenor = -1;
		public const string alpha = "abcdefghijklmnopqrstuvwxyz";
		public const string ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		public static string GetBellNameIndexingFromZero (int n) {
			return bell_names [n].ToString ();
		}

		public static string GetBellNameIndexingFromOne (int n) {
			return bell_names [n - 1].ToString ();
		}

		public static int GetBellIndex (char value) {
			return bell_names.IndexOf (value);
		}
	}

	public enum Parity {
		Even = 0,
		Odd = 1
	}

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
