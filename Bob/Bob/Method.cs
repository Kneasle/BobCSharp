using System;
using System.Collections.Generic;
using System.Linq;

namespace Bob {
	/// <summary>
	/// A class to represent a method.
	/// </summary>
	public class Method {
		/// <summary>
		/// A class which represents a hunt bell in a method.  HuntBell objects can't be changed once created.
		/// </summary>
		public class HuntBell {
			/// <summary>
			/// Exception to allow jumping out of two for loops.
			/// </summary>
			private class GetMeOutOfHereException : Exception { }

			/// <summary>
			/// The number of this bell (indexed from zero).
			/// </summary>
			public int bell_number { get; private set; }

			/// <summary>
			/// A list of places which form the path of this hunt.
			/// </summary>
			public int [] path { get; private set; }

			/// <summary>
			/// The stage of the method which owns this hunt bell.
			/// </summary>
			public Stage stage { get; private set; }

			/// <summary>
			/// True if this hunt bell follows a plain hunting path (in any rotation).
			/// </summary>
			public bool is_plain_hunting { get; private set; }

			/// <summary>
			/// True if this hunt bell follows a treble bobbing path (in any rotation).
			/// </summary>
			public bool is_treble_dodging { get; private set; }

			/// <summary>
			/// True if this hunt bell path is symmetrical.
			/// </summary>
			public bool is_symmetrical { get; private set; }

			/// <summary>
			/// True if this hunt bell spends an equal number of blows in every place it visits.
			/// </summary>
			public bool spends_same_number_of_blows_in_each_place { get; private set; }

			/// <summary>
			/// True if this hunt bell is little (doesn't cover every place).
			/// </summary>
			public bool is_little { get; private set; }

			/// <summary>
			/// The rotated path of this hunt bell, such that if the path is symmetrical, so will this path.  Will be null if no symmetry.
			/// </summary>
			public int [] rotated_path { get; private set; }

			/// <summary>
			/// Generates the rotated path.
			/// </summary>
			private void GenerateRotatedPath () {
				rotated_path = null;

				List<int> place_indices = new List<int> ();

				// Find the index of two consecutive leads
				for (int i = 0; i < path.Length; i++) {
					int i_plus_one = i + 1;
					while (i_plus_one >= path.Length) {
						i_plus_one -= path.Length;
					}

					if (path [i] == path [i_plus_one]) {
						place_indices.Add (i);
					}
				}

				if (place_indices.Count == 0) {
					return;
				}

				// Rotate path so that it is split over the lowest place symmetry point (this path will start and finish at the same place)
				int lowest_place = int.MaxValue;

				foreach (int index in place_indices) {
					int[] rotated_path = new int [path.Length];

					for (int i = 0; i <= index; i++) {
						rotated_path [i + path.Length - index - 1] = path [i];
					}

					for (int i = index + 1; i < path.Length; i++) {
						rotated_path [i - index - 1] = path [i];
					}

					bool is_symm = IsSymmetrical (rotated_path);
					bool is_lower = path [index] < lowest_place;

					if (is_symm && is_lower) {
						this.rotated_path = rotated_path;

						lowest_place = path [index];
					}
				}
			}


			/// <summary>
			/// Function to determine whether this hunt bell is plain hunting.
			/// </summary>
			/// <returns>True if the hunt bell is plain hunting.</returns>
			private bool IsPlainHunting () {
				if (rotated_path is null) {
					return false;
				}

				// Cannot plain hunt if leads are an odd number of changes long.  Will also cause a crash if not returned
				if (path.Length % 2 != 0) {
					return false;
				}

				// Check that path is equal to {0, 1, 2, 3, ... n, n, ... 3, 2, 1, 0}
				int lowest_place = path.Min ();
				for (int i = 0; i < path.Length / 2; i++) {
					if (rotated_path [i] != i + lowest_place || rotated_path [path.Length - i - 1] != i + lowest_place) {
						return false;
					}
				}

				return true;
			}

