using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;

namespace TerraTex_RL_RPG.Lib.Admin
{
    class DevServerCommands : Script
    {
        [Command("save", Group = "dev", SensitiveInfo = false, GreedyArg = true)]
        public void SaveCommand(Client player, string info = "")
        {
            if (DevServer.CheckDevCommandAccess(player))
            {
                Vector3 position = player.position;
                Directory.CreateDirectory(API.getResourceFolder() + "/Logs");
                string path = API.getResourceFolder() + "/Logs/Position.log";

                if (!File.Exists(path))
                {
                    string createText = "" + Environment.NewLine;
                    File.WriteAllText(path, createText);
                }

                string appendText = position.X.ToString("R").Replace(",", ".") + ", " +
                                    position.Y.ToString("R").Replace(",", ".") + ", " +
                                    position.Z.ToString("R").Replace(",", ".") + " // " + info + Environment.NewLine;
                File.AppendAllText(path, appendText);
                player.sendNotification("Dev-System",
                    "Saved Position: X: " + position.X + "; Y: " + position.Y + "; Z: " + position.Z);
            }
        }

        [Command("veh", Group = "dev", SensitiveInfo = false)]
        public void VehCommand(Client player, string vehicleModelName)
        {
            if (DevServer.CheckDevCommandAccess(player) || AdminChecks.CheckAdminLvl(player, 3))
            {
                Vector3 position = player.position.Add(new Vector3(2, 2, 2));
                Vector3 rotation = player.rotation;

                VehicleHash myVehicle = API.vehicleNameToModel(vehicleModelName);
                Random rnd = new Random();
                API.createVehicle(myVehicle, position, rotation, rnd.Next(0, 159), rnd.Next(0, 159));
            }
        }
    }
}