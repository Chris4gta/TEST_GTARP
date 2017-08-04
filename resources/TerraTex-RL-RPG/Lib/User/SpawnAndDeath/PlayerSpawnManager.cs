using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using TerraTex_RL_RPG.Lib.Helper;

namespace TerraTex_RL_RPG.Lib.User.SpawnAndDeath
{
    public static class PlayerSpawnManager
    {
        public static void Spawn(Client player)
        {
            SetPlayerNameTag(player);
            SetPlayerSkin(player);
            player.nametagVisible = true;
            player.position = new Vector3(259.8162, -1204.156, 29.28907);
            player.transparency = 255;
            player.freeze(false);
            player.collisionless = false;


            //void _DISABLE_AUTOMATIC_RESPAWN(BOOL disableRespawn) // 2C2B3493FBF51C71 296574AE
        }

        public static void SetPlayerNameTag(Client player)
        {
            int id = player.getSyncedData("ID");
            string nickname = player.getSyncedData("Nickname");
            string forename = player.getSyncedData("Forename");
            string lastname = player.getSyncedData("Lastname");

            // Additionally set PlayerName in Scoreboard
            TTRPG.Api.exported.scoreboard.setPlayerScoreboardData(player, "Vorname", forename);
            TTRPG.Api.exported.scoreboard.setPlayerScoreboardData(player, "Nachname", lastname);

            // generate Nametag
            string nameTag = "[" + id + "]" + nickname;

            forename = nameTag.Length <= 20 ? forename : (forename[0] + ".");
            nameTag += " (" + forename + " ";

            lastname = nameTag.Length <= 30 ? lastname : (lastname[0] + ".");
            nameTag += lastname + ")";

            TTRPG.Api.setPlayerNametag(player, nameTag);

        }

        public static void SetPlayerSkin(Client player)
        {
            string skin = player.getSyncedData("Skin");

            if (skin.Equals("0"))
            {
                if (player.getSyncedData("Gender").Equals("male"))
                {
                    player.setSkin(TTRPG.Api.pedNameToModel("Michael"));
                }
                else
                {
                    player.setSkin(TTRPG.Api.pedNameToModel("AnitaCutscene"));
                }
            }
            else
            {
                player.setSkin(TTRPG.Api.pedNameToModel(skin));
            }
        }
    }
}