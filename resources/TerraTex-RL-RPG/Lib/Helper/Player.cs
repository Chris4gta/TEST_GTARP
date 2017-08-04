using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;

namespace TerraTex_RL_RPG.Lib.Helper
{
    public static class PlayerHelper
    {
        public static Client GetPlayerFromId(int Id)
        {
            List<Client> clients = TTRPG.Api.getAllPlayers();

            foreach (Client player in clients)
            {
                if (player.getSyncedData("ID") == Id)
                {
                    return player;
                }
            }

            return null;
        }
    }
}