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

			Change plain = new Change ("15738264"); // Cambridge Major's lead end
			PlaceNotation notation = new PlaceNotation ("14", Stage.Major);

			Console.WriteLine (plain * notation);

			Console.ReadLine ();
		}
	}
}
