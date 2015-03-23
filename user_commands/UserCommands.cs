using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace Hats
{

    namespace user_commands
    {
        public class CommandsInterface
        {
            /**
            * Given a simulated user's credentials sends a login request 
            * \param[in] username The username of the simulated user being logged in
            * \param[in] password The password of the simulated user being logged in
            * \param[in] client Client to send the login information to
            * \param[out] Flag indicating success
            */
            static public bool sendLogin(string username, string password, HttpClient client)
            {
                return true;
            }

            /**
            * Sends a request to add a new house with the specified name
            * \param[in] houseName The name of the new house to be added
            * \param[in] client Client to send the house information to
            * \param[out] Flag indicating success
            */
            static public bool sendAddHouse(string houseName, HttpClient client)
            {
                return true;
            }

            /**
            * Send a request to add a new room with the specified name to the specified house
            * \param[in] houseName The name of the housethe room is being added to
            * \param[in] roomName The name of the room being added
            * \param[in] client Client to send the room information to
            * \param[out] Flag indicating success
            */
            static public bool sendAddRoom(string houseName, string roomName, HttpClient client)
            {
                return true;
            }

            /**
            * Sends a request to add a device to a specified room and house
            * \param[in] deviceType The type of device being added
            * \param[in] houseName The name of the house the decive being added to
            * \param[in] roomName The name of the room the device is in
            * \param[in] client Client to send the device information to
            * \param[out] Flag indicating success
            */
            static public bool sendAddDevice(string deviceType, string houseName, string roomName, HttpClient client)
            {
                return true;
            }

            /**
            * Sends a request to update a preference in the application
            * \param[in] preferenceName The name of the preference the update is being applied to
            * \param[in] preferenceSetting The new setting for the preference being updated
            * \param[in] client Client to send the preference update information to
            * \param[out] Flag indicating success
            */
            static public bool sendUpdatePreference(string preferenceName, string preferenceSetting, HttpClient client)
            {
                return true;
            }

            /**
            * Sends a request to update a setting of a device
            * \param[in] deviceID The ID of the device to be updated
            * \param[in] deviceSetting The setting of the specified device to be changed
            * \param[in] settingValue The new value of the setting specified 
            * \param[in] client Client to send the preference update information to
            * \param[out] Flag indicating success
            */
            static public bool sendUpdateDevice(string deviceID, string deviceSetting, string settingValue, HttpClient client)
            {
                return true;
            }

            /**
            * Checks a setting on a device and gets it's value
            * \param[in] deviceID The ID of the device to be checked
            * \param[in] deviceSetting The setting of the specified device to be checked
            * \param[in] client Client to send the query information to
            * \param[out] result String containing the value of the device setting checked
            */
            static public string sendCheckDevice(string deviceID, string deviceSetting, HttpClient client)
            {
                string result = "";
                return result;
            }

            /**
            * Updates a user's current location
            * \param[in] user The user whose location is being updated
            * \param[in] newLocation The setting of the specified device to be checked
            * \param[in] client Client to send the location information to
            * \param[out] Flag indicating success
            */
            static public bool sendUpdateLocation(string user, string newLocation, HttpClient client)
            {
                return true;
            }
        }
    }
}
