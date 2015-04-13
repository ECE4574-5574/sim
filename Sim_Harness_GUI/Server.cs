using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace sim_server
{
public class Server
{
	private string deviceApiHost = "http://52.1.192.214:80/";
	private string appApiHost = "";

	public bool PostDeviceState(UInt64 houseid, UInt64 roomid, UInt64 deviceid, JObject data, JObject timeFactor)
	{
		WebRequest request = WebRequest.Create(deviceApiHost + houseid + "/" + roomid + "/" + deviceid + "/" + timeFactor);
		request.ContentType = "application/json";
		request.Method = "POST";

		using (var streamWriter = new StreamWriter(request.GetRequestStream()))
		{
			streamWriter.Write(data.ToString());
			streamWriter.Flush();
			streamWriter.Close();
		}

		try
		{
			using (var response = request.GetResponse() as HttpWebResponse)
			{
				if (response.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception(String.Format(
						"Server error (HTTP {0}: {1}).",
						response.StatusCode,
						response.StatusDescription));
				}
			}
		}

		catch (WebException)
		{
			//return false;
		}

		return true;
	}
}
}