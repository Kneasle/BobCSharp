using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public class Method {
		private string m_full_notation;
		private PlaceNotation [] m_notations;
		private Change m_lead_end;

		public PlaceNotation [] notations {
			get {
				return m_notations;
			}
		}

		public Change lead_end {
			get {
				return m_lead_end;
			}
		}

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
			m_notations = PlaceNotation.DecodeFullNotation (m_full_notation, stage);
			m_lead_end = Utils.GetEndChange (m_notations);
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
