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
		public int cover;
		public string [] [] calling_positions;

		// Properties
		public Change overall_transposition => PlaceNotation.CombinePlaceNotations (place_notations);
		public bool is_plain => place_notations == null;
		public int length => place_notations.Length;

		// Functions
		private void Init () {
			if (method.lead_length % every != 0) {
				throw new Exception ("Call " + name + " of " + method.title + " repeats at a number of changes which is not a divisor of the lead length. This will cause massive issues with calls not lining up as expected.");
			}
		}

		// Constructors
		public Call (Method method) {
			this.method = method;

			Init ();
		}

		public Call (Method method, string name, string [] notations, PlaceNotation [] place_notations, int every, int from = 0, int cover = -1) {
			this.method = method;
			this.name = name;
			this.notations = notations;
			this.place_notations = place_notations;
			this.every = every;
			this.from = from;
			this.cover = cover == -1 ? place_notations.Length : cover;

			Init ();
		}

		public Call (Method method, string name, string [] notations, PlaceNotation place_notation, int every, int from = 0, int cover = -1) {
			this.method = method;
			this.name = name;
			this.notations = notations;
			this.place_notations = new PlaceNotation [] { place_notation };
			this.every = every;
			this.from = from;
			this.cover = cover == -1 ? place_notations.Length : cover;

			Init ();
		}

		public Call (Method method, string name, string [] notations, PlaceNotation [] place_notations, string [] [] calling_positions, int every, int from = 0, int cover = -1) {
			this.name = name;
			this.notations = notations;
			this.place_notations = place_notations;
			this.method = method;
			this.every = every;
			this.from = from;
			this.cover = cover == -1 ? place_notations.Length : cover;

			Init ();
			this.calling_positions = calling_positions;
		}

		// Static functions
		public static Call LeadEndCall (Method method, string name, string [] notations, PlaceNotation [] place_notations) {
			return new Call (method, name, notations, place_notations, method.lead_length, - place_notations.Length);
		}

		public static Call LeadEndBob (Method method, PlaceNotation [] place_notations) {
			return LeadEndCall (method, bob_name, bob_notations, place_notations);
		}

		public static Call LeadEndBob (Method method, PlaceNotation place_notation) {
			return LeadEndCall (method, bob_name, bob_notations, new PlaceNotation [] { place_notation });
		}

		public static Call LeadEndBob (Method method, string notation) {
			return LeadEndCall (method, bob_name, bob_notations, PlaceNotation.DecodeFullNotation (notation, method.stage));
		}

		public static Call LeadEndSingle (Method method, PlaceNotation [] place_notations) {
			return LeadEndCall (method, single_name, single_notations, place_notations);
		}

		public static Call LeadEndSingle (Method method, PlaceNotation place_notation) {
			return LeadEndCall (method, single_name, single_notations, new PlaceNotation [] { place_notation });
		}

		public static Call LeadEndSingle (Method method, string notation) {
			return LeadEndCall (method, single_name, single_notations, PlaceNotation.DecodeFullNotation (notation, method.stage));
		}

		public static Call LeadEndPlain (Method method, int length = 1) {
			return LeadEndCall (method, plain_name, plain_notations, new PlaceNotation [length]);
		}

		// Static values
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
			get { return m_single_notations ?? new string [] { "S" }; }
			set { m_single_notations = value; }
		}

		private static string [] m_plain_notations = null;
		public static string [] plain_notations {
			get { return m_plain_notations ?? new string [] { "M", "P" }; }
			set { m_plain_notations = value; }
		}
	}
}
