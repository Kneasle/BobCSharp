using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public class Touch {
		public MethodCall [] method_calls;
		private Change m_target_change = Change.null_change;
		public Change target_change {
			get {
				return m_target_change == Change.null_change ? m_target_change : Change.Rounds (stage);
			}
			set {
				m_target_change = value;
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

		public Change [] changes { get; private set; }

		private class CallPoint {
			public Call call;
			public int start_index;

			public CallPoint (Call call, int start_index) {
				this.call = call;
				this.start_index = start_index;
			}
		}

		public void ComputeChanges () {
			List<Change> changes = new List<Change> ();

			int lead_index = 0;
			int sub_lead_index = 0;
			int call_index = 0;

			int absolute_change_index = 0;

			CallPoint current_callpoint = null;

			Change current_change = Change.Rounds (stage);
			Method current_method = method_calls [0].method;

			while (true) {
				// Update lead end indices if need be
				if (sub_lead_index >= current_method.place_notations.Length) {
					sub_lead_index = 0;
					lead_index += 1;
				}

				// Update change
				PlaceNotation notation = current_method.place_notations [sub_lead_index];

				if (current_callpoint != null) {
					notation = current_callpoint.call.place_notations [absolute_change_index - current_callpoint.start_index];
				}

				current_change = current_change.Transpose (notation);

				// Add change to list
				changes.Add (current_change);

				// Stop if it comes round (to whatever is the target change)
				if (current_change == target_change) {
					break;
				}

				// Update indices
				sub_lead_index += 1;
				absolute_change_index += 1;

				// Introduce stop
				if (absolute_change_index > 100000) {
					Console.WriteLine ("Broke the laws of human endurance and got to 100,000 changes without coming round.");
					break;
				}
			}

			this.changes = changes.ToArray ();
		}

		public override string ToString () {
			if (changes == null) {
				return "<Touch: changes not computed yet>";
			}

			if (changes.Length == 0) {
				return "<Touch: changes not computed yet>";
			}

			string output = "";
			foreach (Change c in changes) {
				output += "   " + c.ToString () + "\n";
			}

			output += "(" + changes.Length.ToString () + " changes)";

			return output;
		}

		public Touch (Method method, bool automatically_compute_changes = true) {
			method_calls = new MethodCall [] { new MethodCall (method) };

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

	public interface ICallLocation {

	}
}
