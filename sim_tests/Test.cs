﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Hats.SimWeather;
using Hats.Time;
using NUnit.Framework;
using Sim_Harness_GUI;

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
	public void ServerTest()
	{
		//yay server
		Server s = new Server("http://requestb.in/1ehzgva1");
		Assert.AreEqual("http://requestb.in/1ehzgva1", s.URL);

		string response = s.postMessage("Hello");
		Assert.AreEqual("ok", response);

		//faulty server
		s.URL = "http://requestb.in/1eh1";
		Assert.AreEqual("http://requestb.in/1eh1", s.URL);

		string response2 = s.postMessage("Hello");
		Assert.AreNotEqual("ok", response2);
	}
}
}
