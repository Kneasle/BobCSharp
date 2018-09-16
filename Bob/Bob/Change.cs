using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public struct Change : ITransposition {
		private int [] m_array;

		// Properties
		public int [] array {
			get {
				return m_array;
			}
		}

		public Stage stage {
			get {
				return (Stage)m_array.Length;
			}
		}

		public Parity parity {
			get {
				int swaps = 0;

				int [] temp_order = (int [])m_array.Clone ();

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

		// Functions
		public Change Transpose (ITransposition transposable) {
			int [] in_array = m_array;
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

		public int [] GetArray () {
			return m_array;
		}

		// Constructors
		public Change (int [] array) {
			m_array = array;
		}

		public Change (string text) {
			List<int> converted_list = new List<int> ();

			foreach (char c in text) {
				if (Constants.bell_names.Contains (c)) {
					converted_list.Add (Constants.bell_names.IndexOf (c));
				}
			}

			m_array = converted_list.ToArray ();
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

		public static Change null_change {
			get {
				return new Change (new int [] { -1 });
			}
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
			hashCode = hashCode * -1521134295 + EqualityComparer<int []>.Default.GetHashCode (m_array);
			return hashCode;
		}

		public override string ToString () {
			string output = "";

			foreach (int i in m_array) {
				output += Constants.bell_names [i];
			}

			return output;
		}
	}
}
