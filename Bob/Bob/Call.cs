using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	public class Call {
		public Method method;
		public string name;
		public string [] notations;
		public PlaceNotation [] place_notations;
		public int every;
		public int from;
		public string [] [] calling_positions;

		public Change overall_transposition => PlaceNotation.CombinePlaceNotations (place_notations);

		private void Init () {
			if (method.lead_length % every != 0) {
				throw new Exception ("Call " + name + " of " + method.title + " repeats at a number of changes which is not a multiple of the lead length. This will cause massive issues.");
			}
		}

		public Call (Method method) {
			this.method = method;

			Init ();
		}

		public Call (Method method, string name, string [] notations, PlaceNotation [] place_notations, int every, int from = 0) {
			this.method = method;
			this.name = name;
			this.notations = notations;
			this.place_notations = place_notations;
			this.every = every;
			this.from = from;

			Init ();
		}

		public Call (Method method, string name, string [] notations, PlaceNotation place_notation, int every, int from = 0) {
			this.method = method;
			this.name = name;
			this.notations = notations;
			this.place_notations = new PlaceNotation [] { place_notation };
			this.every = every;
			this.from = from;

			Init ();
		}

		public Call (Method method, string name, string [] notations, PlaceNotation [] place_notations, int every, int from, string [] [] calling_positions) {
			this.name = name;
			this.notations = notations;
			this.place_notations = place_notations;
			this.method = method;
			this.every = every;
			this.from = from;

			Init ();
			this.calling_positions = calling_positions;
		}

		public static Call LeadEndBob (Method method, PlaceNotation [] place_notations) {
			return LeadEndCall (method, bob_name, bob_notations, place_notations);
		}

		public static Call LeadEndBob (Method method, PlaceNotation place_notation) {
			return LeadEndCall (method, bob_name, bob_notations, new PlaceNotation [] { place_notation });
		}

		public static Call LeadEndSingle (Method method, PlaceNotation [] place_notations) {
			return LeadEndCall (method, single_name, single_notations, place_notations);
		}

		public static Call LeadEndSingle (Method method, PlaceNotation place_notation) {
			return LeadEndCall (method, single_name, single_notations, new PlaceNotation [] { place_notation });
		}

		public static Call LeadEndCall (Method method, string name, string [] notations, PlaceNotation [] place_notations) {
			return new Call (method, name, notations, place_notations, method.lead_length, 1 - place_notations.Length);
		}

		public static string bob_name = "Bob";
		public static string single_name = "Single";
		public static string plain_name = "Plain";

		private static string [] m_bob_notations = null;
		public static string [] bob_notations {
			get { return m_bob_notations ?? new string [] { "-", "B" }; }
			set { m_bob_notations = value; }
		}

		private static string [] m_single_notations = null;
		public static string [] single_notations {
			get { return m_single_notations ?? new string [] { "-", "B" }; }
			set { m_single_notations = value; }
		}

		private static string [] m_plain_notations = null;
		public static string [] plain_notations {
			get { return m_plain_notations ?? new string [] { "-", "B" }; }
			set { m_plain_notations = value; }
		}
	}
}
