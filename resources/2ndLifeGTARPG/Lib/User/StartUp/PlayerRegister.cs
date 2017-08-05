using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using TerraTex_RL_RPG.Lib.Helper;

namespace TerraTex_RL_RPG.Lib.User.StartUp
{

    class PlayerRegister : Script
    {

        public PlayerRegister()
        { 
            API.onClientEventTrigger += OnClientEvent;
        }

        public void OnClientEvent(Client player, string eventName, params object[] arguments)
        {
            if (eventName.Equals("onClientStartRegister"))
            {
                JObject data = JObject.Parse((string) arguments[0]);
                string forename = (string) data.GetValue("forename");
                string lastname = (string) data.GetValue("lastname");
                string password = (string) data.GetValue("password");
                string email = (string) data.GetValue("email");
                string history = (string) data.GetValue("history");
                string gender = (string) data.GetValue("gender");
                string birthday = (string) data.GetValue("birthday");
                
                MySqlCommand createUserCommand = TTRPG.Mysql.Conn.CreateCommand();
                createUserCommand.CommandText =
                    "INSERT INTO user (Nickname, Forename, Lastname, Password, Gender, Birthday, History, Last_Fingerprint, EMail, Salt, UUID) " +
                    "VALUES (@nickname, @forename, @lastname, @password, @gender, @birthday, @history, @last_fingerprint, @email, @salt, uuid())";

                string salt = PasswordHelper.GenerateSalt();
                password = PasswordHelper.Hash(password, salt);

                createUserCommand.Parameters.AddWithValue("@nickname", player.name);
                createUserCommand.Parameters.AddWithValue("@forename", forename);
                createUserCommand.Parameters.AddWithValue("@lastname", lastname);
                createUserCommand.Parameters.AddWithValue("@password", password);
                createUserCommand.Parameters.AddWithValue("@gender", gender);
                createUserCommand.Parameters.AddWithValue("@birthday", birthday);
                createUserCommand.Parameters.AddWithValue("@history", history);
                createUserCommand.Parameters.AddWithValue("@last_fingerprint", player.getSyncedData("fingerprint"));
                createUserCommand.Parameters.AddWithValue("@email", email);
                createUserCommand.Parameters.AddWithValue("@salt", salt);

                createUserCommand.ExecuteNonQuery();
                API.consoleOutput("Account " + player.name + "(" + createUserCommand.LastInsertedId + ") was created.");

                player.sendNotification("System", "Dein Account " + player.name + " wurde erstellt. Logge dich nun ein.");

                player.triggerEvent("startLogin", player.name);

            }
        }
    }
}
