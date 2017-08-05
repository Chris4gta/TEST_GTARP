using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraTex_RL_RPG.Lib.Helper;
using System.Threading;

namespace TerraTex_RL_RPG.Lib.Threads
{
    public class UpdateWeather
    {
        private bool _interuped = false;

        public void DoWork()
        {
            TTRPG.Api.consoleOutput("Started DynamicWeather Thread");
            while (!_interuped)
            {
                var _loadedDie = new LoadedDie(new int[] {
                    155, //Extra Sunny
                    155, //Clear
                    155, //Clouds
                    45, //Smog
                    45, //Foggy
                    45, //Overcast
                    45, //Rain
                    45, //Thunder
                    45, //Light rain
                    45, //Smoggy light rain
                    45, //Very light Snow
                    45, //Windy light Snow
                    45, //Light snow
                    45 //Unknown (No Effect)
                });

                int _weatherID = _loadedDie.NextValue();
                string weather = "";
                switch (_weatherID)
                {
                    case 0: weather = "Extra Sunny"; break;
                    case 1: weather = "Clear"; break;
                    case 2: weather = "Clouds"; break;
                    case 3: weather = "Smog"; break;
                    case 4: weather = "Foggy"; break;
                    case 5: weather = "Overcast"; break;
                    case 6: weather = "Rain"; break;
                    case 7: weather = "Thunder"; break;
                    case 8: weather = "Light rain"; break;
                    case 9: weather = "Smoggy light rain"; break;
                    case 10: weather = "Very light Snow"; break;
                    case 11: weather = "Windy light Snow"; break;
                    case 12: weather = "Light snow"; break;
                    case 13: weather = "Unknown (No Effect)"; break;
                }

                TTRPG.Api.consoleOutput("[DynamicWeather] The Weather has changed to " + weather);
                TTRPG.Api.setWeather(_weatherID);

                Thread.Sleep(1200000);
            }

        }

        public void StopThread()
        {
            _interuped = true;
        }
    }
}
