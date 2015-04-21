using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Hats.SimWeather;
using Hats.Time;
using NUnit.Framework;
using Sim_Harness_GUI;
using Newtonsoft.Json;


namespace sim_tests
{
[TestFixture]
public class Test
{
	[Test]
	[ExpectedException(typeof(ArgumentException))]
	public void CreatingTimeFrame()
	{
		var frame = new TimeFrame();
		frame.setTimeFrame(rate: -1.0);
	}

	[Test]
	public void TimeRate()
	{
		const Double SimRate = 2.0;
		var doubleTime = new TimeFrame();

		var then = DateTime.Now;
		var diff = Math.Abs((then - doubleTime.time(then)).TotalSeconds);
		Assert.Less(diff, 1e-3);

		doubleTime.setTimeFrame(rate: SimRate);

		then = DateTime.Now;
		Thread.Sleep(2000);
		var now = DateTime.Now;

		var simSpan = doubleTime.time(now) - doubleTime.time(then);
		var realSpan = now - then;

		var computedRate = simSpan.TotalSeconds / realSpan.TotalSeconds;
		diff = Math.Abs(computedRate - SimRate); 
		Assert.Less(diff, 0.01);
	}

	[Test]
	public void LinearWeather()
	{
		var frame = new TimeFrame();
		var linear = new LinearWeather(frame);

		Assert.IsNaN(linear.Temperature());

		const Double StartingTemp = 75.0;
		TemperatureSetPoint temp_a = new TemperatureSetPoint(DateTime.Now, StartingTemp);
		linear.Add(temp_a);

		Assert.AreEqual(StartingTemp, linear.Temperature());
		Assert.AreEqual(1, linear.SetPoints().Count);
		linear.ClearSetPoints();
		Assert.AreEqual(0, linear.SetPoints().Count);

		var start = DateTime.Now;
		List<TemperatureSetPoint> temps = new List<TemperatureSetPoint>
		{
			new TemperatureSetPoint(start, 0),
			new TemperatureSetPoint(start + new TimeSpan(0, 0, 10), 10),
			new TemperatureSetPoint(start + new TimeSpan(0, 0, 20), 20),
			new TemperatureSetPoint(start + new TimeSpan(0, 0, 30), 10)
		};

		linear.AddRange(temps);
		Assert.AreEqual(temps.Count, linear.SetPoints().Count);

		Assert.AreEqual(temps[0].Temp, linear.Temperature(start));
		Assert.AreEqual(5, linear.Temperature(start + new TimeSpan(0, 0, 5)));
		Assert.AreEqual(15, linear.Temperature(start + new TimeSpan(0, 0, 25)));
	}

	[Test]
	public void InstanceManagerWorkingHouse()
	{
		InstanceManager manager = new InstanceManager();

		Assert.AreEqual(0, manager.getNumberHouses());
		Assert.AreEqual(0, manager.getNumberApps());

		string pathToHouse = Directory.GetCurrentDirectory();

		// get path to the dummyHouseAppWorkingProperly
		pathToHouse += "/../../../dummyHouseAppWorkingProperly/bin/Debug/dummyHouseAppWorkingProperly.exe";

		string pathToApp = pathToHouse;


		string jsonBlob = Directory.GetCurrentDirectory();
		jsonBlob += "/../../../scenarios/example1.json";
		jsonBlob = File.ReadAllText(jsonBlob);
		jsonBlob = jsonBlob.Replace("\n", "");
		jsonBlob = jsonBlob.Replace("\t", "");

		TimeFrame tf = new TimeFrame(DateTime.Now, DateTime.Now, 2.0);
		string tfString = JsonConvert.SerializeObject(tf);

		// Test for working HouseApp
		bool start = manager.startGeneratorProcesses(pathToApp, pathToHouse, tfString, jsonBlob);
		Assert.AreEqual(true, start);
		Assert.AreEqual(1, manager.getNumberHouses());
		Assert.AreEqual(0, manager.getNumberApps());

		manager.killGeneratorProcesses();
		Assert.AreEqual(0, manager.getNumberHouses());
		Assert.AreEqual(0, manager.getNumberApps());

	}

	[Test]
	public void InstanceManagerInvalidHouseExe()
	{
		InstanceManager manager = new InstanceManager();

		Assert.AreEqual(0, manager.getNumberHouses());
		Assert.AreEqual(0, manager.getNumberApps());

		string pathToHouse = Directory.GetCurrentDirectory();

		// get path to the dummyHouseAppWorkingProperly
		pathToHouse += "/../../../dummyHouseAppWorkingProperly/bin/Debug/dummyHouseAppWorkingProperl";

		string pathToApp = pathToHouse;


		string jsonBlob = Directory.GetCurrentDirectory();
		jsonBlob += "/../../../scenarios/example1.json";
		jsonBlob = File.ReadAllText(jsonBlob);
		jsonBlob = jsonBlob.Replace("\n", "");

		TimeFrame tf = new TimeFrame(DateTime.Now, DateTime.Now, 2.0);
		string tfString = JsonConvert.SerializeObject(tf);

		// Test for working HouseApp
		bool start = manager.startGeneratorProcesses(pathToApp, pathToHouse, tfString, jsonBlob);
		Assert.AreEqual(false, start);
		Assert.AreEqual(0, manager.getNumberHouses());
		Assert.AreEqual(0, manager.getNumberApps());
	}

	[Test]
	public void InstanceManagerErrorHouse()
	{
		InstanceManager manager = new InstanceManager();

		Assert.AreEqual(0, manager.getNumberHouses());
		Assert.AreEqual(0, manager.getNumberApps());

		string pathToHouse = Directory.GetCurrentDirectory();

		// get path to the dummyHouseAppWorkingProperly
		pathToHouse += "/../../../dummyHouseAppError/bin/Debug/dummyHouseAppError.exe";

		string pathToApp = pathToHouse;


		string jsonBlob = Directory.GetCurrentDirectory();
		jsonBlob += "/../../../scenarios/example1.json";
		jsonBlob = File.ReadAllText(jsonBlob);
		jsonBlob = jsonBlob.Replace("\n", "");
		jsonBlob = jsonBlob.Replace("\t", "");

		TimeFrame tf = new TimeFrame(DateTime.Now, DateTime.Now, 2.0);
		string tfString = JsonConvert.SerializeObject(tf);

		// Test for working HouseApp
		bool start = manager.startGeneratorProcesses(pathToApp, pathToHouse, tfString, jsonBlob);
		Assert.AreEqual(false, start);
		Assert.AreEqual(0, manager.getNumberHouses());
		Assert.AreEqual(0, manager.getNumberApps());
		manager.killGeneratorProcesses();

	}

	public void ServerTest()
	{
		//yay server
		/*Server s = new Server("http://requestb.in/1ehzgva1");
		Assert.AreEqual("http://requestb.in/1ehzgva1", s.URL);

		string response = s.postMessage("Hello");
		Assert.AreEqual("ok", response);

		//faulty server
		s.URL = "http://requestb.in/1eh1";
		Assert.AreEqual("http://requestb.in/1eh1", s.URL);

		string response2 = s.postMessage("Hello");
		Assert.AreNotEqual("ok", response2);*/

	}
}
}
