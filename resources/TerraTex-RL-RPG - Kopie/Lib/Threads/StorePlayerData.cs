using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace TerraTex_RL_RPG.Lib.Threads
{
    public class StorePlayerData
    {
        private bool _interuped = false;

        public void DoWork()
        {
            TTRPG.Api.consoleOutput("Started User Data Store Thread");

            while (!_interuped)
            {
                List<Client> players = TTRPG.Api.getAllPlayers();

                foreach (Client player in players)
                {
                    if (player.hasSyncedData("loggedin") && (bool)player.getSyncedData("loggedin"))
                    {
                        StartToStorePlayerData(player);
                    }
                }

                Thread.Sleep(900000);
            }
        }

        public void StartToStorePlayerData(Client player)
        {
            StoreTableUserData(player);
            StoreTableUserInventory(player);
        }

        private void StoreTableUserData(Client player)
        {
            string[] fields = { "PlayTime", "RP", "Level", "Skin", "PayDay"};
            Dictionary<string, dynamic> valueReplacements = new Dictionary<string, dynamic>();

            Dictionary<string, Dictionary<string, double>> payDay = new Dictionary<string, Dictionary<string, double>>();
            payDay.Add("Income", (Dictionary<string, double>)player.getData("PayDayIncome"));
            payDay.Add("Outgoings", (Dictionary<string, double>)player.getData("PayDayOutgoings"));
            payDay.Add("LastIncome", (Dictionary<string, double>)player.getData("LastPayDayIncome"));
            payDay.Add("LastOutgoings", (Dictionary<string, double>)player.getData("LastPayDayOutgoings"));
            valueReplacements.Add("PayDay", JObject.FromObject(payDay).ToString());


            BuildAndExecuteTableQuery(player, "user_data", fields, valueReplacements);
        }

        private void StoreTableUserInventory(Client player)
        {
            string[] fields = {"Money", "BankAccount", "Phone"};
			
            Dictionary<string, dynamic> valueReplacements = new Dictionary<string, dynamic>();


            if (player.getSyncedData("Phone") != -1)
            {
                valueReplacements.Add("Phone", player.getSyncedData("Phone"));
            }
            else
            {
                valueReplacements.Add("Phone", null);
            }

            BuildAndExecuteTableQuery(player, "user_inventory", fields, valueReplacements);
        }

        private void BuildAndExecuteTableQuery(Client player, string table, string[] fields, Dictionary<string, dynamic> valueReplacements)
        {
            MySqlCommand updateUserCommand = TTRPG.Mysql.Conn.CreateCommand();

            StringBuilder cmd = new StringBuilder();
            cmd.Append("UPDATE " + table + " SET ");

            for (int i = 0; i < fields.Length; i++)
            {
                cmd.Append(fields[i] + " = @" + fields[i]);
                if (i < fields.Length - 1)
                {
                    cmd.Append(",");
                }

                if (valueReplacements.ContainsKey(fields[i]))
                {
                    updateUserCommand.Parameters.AddWithValue("@" + fields[i], valueReplacements.Get(fields[i]));
                }
                else
                {
                    updateUserCommand.Parameters.AddWithValue("@" + fields[i], player.getSyncedData(fields[i]));
                }

            }

            cmd.Append(" WHERE UserID = @UserID");
            updateUserCommand.CommandText = cmd.ToString();

            // general ID
            updateUserCommand.Parameters.AddWithValue("@UserID", player.getSyncedData("ID"));

            updateUserCommand.ExecuteNonQuery();
        }

        public void StopThread()
        {
            _interuped = true;
        }
    }
}