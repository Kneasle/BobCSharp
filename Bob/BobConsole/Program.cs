using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bob;

namespace BobConsole {
	class Program {
		static void Main (string [] args) {
			Method method = Method.plain_bob_doubles;

			Call bob = Call.LeadEndBob (method, "145");
			Call plain = Call.LeadEndPlain (method);

			Touch touch = new Touch (
				method,
				new BasicCall [] {
					new BasicCall (plain, new CallLocationList ()),
					new BasicCall (plain, new CallLocationList ()),
					new BasicCall (plain, new CallLocationList ()),
					new BasicCall (bob, new CallLocationList ())
				}
			);

			Console.WriteLine (touch);
			Console.ReadLine ();
		}
	}
}
