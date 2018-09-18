using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public class Method {
		public class HuntBell {
			public int bell_number { get; private set; }
			private int [] m_path;
			public int [] path {
				get {
					return m_path;
				}

				set {
					m_path = value;
					
					UpdateRotatedPath ();
				}
			}

			public bool is_plain_hunting;
			public bool is_treble_dodging;
			public bool is_little;

			public int [] rotated_path;

			void UpdateRotatedPath () {
				rotated_path = null;

				// Find the index of two consecutive leads
				int index = -1;
				for (int i = 0; i < path.Length - 1; i++) {
					if (path [i] == 0 && path [i + 1] == 0) {
						index = i;
						break;
					}
				}

				if (index == -1) {
					return;
				}

				// Rotate path so that the consecutive leads are split (this path will start and finish in 0)
				rotated_path = new int [path.Length];
				for (int i = 0; i <= index; i++) {
					rotated_path [i + path.Length - index - 1] = path [i];
				}

				for (int i = index + 1; i < path.Length; i++) {
					rotated_path [i - index - 1] = path [i];
				}
			}

			bool IsPlainHunting () {
				// Cannot plain hunt if leads are an odd number of changes long.  Will also cause a crash if not returned
				if (path.Length % 2 != 0) {
					return false;
				}

				// Find the index of two consecutive leads
				int index = -1;
				for (int i = 0; i < path.Length - 1; i++) {
					if (path [i] == 0 && path [i + 1] == 0) {
						index = i;
						break;
					}
				}

				if (index == -1) {
					return false;
				}

				// Rotate path so that the consecutive leads are split (this path will start and finish in 0)
				int [] rotated_path = new int [path.Length];
				for (int i = 0; i <= index; i ++) {
					rotated_path [i + path.Length - index - 1] = path [i];
				}

				for (int i = index + 1; i < path.Length; i ++) {
					rotated_path [i - index - 1] = path [i];
				}

				// Check that path is equal to {0, 1, 2, 3, ... n, n, ... 3, 2, 1, 0}
				for (int i = 0; i < path.Length / 2; i++) {
					if (rotated_path [i] != i || rotated_path [path.Length - i - 1] != i) {
						return false;
					}
				}

				return true;
			}

			bool IsTrebleDodging () {
				// Check whether lead length is compatible
				if (path.Length % 4 != 0) {
					return false;
				}

				// Check that path is equal to {0, 1, 0, 1, 2, 3, 2, 3, ... n, n, ... 3, 2, 3, 2, 1, 0, 1, 0}
				for (int j = 0; j < path.Length / 4; j += 2) {
					int i = j * 2;
					if (rotated_path [i + 0] != j)     { return false; }
					if (rotated_path [i + 1] != j + 1) { return false; }
					if (rotated_path [i + 2] != j)     { return false; }
					if (rotated_path [i + 3] != j + 1) { return false; }

					int l = path.Length - 1;
					if (rotated_path [l - (i + 0)] != j)     { return false; }
					if (rotated_path [l - (i + 1)] != j + 1) { return false; }
					if (rotated_path [l - (i + 2)] != j)     { return false; }
					if (rotated_path [l - (i + 3)] != j + 1) { return false; }
				}

				return true;
			}

			public HuntBell (int bell_number, int [] path, Stage stage) {
				this.bell_number = bell_number;
				this.path = path;

				is_little = path.Max () < (int)stage - 1;
				is_plain_hunting = IsPlainHunting ();
				is_treble_dodging = IsTrebleDodging ();
			}

			public HuntBell (int bell_number, int [] path, bool is_plain_hunting, bool is_treble_dodging, bool is_little) {
				this.bell_number = bell_number;
				this.path = path;
				this.is_plain_hunting = is_plain_hunting;
				this.is_treble_dodging = is_treble_dodging;
				this.is_little = is_little;
			}
		}

		public enum SymmetryType { Asymmetric, PlainBobLike, GrandsireLike }

		// Fields
		private string m_full_notation;

		public List<Call> calls = new List<Call> ();

		public string name;
		public Catagory catagory;
		public Stage stage;

		public string override_title = "";

		// Properties
		public PlaceNotation [] place_notations { get; private set; }
		public Change[] plain_lead_changes { get; private set; }

		public int lead_length { get; private set; }
		public int leads_in_plain_course { get; private set; }
		public int plain_course_length => lead_length * leads_in_plain_course;

		public Change lead_end { get; private set; }
		public PlaceNotation lead_end_notation { get; private set; }
		public PlaceNotation half_lead_notation { get; private set; }

		public HuntBell[] hunt_bells { get; private set; }
		public SymmetryType symmetry_type { get; private set; }

		public string full_notation {
			get {
				return m_full_notation;
			}
			
			set {
				m_full_notation = value;

				RefreshNotation ();
			}
		}

		public string title {
			get {
				if (override_title != "") {
					return override_title;
				}

				if (catagory == Catagory.Principle) {
					return name + " " + Utils.StageToString (stage);
				} else {
					return name + " " + Utils.CatagoryToString (catagory) + " " + Utils.StageToString (stage);
				}
			}
		}

		public Touch plain_course => new Touch (this);
		public HuntBell main_hunt_bell => hunt_bells.Length > 0 ? hunt_bells [0] : null;
		public bool is_treble_hunting => main_hunt_bell == null ? false : main_hunt_bell.bell_number == 0;

		// Functions
		bool HasPlainBobLikeSymmetry () {
			if (lead_length % 2 != 0) {
				return false;
			}

			for (int i = 0; i < lead_length / 2 - 1; i++) {
				if (place_notations [i] != place_notations [lead_length - 2 - i]) {
					return false;
				}
			}

			return true;
		}

		bool HasGrandsireLikeSymmetry () {
			if (lead_length % 2 != 0) {
				return false;
			}

			for (int i = 0; i < lead_length / 2 - 1; i++) {
				if (place_notations [i + 1] != place_notations [lead_length - 1 - i]) {
					return false;
				}
			}

			return true;
		}

		private void RefreshNotation () {
			place_notations = PlaceNotation.DecodeFullNotation (m_full_notation, stage);
			plain_lead_changes = PlaceNotation.GenerateChangeArray (place_notations);

			lead_end = PlaceNotation.CombinePlaceNotations (place_notations);
			lead_length = place_notations.Length;
			leads_in_plain_course = lead_end.order;

			lead_end_notation = place_notations [place_notations.Length - 1];
			half_lead_notation = lead_length % 2 == 0 ? place_notations [place_notations.Length / 2 - 1] : null;

			symmetry_type = SymmetryType.Asymmetric;
			if (HasPlainBobLikeSymmetry ()) { symmetry_type = SymmetryType.PlainBobLike; }
			if (HasGrandsireLikeSymmetry ()) { symmetry_type = SymmetryType.GrandsireLike; }

			// Hunt bells
			List<int> hunt_bell_numbers = new List<int> ();
			for (int i = 0; i < (int)stage; i++) {
				if (lead_end.array [i] == i) {
					hunt_bell_numbers.Add (i);
				}
			}

			hunt_bells = new HuntBell [hunt_bell_numbers.Count];
			for (int i = 0; i < hunt_bell_numbers.Count; i++) {
				int [] path = new int [lead_length];

				for (int j = 0; j < lead_length; j++) {
					path [j] = plain_lead_changes [j].IndexOf (i);
				}

				hunt_bells [i] = new HuntBell (hunt_bell_numbers [i], path, stage);
			}
		}

		private void GenerateStandardCalls (bool overwrite_current = true) {
			if ((int)stage < 4) {
				return;
			}

			// Case 1: Plain-bob like method
			if (is_treble_hunting) {
				if (symmetry_type == SymmetryType.PlainBobLike) {
					if (lead_end_notation.is_12) {
						if (overwrite_current) {
							calls.Clear ();
						}

						calls.Add (Call.LeadEndBob (this, "14"));

						if (stage == Stage.Doubles) {
							calls.Add (Call.LeadEndSingle (this, "123"));
						} else if (stage > Stage.Doubles) {
							calls.Add (Call.LeadEndSingle (this, "1234"));
						}

						calls.Add (Call.LeadEndPlain (this));
					} else if (lead_end_notation.is_1n && stage > Stage.Doubles) {
						if (overwrite_current) {
							calls.Clear ();
						}

						string n_minus_2 = Constants.GetBellNameIndexingFromOne ((int)stage - 2);
						string n_minus_1 = Constants.GetBellNameIndexingFromOne ((int)stage - 1);
						string n = Constants.GetBellNameIndexingFromOne ((int)stage);

						calls.Add (Call.LeadEndBob (this, "1" + n_minus_2));
						calls.Add (Call.LeadEndBob (this, "1" + n_minus_2 + n_minus_1 + n));

						calls.Add (Call.LeadEndPlain (this));
					}
				} else if (symmetry_type == SymmetryType.GrandsireLike) {
					if (overwrite_current) {
						calls.Clear ();
					}

					calls.Add (Call.LeadEndBob (this, "3.1"));
					calls.Add (Call.LeadEndSingle (this, "3.123"));
					calls.Add (Call.LeadEndPlain (this));
				}
			}
		}

		public Call GetCallByName (string name) {
			foreach (Call call in calls) {
				if (call.name.ToUpper () == name.ToUpper ()) {
					return call;
				}
			}

			return null;
		}

		public Call GetCallByNotation (string notation) {
			foreach (Call call in calls) {
				foreach (string n in call.notations) {
					if (n.ToUpper () == notation.ToUpper ()) {
						return call;
					}
				}
			}

			return null;
		}

		public Touch TouchFromCallList (Call [] calls) {
			BasicCall [] basic_calls = new BasicCall [calls.Length];

			for (int i = 0; i < calls.Length; i++) {
				basic_calls [i] = new BasicCall (calls [i], new CallLocationList ());
			}

			return new Touch (this, basic_calls);
		}

		public Touch TouchFromCallList (string notation) {
			List<Call> calls = new List<Call> ();

			foreach (char c in notation) {
				Call call = GetCallByNotation (c.ToString ());

				if (call != null) {
					calls.Add (call);
				}
			}

			return TouchFromCallList (calls.ToArray ());
		}

		// Constructors
		public Method (string full_notation, string name, Catagory catagory, Stage stage, string override_title = "", bool generate_calls = true) {
			this.name = name [0].ToString ().ToUpper () + name.Substring (1).ToLower ();
			this.catagory = catagory;
			this.stage = stage;
			this.override_title = override_title;

			// Do full notation last, because it relies upon stage and catagory
			this.full_notation = full_notation;

			if (generate_calls) {
				GenerateStandardCalls ();
			}
		}

		// Static functions
		public static Method plain_bob_doubles => new Method ("5.1.5.1.5,125", "Plain", Catagory.Bob, Stage.Doubles);
		public static Method plain_bob_minor => new Method ("x16x16x16,12", "Plain", Catagory.Bob, Stage.Minor);
		public static Method grandsire_triples => new Method ("3,1.7.1.7.1.7.1", "Grandsire", Catagory.SlowCourse, Stage.Triples, "Grandsire Triples");
		public static Method grandsire_doubles => new Method ("3,1.5.1.5.1", "Grandsire", Catagory.SlowCourse, Stage.Doubles, "Grandsire Doubles");
		public static Method cambridge_major => new Method ("x38x14x1258x36x14x58x16x78,12", "Cambridge", Catagory.Surprise, Stage.Major);
	}
}
