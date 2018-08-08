using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public struct PlaceNotation : ITransposition {
		private int [] m_array;
		private string m_notation;

		// Properties
		public int [] array {
			get {
				return m_array;
			}
		}

		public string notation {
			get {
				return m_notation;
			}
		}

		// Functions
		public int [] GetArray () {
			return m_array;
		}

		// Constructors
		public PlaceNotation (string notation, Stage stage) {
			m_array = new int [(int)stage];
			m_notation = notation;
			
			if (x_notations.Contains (notation)) {
				// Inputted an X notation

				if ((int)stage % 2 == 0) {
					for (int i = 0; i < (int)stage; i += 2) {
						m_array [i] = i + 1;
						m_array [i + 1] = i;
					}
				} else {
					throw new XNotationWithTenorCoverException ("X notation used in notation '" + notation + "' with stage '" + stage.ToString () + "'.");
				}
			} else {
				// Inputted a non-X notation

				List<int> places_made = new List<int> ();

				foreach (char c in notation) {
					if (Constants.bell_names.Contains (c)) {
						places_made.Add (Constants.bell_names.IndexOf (c));
					}
				}

				// Implicit places
				int min_place = places_made.Min ();
				int max_place = places_made.Max ();

				if (min_place > 0 && min_place % 2 != 0) {
					places_made.Add (0);
				}

				if (max_place < (int)stage - 1 && ((int)stage - max_place) % 2 == 0) {
					places_made.Add ((int)stage - 1);
				}

				// Compute transposition array
				int i = 0;
				while (i < m_array.Length) {
					if (places_made.Contains (i) || places_made.Contains (i + 1)) {
						m_array [i] = i;

						i += 1;
					} else {
						m_array [i] = i + 1;
						m_array [i + 1] = i;

						i += 2;
					}
				}
			}
		}

		// Constants
		public static string [] x_notations =  new string [] { "X", "-" };

		public static string [] delimiters = new string [] { ".", " " };

		public static string [] leadend_delimiters = new string [] { ",", "LE" };
	}

	public class XNotationWithTenorCoverException : Exception {
		public XNotationWithTenorCoverException () { }

		public XNotationWithTenorCoverException (string message) : base (message) { }

		public XNotationWithTenorCoverException (string message, Exception inner) : base (message, inner) { }
	}
}
