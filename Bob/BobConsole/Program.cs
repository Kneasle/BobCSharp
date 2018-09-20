using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bob;

namespace BobConsole {
	class Program {
		static void Main (string [] args) {
			/*
			Console.WriteLine (Method.grandsire_triples.plain_course);
			Console.WriteLine ("\n\n\n\n\n");
			Console.WriteLine (Method.plain_bob_doubles.TouchFromCallList ("PPPBPS"));
			Console.WriteLine ("\n\n\n\n\n");
			Console.WriteLine (Method.plain_bob_minor.TouchFromCallingPositions ("WsWWsWH"));
			Console.WriteLine ("\n\n\n\n\n");
			*/
			Console.WriteLine (new Method ("345.1.125.123.125.123.5.1.345.125", "Blaston", Stage.Doubles).title);

			Console.ReadLine ();
		}
	}
}
