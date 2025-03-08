using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;

namespace Knife_Round_GoldKingZ.Config
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RangeAttribute : Attribute
    {
        public int Min { get; }
        public int Max { get; }
        public int Default { get; }
        public string Message { get; }

        public RangeAttribute(int min, int max, int defaultValue, string message)
        {
            Min = min;
            Max = max;
            Default = defaultValue;
            Message = message;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CommentAttribute : Attribute
    {
        public string Comment { get; }

        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class BreakLineAttribute : Attribute
    {
        public string BreakLine { get; }

        public BreakLineAttribute(string breakLine)
        {
            BreakLine = breakLine;
        }
    }

    public static class Configs
    {
        public static class Shared {
            public static string? CookiesModule { get; set; }
            public static IStringLocalizer? StringLocalizer { get; set; }
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
                _configData!.Validate();
            }
            else
            {
                _configData = new ConfigData();
                _configData.Validate();
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
                throw new Exception("Config not yet loaded.");

            string json = JsonSerializer.Serialize(configData, SerializationOptions);
            json = Regex.Unescape(json);

            var lines = json.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var newLines = new List<string>();

            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"^\s*""(\w+)""\s*:.*");
                bool isPropertyLine = false;
                PropertyInfo? propInfo = null;

                if (match.Success)
                {
                    string propName = match.Groups[1].Value;
                    propInfo = typeof(ConfigData).GetProperty(propName);

                    var breakLineAttr = propInfo?.GetCustomAttribute<BreakLineAttribute>();
                    if (breakLineAttr != null)
                    {
                        string breakLine = breakLineAttr.BreakLine;

                        if (breakLine.Contains("{space}"))
                        {
                            breakLine = breakLine.Replace("{space}", "").Trim();

                            if (breakLineAttr.BreakLine.StartsWith("{space}"))
                            {
                                newLines.Add("");
                            }

                            newLines.Add("// " + breakLine);
                            newLines.Add("");
                        }
                        else
                        {
                            newLines.Add("// " + breakLine);
                        }
                    }

                    var commentAttr = propInfo?.GetCustomAttribute<CommentAttribute>();
                    if (commentAttr != null)
                    {
                        var commentLines = commentAttr.Comment.Split('\n');
                        foreach (var commentLine in commentLines)
                        {
                            newLines.Add("// " + commentLine.Trim());
                        }
                    }

                    isPropertyLine = true;
                }
                newLines.Add(line);
                if (isPropertyLine && propInfo?.GetCustomAttribute<CommentAttribute>() != null)
                {
                    newLines.Add("");
                }
            }

            File.WriteAllText(_configFilePath, string.Join(Environment.NewLine, newLines), Encoding.UTF8);
        }

        public class ConfigData
        {
            private string? _Version;
            private string? _Link;
            [BreakLine("----------------------------[ ↓ Plugin Info ↓ ]----------------------------{space}")]
            public string Version
            {
                get => _Version!;
                set
                {
                    _Version = value;
                    if (_Version != KnifeRoundGoldKingZ.Instance.ModuleVersion)
                    {
                        Version = KnifeRoundGoldKingZ.Instance.ModuleVersion;
                    }
                }
            }

            public string Link
            {
                get => _Link!;
                set
                {
                    _Link = value;
                    if (_Link != "https://github.com/oqyh/cs2-Knife-Round-GoldKingZ")
                    {
                        Link = "https://github.com/oqyh/cs2-Knife-Round-GoldKingZ";
                    }
                }
            }

            [BreakLine("{space}----------------------------[ ↓ Main Config ↓ ]----------------------------{space}")]
            [Comment("Minimum Number Of Players Required To Enable The Knife Plugin")]
            public int Minimum_Players_Needed  { get; set; }
            [Comment("Count Bots As Players in [Minimum_Players_Needed]?\ntrue = Yes\nfalse = No")]
            public bool Count_Bots_As_Players { get; set; }
            [Comment("Allow Winner Team Knife Round To Choose Team Side?\ntrue = Yes\nfalse = No, Skip Voting And Apply [Send_Winner_Team_To_CT]")]
            public bool After_Winning_Vote_Team_Side { get; set; }
            [Comment("Required [After_Winning_Vote_Team_Side = false]\nSend Winner Team To CT?\ntrue = Yes\nfalse = No, Same Team")]
            public bool Send_Winner_Team_To_CT { get; set; }
            [Comment("Choose How Do You Like Pick Winner Team:\n1 = By Total Alive Players Each Team If Draw Choose CT Winner\n2 = By Total Alive Players Each Team If Draw Choose T Winner\n3 = By Total Healths Each Team If Draw Choose CT Winner\n4 = By Total Healths Each Team If Draw Choose T Winner\n5 = By Total Alive Players Each Team If Draw Check Total Healths Each Team If Draw CT Winner\n6 = By Total Alive Players Each Team If Draw Check Total Healths Each Team If Draw T Winner")]
            [Range(1, 6, 5, "[Knife Round] Get_Winner_Team: is invalid, setting to default value (5) Please Choose From 1 To 6.\n[Knife Round] 1 = By Total Alive Players Each Team If Draw Choose CT Winner\n[Knife Round] 2 = By Total Alive Players Each Team If Draw Choose T Winner\n[Knife Round] 3 = By Total Healths Each Team If Draw Choose CT Winner\n[Knife Round] 4 = By Total Healths Each Team If Draw Choose T Winner\n[Knife Round] 5 = By Total Alive Players Each Team If Draw Check Total Healths Each Team If Draw CT Winner\n[Knife Round] 6 = By Total Alive Players Each Team If Draw Check Total Healths Each Team If Draw T Winner")]
            public int Get_Winner_Team  { get; set; }
            [Comment("Required [After_Winning_Vote_Team_Side = true]\nFreeze All Players When Voting Choose Side Started?\ntrue = Yes\nfalse = No")]
            public bool Freeze_Players_On_Vote_Started { get; set; }
            [Comment("Block Switch Teams On:\n0 = Disabled\n1 = Only Knife Round Only\n2 = Only Knife Round + Voting Team Side\n3 = Only Knife Round + Voting Team Side + Match Live")]
            [Range(0, 3, 2, "[Knife Round] Block_Switching_Teams_On: is invalid, setting to default value (2) Please Choose From 0 To 3.\n[Knife Round] 0 = Disabled\n[Knife Round] 1 = Only Knife Round Only\n[Knife Round] 1 = Only Knife Round + Voting Team Side\n[Knife Round] 1 = Only Knife Round + Voting Team Side + Match Live")]
            public int Block_Switching_Teams_On { get; set; }
            [Comment("Allow All To Talk On Knife Round Started?\ntrue = Yes\nfalse = No")]
            public bool Allow_All_Talk_On_Knife_Round { get; set; }
            [Comment("Give Armor On Knife Round?\n0 = No Armor\n1 = Vest\n2 = Vest And Helmet")]
            [Range(0, 2, 2, "[Knife Round] Give_Armor_On_Knife_Round: is invalid, setting to default value (2) Please Choose From 0 To 2.\n[Knife Round] 0 = No Armor\n[Knife Round] 1 = Vest\n[Knife Round] 1 = Vest And Helmet 1")]
            public int Give_Armor_On_Knife_Round { get; set; }
            [Comment("Knife Round Time (In Mins)")]
            public float Knife_Round_Time { get; set; }
            [Comment("Required [After_Winning_Vote_Team_Side = true]\nVoting Choose Side Time (In Secs)")]
            public int Vote_Team_Side_Time { get; set; }
            [Comment("In [Knife-Round-GoldKingZ/lang/en.json] (hud.message.kniferoundstarted)\nKnife Round Start Center Message Time (In Secs)")]
            public int Knife_Round_Center_Message_Time  { get; set; }
            [Comment("Before Match Live Start How Many Times Would You Like To Restart")]
            public int Restart_On_Match_Live  { get; set; }
            [Comment("Required [After_Winning_Vote_Team_Side = true]\nCommands In Game To Vote To CT Side")]
            public string Commands_In_Game_To_Vote_CT { get; set; }
            [Comment("Required [After_Winning_Vote_Team_Side = true]\nCommands In Game To Vote To T Side")]
            public string Commands_In_Game_To_Vote_T { get; set; }
            [Comment("Note: Must Not Start With cfg/\nCustom Cfg On Warmup Started (Located in csgo/cfg/)")]
            public string Execute_Cfg_On_Warmup { get; set; }
            [Comment("Note: Must Not Start With cfg/\nCustom Cfg On Knife Round Started (Located in csgo/cfg/)")]
            public string Execute_Cfg_On_KnifeRound { get; set; }
            [Comment("Note: Must Not Start With cfg/\nCustom Cfg On Voting Choose Side Started (Located in csgo/cfg/)")]
            public string Execute_Cfg_On_Voting { get; set; }
            [Comment("Note: Must Not Start With cfg/\nCustom Cfg On Match Live Started (Located in csgo/cfg/)")]
            public string Execute_Cfg_On_MatchLive { get; set; }
            [Comment("Enable Debug Plugin In Server Console (Helps You To Debug Issues You Facing)?\ntrue = Yes\nfalse = No")]
            [BreakLine("{space}----------------------------[ ↓ Utilities  ↓ ]----------------------------{space}")]
            public bool EnableDebug { get; set; }

            public ConfigData()
            {
                Version = KnifeRoundGoldKingZ.Instance.ModuleVersion;
                Link = "https://github.com/oqyh/cs2-Knife-Round-GoldKingZ";
                Minimum_Players_Needed = 6;
                Count_Bots_As_Players = false;
                After_Winning_Vote_Team_Side = true;
                Send_Winner_Team_To_CT = true;
                Get_Winner_Team = 5;
                Freeze_Players_On_Vote_Started = true;
                Block_Switching_Teams_On = 2;
                Allow_All_Talk_On_Knife_Round = true;
                Give_Armor_On_Knife_Round = 2;
                Knife_Round_Time  = 1;
                Vote_Team_Side_Time = 30;
                Knife_Round_Center_Message_Time = 7;
                Restart_On_Match_Live = 3;
                Commands_In_Game_To_Vote_CT = "!ct,.ct,ct";
                Commands_In_Game_To_Vote_T = "!t,.t,t";
                Execute_Cfg_On_Warmup = "Knife-Round-GoldKingZ/OnWarmup.cfg";
                Execute_Cfg_On_KnifeRound = "Knife-Round-GoldKingZ/OnKnifeRound.cfg";
                Execute_Cfg_On_Voting = "Knife-Round-GoldKingZ/OnVoting.cfg";
                Execute_Cfg_On_MatchLive = "Knife-Round-GoldKingZ/OnMatchLive.cfg";
                EnableDebug = false;
            }

            public void Validate()
            {
                foreach (var prop in GetType().GetProperties())
                {
                    var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
                    if (rangeAttr != null && prop.PropertyType == typeof(int))
                    {
                        int value = (int)prop.GetValue(this)!;
                        if (value < rangeAttr.Min || value > rangeAttr.Max)
                        {
                            prop.SetValue(this, rangeAttr.Default);
                            Helper.DebugMessage(rangeAttr.Message,false);
                        }
                    }
                }
            }
        }
    }
}