using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public class Touch {
		public class CallPoint {
			public Call call;
			public int start_index;

			public int length => call.length;
			public int end_index => start_index + length - 1;

			public PlaceNotation GetNotationAtIndex (int index) {
				return call.place_notations [index - start_index];
			}

			public CallPoint (Call call, int start_index) {
				this.call = call;
				this.start_index = start_index;
			}
		}

		public MethodCall [] method_calls;
		public BasicCall [] basic_calls;

		private int m_conductor_bell = Constants.tenor;
		public int conductor_bell {
			get {
				return m_conductor_bell == Constants.tenor ? (int)stage - 1 : m_conductor_bell;
			}
			set {
				m_conductor_bell = value;
			}
		}

		private Change m_target_change = null;
		public Change target_change {
			get {
				return m_target_change ?? Change.Rounds (stage);
			}
			set {
				m_target_change = value;
			}
		}

		public Change [] changes { get; private set; }
		public Dictionary<int, Call> calls { get; private set; }

		private Dictionary<int, int> change_repeat_frequencies;

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
		public bool is_true => change_repeat_frequencies.Keys.Count == 1 && change_repeat_frequencies.Keys.ToArray () [0] == 1;
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

		public Stage stage {
			get {
				int max_stage = int.MinValue;

				foreach (MethodCall m in method_calls) {
					max_stage = Math.Max (max_stage, (int)m.method.stage);
				}

				return (Stage)max_stage;
			}
		}

		public int Length {
			get {
				if (changes == null) {
					return -1;
				}

				return changes.Length;
			}
		}

		public Change this [int k] {
			get {
				return changes [k];
			}
		}

		void UpdateChangeData () {
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

			change_repeat_frequencies = new Dictionary<int, int> ();

			foreach (string k in change_repeats.Keys) {
				int v = change_repeats [k];

				try {
					change_repeat_frequencies [v] += 1;
				} catch (KeyNotFoundException) {
					change_repeat_frequencies [v] = 1;
				}
			}
		}

		public void ComputeChanges () {
			List<Change> changes = new List<Change> ();
			calls = new Dictionary<int, Call> ();

			int lead_index = 0;
			int sub_lead_index = 0;
			int call_index = 0;

			int absolute_change_index = 0;

			CallPoint current_callpoint = null;

			Change current_change = Change.Rounds (stage);
			Method current_method = method_calls [0].method;

			while (true) {
				// Update calls
				if (current_callpoint == null) {
					if (basic_calls != null && basic_calls.Length > 0) {
						BasicCall basic_call = basic_calls [call_index];
						Call call = basic_call.call;

						if ((sub_lead_index - call.from) % call.every == 0) {
							if (basic_call.call_location.Evaluate (call, current_change, absolute_change_index, this)) {
								current_callpoint = new CallPoint (call, absolute_change_index);
								calls.Add (absolute_change_index, call);
							}
						}
					}
				} else {
					if (absolute_change_index > current_callpoint.end_index) {
						sub_lead_index += current_callpoint.call.cover;
						if (sub_lead_index >= current_method.place_notations.Length) {
							sub_lead_index = 0;
							lead_index += 1;
						}

						call_index += 1;
						if (call_index >= basic_calls.Length) {
							call_index = 0;
						}

						current_callpoint = null;
					}
				}

				// Update change
				PlaceNotation notation;
				if (current_callpoint == null) {
					notation = current_method.place_notations [sub_lead_index];
				} else {
					notation = current_callpoint.GetNotationAtIndex (absolute_change_index);

					if (notation == null) {
						notation = current_method.place_notations [sub_lead_index + absolute_change_index - current_callpoint.start_index];
					}
				}

				current_change = current_change.Transpose (notation);

				// Add change to list
				changes.Add (current_change);

				// Stop if it comes round (to whatever is the target change)
				if (current_change == target_change) {
					break;
				}

				// Update indices
				if (current_callpoint == null) {
					sub_lead_index += 1;

					if (sub_lead_index >= current_method.place_notations.Length) {
						sub_lead_index = 0;
						lead_index += 1;
					}
				}

				absolute_change_index += 1;

				// Stop if touch probably goes on forever
				if (absolute_change_index > 100000) {
					Console.WriteLine ("Broke the laws of human endurance and got to 100,000 changes without coming round.");
					break;
				}
			}

			this.changes = changes.ToArray ();

			UpdateChangeData ();
		}

		public override string ToString () {
			if (changes == null) {
				return "<Touch: changes not computed yet>";
			}

			if (changes.Length == 0) {
				return "<Touch: changes not computed yet>";
			}

			string output = "";
			for (int i = 0; i < changes.Length; i++) {
				Change c = changes [i];

				char call_symbol = ' ';
				if (calls.Keys.Contains (i + 2) && !calls [i + 2].is_plain) {
					call_symbol = calls [i + 2].preferred_notation;
				}

				output += " " + call_symbol + " " + c.ToString () + "\n";
			}

			output += "(" + changes.Length.ToString () + " changes, " + (is_true ? "true" : "false") + ")";

			return output;
		}

		public Touch (Method method, bool automatically_compute_changes = true, int conductor_bell = Constants.tenor) {
			method_calls = new MethodCall [] { new MethodCall (method) };
			basic_calls = new BasicCall [0];

			m_conductor_bell = conductor_bell;

			if (automatically_compute_changes) {
				ComputeChanges ();
			}
		}

		public Touch (Method method, BasicCall[] calls, bool automatically_compute_changes = true, int conductor_bell = Constants.tenor) {
			method_calls = new MethodCall [] { new MethodCall (method) };
			basic_calls = calls;

			m_conductor_bell = conductor_bell;

			if (automatically_compute_changes) {
				ComputeChanges ();
			}
		}
	}

	public class MethodCall {
		public Method method;
		public ICallLocation location;

		public MethodCall () { }

		public MethodCall (Method method) {
			this.method = method;
		}

		public MethodCall (Method method, ICallLocation location) {
			this.method = method;
			this.location = location;
		}
	}

	public class BasicCall {
		public Call call;
		public ICallLocation call_location;

		public BasicCall () { }

		public BasicCall (Call call, ICallLocation call_location) {
			this.call = call;
			this.call_location = call_location;
		}
	}

	public interface ICallLocation {
		bool Evaluate (Call call, Change start_change, int start_index, Touch touch);
	}

	public class CallLocationList : ICallLocation {
		public bool Evaluate (Call call, Change start_change, int start_index, Touch touch) {
			return true;
		}
	}

	public class CallLocationCallingPosition : ICallLocation {
		public char calling_position;

		public bool Evaluate (Call call, Change start_change, int start_index, Touch touch) {
			Change end_change = start_change * call.overall_transposition;

			bool do_call = end_change.IndexOf (touch.conductor_bell) == call.GetCallingPositionIndex (calling_position);

			//Console.WriteLine (end_change.ToString () + " " + (do_call ? call.preferred_notation : ' '));

			return do_call;
		}

		public CallLocationCallingPosition (char calling_position) {
			this.calling_position = calling_position;
		}
	}
}
