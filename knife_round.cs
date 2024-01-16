using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System.Text;
using System.Diagnostics;

namespace Knife_Round;

public class KnifeRoundConfig : BasePluginConfig
{
    [JsonPropertyName("FreezeOnVote")] public bool FreezeOnVote { get; set; } = true;
    [JsonPropertyName("BlockTeamChangeOnVoteAndKnife")] public bool BlockTeamChangeOnVoteAndKnife { get; set; } = true;
    [JsonPropertyName("AllowAllTalkOnKnifeRound")] public bool AllowAllTalkOnKnifeRound { get; set; } = true;
    [JsonPropertyName("KnifeRoundTimer")] public float KnifeRoundTimer { get; set; } = 1;
    [JsonPropertyName("VoteTimer")] public float VoteTimer { get; set; } = 50; 
}

public class KnifeRound : BasePlugin, IPluginConfig<KnifeRoundConfig> 
{
    public override string ModuleName => "Knife Round";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "Creates An Additional Round With Knifes After Warmup";
    public KnifeRoundConfig Config { get; set; } = new KnifeRoundConfig();
    private Stopwatch stopwatch = new Stopwatch();
    public float mp_roundtime;
    public float mp_roundtime_defuse;
    public int mp_freezetime;
    public bool sv_alltalk;
    public bool sv_deadtalk;
    public bool sv_full_alltalk;
    public bool sv_talk_enemy_dead;
    public bool sv_talk_enemy_living;
    public int currentVotesT;
    public int currentVotesCT;
    public bool knifemode = false;
    public bool CTWINNER = false;
    public bool TWINNER = false;
    public bool BlockTeam = false;
    public bool onroundstart = false;
    public bool knifestarted = false;
    public float timer;
    public string targetPlayerName = "";
    private List<ulong> _rtvCountCT = new();
    private List<ulong> _rtvCountT = new();
    public void OnConfigParsed(KnifeRoundConfig config)
    {
        Config = config;
    }
    public override void Load(bool hotReload)
    {
        timer = Config.VoteTimer;
        RegisterListener<Listeners.OnTick>(OnTick);
        AddCommandListener("jointeam", OnCommandJoinTeam, HookMode.Pre);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        stopwatch.Start();
    }
    private HookResult OnCommandJoinTeam(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (Config.BlockTeamChangeOnVoteAndKnife && BlockTeam)
        {
            return HookResult.Handled;
        }
        return HookResult.Continue;
    }
    public void OnTick()
    {
        if(!knifemode)
        {
            if(TWINNER == true || CTWINNER == true)
            {
                var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
                var countct = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist);
                var countt = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist);
                if (timer > 0)
                {
                    if (stopwatch.ElapsedMilliseconds >= 1000)
                    {
                        timer--;
                        stopwatch.Restart();
                    }
                }
                foreach (var player in playerEntities)
                {
                    if (!player.IsValid) continue;
                    
                    if(TWINNER == true)
                    {
                        if(player.TeamNum == 3)
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.AppendFormat("<font color='green'>Waitng For</font> <font color='red'>T's</font> <font color='green'>To Vote</font>");
                            var centerhtml = builder.ToString();
                            player?.PrintToCenterHtml(centerhtml);
                        }else if(player.TeamNum == 2)
                        {
                            StringBuilder builder = new StringBuilder();
                            var required = (int)Math.Ceiling(countct * 0.6);
                            builder.AppendFormat("<font color='green'>Vote Which Side To Pick</font>");
                            builder.AppendLine("<br>");
                            builder.AppendFormat("<font color='red'>== Timer Left To Vote: {0} Secs =</font>", timer);
                            builder.AppendLine("<br>");
                            builder.AppendFormat("<font color='yellow'>!ct</font> <font color='grey'>To Go CT Side Team</font> ");
                            builder.AppendLine("<br>");
                            builder.AppendFormat("<font color='yellow'>!t</font> <font color='grey'>To Go T Side Team</font>");
                            builder.AppendLine("<br>");
                            builder.AppendFormat("<font color='grey'>Votes On</font> <img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196733677636440167/Ct-patch-small-ezgif.com-resize.png' class=''> <font color='green'>[{0}</font> <font color='grey'>/</font> <font color='green'>{1}]</font>", currentVotesCT, required);
                            builder.AppendLine("<br>");
                            builder.AppendLine("<br>");
                            builder.AppendFormat("<font color='grey'>Votes On</font> <img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196733591003070495/Icon-t-patch-small-ezgif.com-resize.png' class=''> <font color='green'>[{0}</font> <font color='grey'>/</font> <font color='green'>{1}]</font>", currentVotesT, required);
                            var centerhtml = builder.ToString();
                            player?.PrintToCenterHtml(centerhtml);
                        }
                    }else if(CTWINNER == true)
                    {
                        if(player.TeamNum == 3)
                        {
                            StringBuilder builder = new StringBuilder();
                            var required = (int)Math.Ceiling(countct * 0.6);
                            builder.AppendFormat("<font color='green'>Vote Which Side To Pick</font>");
                            builder.AppendLine("<br>");
                            builder.AppendFormat("<font color='red'>== Timer Left To Vote: {0} Secs =</font>", timer);
                            builder.AppendLine("<br>");
                            builder.AppendFormat("<font color='yellow'>!ct</font> <font color='grey'>To Go CT Side Team</font> ");
                            builder.AppendLine("<br>");
                            builder.AppendFormat("<font color='yellow'>!t</font> <font color='grey'>To Go T Side Team</font>");
                            builder.AppendLine("<br>");
                            builder.AppendFormat("<font color='grey'>Votes On</font> <img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196733677636440167/Ct-patch-small-ezgif.com-resize.png' class=''> <font color='green'>[{0}</font> <font color='grey'>/</font> <font color='green'>{1}]</font>", currentVotesCT, required);
                            builder.AppendLine("<br>");
                            builder.AppendLine("<br>");
                            builder.AppendFormat("<font color='grey'>Votes On</font> <img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196733591003070495/Icon-t-patch-small-ezgif.com-resize.png' class=''> <font color='green'>[{0}</font> <font color='grey'>/</font> <font color='green'>{1}]</font>", currentVotesT, required);
                            var centerhtml = builder.ToString();
                            player?.PrintToCenterHtml(centerhtml);
                            
                        }else if(player.TeamNum == 2)
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.AppendFormat("<font color='green'>Waitng For</font> <font color='blue'>CT's</font> <font color='green'>To Vote</font>");
                            var centerhtml = builder.ToString();
                            player?.PrintToCenterHtml(centerhtml);
                        }
                    }
                }
                if (timer < 1)
                {
                    if (TWINNER && currentVotesCT > currentVotesT)
                    {
                        foreach(var pl in Utilities.GetPlayers().FindAll(x => x.IsValid))
                        { 
                            if(pl.TeamNum == 3)
                            {
                                pl.SwitchTeam(CsTeam.Terrorist);
                            }else if(pl.TeamNum == 2)
                            {
                                pl.SwitchTeam(CsTeam.CounterTerrorist);
                            }
                        }

                        Server.NextFrame(() =>
                        {
                            _rtvCountT.Clear();
                            _rtvCountCT.Clear();
                            TWINNER = false;
                            CTWINNER = false;
                            BlockTeam = false;
                            AddTimer(2.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                if(Config.AllowAllTalkOnKnifeRound)
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                            AddTimer(4.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                            AddTimer(6.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                        });
                    }
                    if (CTWINNER && currentVotesCT > currentVotesT)
                    {
                        Server.NextFrame(() =>
                        {
                            _rtvCountT.Clear();
                            _rtvCountCT.Clear();
                            TWINNER = false;
                            CTWINNER = false;
                            BlockTeam = false;
                            AddTimer(2.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                if(Config.AllowAllTalkOnKnifeRound)
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                            AddTimer(4.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                if(Config.AllowAllTalkOnKnifeRound)
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                            AddTimer(6.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                if(Config.AllowAllTalkOnKnifeRound)
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                        });
                    }
                    if (CTWINNER && currentVotesT > currentVotesCT)
                    {
                        foreach(var pl in Utilities.GetPlayers().FindAll(x => x.IsValid))
                        { 
                            if(pl.TeamNum == 3)
                            {
                                pl.SwitchTeam(CsTeam.Terrorist);
                            }else if(pl.TeamNum == 2)
                            {
                                pl.SwitchTeam(CsTeam.CounterTerrorist);
                            }
                        }

                        Server.NextFrame(() =>
                        {
                            _rtvCountT.Clear();
                            _rtvCountCT.Clear();
                            TWINNER = false;
                            CTWINNER = false;
                            BlockTeam = false;
                            AddTimer(2.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                if(Config.AllowAllTalkOnKnifeRound)
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                            AddTimer(4.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                if(Config.AllowAllTalkOnKnifeRound)
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                            AddTimer(6.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                if(Config.AllowAllTalkOnKnifeRound)
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                        });
                    }
                    if (TWINNER && currentVotesT > currentVotesCT)
                    {
                        Server.NextFrame(() =>
                        {
                            _rtvCountT.Clear();
                            _rtvCountCT.Clear();
                            TWINNER = false;
                            CTWINNER = false;
                            BlockTeam = false;
                            AddTimer(2.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                if(Config.AllowAllTalkOnKnifeRound)
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                            AddTimer(4.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                if(Config.AllowAllTalkOnKnifeRound)
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                            AddTimer(6.0f, () =>
                            {
                                Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                                if(Config.AllowAllTalkOnKnifeRound)
                                {
                                    Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                                }
                            });
                        });
                    }
                    
                }
            }
        }else if(knifestarted == true)
            {
                var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
                

                foreach (var player in playerEntities)
                {
                    

                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("<img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196804184209633322/knife-ezgif.com-reverse.png' class=''> <font color='orange'> Knife Round </font> <img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196804174441103440/knife-ezgif.com-resize.png' class=''>");
                    builder.AppendLine("<br>");
                    builder.AppendLine("<br>");
                    builder.AppendLine("<br>");
                    builder.AppendFormat("<font color='purple'>Winner Will Choose Team Side</font>");
                    var centerhtml = builder.ToString();
                    player?.PrintToCenterHtml(centerhtml);
                }
            }
    }
    [GameEventHandler]
    public HookResult OnRoundPreStart(EventRoundPrestart @event, GameEventInfo info)
    {
        if(!onroundstart)
        {
            mp_roundtime = ConVar.Find("mp_roundtime")!.GetPrimitiveValue<float>();
            mp_roundtime_defuse = ConVar.Find("mp_roundtime_defuse")!.GetPrimitiveValue<float>();
            mp_freezetime = ConVar.Find("mp_freezetime")!.GetPrimitiveValue<int>();
            sv_alltalk = ConVar.Find("sv_alltalk")!.GetPrimitiveValue<bool>();
            sv_full_alltalk = ConVar.Find("sv_full_alltalk")!.GetPrimitiveValue<bool>();
            sv_talk_enemy_dead = ConVar.Find("sv_talk_enemy_dead")!.GetPrimitiveValue<bool>();
            sv_talk_enemy_living = ConVar.Find("sv_talk_enemy_living")!.GetPrimitiveValue<bool>();
            sv_deadtalk = ConVar.Find("sv_deadtalk")!.GetPrimitiveValue<bool>();

            knifemode = true;
            onroundstart = true;
            return HookResult.Handled;
        }
        
        if(knifemode)
        {
            BlockTeam = true;
            knifestarted = true;
            AddTimer(10.0f, () =>
            {
                knifestarted = false;
            });
        }else if(!knifemode)
        {
            Server.NextFrame(() =>
            {
                if(TWINNER == true || CTWINNER == true)
                {
                    if(Config.FreezeOnVote)
                    {
                        foreach (var p in Utilities.GetPlayers().FindAll(p => p is { IsValid: true, PawnIsAlive: true }))
                        {
                            if(p.PlayerPawn.Value != null && p.PlayerPawn.Value.IsValid){p.PlayerPawn.Value.MoveType = MoveType_t.MOVETYPE_NONE;}
                        }
                    }else
                    {
                        foreach (var p in Utilities.GetPlayers().FindAll(p => p is { IsValid: true, PawnIsAlive: true }))
                        {
                            if(p.PlayerPawn.Value != null && p.PlayerPawn.Value.IsValid){p.PlayerPawn.Value.MoveType = MoveType_t.MOVETYPE_WALK;}
                        }
                    }
                }
            });
        }
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(knifemode)
        {
            Server.NextFrame(() =>
            {
                Server.ExecuteCommand($"sv_buy_status_override 3; mp_roundtime {Config.KnifeRoundTimer}; mp_roundtime_defuse {Config.KnifeRoundTimer}; mp_give_player_c4 0");
                if(Config.AllowAllTalkOnKnifeRound)
                {
                    Server.ExecuteCommand($"sv_alltalk true; sv_deadtalk true; sv_full_alltalk true; sv_talk_enemy_dead true; sv_talk_enemy_living true;");
                }
                foreach (var p in Utilities.GetPlayers().Where(p => p is { IsValid: true, PawnIsAlive: true }))
                {
                    p.RemoveWeapons();
                    p.GiveNamedItem("weapon_knife");
                }
            });
        }
        return HookResult.Continue;
    }

    [GameEventHandler(HookMode.Post)]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (@event == null || !knifemode) return HookResult.Continue;

        var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");

        int countt = 0;
        int countct = 0;
        int thealth = 0;
        int cthealth = 0;

        foreach (var player in playerEntities)
        {
            if (player == null || !player.IsValid || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid)
                continue;

            int teamNum = player.TeamNum;
            var alive =  player.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE;
            countt = Utilities.GetPlayers().Count(p => player.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE && player.TeamNum == (int)CsTeam.Terrorist);
            countct = Utilities.GetPlayers().Count(p => player.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE && player.TeamNum == (int)CsTeam.CounterTerrorist);
            if (teamNum == 2 && alive)
                thealth = player.PlayerPawn.Value.Health;

            else if (teamNum == 3 && alive)
                cthealth = player.PlayerPawn.Value.Health;
        }

        if (countt > countct)
        {
            BlockTeam = true;
            TWINNER = true;
            knifemode = false;
        }
        else if (countt < countct)
        {
            BlockTeam = true;
            CTWINNER = true;
            knifemode = false;
        }
        else
        {
            if (cthealth > thealth)
            {
                BlockTeam = true;
                CTWINNER = true;
                knifemode = false;
            }
            else if (thealth > cthealth)
            {
                BlockTeam = true;
                TWINNER = true;
                knifemode = false;
            }
            else // thealth == cthealth
            {
                BlockTeam = true;
                CTWINNER = true;
                knifemode = false;
            }
        }
        return HookResult.Continue;
    }
    [ConsoleCommand("css_ct", "change to ct")]
    [CommandHelper(whoCanExecute:CommandUsage.CLIENT_ONLY)]
    public void ChangeToCTTeamCommand(CCSPlayerController? player, CommandInfo cmd)
    {
        if (player == null || !player.IsValid)return;
        
        if(TWINNER && player.TeamNum == 2)
        {
            targetPlayerName = player.PlayerName;
            if(!player.UserId.HasValue || string.IsNullOrEmpty(targetPlayerName))return;
            int voterUserId = player.UserId.Value;
            
            
            if (_rtvCountT.Contains(player!.SteamID))
            {
                _rtvCountT.Remove(player.SteamID);
                currentVotesT =  currentVotesT - 1;
            }

            if (_rtvCountCT.Contains(player!.SteamID))return;
            _rtvCountCT.Add(player.SteamID);
            
            
            var councT = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist);
            var required = (int)Math.Ceiling(councT * 0.6);
            currentVotesCT = _rtvCountCT.Count;

            if (currentVotesCT >= required)
            {
                foreach(var pl in Utilities.GetPlayers().FindAll(x => x.IsValid))
                { 
                    if(pl.TeamNum == 3)
                    {
                        pl.SwitchTeam(CsTeam.Terrorist);
                    }else if(pl.TeamNum == 2)
                    {
                        pl.SwitchTeam(CsTeam.CounterTerrorist);
                    }
                }

                Server.NextFrame(() =>
                {
                    _rtvCountT.Clear();
                    _rtvCountCT.Clear();
                    TWINNER = false;
                    CTWINNER = false;
                    BlockTeam = false;
                    AddTimer(2.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                    AddTimer(4.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                    AddTimer(6.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                });
            }
        }else if(CTWINNER && player.TeamNum == 3)
        {
            targetPlayerName = player.PlayerName;
            if(!player.UserId.HasValue || string.IsNullOrEmpty(targetPlayerName))return;
            int voterUserId = player.UserId.Value;

            if (_rtvCountT.Contains(player!.SteamID))
            {
                _rtvCountT.Remove(player.SteamID);
                currentVotesT =  currentVotesT - 1;
            }
            if (_rtvCountCT.Contains(player!.SteamID))return;
            _rtvCountCT.Add(player.SteamID);

            var councCT = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist);
            var required = (int)Math.Ceiling(councCT * 0.6);
            currentVotesCT = _rtvCountCT.Count;

            if (currentVotesCT >= required)
            {
                Server.NextFrame(() =>
                {
                    _rtvCountT.Clear();
                    _rtvCountCT.Clear();
                    TWINNER = false;
                    CTWINNER = false;
                    BlockTeam = false;
                    AddTimer(2.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                    AddTimer(4.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                    AddTimer(6.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                });
            }
        }
        
    }




    [ConsoleCommand("css_t", "change to t")]
    [CommandHelper(whoCanExecute:CommandUsage.CLIENT_ONLY)]
    public void ChangeToTTeamCommand(CCSPlayerController? player, CommandInfo cmd)
    {
        if (player == null || !player.IsValid)return;
        
        if(CTWINNER && player.TeamNum == 3)
        {
            targetPlayerName = player.PlayerName;
            if(!player.UserId.HasValue || string.IsNullOrEmpty(targetPlayerName))return;
            int voterUserId = player.UserId.Value;
            
            
            if (_rtvCountCT.Contains(player!.SteamID))
            {
                _rtvCountCT.Remove(player.SteamID);
                currentVotesCT =  currentVotesCT - 1;
            }

            if (_rtvCountT.Contains(player!.SteamID))return;
            _rtvCountT.Add(player.SteamID);
            
            var councCT = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist);
            var required = (int)Math.Ceiling(councCT * 0.6);
            currentVotesT = _rtvCountT.Count;

            if (currentVotesT >= required)
            {
                foreach(var pl in Utilities.GetPlayers().FindAll(x => x.IsValid))
                { 
                    if(pl.TeamNum == 3)
                    {
                        pl.SwitchTeam(CsTeam.Terrorist);
                    }else if(pl.TeamNum == 2)
                    {
                        pl.SwitchTeam(CsTeam.CounterTerrorist);
                    }
                }

                Server.NextFrame(() =>
                {
                    _rtvCountT.Clear();
                    _rtvCountCT.Clear();
                    TWINNER = false;
                    CTWINNER = false;
                    BlockTeam = false;
                    AddTimer(2.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                    AddTimer(4.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                    AddTimer(6.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                });
            }
            
        }else if(TWINNER && player.TeamNum == 2)
        {
            targetPlayerName = player.PlayerName;
            if(!player.UserId.HasValue || string.IsNullOrEmpty(targetPlayerName))return;
            int voterUserId = player.UserId.Value;
           
            if (_rtvCountCT.Contains(player!.SteamID))
            {
                _rtvCountCT.Remove(player.SteamID);
                currentVotesCT =  currentVotesCT - 1;
            }

            if (_rtvCountT.Contains(player!.SteamID))return;
            _rtvCountT.Add(player.SteamID);
            
            var councT = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist);
            var required = (int)Math.Ceiling(councT * 0.6);
            currentVotesT = _rtvCountT.Count;

            if (currentVotesT >= required)
            {
                Server.NextFrame(() =>
                {
                    _rtvCountT.Clear();
                    _rtvCountCT.Clear();
                    TWINNER = false;
                    CTWINNER = false;
                    BlockTeam = false;
                    AddTimer(2.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                    AddTimer(4.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                    AddTimer(6.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_buy_status_override -1; mp_freezetime {mp_freezetime}; mp_roundtime {mp_roundtime}; mp_roundtime_defuse {mp_roundtime_defuse}; mp_give_player_c4 1; mp_restartgame 1");
                        if(Config.AllowAllTalkOnKnifeRound)
                        {
                            Server.ExecuteCommand($"sv_alltalk {sv_alltalk}; sv_deadtalk {sv_deadtalk}; sv_full_alltalk {sv_full_alltalk}; sv_talk_enemy_dead {sv_talk_enemy_dead}; sv_talk_enemy_living {sv_talk_enemy_living};");
                        }
                    });
                });
            }
        }
    }
    public override void Unload(bool hotReload)
    {
        _rtvCountT.Clear();
        _rtvCountCT.Clear();
        knifemode = false;
        CTWINNER = false;
        TWINNER = false;
        BlockTeam = false;
        onroundstart = false;
        knifestarted = false;
        targetPlayerName = "";
        currentVotesT = 0;
        currentVotesCT = 0;
    }
    private void OnMapEnd()
    {
        _rtvCountT.Clear();
        _rtvCountCT.Clear();
        knifemode = false;
        CTWINNER = false;
        TWINNER = false;
        BlockTeam = false;
        onroundstart = false;
        knifestarted = false;
        targetPlayerName = "";
        currentVotesT = 0;
        currentVotesCT = 0;
    }
}