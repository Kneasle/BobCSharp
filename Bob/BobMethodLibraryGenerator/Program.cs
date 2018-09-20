using System;
using Bob;

namespace BobMethodLibraryGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
			MethodLibrary.GenerateLibraryFileFromXML ("../../../../BobMethodLibraryGenerator/CCCBR_method_library.xml");

			Console.ReadLine ();
        }
    }
}
