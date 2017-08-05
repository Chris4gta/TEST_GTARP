using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.Elements;

namespace TerraTex_RL_RPG.Lib.Admin
{
    static class AdminChecks
    {
        /// <summary>
        /// This function checks if player has correct adminlvl
        /// AdminLvl Range:
        ///     0 - No Admin
        ///     1 - Supporter
        ///     2 - Moderator / Supermoderator (Moderators with special rights)
        ///     3 - Administrator
        ///     4 - Serverleadership
        /// </summary>
        /// <param name="player">Player to Check</param>
        /// <param name="minLevel">Minimum required Adminlvl</param>
        /// <returns>True or False</returns>
        public static bool CheckAdminLvl(Client player, int minLevel)
        {
            if (player.getSyncedData("Admin") >= minLevel)
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
