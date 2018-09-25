using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A class to store a representation of any touch.
	/// </summary>
	public class Touch {
		/// <summary>
		/// A class to store a pairing of a Call and the point at which it is called.
		/// </summary>
		public class CallPoint {
			/// <summary>
			/// The call which is being called.
			/// </summary>
			public Call call;
			/// <summary>
			/// The index of the first change which the call is called.
			/// </summary>
			public int start_index;

			/// <summary>
			/// The number of changes covered by this <see cref="CallPoint"/>.
			/// </summary>
			public int length => call.length;
			/// <summary>
			/// The index of the last change covered by this <see cref="CallPoint"/>. 
			/// </summary>
			public int end_index => start_index + length - 1;

			/// <summary>
			/// Gets the <see cref="PlaceNotation"/> at a given absolute index since the start of the touch.
			/// </summary>
			/// <param name="index">The absolute index of the <see cref="PlaceNotation"/>.</param>
			/// <returns>The place notation at absolute index `index`.</returns>
			public PlaceNotation GetNotationAtIndex (int index) {
				return call.place_notations [index - start_index];
			}

			/// <summary>
			/// Creates a <see cref="CallPoint"/> from a call and an index.
			/// </summary>
			/// <param name="call">The call being called.</param>
			/// <param name="start_index">The index of the first change of the call.</param>
			public CallPoint (Call call, int start_index) {
				this.call = call;
				this.start_index = start_index;
			}
		}

		/// <summary>
		/// A class to send parameters into <see cref="ICallLocation"/> methods.
		/// </summary>
		public class CallLocationParameters {
			/// <summary>
			/// The current <see cref="Method"/> being rung.
			/// </summary>
			public Method current_method;
			/// <summary>
			/// The last change before the potential call takes effect.
			/// </summary>
			public Change start_change;
			/// <summary>
			/// The number of changes since the start of the touch.
			/// </summary>
			public int start_index;
			/// <summary>
			/// The number of changes since the last lead end.
			/// </summary>
			public int sub_lead_index;
			/// <summary>
			/// The number of lead ends since the start of the touch.
			/// </summary>
			public int lead_index;
			/// <summary>
			/// The touch from which this class is being generated.
			/// </summary>
			public Touch touch;

			/// <summary>
			/// Creates a fully defined <see cref="CallLocationParameters"/> object.
			/// </summary>
			/// <param name="current_method">The current <see cref="Method"/> being rung.</param>
			/// <param name="start_change">The last change before the potential call takes effect.</param>
			/// <param name="start_index">The number of changes since the start of the touch.</param>
			/// <param name="sub_lead_index">The number of changes sinc the last lead end.s</param>
			/// <param name="lead_index">The number of lead ends since the start of the touch.</param>
			/// <param name="touch">The touch from which this class is being generated.</param>
			public CallLocationParameters (Method current_method, Change start_change, int start_index, int sub_lead_index, int lead_index, Touch touch) {
				this.current_method = current_method;
				this.start_change = start_change;
				this.start_index = start_index;
				this.sub_lead_index = sub_lead_index;
				this.lead_index = lead_index;
				this.touch = touch;
			}
		}

		/// <summary>
		/// An interface for denoting where calls can be called.
		/// </summary>
		public interface ICallLocation {
			/// <summary>
			/// This should return true if the given data should result in a call being called.
			/// </summary>
			/// <param name="call">The call that would be called.</param>
			/// <param name="location">The location where the call would be called.</param>
			/// <returns>True if the call should be called.</returns>
			bool EvaluateBasicCall (Call call, CallLocationParameters location);

			/// <summary>
			/// This should return true if the given data should result in a method splice being called.
			/// </summary>
			/// <param name="next_method">The method being spliced to.</param>
			/// <param name="location">The location of the potential call.</param>
			/// <returns>True if the method splice should happen here.</returns>
			bool EvaluateMethodCall (Method next_method, CallLocationParameters location);
		}

		/// <summary>
		/// A class to specify the locations of calls which are given as a list (so calls are always called.
		/// </summary>
		public class CallLocationList : ICallLocation {
			/// <summary>
			/// This should return true if the given data should result in a call being called.
			/// </summary>
			/// <param name="call">The call that would be called.</param>
			/// <param name="location">The location where the call would be called.</param>
			/// <returns>Always true</returns>
			public bool EvaluateBasicCall (Call call, CallLocationParameters location) => true;

			/// <summary>
			/// This should return true if the given data should result in a method splice being called.
			/// </summary>
			/// <param name="next_method">The method being spliced to.</param>
			/// <param name="location">The location of the potential call.</param>
			/// <returns>Always true.</returns>
			public bool EvaluateMethodCall (Method next_method, CallLocationParameters location) => true;
		}

		/// <summary>
		/// A class to specify the location of a calling-position based call.
		/// </summary>
		public class CallLocationCallingPosition : ICallLocation {
			/// <summary>
			/// The notation for the calling position
			/// </summary>
			public char calling_position;

			/// <summary>
			/// This should return true if the given data should result in a call being called.
			/// </summary>
			/// <param name="call">The call that would be called.</param>
			/// <param name="location">The location where the call would be called.</param>
			/// <returns>True if the call should be called.</returns>
			public bool EvaluateBasicCall (Call call, CallLocationParameters location) {
				Change end_change = location.start_change * call.overall_transposition;

				return end_change.IndexOf (location.touch.conductor_bell) == call.GetCallingPositionIndex (calling_position);
			}

			/// <summary>
			/// This should return true if the given data should result in a method splice being called.
			/// </summary>
			/// <param name="next_method">The method being spliced to.</param>
			/// <param name="location">The location of the potential call.</param>
			/// <returns>True if the method splice should happen here.</returns>
			public bool EvaluateMethodCall (Method next_method, CallLocationParameters location) {
				throw new NotImplementedException ();
			}

			/// <summary>
			/// Creates a <see cref="CallLocationCallingPosition"/> class given a calling position notation.
			/// </summary>
			/// <param name="calling_position">The notation of the calling position.</param>
			public CallLocationCallingPosition (char calling_position) {
				this.calling_position = calling_position;
			}
		}

		/// <summary>
		/// An array of method splicing calls (splicing in development).
		/// </summary>
		public MethodCall [] method_calls;
		/// <summary>
		/// An array of basic (e.g. Bob, Single, Plain) calls and their locations.
		/// </summary>
		public BasicCall [] basic_calls;
		
		private int m_conductor_bell = Constants.tenor;
		/// <summary>
		/// The bell from which the touch is called.  If set to <see cref="Constants.tenor"/> (-1), then this is set to the heaviest bell.
		/// </summary>
		public int conductor_bell {
			get {
				return m_conductor_bell == Constants.tenor ? (int)stage - 1 : m_conductor_bell;
			}
			set {
				m_conductor_bell = value;
			}
		}

		private Change m_target_change = null;
		/// <summary>
		/// The change at which the touch will stop.  Defaults to rounds.
		/// </summary>
		public Change target_change {
			get {
				return m_target_change ?? Change.Rounds (stage);
			}

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
					ComputeChanges ();
				}

				return m_changes;
			}
		}

		/// <summary>
		/// A dictionary to store where the calls are placed in the touch.
		/// </summary>
		public Dictionary<int, Call> calls { get; private set; }

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
		/// The stage of the touch (the largest stage of all the methods spliced, allowing different staged methods to be spliced).
		/// </summary>
		public Stage stage {
			get {
				int max_stage = int.MinValue;

				foreach (MethodCall m in method_calls) {
					max_stage = Math.Max (max_stage, (int)m.method.stage);
				}

				return (Stage)max_stage;
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
		public Change this [int i] {
			get {
				return changes [i];
			}
		}

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
		/// Generates all the changes in the touch (could be computationally intensive).  Called once when <see cref="changes"/> is accessed.
		/// </summary>
		private void ComputeChanges () {
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
							CallLocationParameters call_location_parameters = new CallLocationParameters (current_method, current_change, absolute_change_index, sub_lead_index, lead_index, this);

							if (basic_call.call_location.EvaluateBasicCall (call, call_location_parameters)) {
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

			this.m_changes = changes.ToArray ();

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

		/// <summary>
		/// Creates a <see cref="Touch"/> object representing the plain course of a given method.
		/// </summary>
		/// <param name="method">The method who's plain course is being generated.</param>
		public Touch (Method method) {
			method_calls = new MethodCall [] { new MethodCall (method) };
			basic_calls = new BasicCall [0];

			m_conductor_bell = conductor_bell;
		}

		/// <summary>
		/// Creates a <see cref="Touch"/> object representing a single-method touch.
		/// </summary>
		/// <param name="method">The method of the touch.</param>
		/// <param name="calls">The calls which make up the touch.</param>
		/// <param name="conductor_bell">The bell from which the touch is called.</param>
		public Touch (Method method, BasicCall[] calls, int conductor_bell = Constants.tenor) {
			method_calls = new MethodCall [] { new MethodCall (method) };
			basic_calls = calls;

			m_conductor_bell = conductor_bell;
		}
	}

	/// <summary>
	/// A class to store a basic (Bob, Single, Plain) call.
	/// </summary>
	public class BasicCall {
		/// <summary>
		/// The <see cref="Call"/> being called.
		/// </summary>
		public Call call;
		/// <summary>
		/// The location of the call.
		/// </summary>
		public Touch.ICallLocation call_location;

		/// <summary>
		/// Creates a blank <see cref="BasicCall"/>.
		/// </summary>
		public BasicCall () { }

		/// <summary>
		/// Creates a fully-defined <see cref="BasicCall"/>.
		/// </summary>
		/// <param name="call">The call being called.</param>
		/// <param name="call_location">The location of the call.</param>
		public BasicCall (Call call, Touch.ICallLocation call_location) {
			this.call = call;
			this.call_location = call_location;
		}
	}

	/// <summary>
	/// A class to store method splicing calls.
	/// </summary>
	public class MethodCall {
		/// <summary>
		/// The method to be changed to.
		/// </summary>
		public Method method;
		/// <summary>
		/// The location of the call.
		/// </summary>
		public Touch.ICallLocation location;
		/// <summary>
		/// How far from the lead end of the last method the splice takes effect (should be negative).
		/// </summary>
		public int splice_end_index = 0;
		/// <summary>
		/// How far through the lead of the next method the splice starts (should be positive).
		/// </summary>
		public int splice_start_index = 0;

		/// <summary>
		/// Creates a blank <see cref="MethodCall"/> (should only be used for debug purposes).
		/// </summary>
		public MethodCall () { }

		/// <summary>
		/// Creates a <see cref="MethodCall"/> with just a <see cref="Method"/>.  This is the 'go x' call at the start of a <see cref="Touch"/>.
		/// </summary>
		/// <param name="method">The <see cref="Method"/> being spliced to.</param>
		public MethodCall (Method method) {
			this.method = method;
		}

		/// <summary>
		/// Creates a fully-defined <see cref="MethodCall"/> for a lead-end to lead-end splice.
		/// </summary>
		/// <param name="method">The <see cref="Method"/> being spliced to.</param>
		/// <param name="location">The location of the splice.</param>
		public MethodCall (Method method, Touch.ICallLocation location) {
			this.method = method;
			this.location = location;
		}

		/// <summary>
		/// Creates a fully-defined <see cref="MethodCall"/> for any splice.
		/// </summary>
		/// <param name="method">The <see cref="Method"/> being spliced to.</param>
		/// <param name="location">The location of the splice.</param>
		/// <param name="splice_end_index">How far from the lead end of the last method the splice takes effect (should be negative).  See <see cref="splice_end_index"/>.</param>
		/// <param name="splice_start_index">How far through the lead of the next method the splice starts (should be positive).  See <see cref="splice_start_index"/>.</param>
		public MethodCall (Method method, Touch.ICallLocation location, int splice_end_index, int splice_start_index) {
			this.method = method;
			this.location = location;
			this.splice_end_index = splice_end_index;
			this.splice_start_index = splice_start_index;
		}
	}
}