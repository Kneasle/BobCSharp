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
			public string title;
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
			public Method method => new Method (place_notation, title, stage);

			/// <summary>
			/// Creates a <see cref="StoredMethod"/> object.s
			/// </summary>
			/// <param name="title">The title of the method.</param>
			/// <param name="stage">The stage of the method.</param>
			/// <param name="place_notation">The place notation of the method.</param>
			public StoredMethod (string title, Stage stage, string place_notation) {
				this.title = title;
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
		/// <param name="method_data">Set this to override the default method data.</param>
		public MethodLibrary (string method_data = null) {
			string data = method_data ?? Properties.Resources.methods;
			string [] lines = data.Split ('\n');

			List<StoredMethod> stored_methods = new List<StoredMethod> ();

			for (int i = 0; i < lines.Length; i++) {
				if (lines [i] == "") {
					continue;
				}

				string [] parts = lines [i].Split ('|');

				stored_methods.Add (new StoredMethod (parts [0], (Stage)int.Parse (parts [2]), parts [1]));
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

		/// <summary>
		/// Gets the string used to compress small integers into a 1 char storage value.
		/// </summary>
		public const string int_value_lookup = @"0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!£$%^&*()-=_+[]{};'#:@~,./<>?\|`¬";
	}
}
