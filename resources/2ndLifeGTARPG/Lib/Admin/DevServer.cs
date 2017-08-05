using System.Data;
using System.Runtime.Remoting;
using GrandTheftMultiplayer.Server.Elements;

namespace TerraTex_RL_RPG.Lib.Admin
{
    static class DevServer
    { 
        public static bool CheckDevServerLogin(Client player, DataRow userData)
        {
            if (TTRPG.Configs.GetConfig("server").GetElementsByTagName("isDevServer")[0].InnerText.Equals("1") && (int)userData["Dev"] == 0)
            {
                player.sendNotification("System-Error", "Du hast keine Berechtigung, dich auf dem DevServer einzuloggen.", false);
                player.triggerEvent("startLogin", player.name);
                return true;
            }
            return false;
        }

        public static bool CheckDevCommandAccess(Client player)
        {
            if (TTRPG.Configs.GetConfig("server").GetElementsByTagName("isDevServer")[0].InnerText.Equals("1"))
            {
                if (player.getSyncedData("Dev") > 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (player.getSyncedData("Dev") > 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
