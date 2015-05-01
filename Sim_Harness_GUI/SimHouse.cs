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
		_processStarted = false;
		_processError = false;

		_process = new Process();
		/* register a handler */
		_process.EnableRaisingEvents = true;
		_process.Exited += new EventHandler(house_Exited);

	}

	public void Start()
	{
		try{
			_process = Process.Start(_startInformation);
			_processStarted = true;
		}
		catch (Exception e){
			_houseOutput = e.ToString();
			_processStarted =  false;
			return;
		}
		_standardIn = _process.StandardInput;
		_standardOut = _process.StandardOutput;
		_errorOut = _process.StandardError;
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

		if(_processStarted)
		{
			while(_standardOut.EndOfStream && _errorOut.EndOfStream)
			{
				System.Threading.Thread.Sleep(10);
			}

			if(!_standardOut.EndOfStream)
			{
				_processError = false;
				_houseOutput = _standardOut.ReadLine();
			}
			else
			{
				_processError = true; 
				_houseOutput = _errorOut.ReadToEnd();
			}
		}
	}

	/**
	 * Send a message to the 
	 */ 
	public void sendMessage(String msg)
	{
		if(!_process.HasExited)
		{
			_standardIn.WriteLine(msg);
		}
	}

	public override string ToString(){
		string output = "\tHouse Name:        " + _houseName + "\n" +
		                "\tLocation:        " + _process.StartInfo.FileName + "\n" +
		                "\tProcess Started: " + _processStarted + "\n"; 

		if(_processStarted)
		{
			output += 	"\tProcess ID:      " + _process.Id + "\n" +
						"\tProcess Name:    " + _process.ProcessName + "\n" +
						"\tError:           " + _processError + "\n";
		}

		output += 		"\tProcess Output:  " + _houseOutput + "\n";
		return output;
	}

	protected void house_Exited(object sender, System.EventArgs e) {
		Console.WriteLine("House exited!");
	}

	protected Process _process = new Process();
	protected ProcessStartInfo _startInformation;
	protected StreamWriter _standardIn;
	protected StreamReader _standardOut, _errorOut;
	protected String _status, _houseName, _houseOutput;
	protected bool _processStarted;
	public bool ProcessStarted
	{
		get
		{
			return _processStarted;
		}
	}
	protected bool _processError;
	public bool Error
	{
		get
		{
			return _processError;
		}
	}

	// NOTE: the ready signal is gotten via the server, not directly from the sim house
}
}