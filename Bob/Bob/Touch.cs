﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A class to store a representation of any touch.
	/// </summary>
	public class Touch : TouchBase {
		#region Sub-classes and Exceptions
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
			/// The number of attempted calls since the last successful calls.
			/// </summary>
			public int attempted_calls_since_last_call;
			/// <summary>
			/// The number of attempted splices since the last successful splice.
			/// </summary>
			public int attempted_splices_since_last_splice;
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
			/// <param name="attempted_calls_since_last_call">The number of calls since the last successful call.</param>
			/// <param name="attempted_splices_since_last_splice">The number of splices since the last successful splice.</param>
			/// <param name="touch">The touch from which this class is being generated.</param>
			public CallLocationParameters (Method current_method, Change start_change, int start_index, int sub_lead_index, int lead_index, int attempted_calls_since_last_call, int attempted_splices_since_last_splice, Touch touch) {
				this.current_method = current_method;
				this.start_change = start_change;
				this.start_index = start_index;
				this.sub_lead_index = sub_lead_index;
				this.lead_index = lead_index;
				this.attempted_calls_since_last_call = attempted_calls_since_last_call;
				this.attempted_splices_since_last_splice = attempted_splices_since_last_splice;
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
			/// <param name="parameters">The location where the call would be called.</param>
			/// <returns>True if the call should be called.</returns>
			bool EvaluateBasicCall (Call call, CallLocationParameters parameters);

			/// <summary>
			/// This should return true if the given data should result in a method splice being called.
			/// </summary>
			/// <param name="next_method">The method being spliced to.</param>
			/// <param name="parameters">The location of the potential call.</param>
			/// <returns>True if the method splice should happen here.</returns>
			bool EvaluateMethodCall (MethodCall next_method, CallLocationParameters parameters);
		}

		/// <summary>
		/// A class to specify the locations of calls which are given as a list (so calls are always called.
		/// </summary>
		public class CallLocationList : ICallLocation {
			/// <summary>
			/// This should return true if the given data should result in a call being called.
			/// </summary>
			/// <param name="call">The call that would be called.</param>
			/// <param name="parameters">The location where the call would be called.</param>
			/// <returns>Always true</returns>
			public bool EvaluateBasicCall (Call call, CallLocationParameters parameters) => true;

			/// <summary>
			/// This should return true if the given data should result in a method splice being called.
			/// </summary>
			/// <param name="next_method">The method being spliced to.</param>
			/// <param name="parameters">The location of the potential call.</param>
			/// <returns>Always true.</returns>
			public bool EvaluateMethodCall (MethodCall next_method, CallLocationParameters parameters) => true;
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
			/// <param name="parameters">The location where the call would be called.</param>
			/// <returns>True if the call should be called.</returns>
			public bool EvaluateBasicCall (Call call, CallLocationParameters parameters) {
				Change end_change = parameters.start_change * call.overall_transposition;

				return end_change.IndexOf (parameters.touch.conductor_bell) == call.GetCallingPositionIndex (calling_position);
			}

			/// <summary>
			/// This should return true if the given data should result in a method splice being called.
			/// </summary>
			/// <param name="next_method">The method being spliced to.</param>
			/// <param name="parameters">The location of the potential call.</param>
			/// <returns>True if the method splice should happen here.</returns>
			public bool EvaluateMethodCall (MethodCall next_method, CallLocationParameters parameters) {
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
		/// A class to specify that a call should be called after a certain number of attempts.
		/// </summary>
		public class CallLocationCountDown : ICallLocation {
			/// <summary>
			/// The number of attempted calls before the call should actually be called.
			/// </summary>
			public int number_of_attempts { get; private set; }

			/// <summary>
			/// Determines whether a <see cref="BasicCall"/> should be called.
			/// </summary>
			/// <param name="call">The call about to be called.</param>
			/// <param name="parameters">Extra data about the call location.</param>
			/// <returns>True if the call should be called.</returns>
			public bool EvaluateBasicCall (Call call, CallLocationParameters parameters) => number_of_attempts == parameters.attempted_calls_since_last_call;

			/// <summary>
			/// Determines whether a <see cref="MethodCall"/> should be called.
			/// </summary>
			/// <param name="next_method">The <see cref="MethodCall"/> about to be called.</param>
			/// <param name="parameters">Extra data about the call location.</param>
			/// <returns>True if the call should be called.</returns>
			public bool EvaluateMethodCall (MethodCall next_method, CallLocationParameters parameters) => number_of_attempts == parameters.attempted_splices_since_last_splice;

			/// <summary>
			/// Generates a fully-defined <see cref="CallLocationCountDown"/> object.
			/// </summary>
			/// <param name="number_of_attempts">The number of call attempts before actually calling.  Note that if you want the nth call, you should set this to n - 1 (e.g. the 4th call will have had 3 prior attempts)</param>
			public CallLocationCountDown (int number_of_attempts) {
				if (this.number_of_attempts < 0) {
					throw new ArgumentOutOfRangeException ("number_of_attempts", "Cannot have a negative number of attempts before calling.");
				}

				this.number_of_attempts = number_of_attempts;
			}
		}

		/// <summary>
		/// An exception to call if the peals get too long without coming back to rounds.  In lieu of a way of checking if touches never come back to rounds.
		/// </summary>
		public class YourPealRingersDiedOfExhaustionException : Exception {
			/// <summary>
			/// Throws an exception with a message.
			/// </summary>
			/// <param name="message">The message to show.</param>
			public YourPealRingersDiedOfExhaustionException (string message) : base (message) { }
		}

		private class ChangeState {
			int changes_since_last_call;
			int changes_since_last_splice;
			Change change;

			public ChangeState (int changes_since_last_call, int changes_since_last_splice, Change change) {
				this.changes_since_last_call = changes_since_last_call;
				this.changes_since_last_splice = changes_since_last_splice;
				this.change = change;
			}

			public override bool Equals (object obj) {
				var location = obj as ChangeState;
				return location != null &&
					   changes_since_last_call == location.changes_since_last_call &&
					   changes_since_last_splice == location.changes_since_last_splice &&
					   EqualityComparer<Change>.Default.Equals (change, location.change);
			}

			public override int GetHashCode () {
				var hashCode = 218628531;
				hashCode = hashCode * -1521134295 + changes_since_last_call.GetHashCode ();
				hashCode = hashCode * -1521134295 + changes_since_last_splice.GetHashCode ();
				hashCode = hashCode * -1521134295 + EqualityComparer<Change>.Default.GetHashCode (change);
				return hashCode;
			}

			public static bool operator == (ChangeState location1, ChangeState location2) {
				return EqualityComparer<ChangeState>.Default.Equals (location1, location2);
			}

			public static bool operator != (ChangeState location1, ChangeState location2) {
				return !(location1 == location2);
			}
		}
		#endregion

		#region Fields and Properties
		/// <summary>
		/// The starting method of the touch.
		/// </summary>
		public Method start_method;
		/// <summary>
		/// An array of method splicing calls.
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

		/// <summary>
		/// The stage of the touch (the largest stage of all the methods spliced, allowing different staged methods to be spliced).
		/// </summary>
		public override Stage stage {
			get {
				int max_stage = (int)start_method.stage;

				if (method_calls != null) {
					foreach (MethodCall m in method_calls) {
						max_stage = Math.Max (max_stage, (int)m.method.stage);
					}
				}

				return (Stage)max_stage;
			}
		}
		#endregion

		// Functions
		/// <summary>
		/// Generates all the changes in the touch (could be computationally intensive).  Called once when <see cref="TouchBase.changes"/> is accessed.
		/// </summary>
		public override Change[] ComputeChanges () {
			List<Change> changes = new List<Change> ();

			int method_call_length = method_calls == null ? 1 : method_calls.Length;
			int basic_call_length = basic_calls == null ? 1 : basic_calls.Length;

			int max_lead_length = method_calls == null ? 
				start_method.lead_length : 
				Math.Max (start_method.lead_length, method_calls.Select (x => x.method.lead_length).Max ());

			List<Change> [] change_states = new List<Change> [method_call_length * basic_call_length * max_lead_length];

			for (int i = 0; i < change_states.Length; i++) {
				change_states [i] = new List<Change> ();
			}

		    margin_calls = new Dictionary<int, char> ();
			right_hand_calls = new Dictionary<int, string> ();
			lead_ends_line_indices = new List<int> ();

			right_hand_calls.Add (-1, "Go " + start_method.title);

			int lead_index = 0;
			int sub_lead_index = 0;
			int call_index = 0;
			int method_call_index = 0;

			int sub_splice_change_index = 0;
			int sub_splice_lead_index = 0;

			int attempted_calls_since_last_call = 0;
			int attempted_splices_since_last_splice = 0;

			int absolute_change_index = 0;

			CallPoint current_callpoint = null;

			Change current_change = Change.Rounds (stage);
			Method current_method = start_method;

			while (true) {
				#region Update calls
				if (current_callpoint == null) {
					// Not in a call => Could be starting a call
					if (basic_calls != null && basic_calls.Length > 0) {
						BasicCall basic_call = basic_calls [call_index];
						Call call = basic_call.call;

						if ((sub_lead_index - call.from + 1) % call.every == 0) {
							CallLocationParameters call_location_parameters = new CallLocationParameters (current_method, current_change, absolute_change_index, sub_lead_index, lead_index, attempted_calls_since_last_call, attempted_splices_since_last_splice, this);

							if (basic_call.call_location.EvaluateBasicCall (call, call_location_parameters)) {
								current_callpoint = new CallPoint (call, absolute_change_index);

								attempted_calls_since_last_call = 0;

								if (!call.is_plain) {
									margin_calls.Add (absolute_change_index - 1, call.preferred_notation);
								}
							} else {
								attempted_calls_since_last_call += 1;
							}
						}
					}
				} else {
					// In a call, so could be stopping a call
					if (absolute_change_index > current_callpoint.end_index) {
						sub_lead_index += current_callpoint.call.cover;

						if (sub_lead_index >= current_method.place_notations.Length) {
							lead_ends_line_indices.Add (absolute_change_index - 1);

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
				#endregion

				#region Update change
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
				#endregion

				#region Update method splicing calls
				if (method_calls != null && method_calls.Length > 0) {
					MethodCall current_method_call = method_calls [method_call_index];

					if (sub_lead_index == current_method.lead_length + current_method_call.splice_start_index - 1) {
						CallLocationParameters parameters = new CallLocationParameters (current_method, current_change, absolute_change_index, sub_lead_index, lead_index, attempted_calls_since_last_call, attempted_splices_since_last_splice, this);

						if (current_method_call.location.EvaluateMethodCall (current_method_call, parameters)) {
							current_method = current_method_call.method;

							method_call_index += 1;

							sub_lead_index = current_method_call.splice_end_index - 1; // Set to splice_end_index - 1, because later in this iteration of the while loop, 1 will be added to it, to get splice_end_index.

							// Make sure that lead ends get counted even if there was a splice over the lead end.
							if (current_method_call.splice_end_index == 0) {
								sub_lead_index = current_method.lead_length - 1;
							}

							sub_splice_change_index = 0;
							sub_splice_lead_index = 0;

							attempted_splices_since_last_splice = 0;

							right_hand_calls.Add (absolute_change_index, current_method.title);

							if (method_call_index >= method_calls.Length) {
								method_call_index = 0;
							}
						} else {
							attempted_splices_since_last_splice += 1;
						}
					}
				}
				#endregion

				#region Stop if it comes to rounds (to whatever is the target change is)
				if (current_change == target_change) {
					// Add a final lead end.
					int lead_index_to_check = sub_lead_index + 1;
					if (current_callpoint != null && absolute_change_index == current_callpoint.end_index) {
						lead_index_to_check = sub_lead_index + current_callpoint.call.cover;
					}

					if (lead_index_to_check == current_method.lead_length) {
						lead_ends_line_indices.Add (absolute_change_index);
					}

					comes_round = true;

					break;
				}
				#endregion

				#region Update indices & lead ends.
				if (current_callpoint == null) {
					sub_lead_index += 1;

					if (sub_lead_index >= current_method.place_notations.Length) {
						lead_ends_line_indices.Add (absolute_change_index);

						sub_lead_index = 0;
						sub_splice_lead_index += 1;

						lead_index += 1;
					}
				}

				absolute_change_index += 1;
				sub_splice_change_index += 1;
				#endregion

				#region Stop if touch goes on forever.  If 100,000,000 changes are reached, then the code will stop.
				List<Change> change_state_list = change_states [call_index + basic_call_length * (method_call_index + method_call_length * sub_lead_index)];

				if (change_state_list.Contains (current_change)) {
					comes_round = false;

					break;
				}

				change_state_list.Add (current_change);

				// It should never get this far
				if (absolute_change_index > 1e4) {
					throw new YourPealRingersDiedOfExhaustionException ("Broke the laws of human endurance and got to 100,000,000 changes without coming round.");
				}
				#endregion
			}

			return changes.ToArray ();
		}

		/// <summary>
		/// Creates a touch of a single method, and a list of calls in the order they should be called.
		/// </summary>
		/// <param name="method">The method of the touch.</param>
		/// <param name="calls">The calls in the order they should be called.</param>
		public Touch (Method method, Call [] calls) {
			BasicCall [] basic_calls = new BasicCall [calls.Length];

			for (int i = 0; i < calls.Length; i++) {
				basic_calls [i] = new BasicCall (calls [i], new Touch.CallLocationList ());
			}

			this.basic_calls = basic_calls;
			start_method = method;
		}

		/// <summary>
		/// Creates a <see cref="Touch"/> object representing a single-method touch.
		/// </summary>
		/// <param name="method">The method of the touch.</param>
		/// <param name="calls">The calls which make up the touch.</param>
		/// <param name="conductor_bell">The bell from which the touch is called.</param>
		public Touch (Method method, BasicCall[] calls = null, int conductor_bell = Constants.tenor) {
			start_method = method;

			method_calls = null;
			basic_calls = calls;

			m_conductor_bell = conductor_bell;
		}

		/// <summary>
		/// Creates a <see cref="Touch"/> object representing a touch of multiple methods spliced.
		/// </summary>
		/// <param name="start_method">The method to start the touch.  Will be added as a <see cref="MethodCall"/> to the start of `method_calls`.</param>
		/// <param name="method_calls">The splices in methods.</param>
		/// <param name="calls">The calls (Bobs, Singles, etc.) which make up the touch.</param>
		/// <param name="conductor_bell">The bell from which the touch is called.</param>
		public Touch (Method start_method, MethodCall[] method_calls, BasicCall[] calls = null, int conductor_bell = Constants.tenor) {
			this.start_method = start_method;
			this.method_calls = method_calls;

			basic_calls = calls;
			this.conductor_bell = conductor_bell;
		}

		/// <summary>
		/// Creates a <see cref="Touch"/> object representing a touch of multiple methods spliced lead-by-lead.
		/// </summary>
		/// <param name="methods">The methods which will be spliced each lead.</param>
		/// <param name="calls">The calls (Bobs, Singles, etc.) which make up the touch.</param>
		/// <param name="conductor_bell">The bell from which the touch is called.</param>
		public Touch (Method [] methods, BasicCall [] calls = null, int conductor_bell = Constants.tenor) {
			start_method = methods [0];

			method_calls = new MethodCall [methods.Length];

			method_calls [methods.Length - 1] = new MethodCall (methods [0], new CallLocationList ());
			for (int i = 1; i < methods.Length; i++) {
				method_calls [i - 1] = new MethodCall (methods [i], new CallLocationList ());
			}

			basic_calls = calls;
			this.conductor_bell = conductor_bell;
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
		public int splice_start_index = 0;
		/// <summary>
		/// How far through the lead of the next method the splice starts (should be positive).
		/// </summary>
		public int splice_end_index = 0;

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
		/// <param name="splice_end_index">How far from the lead end of the last method the splice takes effect (should be negative).  See <see cref="splice_start_index"/>.</param>
		/// <param name="splice_start_index">How far through the lead of the next method the splice starts (should be positive).  See <see cref="splice_end_index"/>.</param>
		public MethodCall (Method method, Touch.ICallLocation location, int splice_end_index, int splice_start_index = 0) {
			this.method = method;
			this.location = location;
			this.splice_start_index = splice_end_index;
			this.splice_end_index = splice_start_index;
		}
	}
}
