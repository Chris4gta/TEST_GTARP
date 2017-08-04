using System;
using System.Collections.Generic;
using System.Data;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using TerraTex_RL_RPG.Lib.Admin;
using TerraTex_RL_RPG.Lib.Helper;
using TerraTex_RL_RPG.Lib.Threads;
using TerraTex_RL_RPG.Lib.User.Management;
using TerraTex_RL_RPG.Lib.User.SpawnAndDeath;

namespace TerraTex_RL_RPG.Lib.User.StartUp
{
    class PlayerLogin : Script
    {
        public PlayerLogin()
        {
            API.onClientEventTrigger += OnClientEvent;
        }

        public void OnClientEvent(Client player, string eventName, params object[] arguments)
        {
            if (eventName.Equals("onClientStartLogin"))
            {
                string password = (string) arguments[0];

                MySqlCommand doesPlayerExistInDb = TTRPG.Mysql.Conn.CreateCommand();

                doesPlayerExistInDb.CommandText = "SELECT * FROM user WHERE Nickname = @nickname";
                doesPlayerExistInDb.Parameters.AddWithValue("@nickname", player.name);

                DataTable result = new DataTable();

                result.Load(doesPlayerExistInDb.ExecuteReader());

                string salt = (string) result.Rows[0]["Salt"];
                string dbPassword = (string) result.Rows[0]["Password"];

                if (PasswordHelper.Hash(password, salt).Equals(dbPassword))
                {
                    // Password was correct and now Update fingerprint and last login
                    // before starting Login process
                    if (!DevServer.CheckDevServerLogin(player, result.Rows[0]))
                    {
                        MySqlCommand updateUserEntryCommand = TTRPG.Mysql.Conn.CreateCommand();
                        updateUserEntryCommand.CommandText =
                            "UPDATE user SET Last_Fingerprint = @fingerprint, Last_Login=current_timestamp() WHERE Nickname=@nickname";
                        updateUserEntryCommand.Parameters.AddWithValue("fingerprint",
                            player.getSyncedData("fingerprint"));
                        updateUserEntryCommand.Parameters.AddWithValue("nickname", player.name);

                        updateUserEntryCommand.ExecuteNonQuery();

                        EnsureAllDatabaseTableEntries((Int32) result.Rows[0]["ID"]);
                        StartLoginProcess(player, result.Rows[0]);

                        player.setSyncedData("loggedin", true);
                        API.consoleOutput("Account " + player.name + "(" + player.getSyncedData("ID") + ") logged in.");
                        player.invincible = false;
                        PlayerSpawnManager.Spawn(player);

                        TTRPG.Api.exported.scoreboard.setPlayerScoreboardData(player, "ID", result.Rows[0]["ID"].ToString());
                        TTRPG.Api.exported.scoreboard.setPlayerScoreboardData(player, "Nickname", player.name);

                        MoneyManager.RefreshPlayerMoneyDisplay(player);
                        RpLevelManager.CalculateAndSetLevelOfPlayer(player);
                        RpLevelManager.RefreshPlayerRpAndLevelDisplay(player);
                        UpdatePlayerPlayTime.UpdatePlayerPlayTimeDisplay(player);
                    }
                }
                else
                {
                    player.sendNotification("System-Error", "Das Passwort, dass du eingegeben hast, ist fehlerhaft.", false);
                    player.triggerEvent("startLogin", player.name);
                }
            }
        }

        private void EnsureAllDatabaseTableEntries(int dbUserId)
        {
            string[] tables = new string[]
            {
                "user_data",
                "user_inventory"
            };

            foreach (string table in tables)
            {
                MySqlCommand doesPlayerExistInDb = TTRPG.Mysql.Conn.CreateCommand();
                doesPlayerExistInDb.CommandText = "SELECT count(UserID) FROM " + table + " WHERE UserID = @id";
                doesPlayerExistInDb.Parameters.AddWithValue("@id", dbUserId);
                Int32 accounts = Int32.Parse(doesPlayerExistInDb.ExecuteScalar().ToString());
                if (accounts == 0)
                {
                    MySqlCommand createUserCommand = TTRPG.Mysql.Conn.CreateCommand();
                    createUserCommand.CommandText = "INSERT INTO " + table + " (UserID) VALUES (@id)";
                    createUserCommand.Parameters.AddWithValue("@id", dbUserId);
                    createUserCommand.ExecuteNonQuery();
                }
            }
        }

        private void StartLoginProcess(Client player, DataRow dbUser)
        {
            // read all data here and store it to player before starting spawn
            int userId = (int) dbUser["ID"];
            ApplyTableToPlayerUser(player, dbUser);

            DataRow userData = GetDataFromUserTable("user_data", userId);
            ApplyTableToPlayerUserData(player, userData);

            DataRow userInventory = GetDataFromUserTable("user_inventory", userId);
            ApplyTableToPlayerUserInventory(player, userInventory);
        }

        private void ApplyTableToPlayerUser(Client player, DataRow data)
        {
            player.setSyncedData("ID", (int) data["ID"]);
            player.setSyncedData("UUID", (string) data["UUID"]);
            player.setSyncedData("Nickname", (string) data["Nickname"]);
            player.setSyncedData("Forename", (string) data["Forename"]);
            player.setSyncedData("Lastname", (string) data["Lastname"]);
            player.setSyncedData("Gender", (string) data["Gender"]);
            player.setSyncedData("Birthday", DateTime.Parse(data["Birthday"].ToString()));
            player.setSyncedData("History", (string) data["History"]);
            player.setSyncedData("Admin", (int) data["Admin"]);
            player.setSyncedData("Dev", (int) data["Dev"]);
        }

        private void ApplyTableToPlayerUserData(Client player, DataRow data)
        {
            player.setSyncedData("PlayTime", (int) data["PlayTime"]);
            player.setSyncedData("Skin", (string) data["Skin"]);
            player.setSyncedData("RP", (int) data["RP"]);

            Dictionary<string, Dictionary<string, double>> payDay = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>((string) data["PayDay"]);
            player.setData("PayDayIncome", payDay.Get("Income"));
            player.setData("PayDayOutgoings", payDay.Get("Outgoings"));
            player.setData("LastPayDayIncome", payDay.Get("LastIncome"));
            player.setData("LastPayDayOutgoings", payDay.Get("LastOutgoings"));
        }

        private void ApplyTableToPlayerUserInventory(Client player, DataRow data)
        {
            player.setSyncedData("Money", (float)(decimal) data["Money"]);
            player.setSyncedData("BankAccount", (float)(decimal) data["BankAccount"]);
            if (data["Phone"] != DBNull.Value)
            {
                player.setSyncedData("Phone", (int) data["Phone"]);
            }
            else
            {
                player.setSyncedData("Phone", -1);
            }
        }

        private DataRow GetDataFromUserTable(string table, int userId)
        {
            MySqlCommand getUserDataCommand = TTRPG.Mysql.Conn.CreateCommand();
            getUserDataCommand.CommandText = "SELECT * FROM " + table + " WHERE UserID = @id";
            getUserDataCommand.Parameters.AddWithValue("id", userId);
            DataTable result = new DataTable();
            result.Load(getUserDataCommand.ExecuteReader());
            return result.Rows[0];
        }
    }
}