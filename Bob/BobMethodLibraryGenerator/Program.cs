using System;
using System.Collections.Generic;
using Bob;
using System.Xml;

namespace BobMethodLibraryGenerator
{
    class Program
    {
		private enum XMLReaderState {
			WaitingForTitle = 0,
			ReadTitle = 1,
			WaitingForPlaceNotation = 2,
			ReadPlaceNotation = 3
		}

		public static void GenerateLibraryFileFromXML (string file_path) {
			XmlTextReader reader = new XmlTextReader (file_path);

			List<string> library_file_lines = new List<string> ();

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

			System.IO.File.WriteAllLines (MethodLibrary.library_path, library_file_lines.ToArray ());
		}

		static void Main(string[] args)
        {
			GenerateLibraryFileFromXML ("../../../../BobMethodLibraryGenerator/CCCBR_method_library.xml");

			Console.ReadLine ();
        }
    }
}
