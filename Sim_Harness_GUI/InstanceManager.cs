using System;
using System.Collections.Generic;
//using System.Diagnostics;
//using Hats.ServerInterface;
using Hats.Sim;
using Newtonsoft.Json;

namespace Sim_Harness_GUI
{
public class InstanceManager
{
	protected string _scenario;
	protected List<SimHouse> _houses;
	protected List<SimApp> _apps;
	protected string _house_path;
	protected string _app_path;
	public InstanceManager(string scenario, string house_path, string app_path)
	{
		_scenario = scenario;
		_house_path = house_path;
		_app_path = app_path;
		BuildLists();
	}

	public bool Start()
	{
		//TODO: Add validation that everything is ready to launch
		//TODO: Add backing store in case of crash
		foreach(SimHouse house in _houses)
		{
			house.Start();
		}

		foreach(SimApp app in _apps)
		{
			app.Start();
		}

		return true;
	}

	public void Kill()
	{
		foreach(SimHouse house in _houses)
		{
			house.Kill();
		}

		foreach(SimApp app in _apps)
		{
			app.Kill();
		}
	}

	public void BuildLists()
	{
		Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(_scenario);
		List<Dictionary<string, string>> house_dict = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(values["houses"]);
		List<Dictionary<string, string>> app_dict = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(values["users"]);

		foreach(Dictionary<string, string> entry in house_dict)
		{
			_houses.Add(new SimHouse(_scenario, _house_path, entry["name"]));
		}

		foreach(Dictionary<string, string> entry in app_dict)
		{
			_apps.Add(new SimApp(_scenario, _app_path, entry["name"]));
		}
	}
}
}

