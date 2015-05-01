using System;
using System.Diagnostics; //processes
using System.IO;
using System.Collections.Generic; // List
using Hats.Sim; // SimHouse

namespace Sim_Harness_GUI
{
public class InstanceManager{
	protected List<SimHouse> _houses, _errorHouses;
	protected List<SimApp> _apps;
	protected string _timeFrameInfo, _jsonScenario, _appPath, _houseLocation, _status;



	public InstanceManager()
	{
		_houses = new List<SimHouse>();
		_errorHouses = new List<SimHouse>();
		_apps = new List<SimApp>();
		_status = "";
	}



	public int getNumberHouses()
	{
		return _houses.Count;
	}

	public int getNumberApps()
	{
		return _apps.Count;
	}




	//NOTE: names are from the parent's (this program's) perspective

	//this function gets called when the 'startTestButton' button is clicked
	public bool startGeneratorProcesses(string appLocation, string houseLocation, string timeFrameBlob, string testScenarioBlob){

		_timeFrameInfo = timeFrameBlob;
		_houseLocation = houseLocation;
		_appPath = appLocation;
		_jsonScenario = testScenarioBlob;

		_status = "";

		//TODO: read the test Scenario blob. right now it is hard coded to start only one house named "house1"
		prepProcesses();
		startSimHouses();
	
		if(_errorHouses.Count != 0)
		{
			return false;
		}

		//TODO: set up how to start the mobile app




		// Send the "go command to the houses 
		sendGoHouses();

		startOneApp(appLocation);

		return true;
	}

	public void killGeneratorProcesses(){
		_status = "Killing Processes:\n\n";

		foreach(SimHouse house in _houses)
		{
			_status += "\t" + house.Kill() + "\n";
		}

		_houses.Clear();

	}
		

	//  helper functions //

	private string startOneApp(string dir){
		string output = "";

		//load resource files
		string shellScript = dir + "/adb.sh";
		string appInstaller = dir + "/com.homeAutomationApp.apk";


		if(!File.Exists(shellScript)){
			output += "Could not find 'adb.sh' in " + dir + "\n";
			output += "Can not spawn app.";
			return output;
		}
		if(!File.Exists(appInstaller)){
			output += "Could not find 'com.homeAutomationApp.apk' in " + dir + "\n";
			output += "Can not spawn app.";
			return output;
		}

		//this process will send commands to adb.exe, and install the app
		//set information about the process
		ProcessStartInfo p_info = new ProcessStartInfo();

		p_info.FileName = dir + "/launch.sh"; //hack hack hack - use a path join that is OS agnostic here
		p_info.UseShellExecute = true;
		p_info.ErrorDialog = false;
	
		//start the process
		Process p = new Process();
		p.StartInfo = p_info;

		Console.WriteLine(startProcess(ref p, ref p_info));

		return output;
	}

	/**
	 * Attempts to start a process with particualar information. If the process starts
	 * correctly it will retrun true.
	 */
	private bool startProcess(ref Process p, ref ProcessStartInfo ps){
		bool started = false;
		try {
			p = Process.Start(ps);
			started = true;
		}catch(Exception ex){
			Console.WriteLine(ex.ToString());
		}
		return started;
	}

	/*
	private string killProcess(ref Process p){
		string output = "\tProcess ID: " + p.Id + "\n";
		try {
			p.Kill();
			output = string.Concat(output,"\tProcess killed successfully\n");
		} catch (InvalidOperationException ex) {
			output = string.Concat(output, "\tInvalidOperationException thrown - the process has already exited\n");
		} catch (Exception ex) {
			output = string.Concat(output, "\tException thrown while trying to kill the process\n");
			output = string.Concat(output, ex.Message);
			output = string.Concat(output, "\n");
		}
		return output;
	}*/

	/**
	 * This will create every simHouse and every simApp found in the JSON config file
	 */
	private void prepProcesses()
	{
		// TODO: prep all of the app process information
		List<string> houseNames= findHouses();
		foreach(string houseName in houseNames)
		{
			SimHouse newHouse = new SimHouse(_jsonScenario, _houseLocation, houseName);
			_houses.Add(newHouse);
		}
	}

	/**
	 * This function will read in the JSON blob and find every house name and return a list of the house names
	 */
	private List<string> findHouses()
	{
		// TODO: Read in the JSON blob
		List<string> houseNames = new List<string>();
		houseNames.Add("house1");
		return houseNames;
	}

	/**
	 * Attempts to start every single simHouse in the list and returns any that fail to 
	 * open correctly
	 */ 
	private void startSimHouses()
	{
		List<int> houseIndexRemove = new List<int>();
		// Start the process, if it fail to start then add it to the errorHouses
		for(int i = 0; i < _houses.Count; i++)
		{
			_houses[i].Start();
			if(_houses[i].ProcessStarted)
			{
				_houses[i].waitForResponse();
			}
			if(!_houses[i].ProcessStarted || _houses[i].Error)
			{
				_errorHouses.Add(_houses[i]);
				houseIndexRemove.Add(i);
			}
		}
		foreach(int i in houseIndexRemove)
		{
			_houses.RemoveAt(i);
		}
	}

		
	/**
	 * Sends the "go" to each of the SimHouse apps.
	 */ 
	private void sendGoHouses()
	{
		foreach(SimHouse house in _houses)
		{
			house.sendMessage(_timeFrameInfo);
		}
	}

	public override string ToString()
	{
		
		string output = "[Instance Manager]\n\n\tNumber of Houses: " + (_houses.Count + _errorHouses.Count)  + "\n" +
						"\tNumber of Apps: " +_apps.Count + "\n"+
						"\tHouses:\n\n";

		output += "\tSuccessfully run houses:\n\n";
		foreach(SimHouse house in _houses)
		{
			output += house.ToString() + "\n\n";
		}
		output += "\tFailed Houses:\n\n";
		foreach(SimHouse house in _errorHouses)
		{
			output += house.ToString() + "\n\n";
		}
//		return "";
		return output;
	}

} //end class
} //end namespace

