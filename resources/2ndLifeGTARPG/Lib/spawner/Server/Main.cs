using System;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;


namespace GTFuel
{
    public class Spawner : Script
    {
        private Dictionary<Client, NetHandle> _vehicleHistory = new Dictionary<Client, NetHandle>();

        public Spawner()
        {
            API.onClientEventTrigger += onClientEventTrigger;
        }

        public void onClientEventTrigger(Client sender, string name, object[] args)
        {
            /*
            if (name != "CREATE_VEHICLE") return;
            
            int model = (int)args[0];

            if (!Enum.IsDefined(typeof(VehicleHash), model)) return;

            //var rot = API.getEntityRotation(sender.handle);
            //var veh = API.createVehicle((VehicleHash)model, sender.position, new Vector3(0, 0, rot.Z), 0, 0);

            NetHandle Vehicle = API.getPlayerVehicle(sender);
            int VehicleID = Vehicle.Value;
            VehicleFuelScript vfs = new VehicleFuelScript(VehicleID, sender, model);

            if (_vehicleHistory.ContainsKey(sender) && _vehicleHistory[sender] != null && API.doesEntityExist(_vehicleHistory[sender]))
            {
                API.deleteEntity(_vehicleHistory[sender]);
            }

            _vehicleHistory[sender] = vfs.veh;

            API.setPlayerIntoVehicle(sender, vfs.veh, -1);
            */
        }
    }
}