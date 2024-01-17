using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Localization;

namespace Knife_Round;

public class KnifeRoundConfig : BasePluginConfig
{
    [JsonPropertyName("FreezeOnVote")] public bool FreezeOnVote { get; set; } = true;
    [JsonPropertyName("BlockTeamChangeOnVoteAndKnife")] public bool BlockTeamChangeOnVoteAndKnife { get; set; } = true;
    [JsonPropertyName("AllowAllTalkOnKnifeRound")] public bool AllowAllTalkOnKnifeRound { get; set; } = true;
    [JsonPropertyName("KnifeRoundTimer")] public float KnifeRoundTimer { get; set; } = 1;
    [JsonPropertyName("VoteTimer")] public float VoteTimer { get; set; } = 50;
    [JsonPropertyName("MessageKnifeStartTimer")] public float MessageKnifeStartTimer { get; set; } = 25; 
}

public class KnifeRound : BasePlugin, IPluginConfig<KnifeRoundConfig> 
{
    public override string ModuleName => "Knife Round";
    public override string ModuleVersion => "1.0.2";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "Creates An Additional Round With Knifes After Warmup";
    public KnifeRoundConfig Config { get; set; } = new KnifeRoundConfig();
    internal static IStringLocalizer? Stringlocalizer;
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
        Stringlocalizer = Localizer;
    }
    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnTick>(OnTick);
        AddCommandListener("jointeam", OnCommandJoinTeam, HookMode.Pre);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
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
                var countct = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist && !p.IsHLTV);
                var countt = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist && !p.IsHLTV);
                string Close = "</font>";
                string Red = "<font color='Red'>";
                string Cyan = "<font color='cyan'>";
                string Blue = "<font color='blue'>";
                string DarkBlue = "<font color='darkblue'>";
                string LightBlue = "<font color='lightblue'>";
                string Purple = "<font color='purple'>";
                string Yellow = "<font color='yellow'>";
                string Lime = "<font color='lime'>";
                string Magenta = "<font color='magenta'>";
                string Pink = "<font color='pink'>";
                string Grey = "<font color='grey'>";
                string Green = "<font color='green'>";
                string Orange = "<font color='orange'>";
                string NextLine = "<br>";
                string CTIMAGE = "<img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196733677636440167/Ct-patch-small-ezgif.com-resize.png' class=''>";
                string TIMAGE = "<img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196733591003070495/Icon-t-patch-small-ezgif.com-resize.png' class=''>";
                string KnifeLeft = "<img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196733591003070495/Icon-t-patch-small-ezgif.com-resize.png' class=''>";
                string KnifeRight = "<img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196733591003070495/Icon-t-patch-small-ezgif.com-resize.png' class=''>";
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
                            var required = (int)Math.Ceiling(countt * 0.6);

                            builder.AppendFormat(Localizer["When_CT_Lose"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange,NextLine,CTIMAGE, TIMAGE, currentVotesCT, currentVotesT, required,timer, KnifeLeft, KnifeRight);
                            var centerhtml = builder.ToString();
                            player?.PrintToCenterHtml(centerhtml);
                        }else if(player.TeamNum == 2)
                        {
                            StringBuilder builder = new StringBuilder();
                            var required = (int)Math.Ceiling(countt * 0.6);

                            builder.AppendFormat(Localizer["Winner_Message"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange,NextLine,CTIMAGE, TIMAGE, currentVotesCT, currentVotesT, required,timer, KnifeLeft, KnifeRight);
                            var centerhtml = builder.ToString();
                            player?.PrintToCenterHtml(centerhtml);
                        }
                    }else if(CTWINNER == true)
                    {
                        if(player.TeamNum == 3)
                        {
                            StringBuilder builder = new StringBuilder();
                            var required = (int)Math.Ceiling(countct * 0.6);

                            builder.AppendFormat(Localizer["Winner_Message"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange,NextLine,CTIMAGE, TIMAGE, currentVotesCT, currentVotesT, required,timer, KnifeLeft, KnifeRight);
                            var centerhtml = builder.ToString();
                            player?.PrintToCenterHtml(centerhtml);
                            
                        }else if(player.TeamNum == 2)
                        {
                            StringBuilder builder = new StringBuilder();
                            var required = (int)Math.Ceiling(countct * 0.6);

                            builder.AppendFormat(Localizer["When_T_Lose"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange,NextLine,CTIMAGE, TIMAGE, currentVotesCT, currentVotesT, required,timer, KnifeLeft, KnifeRight);
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
                    if (TWINNER && currentVotesT == currentVotesCT || CTWINNER && currentVotesT == currentVotesCT)
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
                
                string Close = "</font>";
                string Red = "<font color='Red'>";
                string Cyan = "<font color='cyan'>";
                string Blue = "<font color='blue'>";
                string DarkBlue = "<font color='darkblue'>";
                string LightBlue = "<font color='lightblue'>";
                string Purple = "<font color='purple'>";
                string Yellow = "<font color='yellow'>";
                string Lime = "<font color='lime'>";
                string Magenta = "<font color='magenta'>";
                string Pink = "<font color='pink'>";
                string Grey = "<font color='grey'>";
                string Green = "<font color='green'>";
                string Orange = "<font color='orange'>";
                string NextLine = "<br>";
                string CTIMAGE = "<img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196733677636440167/Ct-patch-small-ezgif.com-resize.png' class=''>";
                string TIMAGE = "<img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196733591003070495/Icon-t-patch-small-ezgif.com-resize.png' class=''>";
                string KnifeLeft = "<img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196804184209633322/knife-ezgif.com-reverse.png' class=''>";
                string KnifeRight = "<img src='https://cdn.discordapp.com/attachments/1175717468724015144/1196804174441103440/knife-ezgif.com-resize.png' class=''>";

                foreach (var player in playerEntities)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer["Knife_Start_Message"], Close,Red,Cyan,Blue,DarkBlue,LightBlue,Purple,Yellow,Lime,Magenta,Pink,Grey,Green,Orange,NextLine,CTIMAGE, TIMAGE, string.Empty, string.Empty, string.Empty,string.Empty, KnifeLeft, KnifeRight);
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
            AddTimer(Config.MessageKnifeStartTimer, () =>
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
                AddTimer(0.1f, () =>
                {
                    foreach (var p in Utilities.GetPlayers().Where(p => p is { IsValid: true, PawnIsAlive: true }))
                    {
                        p.RemoveWeapons();
                        p.GiveNamedItem("weapon_knife");
                    }
                });
            });
        }
        return HookResult.Continue;
    }

    [GameEventHandler(HookMode.Post)]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (@event == null || !knifemode) return HookResult.Continue;

        var playerEntities = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");
        
        stopwatch.Start();
        timer = Config.VoteTimer;
        int countt = 0;
        int countct = 0;

        foreach (var player in playerEntities)
        {
            if (player == null || !player.IsValid || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid)
                continue;

            if (player.TeamNum == (int)CsTeam.Terrorist && player.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE)
            {
                countt++;
            }
        }

        foreach (var players in playerEntities)
        {
            if (players == null || !players.IsValid || players.PlayerPawn == null || !players.PlayerPawn.IsValid || players.PlayerPawn.Value == null || !players.PlayerPawn.Value.IsValid)
                continue;

            if (players.TeamNum == (int)CsTeam.CounterTerrorist && players.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE)
            {
                countct++;
            }
        }

        if (countt > countct)
        {
            BlockTeam = true;
            TWINNER = true;
            knifemode = false;
        }
        else if (countct > countt)
        {
            BlockTeam = true;
            CTWINNER = true;
            knifemode = false;
        }
        else
        {
            BlockTeam = true;
            CTWINNER = true;
            knifemode = false;
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
            
            
            var councT = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist && !p.IsHLTV);
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

            var councCT = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist && !p.IsHLTV);
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
            
            var councCT = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist && !p.IsHLTV);
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
            
            var councT = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist && !p.IsHLTV);
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