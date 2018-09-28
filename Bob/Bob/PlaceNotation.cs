using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A class to represent any single place notation.  Once created, <see cref="PlaceNotation"/> objects cannot be changed.
	/// </summary>
	public class PlaceNotation : ITransposition {
		// These 3 fields are set by the constructor.
		/// <summary>
		/// The string notation for this place notation.
		/// </summary>
		public string notation { get; private set; }
		/// <summary>
		/// An array representing the transposition caused by this place notation.
		/// </summary>
		public int [] array { get; private set; }
		/// <summary>
		/// An array of places (indexed from 0) made in this place notation.
		/// </summary>
		public int [] places_made { get; private set; }

		/// <summary>
		/// Gets the compacted notation (i.e. with implicit places removed).
		/// </summary>
		public string compact_notation {
			get {
				if (places_made.Length <= 1) {
					return notation;
				}

				if (places_made.Length == 2 && places_made [0] == 0 && places_made [1] == (int)stage - 1) {
					return Constants.bell_names [0].ToString ();
				}

				string output = "";

				List<int> places = places_made.ToList ();

				places.Sort ();

				foreach (int place in places) {
					if (place != 0 && place != (int)stage - 1) {
						output += Constants.bell_names [place];
					}
				}

				return output;
			}
		}

		// Properties
		/// <summary>
		/// Gets the stage of this place notation.
		/// </summary>
		public Stage stage => (Stage)array.Length;

		private bool m_is_12;
		private bool has_set_is_12 = false;
		/// <summary>
		/// True if this place notation is a "12" or a "12n" place notation.
		/// </summary>
		public bool is_12 {
			get {
				if (!has_set_is_12) {
					has_set_is_12 = true;

					if ((int)stage % 2 == 0) {
						m_is_12 = Enumerable.SequenceEqual (places_made, new int [] { 0, 1 });
					} else {
						m_is_12 = Enumerable.SequenceEqual (places_made, new int [] { 0, 1, (int)stage - 1 });
					}
				}

				return m_is_12;
			}
		}

		private bool m_is_1n;
		private bool has_set_is_1n = false;
		/// <summary>
		/// True if this place notation is "1n".  For all odd-staged notations, this returns false.
		/// </summary>
		public bool is_1n {
			get {
				if (!has_set_is_1n) {
					has_set_is_1n = true;

					if ((int)stage % 2 == 0) {
						m_is_1n = Enumerable.SequenceEqual (places_made, new int [] { 0, (int)stage - 1 });
					} else {
						m_is_1n = false;
					}
				}

				return m_is_1n;
			}
		}

		/// <summary>
		/// True if this place notation is an "X" type notation.
		/// </summary>
		public bool is_x => places_made.Length == 0;

		private bool m_has_internal_places;
		private bool has_set_has_internal_places = false;
		/// <summary>
		/// True if this place notation contains internal places (places not at either end of the notation).
		/// </summary>
		public bool has_internal_places {
			get {
				if (!has_set_has_internal_places) {
					has_set_has_internal_places = true;

					for (int i = 1; i < (int)stage - 1; i++) {
						if (places_made.Contains (i)) {
							m_has_internal_places = true;
							return true;
						}
					}

					m_has_internal_places = false;
					return false;
				}

				return m_has_internal_places;
			}
		}

		// Functions
		/// <summary>
		/// Gets the array representing the transposition of this place notation.  Implements <see cref="ITransposition"/>.
		/// </summary>
		/// <returns>An array representing this transposition.</returns>
		public int [] GetArray (Change original) => array;

		/// <summary>
		///	Checks equality between this object and another object.
		/// </summary>
		/// <param name="obj">The other object.</param>
		/// <returns>True if this is equal to `obj`.</returns>
		public override bool Equals (object obj) {
			if (!(obj is PlaceNotation)) {
				return false;
			}

			PlaceNotation other = obj as PlaceNotation;

			if (other.array.Length != array.Length) {
				return false;
			}

			for (int i = 0; i < array.Length; i++) {
				if (other.array [i] != array [i]) {
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets the hash code for this place notation.
		/// </summary>
		/// <returns>The hash code summing up this place notation.</returns>
		public override int GetHashCode () {
			return -1984574666 + EqualityComparer<int []>.Default.GetHashCode (array);
		}

		/// <summary>
		/// Gets the string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString () {
			return notation + " => " + Change.Rounds (stage) * this;
		}

		// Constructors
		/// <summary>
		/// Generates a single <see cref="PlaceNotation"/> object from a string notation and a <see cref="Stage"/>.
		/// </summary>
		/// <param name="notation">The string notation of this place notation.</param>
		/// <param name="stage">The stage of this place notation.</param>
		public PlaceNotation (string notation, Stage stage) {
			array = new int [(int)stage];
			this.notation = notation;
			
			if (x_notations.Contains (notation)) {
				places_made = new int [0];

				// Inputted an X notation
				if ((int)stage % 2 == 0) {
					for (int i = 0; i < (int)stage; i += 2) {
						array [i] = i + 1;
						array [i + 1] = i;
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

				this.places_made = places_made.ToArray ();

				// Compute transposition array
				int i = 0;
				while (i < array.Length) {
					if (places_made.Contains (i) || places_made.Contains (i + 1)) {
						array [i] = i;

						i += 1;
					} else {
						array [i] = i + 1;
						array [i + 1] = i;

						i += 2;
					}
				}
			}
		}

		// Static Stuff
		/// <summary>
		/// Converts a string of place notations (with lead end shortcuts, implicit places, etc.) to an array of <see cref="PlaceNotation"/> objects.
		/// </summary>
		/// <param name="full_notation">The string of place notations.</param>
		/// <param name="stage">The <see cref="Stage"/> of the place notations.</param>
		/// <returns>An array of converted place notations</returns>
		public static PlaceNotation [] DecodeFullNotation (string full_notation, Stage stage) {
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
					List<String> part_to_reverse = segments.GetRange (1, segments.Count - 2);
					string half_lead = segments [segments.Count - 1];
					string lead_end = segments [0];

					segments.Clear ();
					segments.Add (lead_end);
					segments.AddRange (part_to_reverse);
					segments.Add (half_lead);

					part_to_reverse.Reverse ();
					segments.AddRange (part_to_reverse);
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

		/// <summary>
		/// Generates the combined transposition caused by consecutively applying an array of place notations.
		/// </summary>
		/// <param name="notations">The list of place notations, in the order of transposition.</param>
		/// <returns>The change representing the combined transposition.</returns>
		public static Change CombinePlaceNotations (PlaceNotation [] notations) {
			Change change = Change.Rounds (notations [0].stage);

			foreach (PlaceNotation notation in notations) {
				change *= notation;
			}

			return change;
		}

		/// <summary>
		/// Generates a list of every change caused by applying each of `notations`.
		/// </summary>
		/// <param name="notations">The notations to apply.</param>
		/// <param name="start_change">Set the start change to something other than rounds.</param>
		/// <returns></returns>
		public static Change [] GenerateChangeArray (PlaceNotation [] notations, Change start_change = null) {
			Change change = start_change ?? Change.Rounds (notations [0].stage);

			Change [] changes = new Change [notations.Length];
			for (int i = 0; i < notations.Length; i++) {
				change *= notations [i];

				changes [i] = change;
			}

			return changes;
		}

		/// <summary>
		/// Removes implicit places from a given place notation, as well as adding symmetry.
		/// </summary>
		/// <param name="input">The input place notation.</param>
		/// <param name="stage">The stage on which this place notation is being rung.</param>
		/// <returns>The place notation compressed as much as possible.</returns>
		public static string CompressPlaceNotation (string input, Stage stage) {
			Method method = new Method (input, "", stage, null, false);

			string output = "";

			switch (method.symmetry_type) {
				case Method.SymmetryType.Asymmetric:
					for (int i = 0; i < method.lead_length; i++) {
						PlaceNotation current_notation = method.place_notations [i];
						PlaceNotation next_notation = i == method.lead_length - 1 ? null : method.place_notations [i + 1];

						if (current_notation.is_x) {
							output += current_notation.compact_notation;
						} else if (next_notation == null || next_notation.is_x) {
							output += current_notation.compact_notation;
						} else {
							output += current_notation.compact_notation + basic_delimiters [0];
						}
					}

					break;
				case Method.SymmetryType.PlainBobLike:
					for (int i = 0; i < method.lead_length / 2; i++) {
						PlaceNotation current_notation = method.place_notations [i];
						PlaceNotation next_notation = i == method.lead_length / 2 - 1 ? null : method.place_notations [i + 1];

						if (current_notation.is_x) {
							output += current_notation.compact_notation;
						} else if (next_notation == null || next_notation.is_x) {
							output += current_notation.compact_notation;
						} else {
							output += current_notation.compact_notation + basic_delimiters [0];
						}
					}

					output += leadend_delimiters [0] + method.place_notations [method.lead_length - 1].compact_notation;

					break;
				case Method.SymmetryType.GrandsireLike:
					output += method.place_notations [0].compact_notation + leadend_delimiters [0];

					for (int i = 1; i < method.lead_length / 2 + 1; i++) {
						PlaceNotation current_notation = method.place_notations [i];
						PlaceNotation next_notation = i == method.lead_length / 2 ? null : method.place_notations [i + 1];

						if (current_notation.is_x) {
							output += current_notation.compact_notation;
						} else if (next_notation == null || next_notation.is_x) {
							output += current_notation.compact_notation;
						} else {
							output += current_notation.compact_notation + basic_delimiters [0];
						}
					}

					break;
			}

			return output;
		}

		// Constants
		/// <summary>
		/// A customisable array of possible notations for a 'cross' place notations.
		/// </summary>
		public static string [] x_notations =  new string [] { "X", "-" };

		/// <summary>
		/// A customisable array of generic delimiters for between changes.
		/// </summary>
		public static string [] basic_delimiters = new string [] { ".", " " };

		/// <summary>
		/// A customisable array of delimiters to represent lead end symmetry.
		/// </summary>
		public static string [] leadend_delimiters = new string [] { ",", "LE" };

		// Operators
		/// <summary>
		/// Operator which returns true if two <see cref="PlaceNotation"/> objects are equal.
		/// </summary>
		/// <param name="notation1">The left hand notation.</param>
		/// <param name="notation2">The right hand notation.</param>
		/// <returns>True if `notation1` equals `notation2`.</returns>
		public static bool operator == (PlaceNotation notation1, PlaceNotation notation2) => EqualityComparer<PlaceNotation>.Default.Equals (notation1, notation2);

		/// <summary>
		/// Operator which returns true if two <see cref="PlaceNotation"/> objects are not equal.
		/// </summary>
		/// <param name="notation1">The left hand notation.</param>
		/// <param name="notation2">The right hand notation.</param>
		/// <returns>True if `notation1` does not equals `notation2`.</returns>
		public static bool operator != (PlaceNotation notation1, PlaceNotation notation2) {
			return !(notation1 == notation2);
		}
	}

	/// <summary>
	/// An exception to be thrown when the 'X' notation is used on an odd-stage method.
	/// </summary>
	public class XNotationWithTenorCoverException : Exception {
		/// <summary>
		/// Throws a new exception with no message.
		/// </summary>
		public XNotationWithTenorCoverException () { }

		/// <summary>
		/// Throws a new exception with a message.
		/// </summary>
		/// <param name="message">The message to display.</param>
		public XNotationWithTenorCoverException (string message) : base (message) { }

		/// <summary>
		/// Throw a new exception with a message and another exception.
		/// </summary>
		/// <param name="message">The message display.</param>
		/// <param name="inner">The other exception.</param>
		public XNotationWithTenorCoverException (string message, Exception inner) : base (message, inner) { }
	}
}
