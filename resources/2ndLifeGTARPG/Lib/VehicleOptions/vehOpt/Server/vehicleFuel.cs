using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

public class VehicleFuelScript : Script
{
	public double Fuel = 100;
	ArrayList VehicleCars = new ArrayList();
	public string netVehicle;
	public int oldSeconds;
	public DateTime date;
	public Client sPlayer;
	public NetHandle svehicle;
	bool sEngineStatus;
	public double fuelIntervall = 3;
	public VehicleFuelScript()
	{
		API.onUpdate += OnUpdateHandler;
		API.onPlayerEnterVehicle += OnPlayerEnterVehicle;
	}
	
    public VehicleFuelScript( double fuel, NetHandle vehicle){
    
		Fuel = fuel;
		
    }
	
	private void OnPlayerEnterVehicle(Client player, NetHandle vehicle)
    {
		netVehicle = "veh_"+vehicle.ToString();
	
		bool ItemFound = VehicleCars.Contains(netVehicle);
		
		
		VehicleFuelScript vfs = new VehicleFuelScript( 100, vehicle);
		
		if (!ItemFound){
			VehicleCars.Add(netVehicle+","+this.Fuel.ToString());
			//API.sendChatMessageToAll(netVehicle+",100");
		}
		
		oldSeconds = SetOldSeconds();
		
		this.sPlayer = player;
		this.svehicle = vehicle;
		API.triggerClientEvent(player, "update_fuel_client", this.Fuel);
    }
	
	private void GetFuelFromList(List < string > vehListe){
		
	}
	
	public void OnUpdateHandler(){ 			
		bool EngineStatus;
		if(sPlayer != null){
			EngineStatus = API.getVehicleEngineStatus(this.svehicle);
			if(this.sEngineStatus != EngineStatus){
				oldSeconds = SetOldSeconds();
				this.sEngineStatus = EngineStatus;
			}
			if(this.sEngineStatus){
				FuelTimer();
			}
		}
	}
	
	public void FuelTimer(){
		int seconds; 	

		DateTime dt = DateTime.Now; 
		seconds = dt.Minute; 
		
		if (oldSeconds == seconds){
			oldSeconds = seconds + 1;
			UpdateFuel();
		}else if(oldSeconds == 60){
			oldSeconds = 0;
		}else if(oldSeconds == 61){
			oldSeconds = 1;
		}
	}

	private void UpdateFuel(){
		this.Fuel = this.Fuel - fuelIntervall;
		if(this.Fuel == 0){
			API.setVehicleEngineStatus(this.svehicle, false);	
		}
		API.consoleOutput(this.Fuel.ToString());
		API.triggerClientEvent(this.sPlayer, "update_fuel_client", this.Fuel);
	}
	
	private int SetOldSeconds(){
		DateTime odt = DateTime.Now; 
		oldSeconds = odt.Minute;
		oldSeconds = oldSeconds + 1;
		return oldSeconds;
	}
	


}
