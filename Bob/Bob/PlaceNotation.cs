using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public class PlaceNotation : ITransposition {
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

		public Stage stage {
			get {
				return (Stage)m_array.Length;
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

		// Static Stuff
		public static PlaceNotation[] DecodeFullNotation (string full_notation, Stage stage) {
			full_notation = full_notation.ToUpper ();

			List<string> segments = new List<string> ();

			string segment = "";
			int index = 0;
			int leadend_index = -1;

			string GetNextSubstringFrom (string [] list) {
				string delimiter = "";

				foreach (string i in list) {
					if (index + i.Length > full_notation.Length) {
						continue;
					}

					if (full_notation.Substring (index, i.Length) == i) {
						delimiter = i;
					}
				}

				return delimiter;
			}

			while (index < full_notation.Length) {
				// Delimiter
				string delimiter = GetNextSubstringFrom (basic_delimiters);

				if (delimiter != "") {
					index += delimiter.Length;

					if (segment != "") {
						segments.Add (segment);
						segment = "";
					}

					continue;
				}

				// Lead-end
				string le_delimiter = GetNextSubstringFrom (leadend_delimiters);

				if (le_delimiter != "") {
					index += le_delimiter.Length;

					if (segment != "") {
						segments.Add (segment);
						segment = "";
					}

					leadend_index = segments.Count;

					continue;
				}

				// X
				string x = GetNextSubstringFrom (x_notations);
				
				if (x != "") {
					index += x.Length;

					if (segment != "") {
						segments.Add (segment);
						segment = "";
					}

					segments.Add (x);

					continue;
				}

				// Just bog standard notation
				string next_char = full_notation.Substring (index, 1);
				if (Constants.bell_names.Contains (next_char)) {
					segment += next_char;
					index += 1;

					continue;
				}

				// If the loop gets this far, just reject the character,
				// 'cos no-one cares about it
				index += 1;
			}

			// Add the last segment
			if (segment != "") {
				segments.Add (segment);
			}

			// Expand lead-end notation
			if (leadend_index != -1) {
				if (leadend_index == 1) {
					// Grandsire-like method
					throw new NotImplementedException ();
				}

				if (leadend_index == segments.Count - 1) {
					// Plain Bob-like method
					List<String> part_to_reverse = segments.GetRange (0, segments.Count - 2);
					string half_lead = segments [segments.Count - 2];
					string lead_end = segments [segments.Count - 1];

					segments.Clear ();
					segments.AddRange (part_to_reverse);
					segments.Add (half_lead);

					part_to_reverse.Reverse ();
					segments.AddRange (part_to_reverse);
					segments.Add (lead_end);
				}
			}

			// Build notations from strings
			PlaceNotation [] notations = new PlaceNotation [segments.Count];

			for (int i = 0; i < segments.Count; i ++) {
				notations [i] = new PlaceNotation (segments [i], stage);
			}

			return notations;
		}

		public static Change CombinePlaceNotations (PlaceNotation [] notations) {
			Change change = Change.Rounds (notations [0].stage);

			foreach (PlaceNotation notation in notations) {
				change *= notation;
			}

			return change;
		}

		public static string [] x_notations =  new string [] { "X", "-" };

		public static string [] basic_delimiters = new string [] { ".", " " };

		public static string [] leadend_delimiters = new string [] { ",", "LE" };
	}

	public class XNotationWithTenorCoverException : Exception {
		public XNotationWithTenorCoverException () { }

		public XNotationWithTenorCoverException (string message) : base (message) { }

		public XNotationWithTenorCoverException (string message, Exception inner) : base (message, inner) { }
	}
}
