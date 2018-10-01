﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A base class from which other touch objects inherit.  Contains common functionality between method touches and called change touches.
	/// </summary>
	public abstract class TouchBase {
		/// <summary>
		/// A class which stores the state of a change, to check whether or not touches will ever come round.
		/// </summary>
		/// <remarks>
		/// Basically, in a touch which comes round, no change states should ever be repeated.
		/// Thus, if a new change state has already occured in a stored list, then the touch will never come round.
		/// </remarks>
		public class ChangeState {
			/// <summary>
			/// The change reached within the touch.
			/// </summary>
			public Change change { get; private set; }
			/// <summary>
			/// The index within the method calls in the touch.
			/// </summary>
			public int method_call_index { get; private set; }
			/// <summary>
			/// THe index within the calls in the touch.
			/// </summary>
			public int basic_call_index { get; private set; }

			/// <summary>
			/// Determines whether this object equals another object.
			/// </summary>
			/// <param name="obj">The other object.</param>
			/// <returns>True if this object is equal to the other object.</returns>
			public override bool Equals (object obj) {
				if (obj.GetType () != typeof (ChangeState)) {
					return false;
				}

				ChangeState other_state = obj as ChangeState;

				if (other_state.change != change) { return false; }
				if (other_state.method_call_index != method_call_index) { return false; }
				if (other_state.basic_call_index != basic_call_index) { return false; }

				return true;
			}

			/// <summary>
			/// Gets the hash code of the object.
			/// </summary>
			/// <returns>The hash code of this object.</returns>
			public override int GetHashCode () => base.GetHashCode ();

			/// <summary>
			/// Creates a ChangeState object
			/// </summary>
			/// <param name="change">The current change reached in the touch.</param>
			/// <param name="method_call_index">The index within the splices of the touch.</param>
			/// <param name="basic_call_index">The index within the calls of the touch.</param>
			public ChangeState (Change change, int method_call_index, int basic_call_index) {
				this.change = change;
				this.method_call_index = method_call_index;
				this.basic_call_index = basic_call_index;
			}
		}

		/// <summary>
		/// A dictionary to store where margin calls (placed to the left of the changes) are to be placed (for use in <see cref="ToString ()"/>).
		/// </summary>
		public Dictionary<int, char> margin_calls = new Dictionary<int, char> ();
		/// <summary>
		/// A dictionary to store calls placed to the right of the changes
		/// </summary>
		public Dictionary<int, string> right_hand_calls = new Dictionary<int, string> ();
		/// <summary>
		/// A list of the indices of each lead end in this touch.  Used in <see cref="ToString"/>.
		/// </summary>
		public List<int> lead_ends_line_indices = new List<int> ();

		private Change m_target_change = null;
		/// <summary>
		/// The change at which the touch will stop.  Defaults to rounds.
		/// </summary>
		public Change target_change {
			get => m_target_change ?? Change.Rounds (stage);

			set {
				if (value != m_target_change) {
					m_changes = null;
				}

				m_target_change = value;
			}
		}
		
		private Change [] m_changes = null;
		/// <summary>
		/// An array of all the changes in the touch.  Calls <see cref="ComputeChanges"/> once when accessed.
		/// </summary>
		public Change [] changes {
			get {
				if (m_changes is null) {
					m_changes = ComputeChanges ();
				}

				return m_changes;
			}
		}

		/// <summary>
		/// The number of changes in this touch.
		/// </summary>
		public int Length {
			get {
				if (changes == null) {
					return -1;
				}

				return changes.Length;
			}
		}

		/// <summary>
		/// Gets the change at a given index in the touch.
		/// </summary>
		/// <param name="i">The index of the requested change.</param>
		/// <returns>The change at index `i`.</returns>
		public Change this [int i] => changes [i];

		private Dictionary<int, int> m_change_repeat_frequencies = null;
		/// <summary>
		/// A dictionary of (key: the number of times each change repeats, value: the number of changes which repeat this many times).
		/// </summary>
		public Dictionary<int, int> change_repeat_frequencies {
			get {
				if (m_change_repeat_frequencies is null) {
					ComputeChangeRepeatFrequencies ();
				}

				return m_change_repeat_frequencies;
			}
		}

		/// <summary>
		/// True if every possible change is rung once and once only.
		/// </summary>
		public bool is_extent {
			get {
				if (change_repeat_frequencies.Keys.Count != 1) {
					return false;
				}

				if (change_repeat_frequencies.Keys.ToArray () [0] != 1) {
					return false;
				}

				return change_repeat_frequencies [1] == Utils.Factorial ((int)stage);
			}
		}
		/// <summary>
		/// True if every possible change is rung an equal number of times. 
		/// </summary>
		public bool is_multiple_extent {
			get {
				if (change_repeat_frequencies.Keys.Count != 1) {
					return false;
				}

				if (change_repeat_frequencies.Keys.ToArray () [0] != 1) {
					return false;
				}

				return change_repeat_frequencies [1] % Utils.Factorial ((int)stage) == 0;
			}
		}
		/// <summary>
		/// True if no change is repeated more than once.
		/// </summary>
		public bool is_true => change_repeat_frequencies.Keys.Count == 1 && change_repeat_frequencies.Keys.ToArray () [0] == 1;
		/// <summary>
		/// True if this touch could be rung for a quarter peal (i.e. no change is rung more than one more time than any other).
		/// </summary>
		public bool is_quarter_peal_true {
			get {
				int [] keys = change_repeat_frequencies.Keys.ToArray ();

				if (keys.Length > 2 || keys.Length == 0) {
					return false;
				}

				if (keys.Length == 1) {
					return true;
				}

				return Math.Abs (keys [1] - keys [0]) == 1;
			}
		}

		/// <summary>
		/// Set to true if the touch comes round, otherwise false.
		/// </summary>
		public bool comes_round = true;

		/// <summary>
		/// Generates the dictionary of change repeats (could be computationally intensive for long touches).
		/// </summary>
		private void ComputeChangeRepeatFrequencies () {
			if (changes == null) {
				return;
			}

			// Run duplication / falseness
			Dictionary<string, int> change_repeats = new Dictionary<string, int> ();

			foreach (Change change in changes) {
				string c = change.ToString ();

				try {
					change_repeats [c] += 1;
				} catch (KeyNotFoundException) {
					change_repeats [c] = 1;
				}
			}

			m_change_repeat_frequencies = new Dictionary<int, int> ();

			foreach (string k in change_repeats.Keys) {
				int v = change_repeats [k];

				try {
					m_change_repeat_frequencies [v] += 1;
				} catch (KeyNotFoundException) {
					m_change_repeat_frequencies [v] = 1;
				}
			}
		}

		/// <summary>
		/// Computes changes and the change repeat frequencies needed to determine falseness.
		/// </summary>
		public void ComputeAll () {
			ComputeChanges ();
			ComputeChangeRepeatFrequencies ();
		}
		
		/// <summary>
		/// Returns a string representing this touch (could be very large for long touches).
		/// </summary>
		/// <returns>A string representation of this touch.</returns>
		public override string ToString () {
			if (changes == null) {
				return "<Touch: changes not computed yet>";
			}

			if (changes.Length == 0) {
				return "<Touch: changes not computed yet>";
			}

			string output = "   " + Change.Rounds (stage) + (right_hand_calls.ContainsKey (-1) ? " " + right_hand_calls [-1] : "") + "\n";

			for (int i = 0; i < changes.Length; i++) {
				Change c = changes [i];

				char call_symbol = ' ';
				if (margin_calls.Keys.Contains (i)) {
					call_symbol = margin_calls [i];
				}

				if (lead_ends_line_indices.Contains (i)) {
					output += "   ";

					for (int p = 0; p < (int)stage; p++) {
						output += "-";
					}

					output += "\n";
				}

				output += " " + call_symbol + " " + c.ToString () + (right_hand_calls.Keys.Contains (i) ? " " + right_hand_calls [i] : "") + "\n";
			}

			if (!comes_round) {
				output += "<This touch will never come round.>\n";
			}

			output += "(" + changes.Length.ToString () + " changes, " + (is_true ? "true" : "false") + ")";

			return output;
		}

		// Abstract stuff
		/// <summary>
		/// A function to populate the changes array.
		/// </summary>
		/// <returns></returns>
		public abstract Change [] ComputeChanges ();

		/// <summary>
		/// The stage of this touch.
		/// </summary>
		public abstract Stage stage { get; }
	}
}
