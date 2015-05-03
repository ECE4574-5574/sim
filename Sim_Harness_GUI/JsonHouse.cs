using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Sim_Harness_GUI
{
public class JsonHouse
{
	
	private string _name;
	private int _id;
	private JToken _info;
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

	public JsonHouse(JToken house)
	{
		_error = false;
		_info  = house;
		JObject houseObj = JObject.Parse(house.ToString());
		JToken name;
		JToken id;

		// Set Name
		if(houseObj.TryGetValue("name", out name))
		{
			_name = name.ToString();
		}
		else
		{
			_name = "";
			_error = true;
		}

		// Set ID
		houseObj.TryGetValue("id", out id);
		if(id != null)
		{
			_id = (int)id;
		}
		else
		{
			_id = 0;
			_error = true;
		}


	}

	public string serverInfo()
	{
		return _info.ToString();
	}

	public override string ToString()
		{
			return string.Format("[JsonHouse: Name={0}, Id={1}, Error={2}, JsonBlob={3}]", Name, Id, Error, JsonBlob);
		}
}//class
}//namespace

