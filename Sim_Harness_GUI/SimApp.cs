using System;
using System.Diagnostics;

/**
 * SimApp Class
 * Instantiate user apps
 * \author: Nate Hughes <njh2986@vt.edu>
 */
namespace Hats.Sim
{
public class SimApp
{
	/**
 	 * Instantiates a SimApp object - a representation of an end-user's app
 	 * \param[in] scenarioConfig String containing the scenario config in JSON format
 	 */
	public SimApp(String scenarioConfig, string app_path, string user_name, int user_id)
	{
		string args = String.Concat("--user ", user_id);
		args = String.Concat(" --scenario ", scenarioConfig);
		_process = new Process();
		_process.StartInfo.FileName = app_path;
		_process.StartInfo.Arguments = args;	
	}

	// NOTE: the ready signal is gotten via the server, not directly from the app
	public bool Start()
	{
		return _process.Start();
	}

	public void Kill()
	{
		_process.Kill();
		_process.WaitForExit();
	}

	protected Process _process;
}
}