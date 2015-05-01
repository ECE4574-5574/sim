using System;


using System.Net.Http;
using System.Threading.Tasks;

using System.Text;

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
		url =  serverURL;
	}

	public string postMessage(string msg){
		/*WebRequest request = WebRequest.CreateHttp("https://posttestserver.com/post.php");
		request.Method = "POST";
		request.ContentType = "application/json";
		byte[] byteArray = Encoding.UTF8.GetBytes(time);
		Stream data = request.GetRequestStream();
		request.ContentLength = byteArray.Length; //byteArray
		data.Write(byteArray, 0, byteArray.Length);
		data.Close();*/

		var task = MakeRequest(msg);
		task.Wait();

		var response = task.Result;

		var body = response.Content.ReadAsStringAsync().Result;
		return body;

	}

	private static async Task<HttpResponseMessage> MakeRequest(string msg)
	{
		var httpClient = new HttpClient();
		await httpClient.GetAsync(new Uri(url));

		//var stringContent = new StringContent(msg);
		string m = "@" + msg;

		var stringCon = new StringContent(m, Encoding.UTF8, "application/json");

		var response= await httpClient.PostAsync(url, stringCon);	
		return response;

		//var response= await httpClient.PostAsync(url, stringContent);	
		//return response;
	}

}
}