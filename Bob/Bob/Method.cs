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
				get => m_path;

				set {
					m_path = value;
					
					UpdateRotatedPath ();
				}
			}

			public Stage stage { get; private set; }

			public bool is_plain_hunting { get; private set; }
			public bool is_treble_dodging { get; private set; }

			public bool is_symmetrical { get; private set; }
			public bool spends_same_number_of_blows_in_each_place { get; private set; }

			public bool is_little { get; private set; }

			public int [] rotated_path { get; private set; }

			private void UpdateRotatedPath () {
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

			private bool IsPlainHunting () {
				// Cannot plain hunt if leads are an odd number of changes long.  Will also cause a crash if not returned
				if (path.Length % 2 != 0) {
					return false;
				}

				// Check that path is equal to {0, 1, 2, 3, ... n, n, ... 3, 2, 1, 0}
				for (int i = 0; i < path.Length / 2; i++) {
					if (rotated_path [i] != i || rotated_path [path.Length - i - 1] != i) {
						return false;
					}
				}

				return true;
			}

			private bool IsTrebleDodging () {
				// Check whether path is compatible
				if (path.Length % 2 != 0) {
					return false;
				}

				if ((int)stage % 2 == 1) {
					return false;
				}

				int number_of_dodges = path.Length / ((int)stage * 4);

				// Check that path is equal to {0, 1, 0, 1, 2, 3, 2, 3, ... n, n, ... 3, 2, 3, 2, 1, 0, 1, 0}
				for (int j = 0; j < path.Length / 4; j += 2) {
					int i = j * 2;

					for (int d = 0; d < number_of_dodges + 1; d++) {
						if (rotated_path [i + d * 2 + 0] != j) { return false; }
						if (rotated_path [i + d * 2 + 1] != j + 1) { return false; }
						
						int l = path.Length - 1;
						if (rotated_path [l - (i + d * 2 + 0)] != j) { return false; }
						if (rotated_path [l - (i + d * 2 + 1)] != j + 1) { return false; }
					}
				}

				return true;
			}

			private bool IsSymmetrical () {
				// Cannot plain hunt if leads are an odd number of changes long.  Will also cause a crash if not returned
				if (path.Length % 2 != 0) {
					return false;
				}

				// Check that path is equal to {0, 1, 2, 3, ... n, n, ... 3, 2, 1, 0}
				for (int i = 0; i < path.Length / 2; i++) {
					if (rotated_path [i] != rotated_path [path.Length - i - 1]) {
						return false;
					}
				}

				return true;
			}

			private bool SpendsSameNumberOfBlowsInEachPlace () {
				int [] number_of_blows_in_each_place = new int [path.Max () + 1];

				foreach (int p in path) {
					number_of_blows_in_each_place [p] += 1;
				}

				// This code could do with some re-writing.  It works, but is basically unreadable.
				int value = 0;
				for (int i = 0; i < path.Max (); i++) {
					if (number_of_blows_in_each_place [i] != 0) {
						if (value == 0) {
							value = number_of_blows_in_each_place [i];
						} else {
							if (number_of_blows_in_each_place [i] != value) {
								return false;
							}
						}
					}
				}

				return true;
			}

			public HuntBell (int bell_number, int [] path, Stage stage) {
				this.bell_number = bell_number;
				this.path = path;
				this.stage = stage;

				is_plain_hunting = IsPlainHunting ();
				is_treble_dodging = IsTrebleDodging ();

				is_symmetrical = IsSymmetrical ();
				spends_same_number_of_blows_in_each_place = SpendsSameNumberOfBlowsInEachPlace ();

				is_little = path.Max () < (int)stage - 1 || path.Min () > 0;
			}

			public HuntBell (int bell_number, int [] path, Stage stage, bool is_plain_hunting, bool is_treble_dodging, bool is_little) {
				this.bell_number = bell_number;
				this.path = path;
				this.stage = stage;

				this.is_plain_hunting = is_plain_hunting;
				this.is_treble_dodging = is_treble_dodging;
				this.is_little = is_little;
			}
		}

		public class MethodNotClassifiedException : Exception { }

		public enum SymmetryType { Asymmetric, PlainBobLike, GrandsireLike }

		// Fields
		private string m_place_notation;

		public List<Call> calls = new List<Call> ();

		public string name;
		public Classification classification;
		public Stage stage;

		public bool is_little = false;
		public bool is_differential = false;

		public string override_title = null;

		// Properties
		public string place_notation {
			get => m_place_notation;

			set {
				m_place_notation = value;

				RefreshNotation ();
			}
		}
		
		public string title {
			get {
				if (override_title != null) {
					return override_title;
				}

				bool called_differential = is_differential && classification != Classification.Differential;

				if (classification == Classification.Principle) {
					return name + " " + Utils.StageToString (stage);
				} else {
					return name + (called_differential ? " Differential" : "") + (is_little ? " Little" : "") + " " + Utils.CatagoryToString (classification) + " " + Utils.StageToString (stage);
				}
			}
		}

		public PlaceNotation [] place_notations { get; private set; }
		public Change[] plain_lead_changes { get; private set; }

		public int lead_length { get; private set; }
		public int leads_in_plain_course { get; private set; }
		public int plain_course_length => lead_length * leads_in_plain_course;

		public Change lead_end { get; private set; }
		public PlaceNotation lead_end_notation { get; private set; }
		public PlaceNotation half_lead_notation { get; private set; }

		public int [] [] rotating_sets { get; private set; }

		public HuntBell[] hunt_bells { get; private set; }
		public SymmetryType symmetry_type { get; private set; }
		public bool is_place_method { get; private set; }

		public Touch plain_course => new Touch (this);
		public HuntBell main_hunt_bell => hunt_bells.Length > 0 ? hunt_bells [0] : null;
		public bool is_treble_hunting => main_hunt_bell == null ? false : main_hunt_bell.bell_number == 0;

		// Functions
		public void Classify () {
			// These tags will be possibly set to true
			is_little = false;
			is_differential = false;
			classification = Classification.Unclassified;

			// Set the is_differential tag
			int number_of_rotating_non_hunt_bell_sets = 0;
			foreach (int [] set in rotating_sets) {
				if (set.Length > 1) {
					number_of_rotating_non_hunt_bell_sets += 1;
				}
			}

			if (number_of_rotating_non_hunt_bell_sets > 1) {
				is_differential = true;
			}

			// No hunt bells => Principle or Differential
			if (hunt_bells.Length == 0) {
				if (rotating_sets.Length == 1) {
					classification = Classification.Principle;
				} else {
					is_differential = true;

					classification = Classification.Differential;
				}
			}

			// Now keep going with the classification
			if (is_treble_hunting) {
				is_little = main_hunt_bell.is_little;

				if (main_hunt_bell.is_plain_hunting) {
					if (hunt_bells.Length == 1) {
						if (is_place_method) {
							classification = Classification.Place;
						} else {
							classification = Classification.Bob;
						}
					} else {
						if (hunt_bells [1].bell_number == 1 && lead_end_notation.is_12) {
							classification = Classification.SlowCourse;
						} else {
							classification = Classification.Bob;
						}
					}
				} else if (main_hunt_bell.is_treble_dodging) {
					// Detect internal places as the treble hunts between dodges
					bool are_all_internal_places_made = true;
					bool are_no_internal_places_made = true;
					
					int length_of_dodges = lead_length / (main_hunt_bell.path.Max () - main_hunt_bell.path.Min () + 1);

					for (int i = length_of_dodges - 1; i < lead_length; i += length_of_dodges) {
						// This is the half-lead when the treble lies at the back
						if (i == lead_length / 2 - 1) {
							continue;
						}

						// This is the lead end, so internal places don't count
						if (i == lead_length - 1) {
							continue;
						}
						
						// Set the relevent variables
						if (place_notations [i].has_internal_places) {
							are_no_internal_places_made = false;
						} else {
							are_all_internal_places_made = false;
						}
					}

					if (are_no_internal_places_made) {
						classification = Classification.TrebleBob;
					} else if (are_all_internal_places_made) {
						classification = Classification.Surprise;
					} else {
						classification = Classification.Delight;
					}
				} else { // Treble isn't plain hunting or treble dodging
					if (main_hunt_bell.is_symmetrical) {
						if (main_hunt_bell.spends_same_number_of_blows_in_each_place) {
							classification = Classification.TreblePlace;
						} else {
							classification = Classification.Alliance;
						}
					} else {
						classification = Classification.Hybrid;
					}
				}
			}

			// Throw an error if it's still unclassified
			if (classification == Classification.Unclassified) {
				throw new MethodNotClassifiedException ();
			}
		}

		public int [] GetPathOfBell (int bell_index) {
			// Find the rotating set which this bell is in so that we know how many leads long its path
			int[] rotating_set = null;

			foreach (int[] set in rotating_sets) {
				if (set.Contains (bell_index)) {
					rotating_set = set;
					
					break;
				}
			}

			// Check that the set isn't null
			if (rotating_set == null) {
				return null;
			}

			// Generate the list of positions by iterating over the plaincourse changes
			int [] path = new int [lead_length * rotating_set.Length];

			Change current_lead_end = Change.Rounds (stage);
			for (int i = 0; i < rotating_set.Length; i ++) {
				for (int c = 0; c < lead_length; c++) {
					path [i * lead_length + c] = (current_lead_end * plain_lead_changes [c]).IndexOf (bell_index);
				}

				current_lead_end *= lead_end;
			}

			return path;
		}

		public Call GetCallByName (string name) {
			foreach (Call call in calls) {
				if (call.name.ToUpper () == name.ToUpper ()) {
					return call;
				}
			}

			return null;
		}

		public Call GetCallByNotation (char notation) {
			foreach (Call call in calls) {
				foreach (char n in call.notations) {
					if (n.ToString ().ToUpper () == notation.ToString ().ToUpper ()) {
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
				Call call = GetCallByNotation (c);

				if (call != null) {
					calls.Add (call);
				}
			}

			return TouchFromCallList (calls.ToArray ());
		}

		public Touch TouchFromCallingPositions (string notation) {
			List<BasicCall> calls = new List<BasicCall> ();

			Call current_call = GetCallByName (Call.bob_name);
			foreach (char c in notation) {
				if (Constants.alpha.Contains (c)) { // `c` is lower-case, and therefore a call name
					current_call = GetCallByNotation (c);
				}

				if (Constants.ALPHA.Contains (c)) { // `c` is upper-case, and therefore a calling position
					calls.Add (new BasicCall (
						current_call,
						new CallLocationCallingPosition (c)
					));

					current_call = GetCallByName (Call.bob_name);
				}

				// Otherwise `c` is a random character, and should be ignored
			}

			return new Touch (this, calls.ToArray ());
		}

		// Private functions
		private bool HasPlainBobLikeSymmetry () {
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

		private bool HasGrandsireLikeSymmetry () {
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

		private bool IsPlaceMethod () {
			foreach (int[] set in rotating_sets) {
				if (set.Length == 1) {
					continue;
				}

				int [] path = GetPathOfBell (set [0]);

				/* To classify a dodge, we say it must return to a place 
				 * after one blow in a different place:
				 *      
				 *      x
				 *       x      <-- initial place
				 *        x		<-- different place
				 *       x		<-- returning to original place
				 *        x
				 */

				for (int i = 0; i < path.Length; i++) {
					// Fix wrapping around the ends
					int last_index = (i - 1) < 0 ? (i - 1 + path.Length) : (i - 1);
					int current_index = i;
					int next_index = (i + 1) > path.Length - 1 ? (i + 1 - path.Length) : (i + 1);

					// Check if this next_index is a dodge
					if (
						path [last_index] == path [next_index] && 
						Math.Abs (path [next_index] - path [current_index]) == 1
					) {
						return false;
					}
				}
			}

			return true;
		}

		private void RefreshNotation () {
			place_notations = PlaceNotation.DecodeFullNotation (m_place_notation, stage);
			plain_lead_changes = PlaceNotation.GenerateChangeArray (place_notations);

			lead_end = PlaceNotation.CombinePlaceNotations (place_notations);
			lead_length = place_notations.Length;
			leads_in_plain_course = lead_end.order;

			rotating_sets = lead_end.rotating_sets;

			lead_end_notation = place_notations [place_notations.Length - 1];
			half_lead_notation = lead_length % 2 == 0 ? place_notations [place_notations.Length / 2 - 1] : null;

			symmetry_type = SymmetryType.Asymmetric;
			if (HasPlainBobLikeSymmetry ()) { symmetry_type = SymmetryType.PlainBobLike; }
			if (HasGrandsireLikeSymmetry ()) { symmetry_type = SymmetryType.GrandsireLike; }

			is_place_method = IsPlaceMethod ();

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

		// Constructors
		public Method (string place_notation, string name, Classification classification, Stage stage, string override_title = null, bool is_little = false, bool is_differential = false, bool generate_standard_calls = true) {
			this.name = name;
			this.classification = classification;
			this.stage = stage;
			this.is_little = is_little;
			this.is_differential = is_differential;
			this.override_title = override_title;

			// Do full notation last, because it relies upon stage and catagory
			this.place_notation = place_notation;

			if (generate_standard_calls) {
				GenerateStandardCalls ();
			}
		}

		public Method (string place_notation, string name, Stage stage, string override_title = null, bool generate_standard_calls = true) {
			this.name = name;
			this.stage = stage;
			this.override_title = override_title;

			// Do full notation last, because it relies upon this.stage
			this.place_notation = place_notation;

			Classify ();

			if (generate_standard_calls) {
				GenerateStandardCalls ();
			}
		}

		// Static stuff
		public static Method GetMethod (string name) => MethodLibrary.GetMethodByName (name);

		public static Method plain_bob_doubles => new Method ("5.1.5.1.5,125", "Plain", Classification.Bob, Stage.Doubles);
		public static Method plain_bob_minor => new Method ("x16x16x16,12", "Plain", Classification.Bob, Stage.Minor);

		public static Method grandsire_doubles => new Method ("3,1.5.1.5.1", "Grandsire", Classification.Bob, Stage.Doubles, "Grandsire Doubles");
		public static Method grandsire_triples => new Method ("3,1.7.1.7.1.7.1", "Grandsire", Classification.Bob, Stage.Triples, "Grandsire Triples");

		public static Method cambridge_major => new Method ("x38x14x1258x36x14x58x16x78,12", "Cambridge", Classification.Surprise, Stage.Major);
	}
}
