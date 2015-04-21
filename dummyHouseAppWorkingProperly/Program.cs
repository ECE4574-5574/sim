using System;
using Hats.Time;
using Newtonsoft.Json;


namespace dummyHouseAppWorkingProperly
{
class MainClass
{


	static private TimeFrame _time;

	public static int Main(string[] args)
	{

		Console.WriteLine("OK");

		var input = Console.ReadLine();

		while(!input.Equals("q", StringComparison.OrdinalIgnoreCase))
		{
			if(_time != null)
			{
				_time = JsonConvert.DeserializeObject<TimeFrame>(input);
			}
			input = Console.ReadLine();
		}
		return 0;
	}
}
}
