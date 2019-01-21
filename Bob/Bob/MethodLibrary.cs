using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A class to store the CCCBR library of methods.
	/// </summary>
	public class MethodLibrary {
		// Classes/enums
		/// <summary>
		/// A performant representation of a method, to avoid having to make 20,000+ <see cref="Method"/> objets.
		/// </summary>
		public class StoredMethod {
			/// <summary>
			/// The name of the method.
			/// </summary>
			public string name;

			/// <summary>
			/// The title of the method.
			/// </summary>
			public string override_title;

			/// <summary>
			/// The stage of the method.
			/// </summary>
			public Stage stage;

			/// <summary>
			/// The place notation of the method.
			/// </summary>
			public string place_notation;

			/// <summary>
			/// The classification of the method (to speed up loading methods).
			/// </summary>
			public Classification classification;
			
			/// <summary>
			/// True if the tag "Little" should be included in the method title.
			/// </summary>
			public bool is_little;
			
			/// <summary>
			/// True if the tag "Differential" should be included in the method title.
			/// </summary>
			public bool is_differential;

			/// <summary>
			/// The title of the method.
			/// </summary>
			public string title;

			/// <summary>
			/// An array of calls.  If null, calls will automagically be generated.
			/// </summary>
			public StoredCall [] calls;

			/// <summary>
			/// Generates a proper method object for this method class.
			/// </summary>
			public Method method {
				get {
					Method output = new Method (place_notation, name, classification, stage, override_title, is_little, is_differential);

					if (calls != null && calls.Length > 0) {
						// Sort out plain calls
						int? first_every = calls [0].every;
						int? first_from = calls [0].from;

						int? plain_every = calls.All (x => x.every == first_every) ? first_every : null;
						int? plain_from = calls.All (x => x.from == first_from) ? first_from : null;

						if (plain_every == null && plain_from == null) {
							output.calls.Add (Call.LeadEndPlain (output));
						} else {
							output.AddPlainCall (plain_every ?? output.lead_length, plain_from ?? 0, 1);
						}

						// Now just add the others
						foreach (StoredCall call in calls) {
							output.calls.Add (new Call (
								output,
								call.name,
								call.notations,
								PlaceNotation.DecodeFullNotation (call.place_notation, output.stage),
								call.every ?? output.lead_length,
								call.from ?? 0,
								call.cover ?? -1
							));
						}
					}

					return output;
				}
			}

			/// <summary>
			/// Creates a new <see cref="StoredMethod"/> object.
			/// </summary>
			/// <param name="name">The name of the method.</param>
			/// <param name="stage">The stage of the method.</param>
			/// <param name="place_notation">The place notation of the method.</param>
			/// <param name="classification">The classification of the method.</param>
			/// <param name="is_little">True if the tag "Little" should be included in the method title.</param>
			/// <param name="is_differential">True if the tag "Differential" should be included in the method title.</param>
			/// <param name="calls">An array of calls.  If null, calls will automagically be generated.</param>
			/// <param name="override_title">Set this from null to override the title of the method.</param>
			public StoredMethod (string name, Stage stage, string place_notation, Classification classification, bool is_little, bool is_differential, StoredCall [] calls, string override_title = null) {
				this.name = name;
				this.stage = stage;
				this.place_notation = place_notation;
				this.classification = classification;
				this.is_little = is_little;
				this.is_differential = is_differential;
				this.override_title = override_title;
				this.calls = calls;

				title = override_title ?? Method.GenerateTitle (name, stage, classification, is_little, is_differential);
			}
		}

		/// <summary>
		/// A performant representation of call, for use with <see cref="StoredMethod"/> objects.
		/// </summary>
		public class StoredCall {
			/// <summary>
			/// The (full) name of the call.
			/// </summary>
			public string name;

			/// <summary>
			/// The array of possible notations, with the preferred notation first.
			/// </summary>
			public char [] notations;

			/// <summary>
			/// The place notation of the call.
			/// </summary>
			public string place_notation;

			/// <summary>
			/// Every how often the call can be called.  If null, defaults to the method's lead end.
			/// </summary>
			public int? every;

			/// <summary>
			/// The index of the first change of the place notations relative to each repeat of <see cref="every"/>.  If null, defaults to 0.
			/// </summary>
			public int? from;

			/// <summary>
			/// How many changes of the original method are taken over by the call.  If null, defaults to the length of the call.
			/// </summary>
			public int? cover;

			/// <summary>
			/// Generates a <see cref="StoredCall"/> from the compact string representation.
			/// </summary>
			/// <param name="representation"></param>
			public StoredCall (string representation) {
				string [] parts = representation.Split ('|');

				int start_index = 0;
				
				if (parts [0].Contains (':')) {
					int i = parts [0].IndexOf (':');

					string call_name = parts [0].Substring (0, i);

					if (call_name == "Bob") {
						name = Call.bob_name;
						notations = Call.bob_notations;
					} else if (call_name == "Single") {
						name = Call.single_name;
						notations = Call.single_notations;
					} else {
						throw new Exception ("Unknown call name found.");
					}

					place_notation = parts [0].Substring (i + 1);

					start_index = 1;
				} else {
					name = parts [0];
					notations = parts [1].ToArray ();
					place_notation = parts [2];

					start_index = 3;
				}

				int length = parts.Length - start_index;

				if (length == 0) {
					every = null;
					from = null;
					cover = null;
				} else if (length == 1) {
					every = int.Parse (parts [start_index]);
					from = null;
					cover = null;
				} else if (length == 2) {
					string every = parts [start_index];
					string from = parts [start_index + 1];

					this.every = every == "" ? null : (int?)int.Parse (every);
					this.from = int.Parse (from);
					cover = null;
				} else {
					throw new Exception ("Unknown call length found.");
				}
			}

			/// <summary>
			/// Creates a <see cref="StoredCall"/> object.
			/// </summary>
			/// <param name="name">The (full) name of the call.</param>
			/// <param name="notations">The array of possible notations, with the preferred notation first.</param>
			/// <param name="place_notation">The place notation of the call.</param>
			/// <param name="every">Every how often the call can be called.  If null, defaults to the method's lead end.</param>
			/// <param name="from">The index of the last change of the place notations relative to each repeat of <see cref="every"/>.  If null, defaults to 0.</param>
			/// <param name="cover">How many changes of the original method are taken over by the call.  If null, defaults to the length of the call.</param>
			public StoredCall (string name, char [] notations, string place_notation, int? every, int? from, int? cover) {
				this.name = name;
				this.notations = notations;
				this.place_notation = place_notation;
				this.every = every;
				this.from = from;
				this.cover = cover;
			}
		}

		// Non-static stuff
		/// <summary>
		/// An array of <see cref="StoredMethod"/> objects of every method in the library.
		/// </summary>
		public StoredMethod [] stored_methods;

		/// <summary>
		/// Creates a method library from a compressed text file (`methods.txt`).
		/// </summary>
		/// <param name="method_data">Set this to override the default method data.</param>
		public MethodLibrary (string method_data = null) {
			string data = method_data ?? Properties.Resources.methods;
			string [] lines = data.Split ('\n');

			List<StoredMethod> stored_methods = new List<StoredMethod> ();

			for (int i = 0; i < lines.Length; i++) {
				if (lines [i] == "") {
					continue;
				}

				string basic_data = lines [i].Substring (0, lines [i].Length - 1);
				List<StoredCall> calls = null;

				if (lines [i].Contains ('\\')) {
					basic_data = lines [i].Substring (0, lines [i].IndexOf ('\\'));
					
					// Generate calls
					string calls_data = lines [i].Substring (lines [i].IndexOf ('\\') + 1);

					calls = new List<StoredCall> ();

					string call_contents = "";

					foreach (char c in calls_data) {
						if (c == '{') {
							call_contents = "";
						} else if (c == '}') {
							calls.Add (new StoredCall (call_contents));
						} else {
							call_contents += c;
						}
					}
				}

				string [] parts = basic_data.Split ('|');

				string name = parts [0];
				string notation = parts [1];
				Stage stage = (Stage)Constants.int_value_lookup.IndexOf (parts [2] [0]);
				Classification classification = (Classification)Constants.int_value_lookup.IndexOf (parts [2] [1]);
				string override_title = parts.Length == 4 ? parts [3] : null;

				bool is_little = false;
				bool is_differential = false;

				if (parts [2].Length == 3) {
					char c = parts [2] [2];

					if (c == 'l') { is_little = true; }
					if (c == 'd') { is_differential = true; }
					if (c == 'b') { is_differential = is_little = true; }
				}

				stored_methods.Add (new StoredMethod (name, stage, notation, classification, is_little, is_differential, calls?.ToArray (), override_title));
			}

			stored_methods.Sort ((a, b) => a.title.CompareTo (b.title));

			this.stored_methods = stored_methods.ToArray ();
		}

		private IEnumerable<Method> m_all_methods = null;
		/// <summary>
		/// Generates <see cref="Method"/> objects for every method in the CCCBR method library.  Warning! This function creates a lot of memory, so is not to be used lightly.
		/// </summary>
		/// <returns>All the methods as an array.</returns>
		public IEnumerable<Method> GetAllMethods () {
			if (m_all_methods == null) {
				m_all_methods = stored_methods.Select (x => x.method);
			}

			return m_all_methods;
		}

		/// <summary>
		/// Finds a method with a given title in the CCCBR method library.
		/// </summary>
		/// <param name="title">The title of the method.</param>
		/// <returns>The method with the given title (null if no such method exists in the CCCBR library).</returns>
		public static Method GetMethodByTitle (string title) {
			// Use binary search, since the methods are sorted by title
			uint min_index = 0;
			uint max_index = (uint)library.stored_methods.Length;

			while (max_index - min_index > 1) {
				uint mid_index = (min_index + max_index) >> 1; // >> 1 is the same as division and truncation by two

				int comparison = library.stored_methods [mid_index].title.CompareTo (title);

				if (comparison == 0) {
					return library.stored_methods [mid_index].method;
				} else if (comparison < 0) {
					min_index = mid_index;
				} else if (comparison > 0) {
					max_index = mid_index;
				}
			}

			return null;
		}

		private static MethodLibrary m_library = null;
		/// <summary>
		/// Gets/creates a <see cref="MethodLibrary"/> object for the CCCBR method library.
		/// </summary>
		public static MethodLibrary library {
			get {
				if (m_library is null) {
					m_library = new MethodLibrary ();
				}

				return m_library;
			}
		}
	}
}
