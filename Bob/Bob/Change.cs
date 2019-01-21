using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A class to store rows of ringing, as well as all transpositions (Change objects are read-only).
	/// </summary>
	public class Change : ITransposition {
		// Properties
		/// <summary>
		/// The array of integers (indexed from 0) which represent the change.
		/// </summary>
		public int [] array { get; private set; }
		
		/// <summary>
		/// Gets the bell at some index in the change.
		/// </summary>
		/// <param name="i">The index of the bell (starting from 0).</param>
		/// <returns>The index of the bell in that position (starting from 0).</returns>
		public int this [int i] => array [i];

		/// <summary>
		/// Gets the stage of this change.
		/// </summary>
		public Stage stage => (Stage)array.Length;
		
		private Parity m_parity;
		private bool has_calculated_parity = false;
		/// <summary>
		/// Gets the parity (oddness/evennes) of the change.  This depends on how many called changes are required to get to this change.
		/// </summary>
		public Parity parity {
			get {
				if (!has_calculated_parity) {
					has_calculated_parity = true;

					int swaps = 0;

					int [] temp_order = (int [])array.Clone ();

					for (int i = 0; i < (int)stage; i++) {
						for (int j = 0; j < i; j++) {
							if (temp_order [j] > temp_order [j + 1]) {
								swaps += 1;

								// Effect Swap
								int temp_value = temp_order [j];
								temp_order [j] = temp_order [j + 1];
								temp_order [j + 1] = temp_value;
							}
						}
					}

					if (swaps % 2 == 0) {
						m_parity = Parity.Even;
					} else {
						m_parity = Parity.Odd;
					}
				}

				return m_parity;
			}
		}

		private int m_order = -1;
		/// <summary>
		/// Gets the number of times the change can be repeated before returning to rounds.
		/// </summary>
		public int order {
			get {
				if (m_order == -1) {
					Change change = this;

					int m_order = 1;

					while (change != Rounds (stage)) {
						change *= this;

						m_order += 1;
					}

					// This shouldn't need to be here, but otherwise this function doesn't work.
					return m_order;
				}

				return m_order;
			}
		}

		/// <summary>
		/// Gets a cloned copy of the change.
		/// </summary>
		public Change clone {
			get {
				int [] new_array = new int [(int)stage];

				for (int i = 0; i < (int)stage; i++) {
					new_array [i] = array [i];
				}

				return new Change (new_array);
			}
		}

		// Only look it this function's source code if you want to 
		// spend a whole year working out what it does, or if it breaks.
		// It works.  That's enough for me.
		private int [] [] m_rotating_sets = null;
		/// <summary>
		/// Returns a jagged 2D array of sets of bells which follow a common path when this change is repeated.
		/// </summary>
		/// <example>
		/// In the change "1245376", the 1 and 2 follow their own path; the 3, 4 and 5 rotate together, and the 6 and 7 repeatedly swap.
		/// Therefore the array (indexing from 0) would be [ [0], [1], [2, 3, 4], [6, 7] ].
		/// </example>
		public int [] [] rotating_sets {
			get {
				if (m_rotating_sets == null) {
					// Generate dictionary of <bell index, which group it belongs to>
					Dictionary<int, int> set_indices = new Dictionary<int, int> ();

					int index = -1;

					for (int i = 0; i < (int)stage; i++) {
						if (set_indices.Keys.Contains (i)) {
							continue;
						}

						index += 1;

						set_indices.Add (i, index);

						Change change = this;

						if (change [i] == i) {
							continue;
						}

						while (true) {
							if (change [i] == i) {
								break;
							}

							set_indices.Add (change.IndexOf (i), index);

							change *= this;
						}
					}

					// Create an array of lists
					List<int> [] groups = new List<int> [index + 1];

					for (int i = 0; i < groups.Length; i++) {
						groups [i] = new List<int> ();
					}

					foreach (int key in set_indices.Keys) {
						groups [set_indices [key]].Add (key);
					}

					// Convert this array of lists to a jagged array, and store it in m_rotating_sets
					m_rotating_sets = new int [groups.Length] [];

					for (int i = 0; i < groups.Length; i++) {
						m_rotating_sets [i] = groups [i].ToArray ();
					}
				}

				return m_rotating_sets;
			}
		}

		/// <summary>
		/// Gets the rotating sets of this change as a string, to prevent user conversions.
		/// </summary>
		public string rotating_sets_as_string {
			get {
				return RotatingSetsToString (rotating_sets);
			}
		}

		// Functions
		/// <summary>
		/// Transposes this change by another transposable object, e.g. place notations and other changes.
		/// </summary>
		/// <param name="transposable">The transposable object by which this change will be transposed.</param>
		/// <returns>The transposed change.</returns>
		public Change Transpose (ITransposition transposable) {
			int [] in_array = array;
			int [] transpose_array = transposable.GetTranspositionArray (this);

			int size = Math.Max (in_array.Length, transpose_array.Length);

			int [] new_array = new int [size];

			for (int i = 0; i < size; i++) {
				int transpose_index = i;

				if (i < transpose_array.Length) {
					transpose_index = transpose_array [i];
				}

				new_array [i] = transpose_index;

				if (transpose_index <= in_array.Length) {
					new_array [i] = in_array [transpose_index];
				}
			}

			return new Change (new_array);
		}

		/// <summary>
		/// Gets the index of a given bell (all indices from 0).
		/// </summary>
		/// <param name="bell">The bell you want to find the index of (indexing from 0).</param>
		/// <returns>The index (from 0) of the bell.  Returns -1 if the bell isn't in the change.</returns>
		public int IndexOf (int bell) {
			for (int i = 0; i < array.Length; i ++) {
				if (array [i] == bell) {
					return i;
				}
			}

			return -1;
		}

		// Functions which implement interfaces' requirements
		/// <summary>
		/// Gets the array of the change.  Implements <see cref="ITransposition"/>.
		/// </summary>
		/// <returns>The array representing the transposition caused by this change.</returns>
		public int [] GetTranspositionArray (Change other) => array;

		// Constructors
		/// <summary>
		/// Creates a change from an integer array.
		/// </summary>
		/// <param name="array">The array of bells (indexed from 0) in the change you want to create.</param>
		public Change (int [] array) {
			this.array = array;
		}

		/// <summary>
		/// Creates a change from the text (e.g. "13256478").
		/// </summary>
		/// <param name="text">The text representing your change.</param>
		public Change (string text) {
			List<int> converted_list = new List<int> ();

			foreach (char c in text) {
				if (Constants.bell_names.Contains (c)) {
					converted_list.Add (Constants.bell_names.IndexOf (c));
				}
			}

			array = converted_list.ToArray ();
		}

		// Operators
		/// <summary>
		/// Shorthand for transposing a changes by any transposable object.
		/// </summary>
		/// <param name="change">The change which is being transposed.</param>
		/// <param name="transposition">The object which the change is being transposed by.</param>
		/// <returns>The transposed change.</returns>
		public static Change operator * (Change change, ITransposition transposition) {
			return change.Transpose (transposition);
		}

		/// <summary>
		/// Shorthand for transposing a change by a list of place notations, e.g. a list of the place notations in a plain lead of a method.
		/// </summary>
		/// <param name="change">The change which is being transposed.</param>
		/// <param name="place_notations">The list of place notations over which the change will be transposed.</param>
		/// <returns>The transposed change.</returns>
		public static Change operator * (Change change, PlaceNotation [] place_notations) {
			return change.Transpose (PlaceNotation.CombinePlaceNotations (place_notations));
		}
		
		/// <summary>
		/// Checks whether changes `a` and `b` are equal.
		/// </summary>
		/// <param name="a">The left hand change.</param>
		/// <param name="b">The right hand change.</param>
		/// <returns>True if the changes are equal, otherwise false.</returns>
		public static bool operator == (Change a, Change b) {
			return a.Equals (b);
		}

		/// <summary>
		/// Checks whether changes `a` and `b` are not equal.
		/// </summary>
		/// <param name="a">The left hand change.</param>
		/// <param name="b">The right hand change.</param>
		/// <returns>False if the changes are equal, otherwise true.</returns>
		public static bool operator != (Change a, Change b) {
			return !(a == b);
		}

		// Static functions
		/// <summary>
		/// Gets a change representing rounds on a given stage.
		/// </summary>
		/// <param name="stage">The stage which you want rounds for.</param>
		/// <returns>A change representing rounds.</returns>
		public static Change Rounds (Stage stage) {
			int [] array = new int [(int)stage];

			for (int i = 0; i < (int)stage; i++) {
				array [i] = i;
			}

			return new Change (array);
		}

		/// <summary>
		/// Converts a jagged array of rotating sets into a human readable string.
		/// </summary>
		/// <param name="rotating_sets">The jagged array of rotating sets.</param>
		/// <returns>A human readable string representing the rotating sets.</returns>
		public static string RotatingSetsToString (int [] [] rotating_sets) {
			string s = "";

			foreach (int [] arr in rotating_sets) {
				s += "[";

				bool is_first = true;
				foreach (int i in arr) {
					s += (is_first ? "" : " ") + i;
					is_first = false;
				}

				s += "]";
			}

			return s;
		}

		// Equality overrides
		/// <summary>
		/// Determines whether this change equals another object.
		/// </summary>
		/// <param name="obj">The other change.</param>
		/// <returns>True if this change is equal to `obj`, otherwise false.</returns>
		public override bool Equals (object obj) {
			if (!(obj is Change)) {
				return false;
			}

			Change other = (Change)obj;

			if (other.array.Length != array.Length) {
				return false;
			}

			for (int i = 0; i < array.Length; i ++) {
				if (other.array [i] != array [i]) {
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets the integer hash code of the change (the compiler wanted me to do this).
		/// </summary>
		/// <returns>The hash code of this change.</returns>
		public override int GetHashCode () {
			var hashCode = -1323650490;
			hashCode = hashCode * -1521134295 + base.GetHashCode ();
			hashCode = hashCode * -1521134295 + EqualityComparer<int []>.Default.GetHashCode (array);
			return hashCode;
		}

		/// <summary>
		/// Generates a human readable string representation of this change, as you would expect it to be written.
		/// </summary>
		/// <returns>A string representing the change.</returns>
		public override string ToString () {
			string output = "";

			foreach (int i in array) {
				output += Constants.bell_names [i];
			}

			return output;
		}
	}
}
