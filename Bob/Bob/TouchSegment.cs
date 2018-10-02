using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A class to represent a segment of a larger touch (e.g. for proving falseness of sections).
	/// </summary>
	public class TouchSegment : TouchBase {
		private Stage m_stage;
		/// <summary>
		/// The stage of this touch segment.
		/// </summary>
		public override Stage stage => m_stage;

		private Change [] m_changes;
		/// <summary>
		/// Computes the changes in this touch (just returns the stored value).
		/// </summary>
		/// <returns>The changes of this touch.</returns>
		public override Change [] ComputeChanges () => m_changes;

		/// <summary>
		/// Creates a <see cref="TouchSegment"/> class from another <see cref="TouchBase"/> object.
		/// </summary>
		/// <param name="original_touch"></param>
		/// <param name="start_index"></param>
		/// <param name="length"></param>
		public TouchSegment (TouchBase original_touch, int start_index, int length) {
			m_stage = original_touch.stage;

			if (start_index < 0) {
				throw new ArgumentOutOfRangeException ("start_index", "start_index must be greater than 0.");
			}

			if (start_index + length > original_touch.Length) {
				throw new ArgumentOutOfRangeException ("length", "The requested region extends beyond the end of the touch.");
			}

			m_changes = new Change [length];
			for (int i = 0; i < length; i++) {
				m_changes [i] = original_touch.changes [start_index + i];
			}
		}

		/// <summary>
		/// Explicitly creates a <see cref="TouchSegment"/> object.
		/// </summary>
		/// <param name="stage">The stage of the original touch.</param>
		/// <param name="changes">The changes in this touch segment.</param>
		public TouchSegment (Stage stage, Change[] changes) {
			m_stage = stage;
			m_changes = changes;
		}
	}
}
