using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

		// Used in `GenerateExtents`
		private enum TouchTruth { False, True, Extent }

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

		private int [] m_working_bells = null;
		/// <summary>
		/// A list of the indices (from 0) of the working bells (non-hunt bells) in increasing order.
		/// </summary>
		public int [] working_bells {
			get {
				if (m_working_bells == null) {
					List<int> working_bells = new List<int> ();

					for (int i = 0; i < (int)stage; i++) {
						working_bells.Add (i);
					}

					foreach (HuntBell hunt_bell in hunt_bells) {
						working_bells.Remove (hunt_bell.bell_number);
					}

					m_working_bells = working_bells.ToArray ();
				}

				return m_working_bells;
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

		private int [] m_plain_coursing_order = null;
		/// <summary>
		/// Gets the coursing order of the plain course of this method, according to leading order.
		/// </summary>
		public int [] plain_coursing_order {
			get {
				if (m_plain_coursing_order == null) {
					m_plain_coursing_order = GetCoursingOrder ();
				}

				return m_plain_coursing_order;
			}
		}

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

			// Generate the list of positions by iterating over the plain course changes
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
		/// Gets the coursing order of this method at a given lead end.
		/// </summary>
		/// <param name="lead_end">The lead end who's coursing order you want.  If null, will default to rounds.</param>
		/// <param name="assume_plain_bob_like">Setting this to true will tell the function not to look at leading order (makes this more accurate for methods such as Cambridge).</param>
		/// <returns>The coursing order of the method as an array indexed from 0.</returns>
		public int [] GetCoursingOrder (Change lead_end = null, bool assume_plain_bob_like = false) {
			lead_end = lead_end ?? Change.Rounds (stage);

			int [] order = null;

			if (assume_plain_bob_like) {
				// Read off the transposition from the lead end as though the method was Plain Bob
				order = new int [working_bells.Length];

				for (int i = 0; i < working_bells.Length; i++) {
					int ind = i * 2;

					if (ind >= working_bells.Length) {
						ind = working_bells.Length * 2 - ind - 1;
					}

					order [i] = working_bells [ind];
				}
			} else {
				// Actually look at the leading
				List<int> coursing_order = new List<int> ();

				Touch plain_course = this.plain_course;

				foreach (Change change in plain_course.changes) {
					// Calculate the bell which is currently leading
					int leading_bell = lead_end [change [0]];

					// If it's already been counted, discard it
					if (coursing_order.Contains (leading_bell)) {
						continue;
					}

					// If it's not a hunt bell, add it to the list
					if (working_bells.Contains (leading_bell)) {
						coursing_order.Add (leading_bell);
					}

					// Stop the loop if all the bells have been accounted for
					if (coursing_order.Count == working_bells.Length) {
						break;
					}
				}

				order = coursing_order.ToArray ();
			}

			// Rotate the list so the heaviest bell is first
			int max_value = order.Max ();
			int index = order.ToList ().IndexOf (max_value);

			int [] output = new int [order.Length];

			for (int i = 0; i < order.Length; i++) {
				int ind = i + index;

				if (ind >= order.Length) {
					ind -= order.Length;
				}

				output [i] = order [ind];
			}

			// Return the list

			return output;
		}

		/// <summary>
		/// Gets the coursing order of this method at a given lead end, formatted as a string.
		/// </summary>
		/// <param name="lead_end">The lead end who's coursing order you want.  If null, will default to rounds.</param>
		/// <param name="assume_plain_bob_like">Setting this to true will tell the function not to look at leading order (makes this more accurate for methods such as Cambridge).</param>
		/// <param name="discard_heavy_bells_in_plain_coursing_order">This will remove all the heavy bells (tenors) which are in the same order as they appear in the plain course.</param>
		/// <param name="heaviest_bell_to_always_keep">The largest bell which will always be shown in the coursing order (indexed from 0).</param>
		/// <returns>The formatted coursing order.</returns>
		public string GetCoursingOrderString (Change lead_end = null, bool assume_plain_bob_like = false, bool discard_heavy_bells_in_plain_coursing_order = true, int heaviest_bell_to_always_keep = 5) {
			// Get the coursing order
			List <int> coursing_order = GetCoursingOrder (lead_end, assume_plain_bob_like).ToList ();
			
			// Potentially strip off the tenors
			if (discard_heavy_bells_in_plain_coursing_order) {
				// Decide which bells need to be removed from the string.
				List<int> indices_to_remove = new List<int> ();

				for (int i = (int)stage - 1; i > heaviest_bell_to_always_keep; i--) {
					int index = coursing_order.IndexOf (i);

					if (index == plain_coursing_order.ToList ().IndexOf (i)) {
						indices_to_remove.Add (index);
					} else {
						break;
					}
				}

				// Remove the elements from the back of the list, so that the indexes still apply
				indices_to_remove.Sort ();
				indices_to_remove.Reverse ();

				foreach (int index in indices_to_remove) {
					coursing_order.RemoveAt (index);
				}
			}

			// Generate string
			string output = "";

			foreach (int i in coursing_order) {
				output += Constants.GetBellNameIndexingFromZero (i);
			}

			return output;
		}

		/// <summary>
		/// Sets the lead end calls in this method, overwriting them if necessary.
		/// </summary>
		/// <param name="bob_notation">The place notation of the bob.  Can be multiple changes long.</param>
		/// <param name="single_notation">The place notation of the single.  Can be multiple changes long.</param>
		/// <param name="bob_calling_positions">The calling positions of the bob.</param>
		/// <param name="single_calling_positions">The calling positions of the single.</param>
		public void SetLeadEndCalls (string bob_notation = null, string single_notation = null, string [] bob_calling_positions = null, string [] single_calling_positions = null) {
			if (GetCallByName (Call.plain_name) == null) {
				calls.Add (Call.LeadEndPlain (this));
			}

			if (bob_notation != null) {
				Call bob = GetCallByName (Call.bob_name);

				if (bob == null) {
					calls.Add (Call.LeadEndBob (this, bob_notation, bob_calling_positions));
				} else {
					bob.place_notations = PlaceNotation.DecodeFullNotation (bob_notation, stage);
					bob.calling_positions = bob_calling_positions;
				}
			}

			if (single_notation != null) {
				Call single = GetCallByName (Call.single_name);

				if (single == null) {
					calls.Add (Call.LeadEndSingle (this, single_notation, single_calling_positions));
				} else {
					single.place_notations = PlaceNotation.DecodeFullNotation (bob_notation, stage);
					single.calling_positions = bob_calling_positions;
				}
			}
		}

		/// <summary>
		/// Generates and adds a new plain call to this method in the requested location.
		/// </summary>
		/// <param name="every">Every how many changes this call can be called.</param>
		/// <param name="from">How many changes away from every `every` changes the call can be called.  E.g. for Stedman this will be -3, and `every` will be 6.  For lead-end calls this is set automagically.</param>
		/// <param name="length">How many changes long the call is.</param>
		public void AddPlainCall (int every, int from, int length) {
			Call plain = GetCallByName (Call.plain_name);
			if (plain != null) {
				calls.Remove (plain);
			}

			calls.Add (new Call (this, Call.plain_name, Call.plain_notations, new PlaceNotation [length], every, from));
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
		public Touch TouchFromCallList (Call [] calls) => new Touch (this, calls);

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

		/// <summary>
		/// Finds the list of all possible extents, and returns their notations as call lists.  Warning: it's not a good idea to run this on stages higher than Major, because it will take a LONG time.
		/// </summary>
		/// <param name="possible_call_notations">The list of calls notations used to create the extents (includes plain leads).  If null, it will use all possible call notations.</param>
		/// <param name="extent_length_limit">The longest extent notation that will be computed.</param>
		/// <param name="stop_after_extent_number">The function will stop when this many exents are reached.</param>
		/// <param name="add_to_extent_notations">If set to true, the function will continue computing touches which start with a legit extent, thus flagging up unneccessary repeats.</param>
		/// <param name="print">If set to true, this function will print out what it's doing.  Useful for long computations.</param>
		/// <returns>All possible extents of this method, as call lists.</returns>
		public string [] GenerateExtents (string possible_call_notations = null, int extent_length_limit = -1, int stop_after_extent_number = -1, bool add_to_extent_notations = false, bool print = true) => GenerateExtents (possible_call_notations?.ToCharArray (), extent_length_limit, stop_after_extent_number, add_to_extent_notations, print);

		/// <summary>
		/// Finds the list of all possible extents, and returns their notations as call lists.  Warning: it's not a good idea to run this on stages higher than Major, because it will take a LONG time.
		/// </summary>
		/// <param name="possible_call_notations">The list of calls notations used to create the extents (includes plain leads).  If null, it will use all possible call notations.</param>
		/// <param name="extent_length_limit">The longest extent notation that will be computed.</param>
		/// <param name="stop_after_extent_number">The function will stop when this many exents are reached.</param>
		/// <param name="add_to_extent_notations">If set to true, the function will continue computing touches which start with a legit extent, thus flagging up unneccessary repeats.</param>
		/// <param name="print">If set to true, this function will print out what it's doing.  Useful for long computations.</param>
		/// <returns>All possible extents of this method, as call lists.</returns>
		public string [] GenerateExtents (char [] possible_call_notations = null, int extent_length_limit = -1, int stop_after_extent_number = -1, bool add_to_extent_notations = false, bool print = true) {
			// Populate `possible_call_notations` if it's set to null.
			if (possible_call_notations == null) {
				possible_call_notations = new char [calls.Count];

				for (int i = 0; i < calls.Count; i++) {
					possible_call_notations [i] = calls [i].preferred_notation;
				}
			}

			// Create some lists to store data
			List<string> extents = new List<string> ();
			List<string> false_notations = new List<string> ();
			List<string> current_possible_touches = new List<string> ();

			// Generate all the touches of 1 call (this is just all the possible notations)
			foreach (char c in possible_call_notations) {
				current_possible_touches.Add (c.ToString ());
			}

			// Setup some variables for later use
			int every = calls [0].every;
			float number_of_calls_in_an_extent_f = (float)Utils.Factorial ((int)stage) / every;

			int number_of_calls_in_an_extent;

			if (number_of_calls_in_an_extent_f % 1f == 0f) {
				number_of_calls_in_an_extent = (int)number_of_calls_in_an_extent_f;
			} else {
				throw new NotImplementedException ("Extent finder doesn't work for methods whose extent is not a whole number of calls repeats long.");
			}

			int max_notation_length = extent_length_limit == -1 ? number_of_calls_in_an_extent : extent_length_limit;

			// Now generate touches
			for (int l = 0; l < max_notation_length; l++) {
				int num_calls = l + 1;

				if (print) {
					Console.WriteLine ("Computing touches of length " + num_calls + ".");
				}

				// Turn the current touches into longer touches
				if (l > 0) {
					List<string> new_touches = new List<string> ();

					string spacer = l % 5 == 0 ? " " : "";

					foreach (string old_touch in current_possible_touches) {
						foreach (char c in possible_call_notations) {
							new_touches.Add (old_touch + spacer + c);
						}
					}

					// Strip out touches which contain sequences of calls known to be false
					for (int i = new_touches.Count - 1; i >= 0; i--) {
						foreach (string s in false_notations) {
							if (new_touches [i].Contains (s)) {
								if (print) {
									Console.WriteLine ("\t" + new_touches [i] + " rejected because it contains " + s + " which is false.");
								}

								new_touches.RemoveAt (i);

								break;
							}
						}
					}

					// Overwrite the current touch array
					current_possible_touches = new_touches;
				}

				if (print) {
					Console.WriteLine ("\t" + current_possible_touches.Count + " current touches.");
				}

				if (current_possible_touches.Count == 0) {
					if (print) {
						Console.WriteLine ("\t\tStopping because of no touches.");
					}

					return extents.ToArray ();
				}

				// Run the truth checks on these touches
				List<int> extent_indices = new List<int> ();

				// Put the truth check in a function to allow parallelism
				TouchTruth ComputeTouch (string touch_notation) {
					Touch touch = TouchFromCallList (touch_notation);

					// Does the touch come round prematurely
					if (touch.Length <= num_calls * every) {
						if (print)
							Console.WriteLine ("\t\t" + touch_notation + " is not long enough.");

						return TouchTruth.False;
					} else if (!touch.GetSegment (0, num_calls * every).is_true) {
						if (print)
							Console.WriteLine ("\t\t" + touch_notation + " is false.");

						return TouchTruth.False;
					} else if (touch.is_extent) {
						return TouchTruth.Extent;
					}

					return TouchTruth.True;
				}

				#region WARNING! Highly optimised spaghetti code in here.
				int processors = Environment.ProcessorCount;

				List<string> [] notation_slices = new List<string> [processors];

				for (int i = 0; i < notation_slices.Length; i++) {
					notation_slices [i] = new List<string> ();
				}

				for (int i = 0; i < current_possible_touches.Count; i++) {
					notation_slices [Utils.Mod (i, processors)].Add (current_possible_touches [i]);
				}

				TouchTruth [] [] truth_slices = notation_slices.Select (x => x.Select (y => ComputeTouch (y)).ToArray ()).ToArray ();

				List<TouchTruth> truth_checks = new List<TouchTruth> ();
				
				for (int i = 0; i < truth_slices.Length; i++) {
					for (int j = 0; j < truth_slices [i].Length; j++) {
						truth_checks.Add (truth_slices [i] [j]);
					}
				}
				#endregion

				for (int i = 0; i < current_possible_touches.Count; i++) {
					string touch_notation = current_possible_touches [i];

					TouchTruth truth = truth_checks [i];

					if (truth == TouchTruth.False) {
						false_notations.Add (touch_notation);
					} else if (truth == TouchTruth.Extent) {
						extents.Add (touch_notation);

						if (print) {
							Console.WriteLine ("\t\t\t" + touch_notation + " is an extent!");
						}

						extent_indices.Add (i);

						if (extents.Count == stop_after_extent_number) {
							if (print) {
								Console.WriteLine ("Stopping because extent number limit is reached.");
							}

							return extents.ToArray ();
						}
					}
				}

				// Remove the extents from the list of touches which get added to.
				if (!add_to_extent_notations) {
					extent_indices.Reverse ();

					foreach (int i in extent_indices) {
						current_possible_touches.RemoveAt (i);
					}
				}
			}

			// Return the extents which were found
			return extents.ToArray ();
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
			if ((int)stage < 5) {
				return;
			}
			
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
		[Obsolete]
		public static Method plain_bob_doubles => new Method ("5.1.5.1.5,125", "Plain", Classification.Bob, Stage.Doubles);
		/// <summary>
		/// Shortcut to generate Plain Bob Minor. (Only for testing; use <c>Method.GetMethod ("Plain Bob Minor")</c> instead).
		/// </summary>
		[Obsolete]
		public static Method plain_bob_minor   => new Method ("x16x16x16,12", "Plain", Classification.Bob, Stage.Minor);
		/// <summary>
		/// Shortcut to generate Plain Bob Triples. (Only for testing; use <c>Method.GetMethod ("Plain Bob Triples")</c> instead).
		/// </summary>
		[Obsolete]
		public static Method plain_bob_triples => new Method ("7.1.7.1.7.1.7,12", "Plain", Classification.Bob, Stage.Triples);

		/// <summary>
		/// Shortcut to generate Grandsire Doubles. (Only for testing; use <c>Method.GetMethod ("Grandsire Doubles")</c> instead).
		/// </summary>
		[Obsolete]
		public static Method grandsire_doubles => new Method ("3,1.5.1.5.1", "Grandsire", Classification.Bob, Stage.Doubles, "Grandsire Doubles");
		/// <summary>
		/// Shortcut to generate Grandsire Triples. (Only for testing; use <c>Method.GetMethod ("Grandsire Triples")</c> instead).
		/// </summary>
		[Obsolete]
		public static Method grandsire_triples => new Method ("3,1.7.1.7.1.7.1", "Grandsire", Classification.Bob, Stage.Triples, "Grandsire Triples");

		/// <summary>
		/// Shortcut to generate Cambridge Major. (Only for testing; use <c>Method.GetMethod ("Cambridge Surprise Major")</c> instead).
		/// </summary>
		[Obsolete]
		public static Method cambridge_major   => new Method ("x38x14x1258x36x14x58x16x78,12", "Cambridge", Classification.Surprise, Stage.Major);
	}
}
