using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A class to store called changes.
	/// </summary>
	public class CalledChange : ITransposition {
		/// <summary>
		/// An enum to store what method of determining the call is used.
		/// </summary>
		public enum CallingType {
			/// <summary>
			/// Called by calling a given bell up (see <see cref="bell_called_up"/>).
			/// </summary>
			ByBellCalledUp,
			/// <summary>
			/// Called by calling a given bell down (see <see cref="bell_called_down"/>).
			/// </summary>
			ByBellCalledDown,
			/// <summary>
			/// Called by calling a bell in a given place up (see <see cref="place_called_up"/>).
			/// </summary>
			ByPlaceCalledUp
		}

		/// <summary>
		/// The type of called change.
		/// </summary>
		public CallingType calling_type;

		/// <summary>
		/// The bell who will be called up.  (e.g. "&lt;this bell&gt; follow &lt;other bell&gt;").
		/// </summary>
		public int bell_called_up { get; private set; }

		/// <summary>
		/// The bell who will be called down.  (e.g. "&lt;other bell&gt; follow &lt;this bell&gt;").
		/// </summary>
		public int bell_called_down { get; private set; }

		/// <summary>
		/// The place (indexed from zero) of the first bell to be swapped, e.g. `2` would represent "3 follow 4" from rounds.
		/// </summary>
		public int place_called_up { get; private set; }

		private int [] m_array = null;
		/// <summary>
		/// Gets the array caused by this transposition.  Implements <see cref="ITransposition"/>.
		/// </summary>
		/// <returns>The array representing the transposition caused by this called change.</returns>
		public int [] GetTranspositionArray (Change original) {
			if (m_array is null) {
				m_array = new int [(int)original.stage];

				for (int i = 0; i < (int)original.stage; i++) {
					m_array [i] = i;
				}

				int index = place_called_up;
				if (calling_type == CallingType.ByBellCalledUp) {
					index = original.IndexOf (bell_called_up);
				} else if (calling_type == CallingType.ByBellCalledDown) {
					index = original.IndexOf (bell_called_down) - 1;
				}

				m_array [index] = index + 1;
				m_array [index + 1] = index;
			}

			return m_array;
		}

		/// <summary>
		/// Creates a <see cref="CalledChange"/> object.  I don't reccomend using this; use 
		/// </summary>
		/// <param name="calling_type">The way this called change is called.</param>
		/// <param name="bell_called_up">The bell (indexed from 0) which is called up.</param>
		/// <param name="bell_called_down">The bell (indexed from 0) which is called down.</param>
		/// <param name="place_called_up">The place (indexed from 0) of the bell which is called up.</param>
		public CalledChange (CallingType calling_type, int bell_called_up, int bell_called_down, int place_called_up) {
			this.calling_type = calling_type;
			this.bell_called_up = bell_called_up;
			this.bell_called_down = bell_called_down;
			this.place_called_up = place_called_up;
		}



		/// <summary>
		/// Creates a called change guided by place.
		/// </summary>
		/// <param name="place_called_up">The place of the lower bell which will be swapped.</param>
		public static CalledChange ByPlaceCalledUp (int place_called_up) => new CalledChange (CallingType.ByPlaceCalledUp, 0, 0, place_called_up);

		/// <summary>
		/// Creates a called change guided by place.
		/// </summary>
		/// <param name="place_called_up">The place of the lower bell which will be swapped.</param>
		public static CalledChange ByPlaceCalledUp (char place_called_up) => ByPlaceCalledUp (Constants.GetBellIndexFromZero (place_called_up));

		/// <summary>
		/// Creates a called change guided by which bell is called over another bell.
		/// </summary>
		/// <param name="bell_called_up">The number (indexed from 0) of the bell which will moved up a place.</param>
		public static CalledChange ByBellCalledUp (int bell_called_up) => new CalledChange (CallingType.ByBellCalledUp, bell_called_up, 0, 0);
		/// <summary>
		/// Creates a called change guided by which bell is called over another bell.
		/// </summary>
		/// <param name="bell_called_up">The number (indexed from 0) of the bell which will moved up a place.</param>
		public static CalledChange ByBellCalledUp (char bell_called_up) => ByBellCalledUp (Constants.GetBellIndexFromZero (bell_called_up));

		/// <summary>
		/// Creates a called change guided by which bell is called under another bell.
		/// </summary>
		/// <param name="bell_called_down">The number (indexed from 0) of the bell which will moved downra a place.</param>
		public static CalledChange ByBellCalledDown (int bell_called_down) => new CalledChange (CallingType.ByBellCalledDown, 0, bell_called_down, 0);
		/// <summary>
		/// Creates a called change guided by which bell is called under another bell.
		/// </summary>
		/// <param name="bell_called_down">The number (indexed from 0) of the bell which will moved downra a place.</param>
		public static CalledChange ByBellCalledDown (char bell_called_down) => ByBellCalledDown (Constants.GetBellIndexFromZero (bell_called_down));
	}
}
