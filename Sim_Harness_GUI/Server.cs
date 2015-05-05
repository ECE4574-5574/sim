using System;


using System.Net.Http;
using System.Threading.Tasks;
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
using System.Net.Http.Headers;

/**
 * Server Class
 * Establish Connection to server
 * Make requests / receive responses
 * \author: Nate Hughes <njh2986@vt.edu>
 */
namespace Sim_Harness_GUI
{
public class Server
{
	protected static string url;
	public string URL
	{
		get
		{
			return url;
		}
		set
		{
			url = value;
		}
	}


	public Server(string serverURL){
//		Console.WriteLine(serverURL);
		if(serverURL != null)
			url = serverURL;
		else
			url = "fake_server";
	}

	/* return either OK or Invalid Server */
	public string postMessage(string msg){
		/*WebRequest request = WebRequest.CreateHttp("https://posttestserver.com/post.php");
		request.Method = "POST";
		request.ContentType = "application/json";
		byte[] byteArray = Encoding.UTF8.GetBytes(time);
		Stream data = request.GetRequestStream();
		request.ContentLength = byteArray.Length; //byteArray
		data.Write(byteArray, 0, byteArray.Length);
		data.Close();*/
		var body = "";
		var task = MakeRequest(msg);
		try {
			if(task.Status != TaskStatus.Faulted)
			{
				task.Wait();

				var response = task.Result;

				body = response.StatusCode.ToString();

	//			body = response.Content.ReadAsStringAsync().Result;
			}
			else
			{
				body = "Invalid Server";
			}
			return body;
		}
		catch (Exception e) {
			Console.WriteLine(e.Message);
			return "Invalid Server";
		}
	}

	private static async Task<HttpResponseMessage> MakeRequest(string msg)
	{
		var httpClient = new HttpClient();
//		Console.WriteLine(string.Concat(url, "/api/sim/timeframe"));
		await httpClient.GetAsync(new Uri(string.Concat(url, "/api/sim/timeframe")));

		var stringContent = new StringContent(msg, Encoding.UTF8, "application/json");

		var response= await httpClient.PostAsync(url, stringContent);
		Console.WriteLine(response.StatusCode);
		return response;
	}

}
}