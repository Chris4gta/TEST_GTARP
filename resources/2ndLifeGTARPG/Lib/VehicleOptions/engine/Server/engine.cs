using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

public class EngineScript : Script
{
    public EngineScript()
    {
        API.onClientEventTrigger += onClientEventTrigger;
    }


    public void onClientEventTrigger(Client sender, string name, object[] args)
    {
        if (name == "ENGINE_STATUS"){
			
			NetHandle veh = (NetHandle)args[0];
			bool bEngineStatus = (bool)args[1];
			
			API.setVehicleEngineStatus(veh, bEngineStatus);		
			
		}
	}
}
