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
	protected JsonFile _parser;
	protected string _timeFrameInfo, _jsonScenario, _appPath, _houseLocation, _status;
	protected string myOS;



	public InstanceManager()
	{
		_houses = new List<SimHouse>();
		_errorHouses = new List<SimHouse>();
		_apps = new List<SimApp>();
		_status = "";

		//set myOS as either "Unix" for a Mac OS, or "Win32NT" for a Windows OS
		OperatingSystem os = Environment.OSVersion;
		myOS = os.Platform.ToString();
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
		_parser = new JsonFile(testScenarioBlob);


		_status = "";

		//TODO: read the test Scenario blob. right now it is hard coded to start only one house named "house1"
		prepProcesses();
		startSimHouses();
	
		if(_errorHouses.Count != 0)
		{
			return false;
		}

		//TODO: set up how to start the mobile app
		startOneApp(appLocation, testScenarioBlob);

		// Send the "go command to the houses 
		sendGoHouses();

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

	public string startOneApp(string apk_dir, string jsonblob){
		string output = "";
		string blob_to_pass = jsonblob.Replace("\"", "\\\""); //escape the double quotes
		ProcessStartInfo p_info = new ProcessStartInfo();
		p_info.UseShellExecute = false;
		p_info.ErrorDialog = false;

		//set command line arguments to batch/sh file
		//the first argument is the path to the .apk that was passed in through the GUI
		//the second argument is the JSON string to be passed to the app (previously contained in adb.sh)

		//detect operating system and use launch.bat or launch.sh accordingly
		string base_dir = AppDomain.CurrentDomain.BaseDirectory;
		if(myOS == "Unix"){
			p_info.FileName = base_dir + "launch.sh";
			p_info.Arguments = apk_dir + " " + "'" + blob_to_pass + "'";
		}
		else if(myOS == "Win32NT"){
			p_info.FileName = base_dir + "launch.bat";
			p_info.Arguments = apk_dir + " " + blob_to_pass;
		}
		else{
			output = "OS not recognized: " + myOS + "\n";
			return output;
		}

		//start the process
		Process p = new Process();
		p.StartInfo = p_info;

		output += startProcess(ref p, ref p_info);

		return output;
	}

	/**
	 * Attempts to start a process with particualar information. If the process starts
	 * correctly it will retrun true.
	 */
	private string startProcess(ref Process p, ref ProcessStartInfo ps){
		string started = "Process not started";
		try {
			p = Process.Start(ps);
			started = "Process started successfully";
		}catch(Exception ex){
			started = ex.ToString();
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
		//List<string> houseNames= findHouses();
		/*foreach(string houseName in houseNames)
		{
			SimHouse newHouse = new SimHouse(_jsonScenario, _houseLocation, houseName);
			_houses.Add(newHouse);
		}*/
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
			/* check if process is running as expected */
			if(_houses[i].isRunning())
			{
				_houses[i].waitForResponse();
			}
			if(!_houses[i].isRunning() || _houses[i].Error)
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

