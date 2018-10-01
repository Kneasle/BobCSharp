using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob {
	/// <summary>
	/// A class to store all calls.
	/// </summary>
	public class Call {
		/// <summary>
		/// The method to which this call belongs.
		/// </summary>
		public Method method;

		/// <summary>
		/// The full name of this call, e.g. "Bob".
		/// </summary>
		public string name;
		/// <summary>
		/// A list of possible notations of this call. Sould be lower case or a symbol, as calling positions are uppercase.
		/// </summary>
		public char [] notations;

		/// <summary>
		/// The place notations which make up the call.
		/// </summary>
		public PlaceNotation [] place_notations;
		/// <summary>
		/// A list of strings which contain the possible calling positions for the call, indexed from zero.  Each string should be a list of the possible alternative calling positions, e.g. "IB" for In and Before.
		/// </summary>
		public string [] calling_positions = new string [0];

		/// <summary>
		/// Every how many changes the calling position can be called.  Must be a factor of the lead length of the method.  For lead end calls this is set automagically.
		/// </summary>
		public int every;
		/// <summary>
		/// How many changes away from every `every` changes the call can be called.  E.g. for Stedman this will be -3, and `every` will be 6.  For lead-end calls this is set automagically.
		/// </summary>
		public int from;
		/// <summary>
		/// How many plain changes this call covers, because CCCBR now allow calls to extend the lead length of a method.  For lead end calls this is set automagically.
		/// </summary>
		public int cover;

		// Properties
		/// <summary>
		/// The overall transposition caused by the call (note this is not the lead end after the call).
		/// </summary>
		public Change overall_transposition => PlaceNotation.CombinePlaceNotations (place_notations);
		/// <summary>
		/// True if the call is a `plain` call, otherwise false.  In BobC# these exist, so that call list touches (e.g. PPBPS) work properly.
		/// </summary>
		public bool is_plain => name == plain_name;
		/// <summary>
		///	The number of changes which the change covers.
		/// </summary>
		public int length => place_notations.Length;
		/// <summary>
		/// The preferred notation of the call (this is always the first value in the list of notations).
		/// </summary>
		public char preferred_notation => notations [0];

		// Functions
		/// <summary>
		/// Gets the index (from 0) of the calling position denoted by `notation`.
		/// </summary>
		/// <param name="notation">The notation of the calling position who's index is wanted.</param>
		/// <returns>The index of calling position denotated by `notation`.</returns>
		public int GetCallingPositionIndex (char notation) {
			for (int i = 0; i < calling_positions.Length; i++) {
				if (calling_positions [i].Contains (notation)) {
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Initialises the call (right now it just calls an exception if `every` is not a factor of the method's lead length).
		/// </summary>
		private void Init () {
			if (method.lead_length % every != 0) {
				throw new Exception ("Call " + name + " of " + method.title + " repeats at a number of changes which is not a divisor of the lead length. This will cause massive issues with calls not lining up as expected.");
			}
		}

		// Constructors
		/// <summary>
		/// Generates a call without calling positions.
		/// </summary>
		/// <param name="method">The method to which this call belongs.</param>
		/// <param name="name">The full name of the call, e.g. "Bob".</param>
		/// <param name="notations">A list of possible notations, e.g. ['-'. 'b'].  Should be lower case.</param>
		/// <param name="place_notations">A list of the place notations which make up the call.</param>
		/// <param name="every">Every how many changes the call can be called.</param>
		/// <param name="from">How many changes from the `every` changes the call can be called.  E.g. for Stedman, this is -3 and `every` is 6.</param>
		/// <param name="cover">How many plain course changes this change covers (CCCBR now allow calls which change the lead length of the method).</param>
		public Call (Method method, string name, char [] notations, PlaceNotation [] place_notations, int every, int from = 0, int cover = -1) {
			this.method = method;
			this.name = name;
			this.notations = notations;
			this.place_notations = place_notations;
			this.every = every;
			this.from = from;
			this.cover = cover == -1 ? place_notations.Length : cover;

			Init ();
		}

		/// <summary>
		/// Generates a single change call without calling positions.
		/// </summary>
		/// <param name="method">The method to which this call belongs.</param>
		/// <param name="name">The full name of the call, e.g. "Bob".</param>
		/// <param name="notations">A list of possible notations, e.g. ['-'. 'b'].  Should be lower case.</param>
		/// <param name="place_notation">The place notation of the call.</param>
		/// <param name="every">Every how many changes the call can be called.</param>
		/// <param name="from">How many changes from the `every` changes the call can be called.  E.g. for Stedman, this is -3 and `every` is 6.</param>
		/// <param name="cover">How many plain course changes this change covers (CCCBR now allow calls which change the lead length of the method).</param>
		public Call (Method method, string name, char [] notations, PlaceNotation place_notation, int every, int from = 0, int cover = -1) {
			this.method = method;
			this.name = name;
			this.notations = notations;
			place_notations = new PlaceNotation [] { place_notation };
			this.every = every;
			this.from = from;
			this.cover = cover == -1 ? place_notations.Length : cover;

			Init ();
		}

		/// <summary>
		/// Generates a call with calling positions.
		/// </summary>
		/// <param name="method">The method to which this call belongs.</param>
		/// <param name="name">The full name of the call, e.g. "Bob".</param>
		/// <param name="notations">A list of possible notations, e.g. ['-'. 'b'].  Should be lower case.</param>
		/// <param name="place_notations">A list of the place notations which make up the call.</param>
		/// <param name="calling_positions">A list of strings containing (per place) every possible notations (should be upper case).</param>
		/// <param name="every">Every how many changes the call can be called.</param>
		/// <param name="from">How many changes from the `every` changes the call can be called.  E.g. for Stedman, this is -3 and `every` is 6.</param>
		/// <param name="cover">How many plain course changes this change covers (CCCBR now allow calls which change the lead length of the method).</param>
		public Call (Method method, string name, char [] notations, PlaceNotation [] place_notations, string [] calling_positions, int every, int from = 0, int cover = -1) {
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

		/// <summary>
		/// Generates a single change call with calling positions.
		/// </summary>
		/// <param name="method">The method to which this call belongs.</param>
		/// <param name="name">The full name of the call, e.g. "Bob".</param>
		/// <param name="notations">A list of possible notations, e.g. ['-'. 'b'].  Should be lower case or symbols.</param>
		/// <param name="place_notation">The place notation of the call.</param>
		/// <param name="calling_positions">A list of strings containing (per place) every possible notations (should be upper case).</param>
		/// <param name="every">Every how many changes the call can be called.</param>
		/// <param name="from">How many changes from the `every` changes the call can be called.  E.g. for Stedman, this is -3 and `every` is 6.</param>
		/// <param name="cover">How many plain course changes this change covers (CCCBR now allow calls which change the lead length of the method).</param>
		public Call (Method method, string name, char [] notations, PlaceNotation place_notation, string [] calling_positions, int every, int from = 0, int cover = -1) {
			this.name = name;
			this.notations = notations;
			place_notations = new PlaceNotation [] { place_notation };
			this.method = method;
			this.every = every;
			this.from = from;
			this.cover = cover == -1 ? place_notations.Length : cover;

			Init ();
			this.calling_positions = calling_positions;
		}

		// Static functions
		/// <summary>
		/// Creates a given call over the lead end of a given method (without calling positions).
		/// </summary>
		/// <param name="method">The method to which this call belongs.</param>
		/// <param name="name">The full name of the call, e.g. "Bob".</param>
		/// <param name="notations">A list of possible notations, e.g. ['-'. 'b'].  Should be lower case or symbols.</param>
		/// <param name="place_notations">A list of the place notations which make up the call.</param>
		/// <returns>A call over the lead end of the given method.</returns>
		public static Call LeadEndCall (Method method, string name, char [] notations, PlaceNotation [] place_notations) {
			return new Call (method, name, notations, place_notations, method.lead_length, - place_notations.Length);
		}

		/// <summary>
		/// Creates a given call over the lead end of a given method (with calling positions).
		/// </summary>
		/// <param name="method">The method to which this call belongs.</param>
		/// <param name="name">The full name of the call, e.g. "Bob".</param>
		/// <param name="notations">A list of possible notations, e.g. ['-'. 'b'].  Should be lower case or symbols.</param>
		/// <param name="place_notations">A list of the place notations which make up the call.</param>
		/// <param name="calling_positions">A list of strings containing (per place) every possible notations (should be upper case).</param>
		/// <returns>A call over the lead end of the given method.</returns>
		public static Call LeadEndCall (Method method, string name, char [] notations, PlaceNotation [] place_notations, string [] calling_positions) {
			return new Call (method, name, notations, place_notations, calling_positions, method.lead_length, -place_notations.Length);
		}


		/// <summary>
		/// Creates a `bob` call over the lead end of a given method.
		/// </summary>
		/// <param name="method">The method to which the call belongs.</param>
		/// <param name="place_notations">A list of the place notations which make up the call.</param>
		/// <param name="calling_positions">A list of strings containing (per place) every possible notations (should be upper case).  If null, standard calling positions will be generated.</param>
		/// <returns>The `bob` call that was created.</returns>
		public static Call LeadEndBob (Method method, PlaceNotation [] place_notations, string [] calling_positions = null) {
			if (calling_positions is null && standard_calling_positions_bob.ContainsKey (method.stage)) {
				return LeadEndCall (method, bob_name, bob_notations, place_notations, standard_calling_positions_bob [method.stage]);
			} else {
				return LeadEndCall (method, bob_name, bob_notations, place_notations, calling_positions);
			}
		}

		/// <summary>
		/// Creates a `bob` call over the lead end of a given method.
		/// </summary>
		/// <param name="method">The method to which the call belongs.</param>
		/// <param name="place_notation">The place notation of the call.</param>
		/// <param name="calling_positions">A list of strings containing (per place) every possible notations (should be upper case).  If null, standard calling positions will be generated.</param>
		/// <returns>The `bob` call that was created.</returns>
		public static Call LeadEndBob (Method method, PlaceNotation place_notation, string [] calling_positions = null) => LeadEndBob (method, new PlaceNotation [] { place_notation }, calling_positions);

		/// <summary>
		/// Creates a `bob` call over the lead end of a given method.
		/// </summary>
		/// <param name="method">The method to which the call belongs.</param>
		/// <param name="notation">The full place notation of the call, e.g. "3.1" for Grandsire Bobs.</param>
		/// <param name="calling_positions">A list of strings containing (per place) every possible notations (should be upper case).  If null, standard calling positions will be generated.</param>
		/// <returns>The `bob` call that was created.</returns>
		public static Call LeadEndBob (Method method, string notation, string [] calling_positions = null) => LeadEndBob (method, PlaceNotation.DecodeFullNotation (notation, method.stage), calling_positions);


		/// <summary>
		/// Creates a `single` call over the lead end of a given method.
		/// </summary>
		/// <param name="method">The method to which the call belongs.</param>
		/// <param name="place_notations">A list of the place notations which make up the call.</param>
		/// <param name="add_standard_calling_positions">If true, BobC# will generate the standard calls automagically.</param>
		/// <returns>The `single` call that was created.</returns>
		public static Call LeadEndSingle (Method method, PlaceNotation [] place_notations, bool add_standard_calling_positions = true) {
			if (add_standard_calling_positions && standard_calling_positions_single.ContainsKey (method.stage)) {
				return LeadEndCall (method, single_name, single_notations, place_notations, standard_calling_positions_single [method.stage]);
			} else {
				return LeadEndCall (method, single_name, single_notations, place_notations);
			}
		}

		/// <summary>
		/// Creates a `single` call over the lead end of a given method.
		/// </summary>
		/// <param name="method">The method to which the call belongs.</param>
		/// <param name="place_notation">The place notation of the call.</param>
		/// <returns>The `single` call that was created.</returns>
		public static Call LeadEndSingle (Method method, PlaceNotation place_notation) => LeadEndSingle (method, new PlaceNotation [] { place_notation });

		/// <summary>
		/// Creates a `single` call over the lead end of a given method.
		/// </summary>
		/// <param name="method">The method to which the call belongs.</param>
		/// <param name="notation">The full place notation of the call, e.g. "3.123" for Grandsire Singles.</param>
		/// <returns>The `single` call that was created.</returns>
		public static Call LeadEndSingle (Method method, string notation) => LeadEndSingle (method, PlaceNotation.DecodeFullNotation (notation, method.stage));

		/// <summary>
		/// Creates a `plain` call (a call which does nothing) over the lead end of a given method.
		/// </summary>
		/// <param name="method">The method to which the call belongs.</param>
		/// <param name="length">How many changes the call covers.</param>
		/// <returns></returns>
		public static Call LeadEndPlain (Method method, int length = 1) {
			return LeadEndCall (method, plain_name, plain_notations, new PlaceNotation [length]);
		}

		// Static values
		/// <summary>
		/// The standard full name for any `bob` calls.
		/// </summary>
		public static string bob_name = "Bob";
		/// <summary>
		/// The standard full name for any `single` calls.
		/// </summary>
		public static string single_name = "Single";
		/// <summary>
		/// The standard full name for any `plain` calls.
		/// </summary>
		public static string plain_name = "Plain";
		
		private static char [] m_bob_notations = null;
		/// <summary>
		/// List of standard notations for bobs. Defaults to ['-', 'b'].
		/// </summary>
		public static char [] bob_notations {
			get { return m_bob_notations ?? new char [] { '-', 'b' }; }
			set { m_bob_notations = value; }
		}

		private static char [] m_single_notations = null;
		/// <summary>
		/// List of standard notations for singles. Defaults to ['s'].
		/// </summary>
		public static char [] single_notations {
			get { return m_single_notations ?? new char [] { 's' }; }
			set { m_single_notations = value; }
		}

		private static char [] m_plain_notations = null;
		/// <summary>
		/// List of standard notations for plain calls. Defaults to ['m', 'p'].
		/// </summary>
		public static char [] plain_notations {
			get { return m_plain_notations ?? new char [] { 'm', 'p' }; }
			set { m_plain_notations = value; }
		}

		/// <summary>
		/// Gets the standard calling positions for lead-end bobs.
		/// </summary>
		public static Dictionary<Stage, string []> standard_calling_positions_bob {
			get {
				Dictionary<Stage, string []> dict = new Dictionary<Stage, string []> ();

				dict.Add (Stage.Doubles, new string [] { "", "I", "BO", "FM4", "H" });
				dict.Add (Stage.Minor, new string [] { "", "I", "BO", "FM4", "W", "H" });
				dict.Add (Stage.Triples, new string [] { "", "I", "BO", "F4", "W", "M", "H" });
				dict.Add (Stage.Major, new string [] { "", "I", "BO", "F4", "V5", "M", "W", "H" });
				dict.Add (Stage.Caters, new string [] { "", "I", "BO", "F4", "V5", "X6", "W", "M", "H" });
				dict.Add (Stage.Royal, new string [] { "", "I", "BO", "F4", "V5", "X6", "S7", "M", "W", "H" });
				dict.Add (Stage.Cinques, new string [] { "", "I", "BO", "F4", "V5", "X6", "S7", "E8", "W", "M", "H" });
				dict.Add (Stage.Maximus, new string [] { "", "I", "BO", "F4", "V5", "X6", "S7", "E8", "N9", "M", "W", "H" });

				return dict;
			}
		}
		/// <summary>
		/// Gets the standard calling positions for lead-end singles.
		/// </summary>
		public static Dictionary<Stage, string []> standard_calling_positions_single {
			get {
				Dictionary<Stage, string []> dict = new Dictionary<Stage, string []> ();

				dict.Add (Stage.Doubles, new string [] { "", "BI2", "3T", "F4", "H" });
				dict.Add (Stage.Minor, new string [] { "", "BI2", "3T", "FM4", "W", "H" });
				dict.Add (Stage.Triples, new string [] { "", "BI2", "3T", "F4", "W", "M", "H" });
				dict.Add (Stage.Major, new string [] { "", "BI2", "3T", "F4", "V5", "M", "W", "H" });
				dict.Add (Stage.Caters, new string [] { "", "BI2", "3T", "F4", "V5", "X6", "W", "M", "H" });
				dict.Add (Stage.Royal, new string [] { "", "BI2", "3T", "F4", "V5", "X6", "S7", "M", "W", "H" });
				dict.Add (Stage.Cinques, new string [] { "", "BI2", "3T", "F4", "V5", "X6", "S7", "E8", "W", "M", "H" });
				dict.Add (Stage.Maximus, new string [] { "", "BI2", "3T", "F4", "V5", "X6", "S7", "E8", "N9", "M", "W", "H" });

				return dict;
			}
		}
	}
}
