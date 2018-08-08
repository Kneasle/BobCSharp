﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	static class Constants {
		public static string bell_names = "1234567890ETABCDFGHIJKLMNOPQRSUVWYZ";
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

	public enum Parity {
		Even = 0,
		Odd = 1
	}
}
