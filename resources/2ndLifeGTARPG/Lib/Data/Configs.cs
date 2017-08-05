using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using GrandTheftMultiplayer.Shared;

namespace TerraTex_RL_RPG.Lib.Data
{
    public class Configs
    {
        private readonly Dictionary<string, XmlDocument> _xmlConfigs = new Dictionary<string, XmlDocument>();

        public Configs()
        {
            string[] fileEntries = Directory.GetFiles(TTRPG.Api.getResourceFolder() + "/Configs");
            foreach (string fileEntry in fileEntries)
            {
                if (!fileEntry.Contains("_example"))
                {
                    string configName = Path.GetFileNameWithoutExtension(fileEntry);
                    XmlDocument config = new XmlDocument();
                    config.Load(fileEntry);

                    _xmlConfigs.Add(configName, config);

                    TTRPG.Api.consoleOutput("Loaded Configuration: " + configName);
                }
            }

        }

        public XmlDocument GetConfig(string configName)
        {
            return _xmlConfigs.Get(configName);
        }

        public Dictionary<string, XmlDocument> GetConfigs()
        {
            return _xmlConfigs;
        }

        public Boolean ConfigExists(string configName)
        {
            return _xmlConfigs.ContainsKey(configName);
        }
    }
}