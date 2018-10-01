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
			/// Generates a proper method object for this method class.
			/// </summary>
			public Method method => new Method (place_notation, name, classification, stage, override_title, is_little, is_differential);

			/// <summary>
			/// Creates a new <see cref="StoredMethod"/> object.
			/// </summary>
			/// <param name="name">The name of the method.</param>
			/// <param name="stage">The stage of the method.</param>
			/// <param name="place_notation">The place notation of the method.</param>
			/// <param name="classification">The classification of the method.</param>
			/// <param name="is_little">True if the tag "Little" should be included in the method title.</param>
			/// <param name="is_differential">True if the tag "Differential" should be included in the method title.</param>
			/// <param name="override_title">Set this from null to override the title of the method.</param>
			public StoredMethod (string name, Stage stage, string place_notation, Classification classification, bool is_little, bool is_differential, string override_title = null) {
				this.name = name;
				this.stage = stage;
				this.place_notation = place_notation;
				this.classification = classification;
				this.is_little = is_little;
				this.is_differential = is_differential;
				this.override_title = override_title;

				title = override_title ?? Method.GenerateTitle (name, stage, classification, is_little, is_differential);
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

				string [] parts = lines [i].Substring (0, lines [i].Length - 1).Split ('|');

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

				stored_methods.Add (new StoredMethod (name, stage, notation, classification, is_little, is_differential, override_title));
			}

			this.stored_methods = stored_methods.ToArray ();
		}

		/// <summary>
		/// Finds a method with a given title in the CCCBR method library.
		/// </summary>
		/// <param name="title">The title of the method.</param>
		/// <returns>The method with the given title (null if no such method exists in the CCCBR library).</returns>
		public static Method GetMethodByTitle (string title) {
			foreach (StoredMethod stored_method in library.stored_methods) {
				if (stored_method.title == title) {
					return stored_method.method;
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
