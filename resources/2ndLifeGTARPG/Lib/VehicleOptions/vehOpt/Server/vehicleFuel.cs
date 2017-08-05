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
    private int VehicleID { get; set; }
    public Vehicle veh;
    public Dictionary<Client, List<NetHandle>> VehicleList = new Dictionary<Client, List<NetHandle>>();
    public Dictionary<NetHandle, double> FuelList = new Dictionary<NetHandle, double>();

    public VehicleFuelScript()
	{
		API.onPlayerEnterVehicle += OnPlayerEnterVehicle;
        API.onClientEventTrigger += OnClientEvent;
    }

    public VehicleFuelScript(int vehicleID, Client sender, int model)
    {
        VehicleID = vehicleID;
        var rot = API.getEntityRotation(sender.handle);
        veh = API.createVehicle((VehicleHash)model, sender.position, new Vector3(0, 0, rot.Z), 0, 0);
    }

    public void OnClientEvent(Client player, string eventName, params object[] args) //arguments param can contain multiple params
    {
        NetHandle vehicle = API.getPlayerVehicle(player);
        if (eventName == "UPDATE_FUEL")
        {
            
            double CurrentTank;


            for (int i = FuelList.Count - 1; i >= 0; i--)
            {
                var item = FuelList.ElementAt(i);
                var itemKey = item.Key;
                var itemValue = item.Value;
                bool EngineStatus = API.getVehicleEngineStatus(itemKey);
                API.sendChatMessageToAll("Motor an " + EngineStatus);
                if (EngineStatus)
                {
                    CurrentTank = FuelList.Get(itemKey);
                    CurrentTank = CurrentTank - 1;
                        
                    if (CurrentTank <= 0)
                    {
                        CurrentTank = 0;
                        API.setVehicleEngineStatus(itemKey, false);
                    }
                    API.sendChatMessageToAll("FahrzeugID " + itemKey + " Tank " + itemValue);
                    FuelList.Set(itemKey, CurrentTank);

                    if (API.isPlayerInAnyVehicle(player))
                    {
                        API.triggerClientEvent(player, "update_fuel_client", FuelList.Get(vehicle));
                    }
                }
            }
        }

        if (eventName == "GET_VEHICLE_FUEL")
        {
            API.triggerClientEvent(player, "SEND_VEHICLE_FUEL", FuelList.Get(vehicle));
        }
    }

    private void OnPlayerEnterVehicle(Client sender, NetHandle vehicle)
    {
        if (!FuelList.ContainsKey( vehicle ))
        {
            FuelList.Add(vehicle, Fuel);
            API.sendChatMessageToAll("Auto zur Liste hinzugefügt.");
        }
		API.triggerClientEvent(sender, "update_fuel_client", FuelList.Get(vehicle));
    }
	
}
