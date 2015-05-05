# sim_harness
This repository contains the simulation harness for the operating a simulation of the HATS simulation.

## usage
Open the GUI, fill in correct server url:
http://serverapi1.azurewebsites.net

(refer to https://github.com/ECE4574-5574/ServerAPI.git for the lastest server address)

For *Scenarios Directory*, choose scenario folder at https://github.com/ECE4574-5574/sim_harness/tree/master/scenarios.

For *App Simulator* location, choose apk file from the app team.

For *House Simulator* location, choose house.exe, which is located at Automation_System/Devices/House/bin/Debug after complie.

From the GUI user can set simulation time for the system as well as time frame speed. Then choose one of scenarios from *Test Scenarios* and press *Load Scenario* button. Press *Start Test* button to start simulation. Press *End Test* to stop simulation.
