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
			/// The stage of the method.
			/// </summary>
			public Stage stage;
			/// <summary>
			/// The place notation of the method.
			/// </summary>
			public string place_notation;

			/// <summary>
			/// Generates a proper method object for this method class.
			/// </summary>
			public Method method => new Method (place_notation, name, stage);

			/// <summary>
			/// Creates a <see cref="StoredMethod"/> object.s
			/// </summary>
			/// <param name="name">The name of the method.</param>
			/// <param name="stage">The stage of the method.</param>
			/// <param name="place_notation">The place notation of the method.</param>
			public StoredMethod (string name, Stage stage, string place_notation) {
				this.name = name;
				this.stage = stage;
				this.place_notation = place_notation;
			}
		}

		// Non-static stuff
		/// <summary>
		/// An array of <see cref="StoredMethod"/> objects of every method in the library.
		/// </summary>
		public StoredMethod [] stored_methods;

		/// <summary>
		/// Creates a method library from a compressed text file.
		/// </summary>
		/// <param name="override_path">Set this to override the default path (<see cref="library_path"/>).</param>
		public MethodLibrary (string override_path = null) {
			string path = override_path ?? library_path;

			string [] lines = System.IO.File.ReadAllLines (path);

			stored_methods = new StoredMethod [lines.Length];

			for (int i = 0; i < lines.Length; i++) {
				string [] parts = lines [i].Split ('|');

				stored_methods [i] = new StoredMethod (parts [0], Stage.Doubles, parts [1]);
			}
		}

		// Static stuff
		/// <summary>
		/// The default path to the compressed version of the CCCBR method library.
		/// </summary>
		public static string library_path = "../../../../Bob/compressed_method_library.txt";

		/// <summary>
		/// Finds a method with a given title in the CCCBR method library.
		/// </summary>
		/// <param name="title">The title of the method.</param>
		/// <returns>The method with the given title (null if no such method exists in the CCCBR library).</returns>
		public static Method GetMethodByTitle (string title) {
			foreach (StoredMethod stored_method in library.stored_methods) {
				if (stored_method.name == title) {
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
