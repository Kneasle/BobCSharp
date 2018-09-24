using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public class MethodLibrary {
		// Classes/enums
		public class StoredMethod {
			public string name;
			public Stage stage;
			public string place_notation;

			public Method method => new Method (place_notation, name, stage);

			public StoredMethod (string name, Stage stage, string place_notation) {
				this.name = name;
				this.stage = stage;
				this.place_notation = place_notation;
			}
		}

		private enum XMLReaderState {
			WaitingForTitle = 0,
			ReadTitle = 1,
			WaitingForPlaceNotation = 2,
			ReadPlaceNotation = 3
		}

		// Non-static stuff
		public StoredMethod [] stored_methods;

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
		public static string library_path = "../../../../Bob/compressed_method_library.txt";

		public static void GenerateLibraryFileFromXML (string file_path) {
			XmlTextReader reader = new XmlTextReader (file_path);

			List <string> library_file_lines = new List<string> ();
			
			string method_title = null;
			string method_place_notation = null;

			XMLReaderState state = XMLReaderState.ReadPlaceNotation;

			while (reader.Read ()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element: // The node is an element.
						if (reader.Name == "title" && state == XMLReaderState.ReadPlaceNotation) {
							state = XMLReaderState.WaitingForTitle;
						}
						if (reader.Name == "notation" && state == XMLReaderState.ReadTitle) {
							state = XMLReaderState.WaitingForPlaceNotation;
						}
						break;
					case XmlNodeType.Text: //Display the text in each element.
						if (state == XMLReaderState.WaitingForTitle) {
							method_title = reader.Value;
							state = XMLReaderState.ReadTitle;
						}
						if (state == XMLReaderState.WaitingForPlaceNotation) {
							method_place_notation = reader.Value;
							state = XMLReaderState.ReadPlaceNotation;
						}
						break;
					case XmlNodeType.EndElement: //Display the end of the element.
						if (reader.Name == "method") {
							library_file_lines.Add (method_title + "|" + method_place_notation);
						}
						break;
				}
			}

			Console.WriteLine (library_file_lines.Count.ToString () + " methods copied.");

			System.IO.File.WriteAllLines (library_path, library_file_lines.ToArray ());
		}

		public static Method GetMethodByTitle (string name) {
			foreach (StoredMethod stored_method in library.stored_methods) {
				if (stored_method.name == name) {
					return stored_method.method;
				}
			}

			return null;
		}

		private static MethodLibrary m_library = null;
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