			/// <summary>
			/// Function to determine whether this hunt bell is treble dodging.
			/// </summary>
			/// <returns>True if the hunt bell is treble dodging.</returns>
			private bool IsTrebleDodging () {
				if (rotated_path is null) {
					return false;
				}

				int min = path.Min ();
				int max = path.Max ();
				int range = max - min + 1;

				// Check whether path is compatible
				if (path.Length % 2 != 0) {
					return false;
				}

				if (range % 2 != 0) {
					return false;
				}

				float number_of_dodges_f = (float)path.Length / range / 2 - 1;

				if (number_of_dodges_f % 1f != 0f) {
					return false;
				}

				int number_of_dodges = (int)number_of_dodges_f;

				if (number_of_dodges == 0) {
					return false;
				}

				// Check that path is equal to {0, 1, 0, 1, 2, 3, 2, 3, ... n, n, ... 3, 2, 3, 2, 1, 0, 1, 0}
				for (int j = 0; j < range; j += 2) {
					int i = j * (number_of_dodges + 1);

					for (int d = 0; d < number_of_dodges + 1; d++) {
						if (rotated_path [i + d * 2 + 0] != min + j) { return false; }
						if (rotated_path [i + d * 2 + 1] != min + j + 1) { return false; }
						
						int l = path.Length - 1;
						if (rotated_path [l - (i + d * 2 + 0)] != min + j) { return false; }
						if (rotated_path [l - (i + d * 2 + 1)] != min + j + 1) { return false; }
					}
				}

				return true;
			}

			/// <summary>
			/// Function to determine whether this hunt bell path is symmetrical.
			/// </summary>
			/// <returns>True if the hunt bell path is symmetrical.</returns>
			private bool IsSymmetrical (int [] path = null) {
				if (path is null) {
					return rotated_path != null;
				}

				// Cannot plain hunt if leads are an odd number of changes long.  Will also cause a crash if not returned
				if (path.Length % 2 != 0) {
					return false;
				}

				// Check that path is equal to {0, 1, 2, 3, ... n, n, ... 3, 2, 1, 0}
				for (int i = 0; i < path.Length / 2; i++) {
					if (path [i] != path [path.Length - i - 1]) {
						return false;
					}
				}

				return true;
			}

