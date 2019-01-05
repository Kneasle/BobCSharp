using System;
using System.Collections.Generic;
using Bob;
using System.Linq;
using System.Xml;

namespace BobMethodLibraryGenerator {
    class GenerateLibrary {
		public static void GenerateLibraryFileFromXML (string file_path) {
			XmlTextReader reader = new XmlTextReader (file_path);

			List<string> library_file_lines = new List<string> ();

			string extra_method_string = Properties.Resources.ExtraMethods;
			string extra_call_string = Properties.Resources.ExtraCalls;

			// Read extra files
			Dictionary<string, string> extra_calls = new Dictionary<string, string> ();
			foreach (string s in extra_call_string.Split ('\n')) {
				if (s == "") {
					continue;
				}

				string line = s.Substring (0, s.Length - 1);

				int i = line.IndexOf ('|');

				extra_calls.Add (line.Substring (0, i), line.Substring (i + 1));
			}

			string method_title = null;
			string method_name = null;
			string method_place_notation = null;
			string method_classification = null;
			bool is_little = false;
			bool is_differential = false;

			int number_of_stupid_methods = 0;

			HashSet<char> chars = new HashSet<char> ();

			while (!reader.EOF) {
				if (reader.NodeType == XmlNodeType.Element) {
					if (reader.Name == "classification") {
						if (reader.IsEmptyElement) {
							method_classification = Utils.ClassificationToString (Classification.Principle);

							is_little = false;
							is_differential = false;

							reader.Read ();
						} else {
							is_little = reader.GetAttribute ("little") != null;
							is_differential = reader.GetAttribute ("differential") != null;

							method_classification = reader.ReadInnerXml ();
						}
					} else if (reader.Name == "name") {
						if (reader.IsEmptyElement) {
							method_name = "";

							reader.Read ();
						} else {
							method_name = reader.ReadInnerXml ();
						}
					} else if (reader.Name == "title") {
						method_title = reader.ReadInnerXml ();
					} else if (reader.Name == "notation") {
						method_place_notation = reader.ReadInnerXml ();
					} else {
						reader.Read ();
					}
				} else if (reader.NodeType == XmlNodeType.EndElement) {
					if (reader.Name == "method") {
						Stage stage = Utils.GetStageOfMethodFromMethodTitle (method_title);

						method_place_notation = PlaceNotation.CompressPlaceNotation (method_place_notation, stage);

						Method method = new Method (method_place_notation, method_name, stage, null, false);

						foreach (char c in method_title) {
							chars.Add (c);
						}

						bool should_overwrite_title;

						if (method.title == method_title) {
							should_overwrite_title = false;
						} else {
							number_of_stupid_methods += 1;
							Console.WriteLine (method_title + " != " + method.title);
							should_overwrite_title = true;
						}
						
						string tag_string = "";
						if (is_differential) {
							if (is_little) {
								tag_string = "b"; // b = both
							} else {
								tag_string = "d"; // d = differential
							}
						} else {
							if (is_little) {
								tag_string = "l"; // l = little
							}
						}

						library_file_lines.Add (
							method_name + "|" +
							method_place_notation + "|" +
							Constants.int_value_lookup [(int)stage] +
							Constants.int_value_lookup [(int)Utils.StringToClassification (method_classification)] +
							tag_string +
							(should_overwrite_title ? "|" + method_title : "") + 
							(extra_calls.ContainsKey (method_title) ? "\\" + extra_calls [method_title] : "")
						);
					}

					reader.Read ();
				} else {
					reader.Read ();
				}
			}

			foreach (char c in chars.OrderBy (x => (int)x)) {
				Console.Write (true ? c.ToString () : (int)c + " ");
			}

			Console.Write ("\n");

			Console.WriteLine (library_file_lines.Count.ToString () + " methods copied.  " + number_of_stupid_methods + " of them are stupid.");

			System.IO.File.WriteAllLines ("../../../../Bob/methods.txt", library_file_lines.ToArray ());
		}

		static void Main(string[] args)
        {
			GenerateLibraryFileFromXML ("../../../../BobMethodLibraryGenerator/CCCBR_method_library.xml");

			Console.ReadLine ();
        }
    }
}
