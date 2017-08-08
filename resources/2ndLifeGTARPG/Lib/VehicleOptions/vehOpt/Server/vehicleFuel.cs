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

namespace GTFuel
{

    public class VehicleFuelScript : Script
    {
        public float Fuel = 100f;
        ArrayList VehicleCars = new ArrayList();
        public string netVehicle;
        public int oldSeconds;
        public DateTime date;
        public Client sPlayer;
        public NetHandle svehicle;
        public double fuelIntervall = 3;
        private int VehicleID { get; set; }
        public Vehicle veh;
        public Dictionary<Client, List<NetHandle>> VehicleList = new Dictionary<Client, List<NetHandle>>();
        public Dictionary<NetHandle, float> FuelList = new Dictionary<NetHandle, float>();
        //public Dictionary<NetHandle, float> FuelList = new Dictionary<NetHandle, float>();

        public VehicleFuelScript()
        {
            
            API.onClientEventTrigger += OnClientEvent;
        }

        public VehicleFuelScript(int vehicleID, Client sender, int model)
        {
            VehicleID = vehicleID;
            var rot = API.getEntityRotation(sender.handle);
            veh = API.createVehicle((VehicleHash)model, sender.position, new Vector3(0, 0, rot.Z), 0, 0);
            API.sendChatMessageToAll("Auto hash: "+ model);
        }

        public void OnClientEvent(Client player, string eventName, params object[] args)
        {//arguments param can contain multiple params
            
            NetHandle vehicle = API.getPlayerVehicle(player);

            if (eventName == "UPDATE_FUEL")
            {
                float CurrentTank;

                
                for (int i = FuelList.Count - 1; i >= 0; i--)
                {
                    var item = FuelList.ElementAt(i);
                    var itemKey = item.Key;
                    var itemValue = item.Value;
                    bool EngineStatus = API.getVehicleEngineStatus(itemKey);
                    
                    if (EngineStatus)
                    {
                        CurrentTank = FuelList.Get(itemKey);
                        CurrentTank = CurrentTank - 0.02f;

                        if (CurrentTank <= 0f)
                        {
                            CurrentTank = 0f;
                            API.setVehicleEngineStatus(itemKey, false);
                        }
                        
                        FuelList.Set(itemKey, CurrentTank);

                        if (API.isPlayerInAnyVehicle(player))
                        {
                            API.triggerClientEvent(player, "update_fuel_client", FuelList.Get(vehicle));
                        }
                    }
                }
            }
        }

        

    }

}