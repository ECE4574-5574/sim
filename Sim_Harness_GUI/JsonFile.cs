using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sim_Harness_GUI
{
public class JsonFile
{
	private Dictionary<int, JsonUser> _users;
	private Dictionary<int, JsonHouse> _houses;
	private bool _error;

	public Dictionary<int, JsonUser> Users
	{
		get
		{
			return _users;
		}
	}

	public Dictionary<int, JsonHouse> Houses
	{
		get
		{
			return _houses;
		}
	}

	public bool Error
	{
		get
		{
			return _error;
		}
	}

	public JsonFile(String jsonConfig)
	{
		_users = new Dictionary<int, JsonUser>();
		_houses = new Dictionary<int, JsonHouse>();
		/* check if string is in correct json format */
		try {
			var obj = JToken.Parse(jsonConfig);
//			Console.WriteLine(obj.ToString(Newtonsoft.Json.Formatting.Indented));
		}
		catch(JsonReaderException jex) {
			//Exception in parsing json
			Console.WriteLine(jex.Message);
		}
		catch (Exception ex) //some other exception
		{
			Console.WriteLine(ex.ToString());
		}
		_error = jsonStringParser(jsonConfig);
	}

	private bool jsonStringParser(string jsonConfig)
	{
		bool wasError = false;

		if(String.IsNullOrEmpty(jsonConfig))
		{
			return false;
		}

		JObject info = null;

		try
		{
			info = JObject.Parse(jsonConfig);
		}
		catch(JsonException ex)
		{
			var error = String.Format("Scenario parsing error: {0}", ex.Message);
			Console.WriteLine(error);
			return true;
		}

		JToken house_list;
		JToken user_list;

		// Get all houses
		if(info.TryGetValue("houses", out house_list))
		{
			IJEnumerable<JToken> houses = house_list.Children();

			foreach(JToken house in houses)
			{
				JsonHouse newHouse = new JsonHouse(house);
				_houses.Add(newHouse.Id, newHouse);
				if(newHouse.Error)
				{
					wasError = true;
				}
			}
		}

		// Get all users
		if(info.TryGetValue("users", out user_list))
		{
			IJEnumerable<JToken> users = user_list.Children();

			foreach(JToken user in users)
			{
				JsonUser newUser = new JsonUser(user);
				_users.Add(newUser.Id, newUser);
				if(newUser.Error)
				{
					wasError = true;
				}
			}
		}
		return wasError;
	}

	public override string ToString()
		{
			return string.Format("[JsonFile: Users={0}, Houses={1}, Error={2}]", Users, Houses, Error);
		}


}//class
}//namespace

