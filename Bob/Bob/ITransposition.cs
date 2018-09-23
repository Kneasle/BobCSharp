using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// An interface for any transposable object, e.g. Change or PlaceNotation.
	/// </summary>
	public interface ITransposition {
		/// <summary>
		/// A function to get an integer array representing the transposition of this object.
		/// </summary>
		/// <returns>An integer array representing the transposition.</returns>
		int [] GetArray ();
	}
}
