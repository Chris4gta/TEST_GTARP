using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using TerraTex_RL_RPG.Lib.Data;
using TerraTex_RL_RPG.Lib.Threads;

namespace TerraTex_RL_RPG
{
    public class TTRPG : Script
    {
        private static Database _mysql;
        private static Configs _configs;
        private static API _api;
        private static StorePlayerData _storePlayerDataThread;
        private static UpdatePlayerPlayTime _updatePlayerPlayTimeThread;
        private static UpdateWeather _dynamicWeatherThread;

        public static Database Mysql => _mysql;

        public static Configs Configs => _configs;

        public static API Api => _api;

        public static StorePlayerData StorePlayerDataThread => _storePlayerDataThread;
        public static UpdatePlayerPlayTime UpdatePlayerPlayTimeThread => _updatePlayerPlayTimeThread;

        public TTRPG()
        {
            _api = API;
            API.onResourceStart += PrepareStartUp;
        }

        public void PrepareStartUp()
        {
            TtStartUp();
        }

        public static void TtStartUp()
        {
            _configs = new Configs();
            _mysql = new Database();

            if (!Configs.ConfigExists("server"))
            {
                _api.consoleOutput(LogCat.Fatal, "Configuration for server is missing in Configs directory.");
                _api.stopResource(_api.getThisResource());
            }

            _api.consoleOutput("Starting TerraTex_RL_RPG Gamemode");

            // start Player Threads
            _storePlayerDataThread = new StorePlayerData();
            _api.startThread(_storePlayerDataThread.DoWork);

            _updatePlayerPlayTimeThread = new UpdatePlayerPlayTime();
            _api.startThread(_updatePlayerPlayTimeThread.DoWork);

            _dynamicWeatherThread = new UpdateWeather();
            _api.startThread(_dynamicWeatherThread.DoWork);

            
            _api.exported.scoreboard.addScoreboardColumn("Level", "Level", 120);
            _api.exported.scoreboard.addScoreboardColumn("PlayTime", "PlayTime", 120);
            _api.exported.scoreboard.addScoreboardColumn("Nachname", "Nachname", 175);
            _api.exported.scoreboard.addScoreboardColumn("Vorname", "Vorname", 175);
            _api.exported.scoreboard.addScoreboardColumn("ID", "ID", 40);
           
        }
    }
}