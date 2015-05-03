using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sim_Harness_GUI
{
public class JsonUser
{
	private JToken _info;
	private string _name;
	private int _id;
	private string _password;
	private bool _error;

	public string Name
	{
		get
		{
			return _name;
		}
	}

	public int Id
	{
		get
		{
			return _id;
		}
	}

	public string Password
	{
		get
		{
			return _password;
		}
	}

	public bool Error
	{
		get
		{
			return _error;
		}
	}

	public string JsonBlob
	{
		get
		{
			return _info.ToString();
		}
	}

	public JsonUser(JToken user)
	{
		_info  = user;
		_error = false;

		JObject userObj = JObject.Parse(user.ToString());
		JToken name;
		JToken id;
		JToken password;


		// Try to get the name
		if(userObj.TryGetValue("Username", out name))
		{
			_name = name.ToString();
		}
		else
		{
			_name = "";
			_error = true;
		}

		// Set ID
		userObj.TryGetValue("UserID", out id);
		if(id != null)
		{
			_id = (int)id;
		}
		else
		{
			_id = 0;
			_error = true;
		}

		// Set Password
		if(userObj.TryGetValue("Password", out password))
		{
			_password = password.ToString();
		}
		else
		{
			_password = "";
			_error = true;
		}
	}


	public string serverInfo()
	{
		return _info.ToString();
	}
		

	public override string ToString()
		{
			return string.Format("[JsonUser: Name={0}, Id={1}, Password={2}, Error={3}, JsonBlob={4}]", Name, Id, Password, Error, JsonBlob);
		}


}//class
}//namespace

