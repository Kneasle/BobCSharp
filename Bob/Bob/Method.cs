using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public class Method {
		private string m_full_notation;
		public PlaceNotation [] place_notations { get; private set; }
		public Change lead_end { get; private set; }
		public int lead_length { get; private set; }

		public string name;
		public Catagory catagory;
		public Stage stage;

		// Properties
		public string full_notation {
			get {
				return m_full_notation;
			}
			
			set {
				m_full_notation = value;

				RefreshNotation ();
			}
		}

		public string title {
			get {
				if (catagory == Catagory.Principle) {
					return name + " " + Utils.StageToString (stage);
				} else {
					return name + " " + Utils.CatagoryToString (catagory) + " " + Utils.StageToString (stage);
				}
			}
		}

		// Functions
		private void RefreshNotation () {
			place_notations = PlaceNotation.DecodeFullNotation (m_full_notation, stage);
			lead_end = PlaceNotation.CombinePlaceNotations (place_notations);
			lead_length = place_notations.Length;
		}

		// Constructors
		public Method (string full_notation, string name, Catagory catagory, Stage stage) {
			this.name = name [0].ToString ().ToUpper () + name.Substring (1).ToLower ();
			this.catagory = catagory;
			this.stage = stage;

			// Do full notation last, because it relies upon stage and catagory
			this.full_notation = full_notation;
		}
	}
}
