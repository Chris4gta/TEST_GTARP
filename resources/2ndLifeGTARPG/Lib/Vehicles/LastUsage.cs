using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;

namespace TerraTex_RL_RPG.Lib.Vehicles
{
    public class LastUsage : Script
    {
        public LastUsage()
        {
            API.onPlayerExitVehicle += OnPlayerExitVehicleHandler;
        }

        public void OnPlayerExitVehicleHandler(Client player, NetHandle vehicle)
        {
            API.setEntityData(vehicle, "last-driver-name", player.name);
            API.setEntityData(vehicle, "last-driver-id", player.getSyncedData("ID"));
            API.setEntityData(vehicle, "last-driver-time", DateTime.Now);
        }
    }
}