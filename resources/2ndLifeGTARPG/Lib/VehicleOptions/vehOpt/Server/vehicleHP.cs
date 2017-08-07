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

public class VehicleHPScript : Script
{
    public VehicleHPScript()
    {
		
		
    }
	public int currentVehicleHP;	
	
	private void setVehicleHealth(NetHandle vehicle, float vehicleHP){
		API.setVehicleHealth(vehicle, vehicleHP);
	}
	
	private float GetCurrentVehicleHP(NetHandle vehicle){
		float currentVehicleHP = API.getVehicleHealth(vehicle);
		return currentVehicleHP;
	}
}
