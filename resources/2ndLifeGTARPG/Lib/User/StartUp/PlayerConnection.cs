using System;
using System.Text.RegularExpressions;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using MySql.Data.MySqlClient;

namespace TerraTex_RL_RPG.Lib.User.StartUp
{
    public class PlayerConnection : Script
    {
        public PlayerConnection()
        {
            API.onClientEventTrigger += OnClientEvent;
            API.onPlayerConnected += OnPlayerConnectedEventHandler;
        }

        public void OnPlayerConnectedEventHandler(Client player)
        {
            Regex nickTest = new Regex("^[A-Za-z0-9-_ÄÖÜßäüö]*$");
            if (!nickTest.IsMatch(player.name))
            {
                player.kick("Dein Nickname enthält nicht erlaubte Symbole!");
            }

            if (!player.isCEFenabled)
            {
                player.kick("Dieser Server erfordert, das CEF aktiviert ist!");
            }

            player.nametagVisible = false;
            player.invincible = true;
            player.position = new Vector3(0,0,200);
            player.transparency = 0;
            player.freeze(true);
            player.collisionless = true;
        }
        
        public void OnClientEvent(Client player, string eventName, params object[] arguments)
        {
            if (eventName.Equals("onClientResourceStarted"))
            {
                MySqlCommand doesPlayerExistInDb = TTRPG.Mysql.Conn.CreateCommand();

                doesPlayerExistInDb.CommandText = "SELECT count(ID) FROM user WHERE Nickname = @nickname";
                doesPlayerExistInDb.Parameters.AddWithValue("@nickname", player.name);

                Int32 accounts = Int32.Parse(doesPlayerExistInDb.ExecuteScalar().ToString());
                
                player.setSyncedData("fingerprint", arguments[0]);

                if (accounts == 1)
                {
                    player.triggerEvent("startLogin", player.name);
                }
                else
                {
                    player.triggerEvent("startRegister", player.name);
                }
            }
        }
    }
}