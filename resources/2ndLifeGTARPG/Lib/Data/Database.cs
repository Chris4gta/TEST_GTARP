using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Xml;
using GrandTheftMultiplayer.Server.Constant;
using MySql.Data.MySqlClient;

namespace TerraTex_RL_RPG.Lib.Data
{
    public class Database
    {
        private MySqlConnection _conn;

        public Database()
        {
            if (!TTRPG.Configs.ConfigExists("database"))
            {
                TTRPG.Api.consoleOutput(LogCat.Fatal, "Configuration for database is missing in Configs directory.");
                TTRPG.Api.stopResource(TTRPG.Api.getThisResource());
            }

            Connect();
        }

        private void Connect()
        {
            try
            {
                XmlDocument databaseConfig = TTRPG.Configs.GetConfig("database");
                string host = databaseConfig.GetElementsByTagName("host")[0].InnerText;
                string user = databaseConfig.GetElementsByTagName("user")[0].InnerText;
                string password = databaseConfig.GetElementsByTagName("password")[0].InnerText;
                string database = databaseConfig.GetElementsByTagName("database")[0].InnerText;

                string myConnectionString = "server=" + host + ";uid=" + user + ";" + "pwd=" + password + ";database=" +
                                            database + ";Allow Zero Datetime=true;";

                try
                {
                    _conn = new MySqlConnection(myConnectionString);
                    _conn.Open();

                    TTRPG.Api.consoleOutput(LogCat.Info, "Database Connection created.");
                }
                catch (MySqlException ex)
                {
                    TTRPG.Api.consoleOutput(LogCat.Fatal, ex.Message);
                    TTRPG.Api.stopResource(TTRPG.Api.getThisResource());
                }
            }
            catch (NullReferenceException ex)
            {
                TTRPG.Api.consoleOutput(LogCat.Fatal, "Configuration for database is not correct: " + ex.Message);
                TTRPG.Api.stopResource(TTRPG.Api.getThisResource());
            }
        }

        public MySqlConnection Conn
        {
            get
            {
                if (_conn.State == ConnectionState.Open)
                {
                    return _conn;
                }
                else
                {
                    _conn.Open();
                    TTRPG.Api.consoleOutput(LogCat.Info, "Database Connection recreated.");

                    return _conn;
                }
            }
        }
    }
}