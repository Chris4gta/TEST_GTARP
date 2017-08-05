using System;
using System.Runtime.InteropServices.WindowsRuntime;
using GrandTheftMultiplayer.Server.Elements;
using MySql.Data.MySqlClient;

namespace TerraTex_RL_RPG.Lib.User.Management
{
    public static class MoneyManager
    {
        public static float GetPlayerMoney(Client player)
        {
            return player.getSyncedData("Money");
        }

        public static float GetPlayerBank(Client player)
        {
            return player.getSyncedData("BankAccount");
        }

        public enum Categorys
        {
            PlayerToPlayer,
            PayDay,
            Job,
            Other
        }
        
        public static void ChangePlayerMoney(Client player, float amount, bool bankPay,
            Categorys category, string reason, string additionalDataAsJson)
        {
            float money = bankPay ? GetPlayerBank(player) : GetPlayerMoney(player);
            if (bankPay)
            {
                player.setSyncedData("BankAccount", money + amount);
            }
            else
            {
                player.setSyncedData("Money", money + amount);
            }

            MySqlCommand logInsertCommand = TTRPG.Mysql.Conn.CreateCommand();
            logInsertCommand.CommandText = "INSERT INTO log_player_money (UserID, Typ, Category, Amount, Reason, AdditionalData) VALUES (@UserID, @Typ, @Category, @Amount, @Reason, @AdditionalData)";
            logInsertCommand.Parameters.AddWithValue("@UserID", player.getSyncedData("ID"));
            logInsertCommand.Parameters.AddWithValue("@Typ", bankPay ? "BANKACCOUNT" : "MONEY");
            logInsertCommand.Parameters.AddWithValue("@Category", category);
            logInsertCommand.Parameters.AddWithValue("@Amount", Math.Round(amount, 2));
            logInsertCommand.Parameters.AddWithValue("@Reason", reason);
            logInsertCommand.Parameters.AddWithValue("@AdditionalData", additionalDataAsJson);

            logInsertCommand.ExecuteNonQuery();

            RefreshPlayerMoneyDisplay(player);
        }

        public static void RefreshPlayerMoneyDisplay(Client player)
        {
            player.triggerEvent("RefreshMoneyUI", player.getSyncedData("Money"));
        }

        public static bool CanPlayerPayMoney(Client player, float price)
        {
            return GetPlayerMoney(player) >= price;
        }

        public static bool CanPlayerPayBank(Client player, float price)
        {
            return GetPlayerBank(player) >= price;
        }
    }
}