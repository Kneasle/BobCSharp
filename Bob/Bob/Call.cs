using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public class Call {
		public Method method;

		public string name;
		public char [] notations;

		public PlaceNotation [] place_notations;
		public string [] calling_positions;

		public int every;
		public int from;
		public int cover;

		// Properties
		public Change overall_transposition => PlaceNotation.CombinePlaceNotations (place_notations);
		public bool is_plain => name == plain_name;
		public int length => place_notations.Length;
		public char preferred_notation => notations [0];

		// Functions
		public int GetCallingPositionIndex (char c) {
			for (int i = 0; i < calling_positions.Length; i++) {
				if (calling_positions [i].Contains (c)) {
					return i;
				}
			}

			return -1;
		}

		private void Init () {
			if (method.lead_length % every != 0) {
				throw new Exception ("Call " + name + " of " + method.title + " repeats at a number of changes which is not a divisor of the lead length. This will cause massive issues with calls not lining up as expected.");
			}
		}

		// Constructors
		public Call (Method method) {
			this.method = method;

			Init ();
		}

		public Call (Method method, string name, char [] notations, PlaceNotation [] place_notations, int every, int from = 0, int cover = -1) {
			this.method = method;
			this.name = name;
			this.notations = notations;
			this.place_notations = place_notations;
			this.every = every;
			this.from = from;
			this.cover = cover == -1 ? place_notations.Length : cover;

			Init ();
		}

		public Call (Method method, string name, char [] notations, PlaceNotation place_notation, int every, int from = 0, int cover = -1) {
			this.method = method;
			this.name = name;
			this.notations = notations;
			this.place_notations = new PlaceNotation [] { place_notation };
			this.every = every;
			this.from = from;
			this.cover = cover == -1 ? place_notations.Length : cover;

			Init ();
		}

		public Call (Method method, string name, char [] notations, PlaceNotation [] place_notations, string [] calling_positions, int every, int from = 0, int cover = -1) {
			this.name = name;
			this.notations = notations;
			this.place_notations = place_notations;
			this.method = method;
			this.every = every;
			this.from = from;
			this.cover = cover == -1 ? place_notations.Length : cover;

			Init ();
			this.calling_positions = calling_positions;
		}

		// Static functions
		public static Call LeadEndCall (Method method, string name, char [] notations, PlaceNotation [] place_notations) {
			return new Call (method, name, notations, place_notations, method.lead_length, - place_notations.Length);
		}

		public static Call LeadEndCall (Method method, string name, char [] notations, PlaceNotation [] place_notations, string [] calling_positions) {
			return new Call (method, name, notations, place_notations, calling_positions, method.lead_length, -place_notations.Length);
		}


		public static Call LeadEndBob (Method method, PlaceNotation [] place_notations, bool add_standard_calling_positions = true) {
			if (add_standard_calling_positions) {
				return LeadEndCall (method, bob_name, bob_notations, place_notations, standard_calling_positions_bob [method.stage]);
			} else {
				return LeadEndCall (method, bob_name, bob_notations, place_notations);
			}
		}

		public static Call LeadEndBob (Method method, PlaceNotation place_notation) {
			return LeadEndCall (method, bob_name, bob_notations, new PlaceNotation [] { place_notation });
		}

		public static Call LeadEndBob (Method method, string notation) {
			return LeadEndCall (method, bob_name, bob_notations, PlaceNotation.DecodeFullNotation (notation, method.stage));
		}


		public static Call LeadEndSingle (Method method, PlaceNotation [] place_notations, bool add_standard_calling_positions = true) {
			if (add_standard_calling_positions) {
				return LeadEndCall (method, single_name, single_notations, place_notations, standard_calling_positions_single [method.stage]);
			} else {
				return LeadEndCall (method, single_name, single_notations, place_notations);
			}
		}

		public static Call LeadEndSingle (Method method, PlaceNotation place_notation) {
			return LeadEndCall (method, single_name, single_notations, new PlaceNotation [] { place_notation });
		}

		public static Call LeadEndSingle (Method method, string notation) {
			return LeadEndCall (method, single_name, single_notations, PlaceNotation.DecodeFullNotation (notation, method.stage));
		}


		public static Call LeadEndPlain (Method method, int length = 1) {
			return LeadEndCall (method, plain_name, plain_notations, new PlaceNotation [length]);
		}

		// Static values
		public static string bob_name = "Bob";
		public static string single_name = "Single";
		public static string plain_name = "Plain";

		private static char [] m_bob_notations = null;
		public static char [] bob_notations {
			get { return m_bob_notations ?? new char [] { '-', 'b' }; }
			set { m_bob_notations = value; }
		}

		private static char [] m_single_notations = null;
		public static char [] single_notations {
			get { return m_single_notations ?? new char [] { 's' }; }
			set { m_single_notations = value; }
		}

		private static char [] m_plain_notations = null;
		public static char [] plain_notations {
			get { return m_plain_notations ?? new char [] { 'm', 'p' }; }
			set { m_plain_notations = value; }
		}

		public static Dictionary<Stage, string []> standard_calling_positions_bob {
			get {
				Dictionary<Stage, string []> dict = new Dictionary<Stage, string []> ();

				dict.Add (Stage.Doubles, new string [] { "", "I", "BO", "FM4", "H" });
				dict.Add (Stage.Minor, new string [] { "", "I", "BO", "FM4", "W", "H" });
				dict.Add (Stage.Triples, new string [] { "", "I", "BO", "F4", "W", "M", "H" });
				dict.Add (Stage.Major, new string [] { "", "I", "BO", "F4", "V5", "M", "W", "H" });
				dict.Add (Stage.Caters, new string [] { "", "I", "BO", "F4", "V5", "X6", "W", "M", "H" });
				dict.Add (Stage.Royal, new string [] { "", "I", "BO", "F4", "V5", "X6", "S7", "M", "W", "H" });
				dict.Add (Stage.Cinques, new string [] { "", "I", "BO", "F4", "V5", "X6", "S7", "E8", "W", "M", "H" });
				dict.Add (Stage.Maximus, new string [] { "", "I", "BO", "F4", "V5", "X6", "S7", "E8", "N9", "M", "W", "H" });

				return dict;
			}
		}
		public static Dictionary<Stage, string []> standard_calling_positions_single {
			get {
				Dictionary<Stage, string []> dict = new Dictionary<Stage, string []> ();

				dict.Add (Stage.Doubles, new string [] { "", "BI2", "3", "F4", "H" });
				dict.Add (Stage.Minor, new string [] { "", "BI2", "3", "FM4", "W", "H" });
				dict.Add (Stage.Triples, new string [] { "", "BI2", "3", "F4", "W", "M", "H" });
				dict.Add (Stage.Major, new string [] { "", "BI2", "3", "F4", "V5", "M", "W", "H" });
				dict.Add (Stage.Caters, new string [] { "", "BI2", "3", "F4", "V5", "X6", "W", "M", "H" });
				dict.Add (Stage.Royal, new string [] { "", "BI2", "3", "F4", "V5", "X6", "S7", "M", "W", "H" });
				dict.Add (Stage.Cinques, new string [] { "", "BI2", "3", "F4", "V5", "X6", "S7", "E8", "W", "M", "H" });
				dict.Add (Stage.Maximus, new string [] { "", "BI2", "3", "F4", "V5", "X6", "S7", "E8", "N9", "M", "W", "H" });

				return dict;
			}
		}
	}
}
