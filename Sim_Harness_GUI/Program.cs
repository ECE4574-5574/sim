using System;
using Gtk;

using System.Net.Http;
using System.Threading.Tasks;


namespace Sim_Harness_GUI
{
class MainClass
{
	public static void Main(string[] args)
	{		
		Application.Init();
		MainWindow win = new MainWindow();
		win.Show();
		Application.Run();
	}

}
}
