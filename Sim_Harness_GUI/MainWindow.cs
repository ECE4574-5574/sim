using System;
using Gtk;
using Sim_Harness_GUI;
using System.IO;

public partial class MainWindow: Gtk.Window
{
	protected InstanceManager _instances;
	protected DateTime _simTime;

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		Console.WriteLine("Build");
		Build();
		_simTime = DateTime.UtcNow;
		//_simTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hourSpinBox.ValueAsInt, minSpinBox.ValueAsInt, 0);

	}


	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Console.WriteLine("Deleted");
		Application.Quit();
		a.RetVal = true;
	}

	protected void OnStartTestButtonClicked(object sender, EventArgs e)
	{
		String jsonStartString = buildStartString();
		currentTestTextview.Buffer.Text = jsonStartString;
		//_instances = new InstanceManager(scenarioDirectoryText.Text, houseSimLocationEntry.Text, appSimLocationEntry.Text);
		//_instances.Start();
		startTestButton.Sensitive = false;
		endTestButton.Sensitive = true;



	}

	protected void OnLoadScenarioButton(object sender, EventArgs e)
	{
		var item = new Gtk.TreeIter();
		this.testScenarioComboBox.GetActiveIter(out item);

		//TODO: Read in file, prep for launch here
		Console.WriteLine(this.testScenarioComboBox.Model.GetValue(item, 1));
	}

	protected void OnAppSimulatorChooseFileButtonClicked(object sender, EventArgs e)
	{
		this.appSimLocationEntry.Text = this.selectFile();
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
	 * Makes sure the three files selected are 
	 */
	private bool checkFiles()
	{
		if(File.Exists(scenarioDirectoryText.Text + "/" + testScenarioComboBox.ActiveText + ".json") && File.Exists(appSimLocationEntry.Text) && File.Exists(houseSimLocationEntry.Text))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private string buildStartString()
	{
		string jsonString = "{\n\t\"TimeFrame\": {" +
		                    "\n\t\t\"wall\": \"" + DateTime.Now.ToString("O") + "\"," +
							"\n\t\t\"sim\": \"" + _simTime.ToString("O") + "\"," +
		                    "\n\t\t\"rate\": " + timeFrameSpeedSpinbutton.Text +
		                    "\n\t}\n}";

		return jsonString;
	}

	protected void OnEndTestButtonClicked(object sender, EventArgs e)
	{
		//_instances.Kill();
		endTestButton.Sensitive = false;
		startTestButton.Sensitive = true;
	}
		

	protected void OnSpinbutton1ValueChanged (object sender, EventArgs e)
	{
//			TimeSpan inputTime = new TimeSpan(hourSpinBox.ValueAsInt, minSpinBox.ValueAsInt, 0);
//			_simTime = _simTime.Date + inputTime;	
		_simTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hourSpinBox.ValueAsInt, minSpinBox.ValueAsInt, 0, DateTimeKind.Local);
	}

	protected void OnMinSpinBoxValueChanged (object sender, EventArgs e)
	{
//			TimeSpan inputTime = new TimeSpan(hourSpinBox.ValueAsInt, minSpinBox.ValueAsInt, 0);
//			_simTime = _simTime.Date + inputTime;	

		_simTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hourSpinBox.ValueAsInt, minSpinBox.ValueAsInt, 0, DateTimeKind.Local);

	}
		

}
