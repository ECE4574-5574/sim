using System;
using Gtk;
using Hats.Time;
using Newtonsoft.Json;
using Sim_Harness_GUI;
using System.IO;

using System.Web;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;

using System.Net.Http;
using System.Threading.Tasks;



public partial class MainWindow: Gtk.Window
{
	protected InstanceManager _instances;
	protected String jsonBlob;
	string urlserver;
	string houseserver;
	string userserver;

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		Build();

	}


	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		_instances.killGeneratorProcesses();
		Application.Quit();
		a.RetVal = true;
	}
		
	protected void OnLoadScenarioButton(object sender, EventArgs e)
	{
		var item = new Gtk.TreeIter();
		this.testScenarioComboBox.GetActiveIter(out item);

		//TODO: Read in file, prep for launch here right now the "house1" is hard coded in
		this.testScenarioComboBox.Model.GetValue(item,1);
		// Make sure a valid file was selected
		if(this.testScenarioComboBox.Model.GetValue(item,1) != null && File.Exists(this.testScenarioComboBox.Model.GetValue(item,1).ToString()))
		{
			testSenarioTextview.Buffer.Text = File.ReadAllText(this.testScenarioComboBox.Model.GetValue(item,1).ToString());
			jsonBlob = testSenarioTextview.Buffer.Text;

			// Remove every new line and tab otherwise it will not work as a command line argument
			jsonBlob = jsonBlob.Replace("\n", "");
			jsonBlob = jsonBlob.Replace("\t", "");
		}


	}

	protected void OnAppSimulatorChooseFileButtonClicked(object sender, EventArgs e)
	{
		//this.appSimLocationEntry.Text = this.selectFile();
		Gtk.FileChooserDialog chooser = new Gtk.FileChooserDialog("Select Directory of App Files",
			this,
			FileChooserAction.SelectFolder,
			"Cancel", ResponseType.Cancel,
			"Select", ResponseType.Accept);

		if(chooser.Run() == (int)ResponseType.Accept)
		{
			this.appSimLocationEntry.Text = chooser.Filename;
		}

		chooser.Destroy();
	}

	protected void OnHouseSimLocationButtonClicked(object sender, EventArgs e)
	{
		this.houseSimLocationEntry.Text = this.selectFile();
	}

	protected String selectFile()
	{
		String returnText = "";
		Gtk.FileChooserDialog filechooser =
			new Gtk.FileChooserDialog("Choose the file to select",
				this,
				FileChooserAction.Open,
				"Cancel", ResponseType.Cancel,
				"Select", ResponseType.Accept);

		if(filechooser.Run() == (int)ResponseType.Accept)
		{
			returnText = filechooser.Filename;
		}

		filechooser.Destroy();
		return returnText;
	}

	protected void OnScenarioDirectoryLoad(object sender, EventArgs e)
	{
		Gtk.FileChooserDialog chooser = new Gtk.FileChooserDialog("Select Scenario Directory",
			this,
			FileChooserAction.SelectFolder,
			"Cancel", ResponseType.Cancel,
			"Select", ResponseType.Accept);

		if(chooser.Run() == (int)ResponseType.Accept)
		{
			this.buildScenarioList(chooser.Filename);
			this.scenarioDirectoryText.Text = chooser.Filename;
		}

		chooser.Destroy();
	}

	protected void buildScenarioList(String dir)
	{
		var newList = new ListStore(typeof(string), typeof(string));
		foreach(string file in Directory.EnumerateFiles(dir, "*.json"))
		{
			string scenario = System.IO.Path.GetFileNameWithoutExtension(file);
			Console.WriteLine("Adding " + scenario);
			newList.AppendValues(scenario, file);
		}
		this.testScenarioComboBox.Model = newList;
		this.testScenarioComboBox.Active = 0;
	}

	protected void OnScenarioDirectoryTextChanged(object sender, EventArgs e)
	{
		changeStartButton();
	}

	protected void OnAppSimLocationEntryChanged(object sender, EventArgs e)
	{
		changeStartButton();
	}

	protected void OnHouseSimLocationEntryChanged(object sender, EventArgs e)
	{
		changeStartButton();
	}

	/**
	 * Changes the start button to clickable if the files are valid
	 */ 
	private void changeStartButton()
	{
		if(checkFiles())
		{
			startTestButton.Sensitive = true;
		}
		else
		{
			startTestButton.Sensitive = false;
		}
	}

	/**
	 * Makes sure the two files selected (json scenario and house exe) are valid files.
	 */
	private bool checkFiles()
	{
		if(File.Exists(scenarioDirectoryText.Text + "/" + testScenarioComboBox.ActiveText + ".json") && File.Exists(houseSimLocationEntry.Text))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/**
	 * Builds the TimeFrame JSON string to be passed to the house for starting information.
	 */ 
	private string buildStartString()
	{
		// Create a DateTime for when all processes should start. It will be one minute from the current time.
		DateTime wallTime = DateTime.Now;
		TimeSpan oneMin = new TimeSpan(0,1, 0);
		wallTime.Add(oneMin);
		DateTime simTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hourSpinBox.ValueAsInt, minSpinBox.ValueAsInt, 0, DateTimeKind.Local);

		// Construct the timeframe object to be passed
		TimeFrame newTime = new TimeFrame(wallTime, simTime, timeFrameSpeedSpinbutton.Value);

		// Automatically construct the JSON string to be passed
		string jsonString = JsonConvert.SerializeObject(newTime);

		return jsonString;
	}

	protected void OnStartTestButtonClicked(object sender, EventArgs e)
	{
		_instances = new InstanceManager();
		String jsonStartString = buildStartString();

		// build obj
		Server s = new Server(urlserver);

		// sendmsg store response in serverResponse
		string serverResponse = s.postMessage(jsonStartString);

		currentTestTextview.Buffer.Text = "Make request to server:\n\n\t" + jsonStartString + "\n\n\tServer: " + urlserver + "\n\n\tResponse: " + serverResponse + "\n\n----------------------------------\n\n";


		//Create Houses and Users in the database

		//start prepopulation


		List<string> houseList = new List<string> {"House1", "House2", "House3"};
		List<string> userList = new List<string> {"User1", "User2", "User3"};

		//Parsing function call, returns list of houses
		//parsing function call, returns list of users

		Server h = new Server(houseserver);

		for(int i = 0; i < houseList.Count; i++)
		{
			h.postMessage(houseList[i]);
		}
			
		Server u = new Server(userserver);

		for(int j = 0; j < userList.Count; j++)
		{
			u.postMessage(userList[j]);
		}
			
		//end prepopulation

		currentTestTextview.Buffer.Text += "Attempting to open the Generator processes..\n\n";
		bool successfullStart = _instances.startGeneratorProcesses(appSimLocationEntry.Text, houseSimLocationEntry.Text, jsonStartString, jsonBlob);

		currentTestTextview.Buffer.Text += _instances.ToString();

		if(successfullStart){
			startTestButton.Sensitive = false;
			endTestButton.Sensitive = true;
		}

	}
		
	protected void OnEndTestButtonClicked(object sender, EventArgs e)
	{
		currentTestTextview.Buffer.Text += "\n\n";
		_instances.killGeneratorProcesses();
		currentTestTextview.Buffer.Text += _instances.ToString();
		endTestButton.Sensitive = false;
		startTestButton.Sensitive = true;
	}

	protected void OnServerURLEntryChanged (object sender, EventArgs e)
	{
		urlserver = serverURLEntry.Text;
		//throw new NotImplementedException ();
	}

	protected void OnHouseURLentryChanged (object sender, EventArgs e)
	{
		houseserver = HouseURLentry.Text;
		//throw new NotImplementedException ();
	}

	/*protected void OnUserURLentryChanged (object sender, EventArgs e)
	{
		userserver = userURLentry.Text;
		//throw new NotImplementedException ();
	}*/

	protected void OnUserURLentryTextInserted (object o, TextInsertedArgs args)
	{
		userserver = userURLentry.Text;
		//throw new NotImplementedException ();
	}
}

