using System;
using System.Collections.Generic;
using Bob;
using System.Xml;

namespace BobMethodLibraryGenerator {
    class Program {
		private enum XMLReaderState {
			WaitingForName = 0,
			ReadName = 1,
			WaitingForTitle = 2,
			ReadTitle = 3,
			WaitingForPlaceNotation = 4,
			ReadPlaceNotation = 5
		}

		public static void GenerateLibraryFileFromXML (string file_path) {
			XmlTextReader reader = new XmlTextReader (file_path);

			List<string> library_file_lines = new List<string> ();

			string method_title = null;
			string method_name = null;
			string method_place_notation = null;

			int number_of_stupid_methods = 0;

			XMLReaderState state = XMLReaderState.ReadPlaceNotation;

			while (reader.Read ()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element: // The node is an element.
						if (reader.Name == "name" && state == XMLReaderState.ReadPlaceNotation) {
							state = XMLReaderState.WaitingForName;
						}
						if (reader.Name == "title" && state == XMLReaderState.ReadName) {
							state = XMLReaderState.WaitingForTitle;
						}
						if (reader.Name == "notation" && state == XMLReaderState.ReadTitle) {
							state = XMLReaderState.WaitingForPlaceNotation;
						}
						break;
					case XmlNodeType.Text: //Display the text in each element.
						if (state == XMLReaderState.WaitingForName) {
							method_name = reader.Value;
							state = XMLReaderState.ReadName;
						}
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
							Stage stage = Utils.GetStageOfMethodFromMethodTitle (method_title);

							method_place_notation = PlaceNotation.CompressPlaceNotation (method_place_notation, stage);

							Method method = new Method (method_place_notation, method_name, stage, null, false);

							if (method.title == method_title) {
								library_file_lines.Add (method_name + "|" + method_place_notation + "|" + (int)stage);
							} else {
								number_of_stupid_methods += 1;
								Console.WriteLine (method_title + " != " + method.title);
								library_file_lines.Add (method_name + "|" + method_place_notation + "|" + (int)stage + "|" + method_title);
							}
						}
						break;
				}
			}

			Console.WriteLine (library_file_lines.Count.ToString () + " methods copied.  " + number_of_stupid_methods + " of them are stupid.");

			System.IO.File.WriteAllLines (MethodLibrary.library_path, library_file_lines.ToArray ());
		}

		static void Main(string[] args)
        {
			GenerateLibraryFileFromXML ("../../../../BobMethodLibraryGenerator/CCCBR_method_library.xml");

			Console.ReadLine ();
        }
    }
}
