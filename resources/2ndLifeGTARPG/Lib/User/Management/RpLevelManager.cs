using GrandTheftMultiplayer.Server.Elements;

namespace TerraTex_RL_RPG.Lib.User.Management
{
    public static class RpLevelManager
    {
        private static int _minLevelOneValue = 30;
        private static double _factorPerLevel = 1.15;

        public static void CalculateAndSetLevelOfPlayer(Client player)
        {
            int rp = player.getSyncedData("RP");
            int level = 0;
            double calcedRp = _minLevelOneValue;
            double previousRp = 0;
            while (calcedRp <= rp)
            {
                level++;
                previousRp = calcedRp;
                calcedRp = 30 + calcedRp * _factorPerLevel;
            }

            player.setSyncedData("Level", level);
            player.setSyncedData("RpNextLevel", calcedRp);
            player.setSyncedData("RpPreviousLevel", previousRp);
        }

        public static void AddRpToPlayer(Client player, int rp, bool showNotification)
        {
            int lastLevel = player.getSyncedData("Level");
            player.setSyncedData("RP", player.getSyncedData("RP") + rp);
            CalculateAndSetLevelOfPlayer(player);

            if (showNotification)
            {
                player.sendNotification("Level-System", "Du hast " + rp + " RP erhalten!", true);
            }

            if (lastLevel != player.getSyncedData("Level"))
            {
                player.triggerEvent("NewLevel", player.getSyncedData("Level"));
            }

            RefreshPlayerRpAndLevelDisplay(player);
        }

        public static void RefreshPlayerRpAndLevelDisplay(Client player)
        {
            player.triggerEvent("RefreshRpAndLevel",
                player.getSyncedData("Level"),
                player.getSyncedData("RP"),
                player.getSyncedData("RpNextLevel"),
                player.getSyncedData("RpPreviousLevel")
            );

            TTRPG.Api.exported.scoreboard.setPlayerScoreboardData(player, "Level", player.getSyncedData("Level").ToString());
        }
    }
}