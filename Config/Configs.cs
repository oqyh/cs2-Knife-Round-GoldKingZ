using System.Text.Json;
using System.Text.Json.Serialization;

namespace Knife_Round_GoldKingZ.Config
{
    public static class Configs
    {
        public static class Shared {
            public static string? CookiesModule { get; set; }
        }
        
        private static readonly string ConfigDirectoryName = "config";
        private static readonly string ConfigFileName = "config.json";
        private static string? _configFilePath;
        private static ConfigData? _configData;

        private static readonly JsonSerializerOptions SerializationOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            },
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static bool IsLoaded()
        {
            return _configData is not null;
        }

        public static ConfigData GetConfigData()
        {
            if (_configData is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            
            return _configData;
        }

        public static ConfigData Load(string modulePath)
        {
            var configFileDirectory = Path.Combine(modulePath, ConfigDirectoryName);
            if(!Directory.Exists(configFileDirectory))
            {
                Directory.CreateDirectory(configFileDirectory);
            }

            _configFilePath = Path.Combine(configFileDirectory, ConfigFileName);
            if (File.Exists(_configFilePath))
            {
                _configData = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(_configFilePath), SerializationOptions);
            }
            else
            {
                _configData = new ConfigData();
            }

            if (_configData is null)
            {
                throw new Exception("Failed to load configs.");
            }

            SaveConfigData(_configData);
            
            return _configData;
        }

        private static void SaveConfigData(ConfigData configData)
        {
            if (_configFilePath is null)
            {
                throw new Exception("Config not yet loaded.");
            }
            string json = JsonSerializer.Serialize(configData, SerializationOptions);


            File.WriteAllText(_configFilePath, json);
        }

        public class ConfigData
        {
            public bool EnableVoteTeamSideAfterWinning { get; set; }
            public bool FreezePlayersOnVoteStarted { get; set; }
            public bool BlockTeamChangeOnVotingAndKnifeRound { get; set; }
            public bool AllowAllTalkOnKnifeRound { get; set; }
            private int _GiveArmorOnKnifeRound;
            public int GiveArmorOnKnifeRound
            {
                get => _GiveArmorOnKnifeRound;
                set
                {
                    _GiveArmorOnKnifeRound = value;
                    if (_GiveArmorOnKnifeRound < 0 || _GiveArmorOnKnifeRound > 2)
                    {
                        GiveArmorOnKnifeRound = 2;
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                        Console.WriteLine("[Knife-Round-GoldKingZ] GiveArmorOnKnifeRound: is invalid, setting to default value (2) Please Choose 0 or 1 or 2.");
                        Console.WriteLine("[Knife-Round-GoldKingZ] GiveArmorOnKnifeRound (0) = No Armor");
                        Console.WriteLine("[Knife-Round-GoldKingZ] GiveArmorOnKnifeRound (1) = Give Armor Without Helmet");
                        Console.WriteLine("[Knife-Round-GoldKingZ] GiveArmorOnKnifeRound (2) = Give Armor With Helmet");
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||| I N V A L I D ||||||||||||||||||||||||||||||||||||||||||||||||");
                    }
                }
            }
            public float KnifeRoundXTimeInMins { get; set; }
            public float VoteXTimeInSecs { get; set; }
            public float GiveHUDMessageOnStartKnifeRoundForXSecs  { get; set; }
            public int AfterWinningRestartXTimes  { get; set; }
            public string CommandsInGameToVoteCT { get; set; }
            public string CommandsInGameToVoteT { get; set; }
            public string empty { get; set; }
            public string Information_For_You_Dont_Delete_it { get; set; }
            
            public ConfigData()
            {
                EnableVoteTeamSideAfterWinning = true;
                FreezePlayersOnVoteStarted = true;
                BlockTeamChangeOnVotingAndKnifeRound = true;
                AllowAllTalkOnKnifeRound = true;
                GiveArmorOnKnifeRound = 2;
                KnifeRoundXTimeInMins  = 1;
                VoteXTimeInSecs = 50;
                GiveHUDMessageOnStartKnifeRoundForXSecs = 15;
                AfterWinningRestartXTimes = 3;
                CommandsInGameToVoteCT = "!ct,.ct,ct";
                CommandsInGameToVoteT = "!t,.t,t";
                empty = "-----------------------------------------------------------------------------------";
                Information_For_You_Dont_Delete_it = " Vist  [https://github.com/oqyh/cs2-Knife-Round-GoldKingZ/tree/main?tab=readme-ov-file#-configuration-] To Understand All Above";
            }
        }
    }
}