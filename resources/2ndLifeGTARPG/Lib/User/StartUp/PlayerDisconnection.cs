using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;

namespace TerraTex_RL_RPG.Lib.User.StartUp
{
    public class PlayerDisconnection : Script
    {
        public PlayerDisconnection()
        {
            API.onPlayerDisconnected += OnPlayerDisconnectedEventHandler;
        }

        public void OnPlayerDisconnectedEventHandler(Client player, string reason)
        {
            if (player.hasSyncedData("loggedin") && player.getSyncedData("loggedin"))
            {
                player.setSyncedData("loggedin", false);
                TTRPG.StorePlayerDataThread.StartToStorePlayerData(player);

                API.consoleOutput("Account " + player.name + "(" + player.getSyncedData("ID") + ") left the server: " +
                                  reason);
            }
        }
    }
}