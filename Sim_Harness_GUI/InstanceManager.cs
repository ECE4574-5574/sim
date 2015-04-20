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

	//this function gets called when the 'startTestButton' button is clicked
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
		startOneApp(appLocation);



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

	private string startOneApp(string dir){
		string output = "";

		//load resource files
		string location_of_terminal = @"C:\Windows\System32\cmd.exe";
		string batchFile = dir + "\\spawn_app.bat";
		string shellScript = dir + "\\adb.sh";
		string appInstaller = dir + "\\com.homeAutomationApp.apk";
		StreamReader sr = new StreamReader(dir + "\\location_of_adb_dir.txt");
		string adbExe = sr.ReadLine() + "\\adb";
		sr.Close();

		//check for non-existent files
		if(!File.Exists(batchFile)){
			output += "Could not find 'spawn_app.bat' in " + dir + "\n";
			output += "Can not spawn app.";
			return output;
		}
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
		if(!File.Exists(adbExe + ".exe")){
			output += "Could not find 'adb.exe' in " + adbExe + "\n";
			output += "Can not spawn app.";
			return output;
		}

		//this process will send commands to adb.exe, and install the app
		//set information about the process
		ProcessStartInfo p_info = new ProcessStartInfo();
		StreamWriter child_stdin;
		StreamReader child_stdout;
		StreamReader child_stderr;
		p_info.FileName = location_of_terminal;
		p_info.UseShellExecute = false;
		p_info.ErrorDialog = false;
		p_info.RedirectStandardError = true;
		p_info.RedirectStandardInput = true;
		p_info.RedirectStandardOutput = true;

		//start the process
		Process p = new Process();
		p.StartInfo = p_info;
		Console.WriteLine(startProcess(ref p, ref p_info));
		child_stdin = p.StandardInput;
		child_stdout = p.StandardOutput;
		child_stderr = p.StandardError;

		//send the batch file to the process
		string command_1 = adbExe + " shell pm uninstall com.homeAutomationApp";
		string command_2 = adbExe + " push " + appInstaller + " /data/local/";
		string command_3 = adbExe + " push " + shellScript + " /data/local/";
		string command_4 = adbExe + " shell pm install /data/local/com.HomeAutomationApp.apk";
		child_stdin.WriteLine(command_1);
		child_stdout.ReadLine();
		child_stdin.WriteLine(command_2);
		child_stdout.ReadLine();
		child_stdin.WriteLine(command_3);
		child_stdout.ReadLine();
		child_stdin.WriteLine(command_4);
		while(child_stdout.ReadLine() != "Success"){}


		p.Kill();
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

