using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// An object which stores a touch of called changes, and can run falseness checks.
	/// </summary>
	public class CalledChangeTouch : TouchBase {
		private Stage m_stage;
		/// <summary>
		/// Gets the stage of this touch of called changes.
		/// </summary>
		public override Stage stage => m_stage;

		/// <summary>
		/// List of the called changes in this touch.
		/// </summary>
		public CalledChange [] called_changes;

		/// <summary>
		/// Computes the array of changes in this set of called changes.
		/// </summary>
		/// <returns>A change array caused by calling these called changes.</returns>
		public override Change [] ComputeChanges () {
			List<Change> changes = new List<Change> ();
			List<ChangeState> change_states = new List<ChangeState> ();

			int call_index = 0;
			Change current_change = Change.Rounds (stage);

			while (true) {
				current_change *= called_changes [call_index];

				changes.Add (current_change);

				if (current_change == target_change) {
					break;
				}

				call_index += 1;

				if (call_index == called_changes.Length) {
					call_index = 0;
				}

				// Break if touches don't come round
				ChangeState current_state = new ChangeState (current_change, 0, call_index);

				if (change_states.Contains (current_state)) {
					comes_round = false;

					break;
				}

				change_states.Add (current_state);

				if (changes.Count > 1e5) {
					throw new Touch.YourPealRingersDiedOfExhaustionException ("Touch of called changes lasted 100,000 changes without coming round.");
				}
			}

			return changes.ToArray ();
		}

		/// <summary>
		/// Creates a new touch of called changes.
		/// </summary>
		/// <param name="stage">The stage of bells on which this touch should be rung.</param>
		/// <param name="called_changes">The called changes which make up the touch.</param>
		public CalledChangeTouch (Stage stage, CalledChange [] called_changes) {
			m_stage = stage;

			this.called_changes = called_changes;
		}
	}
}
