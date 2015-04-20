using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hats
{
    namespace Reader
    {
        //Stores lists of users and houses from the JSON file
        public class Configuration
        {
            public List<Users> users { get; set; }
            public List<Houses> houses { get; set; }
        }
        //Class the user ID is stored in upon reading the JSON file
        public class Users
        {
            public string UserID { get; set; }
        }
        //Holds the name of a house and the devices in the house
        public class Houses
        {
            public string name { get; set; }
            public List<devices> devices { get; set; }
        }
        //Holds the name of a device
        public class devices
        {
            public string name { get; set; }
        }
        //Holds a list of rooms
        public class Rooms
        {   
            public List<RoomID> RoomIDs { get; set; }
            
        }
        //Holds the name of a room
        public class RoomID
        {
            public string name { get; set; }
        }

        //Holds lists of the usernames, house Ids, and device Ids after they are located in the JSON file
        public class toVerify
        {
            public List<string> userNames  = new List<string>();
            public List<string> houseIDs = new List<string>();
            public List<string> deviceIDs = new List<string>();
        }
        public class Reader
        {
            //reads the JSON file and parses out the usernames, house IDs, and device IDs to be checked with server
            //before starting the simulation. 
            //Inputs: string fileName: name of the json file
            //Outputs: toVerify: a toVerify object containing lists of Strings for the usernames and house/device IDs
            public toVerify readFile(string fileName)
            {
                toVerify output = new toVerify();
                //var myConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(System.IO.File.ReadAllText(fileName));


                /*foreach (Users item in myConfig.users)
                {
                    output.userNames.Add(item.UserID);
                }
               
                foreach(Houses item in myConfig.houses)
                {
                    output.houseIDs.Add(item.name);
                    foreach(devices item2 in item.devices)
                    {

                        output.deviceIDs.Add(item2.name);
                    }

                } */
                return (output);
            }
        }
    }
}

