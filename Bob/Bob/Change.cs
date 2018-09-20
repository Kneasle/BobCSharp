using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public class Change : ITransposition {
		// Properties
		public int [] array { get; private set; }
		
		public int this [int i] => array [i];

		public Stage stage => (Stage)array.Length;

		public Parity parity {
			get {
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
					return Parity.Even;
				} else {
					return Parity.Odd;
				}
			}
		}

		public int order {
			get {
				Change change = this;

				int steps = 1;
				while (change != Change.Rounds (stage)) {
					change *= this;
					steps += 1;
				}

				return steps;
			}
		}

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
		public int [] [] rotating_sets {
			get {
				// Generate dictionary of <bell index, which group it belongs to>
				Dictionary<int, int> set_indices = new Dictionary<int, int> ();

				int index = -1;

				for (int i = 0; i < (int)stage; i ++) {
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

				// Convert this array of lists to a jagged array
				int [] [] output = new int [groups.Length] [];

				for (int i = 0; i < groups.Length; i++) {
					output [i] = groups [i].ToArray ();
				}

				return output;
			}
		}

		public string rotating_sets_as_string {
			get {
				return RotatingSetsToString (rotating_sets);
			}
		}

		// Functions
		public Change Transpose (ITransposition transposable) {
			int [] in_array = array;
			int [] transpose_array = transposable.GetArray ();

			int size = Math.Max (in_array.Length, transpose_array.Length);

			int [] new_array = new int [size];

			for (int i = 0; i < size; i++) {
				int transpose_index = i;

				if (i <= transpose_array.Length) {
					transpose_index = transpose_array [i];
				}

				new_array [i] = transpose_index;

				if (transpose_index <= in_array.Length) {
					new_array [i] = in_array [transpose_index];
				}
			}

			return new Change (new_array);
		}

		public int IndexOf (int bell) {
			for (int i = 0; i < array.Length; i ++) {
				if (array [i] == bell) {
					return i;
				}
			}

			return -1;
		}

		// Functions which implement interfaces' requirements
		public int [] GetArray () {
			return array;
		}

		// Constructors
		public Change (int [] array) {
			this.array = array;
		}

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
		public static Change operator * (Change change, ITransposition transposition) {
			return change.Transpose (transposition);
		}

		public static bool operator == (Change a, Change b) {
			return a.Equals (b);
		}

		public static bool operator != (Change a, Change b) {
			return !(a == b);
		}

		// Static functions
		public static Change Rounds (Stage stage) {
			int [] array = new int [(int)stage];

			for (int i = 0; i < (int)stage; i++) {
				array [i] = i;
			}

			return new Change (array);
		}

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

		public override int GetHashCode () {
			var hashCode = -1323650490;
			hashCode = hashCode * -1521134295 + base.GetHashCode ();
			hashCode = hashCode * -1521134295 + EqualityComparer<int []>.Default.GetHashCode (array);
			return hashCode;
		}

		public override string ToString () {
			string output = "";

			foreach (int i in array) {
				output += Constants.bell_names [i];
			}

			return output;
		}
	}
}
