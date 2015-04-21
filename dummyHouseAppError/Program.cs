using System;
using System.IO;

namespace dummyHouseAppError
{
class MainClass
{
	public static int Main(string[] args)
	{
		TextWriter errorWriter = Console.Error;
		errorWriter.WriteLine("ERROR: THERE WAS AN ERROR");
		return -1;
	}
}
}
