using System;
using System.IO;
using System.Diagnostics;


/**
 * SimHouse Class
 * Instantiate simulated houses
 * \author: Nate Hughes <njh2986@vt.edu>
 */
namespace Hats.Sim
{
public class SimHouse
{	
	/**
 	 * Instantiates a SimHouse object - a simulated house
 	 * \param[in] scenarioConfig String containing the scenario config in JSON format
 	 */
	public SimHouse(String scenarioConfig, string app_path, string house_id)
	{
		_startInformation = new ProcessStartInfo();
		_startInformation.FileName = app_path;
		_startInformation.Arguments = "--house_id=" + house_id + " --test_scenario='" + scenarioConfig + "'";
		_startInformation.RedirectStandardInput = true;
		_startInformation.RedirectStandardOutput = true;
		_startInformation.RedirectStandardError = true;
		_startInformation.UseShellExecute = false;
		_houseName = house_id;
	}

	public bool Start()
	{
		bool success = true;
		try{
			_process = Process.Start(_startInformation);
		}
		catch (Exception e){
			Console.Write(e.ToString());
			success =  false;
		}

		_standardIn = _process.StandardInput;
		_standardOut = _process.StandardOutput;
		_errorOut = _process.StandardError;
		return success;
	}

	public string Kill()
	{
		string output = "Killing Process: " + _process.Id + "\n";
		try{
			_process.Kill();
			_process.WaitForExit();
		}catch (InvalidOperationException ex) {
			output += string.Concat(output, "\tInvalidOperationException thrown - the process has already exited\n");
		} catch (Exception ex) {
			output += string.Concat(output, "\tException thrown while trying to kill the process\n");
			output += string.Concat(output, ex.Message);
			output += string.Concat(output, "\n");
		}
		return output;
	}

	public void waitForResponse()
	{
		while(_standardOut.EndOfStream && _errorOut.EndOfStream)
		{
			System.Threading.Thread.Sleep(10);
		}

		if(!_standardOut.EndOfStream)
		{
			_error = false;
			_errorMessage = _standardOut.ReadToEnd();
		}
		else
		{
			_error = true; 
			_errorMessage = _errorOut.ReadToEnd();
		}
			
	}

	public bool startUpSuccessfull()
	{
		return !_error;
	}

	/**
	 * Send a message to the 
	 */ 
	public void sendMessage(String msg)
	{
		_standardIn.WriteLine(msg);
	}

	public override string ToString(){
		string output = "House Name:   " + _houseName + "\n" +
						"\tLocation:     " + _process.StartInfo.FileName + "\n" +
						"\tProcess ID:   " + _process.Id + "\n" +
						"\tProcess Name: " + _process.ProcessName + "\n";
		if(_error)
		{
			output += "\tError Message:\n\n" + _errorMessage;
		}
		else
		{
			output += "\tResponse:    " + _errorMessage;
		}
		return output;
	}

	protected Process _process;
	protected ProcessStartInfo _startInformation;
	protected StreamWriter _standardIn;
	protected StreamReader _standardOut, _errorOut;
	protected String _errorMessage, _houseName, exitMessage;
	protected bool _error;
	// NOTE: the ready signal is gotten via the server, not directly from the sim house
}
}