using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public static class Utils {
		public static string StageToString (Stage stage) {
			if (stage == Stage.TwentyTwo) {
				return "Twenty-two";
			}

			return stage.ToString ();
		}

		public static string CatagoryToString (Catagory catagory) {
			if (catagory == Catagory.SlowCourse) {
				return "Slow Course";
			}

			if (catagory == Catagory.TreblePlace) {
				return "Treble Place";
			}

			if (catagory == Catagory.TrebleBob) {
				return "Treble Bob";
			}

			return catagory.ToString ();
		}

		public static Change GetEndChange (PlaceNotation [] notations) {
			Change change = Change.Rounds (notations [0].stage);

			foreach (PlaceNotation notation in notations) {
				change *= notation;
			}

			return change;
		}
	}
}
