using System;
using System.Diagnostics; //processes
using System.IO;
using System.Collections.Generic; // List
using Hats.Sim; // SimHouse

namespace Sim_Harness_GUI
{
public class InstanceManager{
	protected List<SimHouse> _houses;
	protected List<SimApp> _apps;
	protected string _timeFrameInfo, _jsonScenario, _appPath, _houseLocation, _status;



	public InstanceManager()
	{
		_houses = new List<SimHouse>();
		_apps = new List<SimApp>();
	}


	//NOTE: names are from the parent's (this program's) perspective

	public bool startGeneratorProcesses(string appLocation, string houseLocation, string timeFrameBlob, string testScenarioBlob){

		_timeFrameInfo = timeFrameBlob;
		_houseLocation = houseLocation;
		_appPath = appLocation;

		_status = "";


		//TODO: read the test Scenario blob. right now it is hard coded to start only one house named "house1"
		prepProcesses();
		List<SimHouse> issueHouses = startSimHouses();

		if(issueHouses.Count != 0)
		{
			return false;
		}

		//TODO: set up how to start the mobile app




		// Send the "go command to the houses 
		sendGoHouses();


		return true;
	}

	public string killGeneratorProcesses(){
		string output = "Killing Processes:\n\n";

		foreach(SimHouse house in _houses)
		{
			output = "\t" + house.Kill() + "\n";
		}

		return output;
	}
		

	//  helper functions //

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
	private List<SimHouse> startSimHouses()
	{
		List<SimHouse> errorHouses = new List<SimHouse>();
		// Start the process, if it fail to start then add it to the errorHouses
		foreach(SimHouse house in _houses)
		{
			
			house.Start();
			house.waitForResponse();
			if(!house.startUpSuccessfull())
			{
				errorHouses.Add(house);
			}
		}

		updateStatusStartSimHouses(errorHouses);

		return errorHouses;

	}

	/**
	 * Updates the _status string after the startSimHouses() funciton is called
	 * 
	 * @param errorHouses a list of all the houses that failed to start up correctly
	 */ 
	protected void updateStatusStartSimHouses(List<SimHouse> errorHouses)
	{
		if(errorHouses.Count != 0)
		{
			_status += "\tIssue starting up these houses:\n\n";
			foreach(SimHouse house in errorHouses)
			{
				_status += "\t" + house.ToString() + "\n\n";
			}
		}
		else
		{
			// All houses have sucessfully started up
			_status += "\tSucessfully started up " + _houses.Count + " houses. Here are is the information:\n";
			foreach(SimHouse house in _houses)
			{
				_status += house.ToString() + "\n\n";
			}
		}
	}
		
	/**
	 * Sends the to each of the SimHouse apps.
	 */ 
	private void sendGoHouses()
	{
		foreach(SimHouse house in _houses)
		{
			house.sendMessage(_timeFrameInfo);
		}
		// Update the status string
		_status += "Sending TimeFrame to Houses: \n\t" + _timeFrameInfo;
	}

	public override string ToString()
	{
		string output = "[Instance Manager]\n\n\tNumber of Houses: " + _houses.Count + "\n\t" +
						"Number of Apps: " +_apps.Count + "\n\t" +
						"Status:\n\n" + _status;
		return output;
	}

} //end class
} //end namespace