			/// <summary>
			/// Function to determine whether this hunt bell spends the same number of blows in every place it visits.
			/// </summary>
			/// <returns>True if the number of blows in each place is equal.</returns>
			private bool SpendsSameNumberOfBlowsInEachPlace () {
				int [] number_of_blows_in_each_place = new int [path.Max () + 1];

				foreach (int p in path) {
					number_of_blows_in_each_place [p] += 1;
				}

				// This code could do with some re-writing.  It works, but is basically unreadable.
				int value = 0;
				for (int i = 0; i <= path.Max (); i++) {
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


			/// <summary>
			/// Creates a hunt bell object, and lets BobC# set all the boolean values.
			/// </summary>
			/// <param name="bell_number">The index (from 0) of the bell represented by this hunt bell.</param>
			/// <param name="path">The path of this hunt bell as a list of places (indexed from 0).</param>
			/// <param name="stage">The stage of the method which this hunt bell is part of.</param>
			public HuntBell (int bell_number, int [] path, Stage stage) {
				this.bell_number = bell_number;
				this.path = path;
				this.stage = stage;

				GenerateRotatedPath ();

				is_plain_hunting = IsPlainHunting ();
				is_treble_dodging = IsTrebleDodging ();

				is_symmetrical = IsSymmetrical ();
				spends_same_number_of_blows_in_each_place = SpendsSameNumberOfBlowsInEachPlace ();

				is_little = path.Max () < (int)stage - 1 || path.Min () > 0;
			}

			/// <summary>
			/// Creates a hunt bell object, setting all the boolean values manually (not reccomended).
			/// </summary>
			/// <param name="bell_number">The index (from 0) of the bell represented by this hunt bell.</param>
			/// <param name="path">The path of this hunt bell as a list of places (indexed from 0).</param>
			/// <param name="stage">The stage of the method which this hunt bell is part of.</param>
			/// <param name="is_plain_hunting">Whether the hunt bell is plain hunting.</param>
			/// <param name="is_treble_dodging">Whether the hunt bell is treble dodging.</param>
			/// <param name="is_little">Whether the hunt bell is little.</param>
			public HuntBell (int bell_number, int [] path, Stage stage, bool is_plain_hunting, bool is_treble_dodging, bool is_little) {
				this.bell_number = bell_number;
				this.path = path;
				this.stage = stage;

				GenerateRotatedPath ();

				this.is_plain_hunting = is_plain_hunting;
				this.is_treble_dodging = is_treble_dodging;
				this.is_little = is_little;
			}
		}

		/// <summary>
		/// An exception to be thrown when a method can't be classified.
		/// </summary>
		public class MethodNotClassifiedException : Exception { }

		/// <summary>
		/// An enum to store the symmetry type of a method (used, e.g. for generating calls).
		/// </summary>
		public enum SymmetryType {
			/// <summary>
			/// This method has asymmetric place notation. Could still have a symmetrical line, e.g. Stedman.
			/// </summary>
			Asymmetric,
			/// <summary>
			/// This method is symmetric over the treble's lead.
			/// </summary>
			PlainBobLike,
			/// <summary>
			/// This methodo is symmetric over the blow after the treble's lead.
			/// </summary>
			GrandsireLike
		}

		// Fields
		/// <summary>
		/// A private field for the string place notation.
		/// </summary>
		private string m_place_notation;

		/// <summary>
		/// A customisable list of call objects, which define the calls in this method.
		/// </summary>
		public List<Call> calls = new List<Call> ();

		/// <summary>
		/// The name of the method (the first part of the title, e.g. "Cambridge").
		/// </summary>
		public string name;
		/// <summary>
		/// The classification of the method (e.g. "Surprise").
		/// </summary>
		public Classification classification;
		/// <summary>
		/// The stage of this method.
		/// </summary>
		public Stage stage;

		/// <summary>
		/// True if the method should have the tag "Little" in its name.
		/// </summary>
		public bool is_little = false;
		/// <summary>
		/// True if the method should have the tag "Differential" in its name.
		/// </summary>
		public bool is_differential = false;

		/// <summary>
		/// Set this to override the auto-generated title.  Defaults to null.
		/// </summary>
		public string override_title = null;

		// Properties
		/// <summary>
		/// The string form of this method's place notation.  Setting this will update all the other fields.
		/// </summary>
		public string place_notation {
			get => m_place_notation;

			set {
				m_place_notation = value;

				RefreshNotation ();
			}
		}
		
		/// <summary>
		/// The title of the method.  If `override_title` is null, this will generate the title automagically.
		/// </summary>
		public string title => override_title ?? GenerateTitle (name, stage, classification, is_little, is_differential);

		private PlaceNotation [] m_place_notations = null;
		/// <summary>
		/// The decoded place notations for the method.
		/// </summary>
		public PlaceNotation [] place_notations {
			get {
				m_place_notations = m_place_notations ?? PlaceNotation.DecodeFullNotation (m_place_notation, stage);

				return m_place_notations;
			}
		}

		private Change [] m_plain_lead_changes = null;
		/// <summary>
		/// A list of changes in the plain lead (starting from the change after rounds).
		/// </summary>
		public Change [] plain_lead_changes {
			get {
				m_plain_lead_changes = m_plain_lead_changes ?? PlaceNotation.GenerateChangeArray (place_notations);

				return m_plain_lead_changes;
			}
		}

		private Change m_lead_end = null;
		/// <summary>
		/// The lead end change of the method.
		/// </summary>
		public Change lead_end {
			get {
				m_lead_end = m_lead_end ?? PlaceNotation.CombinePlaceNotations (place_notations);

				return m_lead_end;
			}
		}

		/// <summary>
		/// The last place notation of the method (even if the treble is not a hunt bell).
		/// </summary>
		public PlaceNotation lead_end_notation => place_notations [place_notations.Length - 1];
		/// <summary>
		/// The middle place notation of the method (and null if the lead length is odd).
		/// </summary>
		public PlaceNotation half_lead_notation => lead_length % 2 == 0 ? place_notations [place_notations.Length / 2 - 1] : null;

		private int m_leads_in_plain_course = -1;
		/// <summary>
		/// The number of leads in the plain course of this method.
		/// </summary>
		public int leads_in_plain_course {
			get {
				if (m_leads_in_plain_course == -1) {
					m_leads_in_plain_course = lead_end.order;
				}

				return m_leads_in_plain_course;
			}
		}

		/// <summary>
		/// The number of changes in one lead of this method.
		/// </summary>
		public int lead_length => place_notations.Length;
		/// <summary>
		/// The total number of changes in a plain course of this method.
		/// </summary>
		public int plain_course_length => lead_length * leads_in_plain_course;

		private int [] [] m_rotating_sets = null;
		/// <summary>
		/// Gets the rotating sets of the lead end of the method (see <see cref="Change.rotating_sets"/>).
		/// </summary>
		public int [] [] rotating_sets {
			get {
				m_rotating_sets = m_rotating_sets ?? lead_end.rotating_sets;

				return m_rotating_sets;
			}
		}

		private HuntBell [] m_hunt_bells = null;
		/// <summary>
		/// A list of <see cref="HuntBell"/> object representing the hunt bells in this method. 
		/// </summary>
		public HuntBell [] hunt_bells {
			get {
				if (m_hunt_bells is null) {
					List<int> hunt_bell_numbers = new List<int> ();
					for (int i = 0; i < (int)stage; i++) {
						if (lead_end.array [i] == i) {
							hunt_bell_numbers.Add (i);
						}
					}

					m_hunt_bells = new HuntBell [hunt_bell_numbers.Count];
					for (int i = 0; i < hunt_bell_numbers.Count; i++) {
						int [] path = new int [lead_length];

						for (int j = 0; j < lead_length; j++) {
							path [j] = plain_lead_changes [j].IndexOf (hunt_bell_numbers [i]);
						}

						m_hunt_bells [i] = new HuntBell (hunt_bell_numbers [i], path, stage);
					}
				}

				return m_hunt_bells;
			}
		}

		private SymmetryType m_symmetry_type;
		private bool has_computed_symmetry_type = false;
		/// <summary>
		/// The symmetry type of the method.
		/// </summary>
		public SymmetryType symmetry_type {
			get {
				if (!has_computed_symmetry_type) {
					has_computed_symmetry_type = true;

					if (HasPlainBobLikeSymmetry ()) {
						m_symmetry_type = SymmetryType.PlainBobLike;
					}
					else if (HasGrandsireLikeSymmetry ()) {
						m_symmetry_type = SymmetryType.GrandsireLike;
					}
					else {
						m_symmetry_type = SymmetryType.Asymmetric;
					}
				}

				return m_symmetry_type;
			}
		}

		private bool m_is_place_method;
		private bool has_computed_is_place_method = false;
		/// <summary>
		/// True if all the working bells never dodge (or make points, snap leads, etc).
		/// </summary>
		public bool is_place_method {
			get {
				if (!has_computed_is_place_method) {
					m_is_place_method = IsPlaceMethod ();

					return m_is_place_method;
				}

				return m_is_place_method;
			}
		}

		private HuntBell m_main_hunt_bell = null;
		private HuntBell GetMainHuntBell () {
			if (hunt_bells.Length == 0) {
				return null;
			} else if (hunt_bells.Length == 1) {
				return hunt_bells [0];
			} else {
				foreach (HuntBell h in hunt_bells) {
					if (h.is_symmetrical) {
						return h;
					}
				}

				return hunt_bells [0];
			}
		}
		/// <summary>
		/// The 'main' hunt bell of this method.  This is the lowest symmetrical hunt bell, or the lowest hunt bell if no symmetrical hunt bells exists.
		/// </summary>
		public HuntBell main_hunt_bell {
			get {
				m_main_hunt_bell = m_main_hunt_bell ?? GetMainHuntBell ();

				return m_main_hunt_bell;
			}
		}

		/// <summary>
		/// Generates a <see cref="Touch"/> object representing the plain course of this method.
		/// </summary>
		public Touch plain_course => new Touch (this);
		/// <summary>
		/// True if the Treble is plain hunting.
		/// </summary>
		public bool is_treble_hunting => main_hunt_bell == null ? false : main_hunt_bell.bell_number == 0;

		// Functions
		/// <summary>
		/// Automagically sets the <see cref="classification"/> variable.
		/// </summary>
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
			} else {
				is_little = main_hunt_bell.is_little;

				if (main_hunt_bell.is_plain_hunting) {
					if (hunt_bells.Length == 1) {
						if (is_place_method) {
							classification = Classification.Place;
						} else {
							classification = Classification.Bob;
						}
					} else {
						bool is_two_hunt_bell_and_symmetrical = false;
						foreach (HuntBell h in hunt_bells) {
							if (h.bell_number == 1 && h.is_symmetrical) {
								is_two_hunt_bell_and_symmetrical = true;
							}
						}

						if (is_two_hunt_bell_and_symmetrical && lead_end_notation.places_made.Contains (1)) {
							classification = Classification.SlowCourse;
						} else {
							if (is_place_method) {
								classification = Classification.Place;
							} else {
								classification = Classification.Bob;
							}
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

		/// <summary>
		/// Finds the path of any bell as a list of places (indexed from 0).  This is not always the length of the plain course.
		/// </summary>
		/// <param name="bell_index">The index (from 0) of the bell.</param>
		/// <returns>A list of places (index from 0) which represent the bell's path.</returns>
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

		/// <summary>
		/// Finds a <see cref="Call"/> object by it's full name (see <see cref="Call.name"/>).
		/// </summary>
		/// <param name="name">The full name of the call (see <see cref="Call.name"/>).</param>
		/// <returns>The <see cref="Call"/> object with the given name, or null.</returns>
		public Call GetCallByName (string name) {
			foreach (Call call in calls) {
				if (call.name.ToUpper () == name.ToUpper ()) {
					return call;
				}
			}

			return null;
		}

		/// <summary>
		/// Finds a <see cref="Call"/> object by it's notation (see <see cref="Call.notations"/>).
		/// </summary>
		/// <param name="notation">The notation of the call (see <see cref="Call.notations"/>).</param>
		/// <returns>The <see cref="Call"/> object with the given notation, or null.</returns>
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

		/// <summary>
		/// Generates a <see cref="Touch"/> object from an array of calls in the order that they should be called.
		/// </summary>
		/// <param name="calls">An array of <see cref="Call"/> objects in the order that they're called.</param>
		/// <returns>The requested <see cref="Touch"/> object.</returns>
		public Touch TouchFromCallList (Call [] calls) {
			BasicCall [] basic_calls = new BasicCall [calls.Length];

			for (int i = 0; i < calls.Length; i++) {
				basic_calls [i] = new BasicCall (calls [i], new Touch.CallLocationList ());
			}

			return new Touch (this, basic_calls);
		}

		/// <summary>
		/// Generates a <see cref="Touch"/> object from a string of call notations, in upper- or lower-case.
		/// </summary>
		/// <param name="notation">The string of call notations in the order that they'll be called.</param>
		/// <returns>The requested <see cref="Touch"/> object.</returns>
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

		/// <summary>
		/// Generates a <see cref="Touch"/> object from a string representing the calling positions.
		/// </summary>
		/// <param name="notation">An array of <see cref="Call"/> objects in the order that they're called.</param>
		/// <param name="conductor_bell">The bell index (from 0) from which the touch is called.</param>
		/// <returns>The requested <see cref="Touch"/> object.</returns>
		public Touch TouchFromCallingPositions (string notation, int conductor_bell = Constants.tenor) {
			List<BasicCall> calls = new List<BasicCall> ();

			Call current_call = GetCallByName (Call.bob_name);
			foreach (char c in notation) {
				if (Constants.alpha.Contains (c)) { // `c` is lower-case, and therefore a call name
					current_call = GetCallByNotation (c);
				}

				if (Constants.ALPHA.Contains (c)) { // `c` is upper-case, and therefore a calling position
					calls.Add (new BasicCall (
						current_call,
						new Touch.CallLocationCallingPosition (c)
					));

					current_call = GetCallByName (Call.bob_name);
				}

				// Otherwise `c` is a random character, and should be ignored
			}

			return new Touch (this, calls.ToArray (), conductor_bell);
		}

		// Private functions
		/// <summary>
		/// Determines whether this method has Plain Bob-like symmetry.
		/// </summary>
		/// <returns>True if this method has Plain Bob-like symmetry.</returns>
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

		/// <summary>
		/// Determines whether this method has Grandsire-like symmetry.
		/// </summary>
		/// <returns>True if this method has Grandsire-like symmetry.</returns>
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

		/// <summary>
		/// Determines whether the working bells only make places.
		/// </summary>
		/// <returns>False if any working bell makes any dodges (or snaps, points, etc.).</returns>
		private bool IsPlaceMethod () {
			foreach (int[] set in rotating_sets) {
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
					if (path [last_index] == path [next_index]) {
						if (Math.Abs (path [next_index] - path [current_index]) == 1) {
							return false;
						}
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Called whenever the method's <see cref="place_notation"/> is updated.
		/// </summary>
		private void RefreshNotation () {
			m_place_notations = null;
			m_plain_lead_changes = null;

			m_lead_end = null;
			m_leads_in_plain_course = -1;

			m_rotating_sets = null;

			has_computed_symmetry_type = false;
			has_computed_is_place_method = false;
			
			m_hunt_bells = null;
		}

		/// <summary>
		/// Generates standard (Bob, Single and Plain) <see cref="Call"/> objects for this method.
		/// </summary>
		/// <param name="overwrite_current">Determines whether to overwrite the current call array.</param>
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

						char n_minus_2 = Constants.GetBellNameIndexingFromOne ((int)stage - 2);
						char n_minus_1 = Constants.GetBellNameIndexingFromOne ((int)stage - 1);
						char n = Constants.GetBellNameIndexingFromOne ((int)stage);

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
		/// <summary>
		/// Generates a method and specifying all classifications.
		/// </summary>
		/// <param name="place_notation">The place notation of the method, e.g. "x3x4x25x36x4x5x6x7,2". Can have implicit places.</param>
		/// <param name="name">The name of the method, e.g. "Cambridge".</param>
		/// <param name="classification">The classification of the method, e.g. <see cref="Classification.Surprise"/></param>
		/// <param name="stage">The stage of the method, e.g. <see cref="Stage.Major"/></param>
		/// <param name="override_title">Set this to manually set the <see cref="title"/> value, e.g. for Gransire.</param>
		/// <param name="is_little">True if the "Little" tag should appear in the method title.</param>
		/// <param name="is_differential">True if the "Differential" tag should appear in the method title.</param>
		/// <param name="generate_standard_calls">If, true BobC# will automagically set the standard Bob, Single and Plain calls.</param>
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

		/// <summary>
		/// Generates a method without specifying any classifications, leaving BobC# to classify it for you.
		/// </summary>
		/// <param name="place_notation">The place notation of the method, e.g. "x3x4x25x36x4x5x6x7,2". Can have implicit places.</param>
		/// <param name="name">The name of the method, e.g. "Cambridge".</param>
		/// <param name="stage">The stage of the method, e.g. <see cref="Stage.Major"/></param>
		/// <param name="override_title">Set this to manually set the <see cref="title"/> value, e.g. for Gransire.</param>
		/// <param name="generate_standard_calls">If, true BobC# will automagically set the standard Bob, Single and Plain calls.</param>
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
		/// <summary>
		/// Gets a method with a given title from the CCCBR method library (null if no method of that title exists).
		/// </summary>
		/// <param name="title">The title of the method you want to find.</param>
		/// <returns>The <see cref="Method"/> object with that title (or null).</returns>
		public static Method GetMethod (string title) => MethodLibrary.GetMethodByTitle (title);

		/// <summary>
		/// Generates a method's title given a load of attributes
		/// </summary>
		/// <param name="name">The name of the method.</param>
		/// <param name="stage">The stage of the method.</param>
		/// <param name="classification">The classification of the method.</param>
		/// <param name="is_little">True if the tag "Little" should be included in the title.</param>
		/// <param name="is_differential">True if the tag "Differential" should be included in the title.</param>
		/// <returns></returns>
		public static string GenerateTitle (string name, Stage stage, Classification classification, bool is_little, bool is_differential) {
			bool called_differential = is_differential && classification != Classification.Differential;

			string output;

			if (classification == Classification.Principle) {
				output = name + " " + Utils.StageToString (stage);
			} else {
				output = name + (called_differential ? " Differential" : "") + (is_little ? " Little" : "") + " " + Utils.ClassificationToString (classification) + " " + Utils.StageToString (stage);
			}

			if (name == "") {
				return output.Substring (1);
			} else {
				return output;
			}
		}

		/// <summary>
		/// Shortcut to generate Plain Bob Doubles. (Only for testing; use <c>Method.GetMethod ("Plain Bob Doubles")</c> instead).
		/// </summary>
		public static Method plain_bob_doubles => new Method ("5.1.5.1.5,125", "Plain", Classification.Bob, Stage.Doubles);
		/// <summary>
		/// Shortcut to generate Plain Bob Minor. (Only for testing; use <c>Method.GetMethod ("Plain Bob Minor")</c> instead).
		/// </summary>
		public static Method plain_bob_minor => new Method ("x16x16x16,12", "Plain", Classification.Bob, Stage.Minor);
		/// <summary>
		/// Shortcut to generate Plain Bob Triples. (Only for testing; use <c>Method.GetMethod ("Plain Bob Triples")</c> instead).
		/// </summary>
		public static Method plain_bob_triples => new Method ("7.1.7.1.7.1.7,12", "Plain", Classification.Bob, Stage.Triples);

		/// <summary>
		/// Shortcut to generate Grandsire Doubles. (Only for testing; use <c>Method.GetMethod ("Grandsire Doubles")</c> instead).
		/// </summary>
		public static Method grandsire_doubles => new Method ("3,1.5.1.5.1", "Grandsire", Classification.Bob, Stage.Doubles, "Grandsire Doubles");
		/// <summary>
		/// Shortcut to generate Grandsire Triples. (Only for testing; use <c>Method.GetMethod ("Grandsire Triples")</c> instead).
		/// </summary>
		public static Method grandsire_triples => new Method ("3,1.7.1.7.1.7.1", "Grandsire", Classification.Bob, Stage.Triples, "Grandsire Triples");

		/// <summary>
		/// Shortcut to generate Cambridge Major. (Only for testing; use <c>Method.GetMethod ("Cambridge Surprise Major")</c> instead).
		/// </summary>
		public static Method cambridge_major => new Method ("x38x14x1258x36x14x58x16x78,12", "Cambridge", Classification.Surprise, Stage.Major);
	}
}
