using System;
using System.Messaging; //message queues
using System.Diagnostics; //processes

namespace Sim_Harness_GUI
{
public class InstanceManager{

	ProcessStartInfo appGenerator_info = new ProcessStartInfo();
	ProcessStartInfo houseGenerator_info = new ProcessStartInfo();
	Process appGenerator, houseGenerator;
	MessageQueue appQueue_r, appQueue_w, houseQueue_r, houseQueue_w;
	string appQueueName_r = @".\private$\appQueueRead";
	string appQueueName_w = @".\private$\appQueueWrite";
	string houseQueueName_r = @".\private$\houseQueueRead";
	string houseQueueName_w = @".\private$\houseQueueWrite";
	//NOTE: names are from the parent's (this program's) perspective

	public string startGeneratorProcesses(string appGeneratorLocation, string houseGeneratorLocation){
		string output = "";

		//open the message queues
		openMessageQueue(ref appQueue_r, ref appQueueName_r);
		openMessageQueue(ref appQueue_w, ref appQueueName_w);
		openMessageQueue(ref houseQueue_r, ref houseQueueName_r);
		openMessageQueue(ref houseQueue_w, ref houseQueueName_w);

		//set process settings
		appGenerator_info.FileName = appGeneratorLocation;
		houseGenerator_info.FileName = houseGeneratorLocation;
		appGenerator_info.Arguments = string.Concat(appQueueName_r, " ", appQueueName_w);
		houseGenerator_info.Arguments = string.Concat(houseQueueName_r, " ", houseQueueName_w);

		//start the processes
		output += "App: ";
		output += startProcess(ref appGenerator, ref appGenerator_info);
		output += "House: ";
		output += startProcess(ref houseGenerator, ref houseGenerator_info);

		return output;
	}

	public string killGeneratorProcesses(){
		string output = "";

		output += ("App: ");
		output += (killProcess(ref appGenerator));
		output += ("House: ");
		output += (killProcess(ref houseGenerator));

		return output;
	}

	public string sendJSON(string json){
		string output = "";

		//send the first JSON string through the queue
		sendMessage(ref appQueue_w, json);
		sendMessage(ref houseQueue_w, json);

		//receive confirmation messages
		output += "App: ";
		output += receiveMessage(ref appQueue_r);
		output += "\n";
		output += "House: ";
		output += receiveMessage(ref houseQueue_r);
		output += "\n";

		return output;
	}

	public string tuesdayDemo(){
		return "";
	}



	//  helper functions //

	private string startProcess(ref Process p, ref ProcessStartInfo ps) {
		string output = "";
		try {
			
			p = Process.Start(ps);
			output = string.Concat(output,"Process started successfully\n");
		} catch (Exception ex) {
			output = string.Concat(output, "Unable to start process from location: ");
			output = string.Concat(output, ps.FileName);
			output = string.Concat(output, "\n");
		}
		return output;
	}

	private string killProcess(ref Process p){
		string output = "";
		try {
			p.Kill();
			output = string.Concat(output,"Process killed successfully\n");
		} catch (InvalidOperationException ex) {
			output = string.Concat(output, "InvalidOperationException thrown - the process has already exited\n");
		} catch (Exception ex) {
			output = string.Concat(output, "Exception thrown while trying to kill the process\n");
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

