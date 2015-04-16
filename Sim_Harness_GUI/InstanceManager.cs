using System;
using System.Messaging; //message queues
using System.Diagnostics; //processes
using System.IO;

namespace Sim_Harness_GUI
{
public class InstanceManager{

	ProcessStartInfo appGenerator_info = new ProcessStartInfo();
	ProcessStartInfo houseGenerator_info = new ProcessStartInfo();
	Process appGenerator, houseGenerator;

	StreamWriter houseStandardIn, appStandardIn;
	StreamReader houseStandardOut, appStandardOut, houseErrorOut;


	//NOTE: names are from the parent's (this program's) perspective

	public string startGeneratorProcesses(string appGeneratorLocation, string houseGeneratorLocation, String timeFrameBlob, String testScenarioBlob){
		string output = "";


		//TODO: set up how to start the mobile app
		//set process settings app
		//appGenerator_info.FileName = appGeneratorLocation;
		//appGenerator_info.Arguments = string.Concat("--house_id=", appQueueName_r, "--test_scenario='", testScenarioBlob);



		// set process setting for house
		houseGenerator_info.FileName = houseGeneratorLocation;
		houseGenerator_info.Arguments = "--house_id=house1 --test_scenario='" + testScenarioBlob + "'";
		houseGenerator_info.RedirectStandardInput = true;
		houseGenerator_info.RedirectStandardOutput = true;
		houseGenerator_info.RedirectStandardError = true;
		houseGenerator_info.UseShellExecute = false;

		output += "House:\n";

		bool houseStarted = startProcess(ref houseGenerator, ref houseGenerator_info);
		if(houseStarted)
		{


			// Set standard input and standard outputs
			houseStandardIn = houseGenerator.StandardInput;
			houseStandardOut = houseGenerator.StandardOutput;
			houseErrorOut = houseGenerator.StandardError;

			System.Threading.Thread.Sleep(1000);

			if(houseGenerator.HasExited)
			{
				output += "\n\tERROR: " + houseErrorOut.ReadToEnd() + "\n\n";
			}
			else
			{
				output += "\tHouse has stated up correctly\n\tProccess ID: " + houseGenerator.Id + "\n";
				// Read in and look for the "OK"
				String processOutput = houseStandardOut.ReadLine();
				output += "\tProcess Output: " + processOutput + "\n";
				if(processOutput == "OK")
				{
					output += "\tWriting timeframe:\n" + timeFrameBlob;
					houseStandardIn.WriteLine(timeFrameBlob);
				}else
				{
					output += "ERROR INSIDE PROCESS AND DID NOT RECIEVE OK\n";
				}
			}

		}
		return output;
	}

	public string killGeneratorProcesses(){
		string output = "Killing processes...\n\n";

		//output += ("App: ");
		//output += (killProcess(ref appGenerator));

		output += ("House: \n\n");
		output += (killProcess(ref houseGenerator));

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
	}

	private void openMessageQueue(ref MessageQueue mQueue, ref string mQueue_name){
		if (!MessageQueue.Exists(mQueue_name))
			mQueue = MessageQueue.Create(mQueue_name); //create the queue if it doesn't exist
		else {
			mQueue = new MessageQueue(mQueue_name); //connect to the existing queue
			mQueue.Purge(); //delete any messages that might be in the existing queue
		}
	}

	private void sendMessage(ref MessageQueue mQueue, string message) {
		System.Messaging.Message messagetosend = new System.Messaging.Message();
		messagetosend.Body = message;
		mQueue.Send(messagetosend);
	}

	private string receiveMessage(ref MessageQueue mQueue) {
		string output = "";
		System.Messaging.Message received_message = new System.Messaging.Message();
		received_message = mQueue.Receive(); //blocking
		received_message.Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" });
		output = (string)received_message.Body;
		return output;
	}
		


} //end class
} //end namespace

