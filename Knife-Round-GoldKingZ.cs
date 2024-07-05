using System.Text;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Memory;
using Microsoft.Extensions.Localization;
using Knife_Round_GoldKingZ.Config;

namespace Knife_Round_GoldKingZ;

[MinimumApiVersion(164)]
public class KnifeRoundGoldKingZ : BasePlugin
{
    public override string ModuleName => "Knife Round (Creates An Additional Round With Knifes After Warmup)";
    public override string ModuleVersion => "1.1.1";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
	internal static IStringLocalizer? Stringlocalizer;
	

    public override void Load(bool hotReload)
    {
        Configs.Load(ModuleDirectory);
        Stringlocalizer = Localizer;
        Configs.Shared.CookiesModule = ModuleDirectory;

        AddCommandListener("jointeam", OnCommandJoinTeam, HookMode.Pre);

        RegisterEventHandler<EventPlayerChat>(OnEventPlayerChat, HookMode.Post);
        RegisterEventHandler<EventPlayerSpawn>(OnEventPlayerSpawn, HookMode.Post);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventRoundPrestart>(OnEventRoundPrestart);
        RegisterEventHandler<EventRoundEnd>(OnEventRoundEnd);

        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
    }

    public void OnTick()
    {
        if(!Globals.DisableKnife && !Globals.PrepareKnifeRound)
        {
            if(Globals.KnifeRoundStarted && Globals.KnifeModeStartMessage && !string.IsNullOrEmpty(Localizer["hud.message.kniferoundstarted"]))
            {
                foreach (var allplayers in Helper.GetAllController())
                {
                    if (allplayers == null || !allplayers.IsValid || allplayers.IsBot || allplayers.IsHLTV) continue;

                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Localizer["hud.message.kniferoundstarted"]);
                    var centerhtml = builder.ToString();
                    allplayers.PrintToCenterHtml(centerhtml);
                }
            }else if(Configs.GetConfigData().EnableVoteTeamSideAfterWinning && !Globals.KnifeRoundStarted)
            {
                if(Globals.TWINNER == true || Globals.CTWINNER == true)
                {
                    if (Globals.Timer > 0)
                    {
                        if (Globals.Stopwatch.ElapsedMilliseconds >= 1000)
                        {
                            Globals.Timer--;
                            Globals.Stopwatch.Restart();
                        }

                        foreach (var allplayers in Helper.GetAllController())
                        {
                            if (allplayers == null || !allplayers.IsValid || allplayers.IsBot || allplayers.IsHLTV) continue;
                            
                            if(Globals.TWINNER)
                            {
                                if(allplayers.TeamNum == (byte)CsTeam.CounterTerrorist && !string.IsNullOrEmpty(Localizer["hud.message.loseteam.ct"]))
                                {
                                    StringBuilder builder = new StringBuilder();
                                    var countt = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist && !p.IsBot && !p.IsHLTV);
                                    var required = (int)Math.Ceiling(countt * 0.6);
                                    builder.AppendFormat(Localizer["hud.message.loseteam.ct", Globals.Timer, Globals.currentVotesCT, Globals.currentVotesT, required]);
                                    var centerhtml = builder.ToString();
                                    allplayers.PrintToCenterHtml(centerhtml);
                                }else if(allplayers.TeamNum == (byte)CsTeam.Terrorist && !string.IsNullOrEmpty(Localizer["hud.message.winnerteam"]))
                                {
                                    StringBuilder builder = new StringBuilder();
                                    var countt = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist && !p.IsBot && !p.IsHLTV);
                                    var required = (int)Math.Ceiling(countt * 0.6);
                                    builder.AppendFormat(Localizer["hud.message.winnerteam", Globals.Timer, Globals.currentVotesCT, Globals.currentVotesT, required]);
                                    var centerhtml = builder.ToString();
                                    allplayers.PrintToCenterHtml(centerhtml);
                                }
                            }else if(Globals.CTWINNER)
                            {
                                if(allplayers.TeamNum == (byte)CsTeam.Terrorist && !string.IsNullOrEmpty(Localizer["hud.message.loseteam.t"]))
                                {
                                    StringBuilder builder = new StringBuilder();
                                    var counct = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist && !p.IsBot && !p.IsHLTV);
                                    var required = (int)Math.Ceiling(counct * 0.6);
                                    builder.AppendFormat(Localizer["hud.message.loseteam.t", Globals.Timer, Globals.currentVotesCT, Globals.currentVotesT, required]);
                                    var centerhtml = builder.ToString();
                                    allplayers.PrintToCenterHtml(centerhtml);
                                }else if(allplayers.TeamNum == (byte)CsTeam.CounterTerrorist && !string.IsNullOrEmpty(Localizer["hud.message.winnerteam"]))
                                {
                                    StringBuilder builder = new StringBuilder();
                                    var countt = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist && !p.IsBot && !p.IsHLTV);
                                    var required = (int)Math.Ceiling(countt * 0.6);
                                    builder.AppendFormat(Localizer["hud.message.winnerteam", Globals.Timer, Globals.currentVotesCT, Globals.currentVotesT, required]);
                                    var centerhtml = builder.ToString();
                                    allplayers.PrintToCenterHtml(centerhtml);
                                }
                            }
                        }
                    }

                    if (Globals.Timer < 1)
                    {
                        if ((Globals.TWINNER && Globals.currentVotesCT > Globals.currentVotesT) || (Globals.CTWINNER && Globals.currentVotesT > Globals.currentVotesCT))
                        {
                            Helper.SwitchTeams(Globals.TWINNER ? CsTeam.CounterTerrorist : CsTeam.Terrorist);
                            ResetValuesAndExecuteCommands(); 
                        }

                        if ((Globals.TWINNER && Globals.currentVotesT > Globals.currentVotesCT) || (Globals.CTWINNER && Globals.currentVotesCT > Globals.currentVotesT))
                        {
                            ResetValuesAndExecuteCommands();
                        }

                        if ((Globals.TWINNER || Globals.CTWINNER) && Globals.currentVotesT == Globals.currentVotesCT)
                        {
                            ResetValuesAndExecuteCommands();
                        }

                        Globals.Stopwatch.Stop();
                        Globals.Timer = 0;
                    }
                }
            }
        }
    }

    public HookResult OnEventRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if(Globals.DisableKnife || @event == null)return HookResult.Continue;

        if(Globals.KnifeRoundStarted)
        {
            Globals.Stopwatch.Start();
            Globals.Timer = Configs.GetConfigData().VoteXTimeInSecs;

            int countt = 0;
            int countct = 0;

            foreach (var ctplayers in Helper.GetCounterTerroristController())
            {
                if(ctplayers == null || !ctplayers.IsValid || ctplayers.PlayerPawn == null || !ctplayers.PlayerPawn.IsValid || ctplayers.PlayerPawn.Value == null || !ctplayers.PlayerPawn.Value.IsValid)continue;

                if (ctplayers.TeamNum == (int)CsTeam.CounterTerrorist && ctplayers.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE)
                {
                    countct++;
                }
            }

            foreach (var tplayers in Helper.GetTerroristController())
            {
                if(tplayers == null || !tplayers.IsValid || tplayers.PlayerPawn == null || !tplayers.PlayerPawn.IsValid || tplayers.PlayerPawn.Value == null || !tplayers.PlayerPawn.Value.IsValid)continue;

                if (tplayers.TeamNum == (int)CsTeam.Terrorist && tplayers.PlayerPawn.Value.LifeState == (byte)LifeState_t.LIFE_ALIVE)
                {
                    countt++;
                }
            }

            if (countt > countct)
            {
                Globals.BlockTeam = true;
                Globals.TWINNER = true;
                Globals.KnifeRoundStarted = false;
            }
            else if (countct > countt)
            {
                Globals.BlockTeam = true;
                Globals.CTWINNER = true;
                Globals.KnifeRoundStarted = false;
            }
            else
            {
                Globals.BlockTeam = true;
                Globals.CTWINNER = true;
                Globals.KnifeRoundStarted = false;
            }

            if(!Configs.GetConfigData().EnableVoteTeamSideAfterWinning)
            {
                ResetValuesAndExecuteCommands();
            }
        }
        return HookResult.Continue;
    }

    public HookResult OnEventPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if(Globals.DisableKnife || @event == null)return HookResult.Continue;

        var player = @event.Userid;
        if (player == null || !player.IsValid)return HookResult.Continue;

        if(Globals.RemoveWeapons)
        {
            Server.NextFrame(() =>
            {
                AddTimer(0.1f, () =>
                {
                    Helper.DropAllWeaponsAndDelete(player);

                    if(Configs.GetConfigData().GiveArmorOnKnifeRound == 1)
                    {
                        player.GiveNamedItem("item_kevlar");
                    }else if(Configs.GetConfigData().GiveArmorOnKnifeRound == 2)
                    {
                        player.GiveNamedItem("item_assaultsuit");
                    }
                }, TimerFlags.STOP_ON_MAPCHANGE);
                
            });
        }
        
        if(Configs.GetConfigData().FreezePlayersOnVoteStarted)
        {
            if(Globals.TWINNER == true || Globals.CTWINNER == true)
            {
                if(player.PlayerPawn != null && player.PlayerPawn.IsValid && player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid)
                {
                    Server.NextFrame(() =>
                    {
                        Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 0);
                        Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                    });
                }
                
            }else
            {
                if(player.PlayerPawn != null && player.PlayerPawn.IsValid && player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid)
                {
                    Server.NextFrame(() =>
                    {
                        Schema.SetSchemaValue(player.PlayerPawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 2);
                        Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
                    });
                }
            }
        }
        return HookResult.Continue;
    }
    public HookResult OnEventRoundPrestart(EventRoundPrestart @event, GameEventInfo info)
    {
        if(Globals.DisableKnife ||@event == null)return HookResult.Continue;
        
        if(Globals.PrepareKnifeRound)
        {
            if(Configs.GetConfigData().CountBotsAsPlayers && Helper.GetPlayersCount(true) < Configs.GetConfigData().MinimumPlayersToEnableKnifePlugin || !Configs.GetConfigData().CountBotsAsPlayers && Helper.GetPlayersCount(false) < Configs.GetConfigData().MinimumPlayersToEnableKnifePlugin)
            {
                string roundtime = Globals.mp_roundtime.ToString().Replace(',', '.');
                string roundtimeDefuse = Globals.mp_roundtime_defuse.ToString().Replace(',', '.');
                string teamIntroTime = Globals.mp_team_intro_time.ToString().Replace(',', '.');
                Server.ExecuteCommand($"mp_team_intro_time {teamIntroTime}; sv_buy_status_override -1; mp_roundtime {roundtime}; mp_roundtime_defuse {roundtimeDefuse}; mp_give_player_c4 1");
                if (Configs.GetConfigData().AllowAllTalkOnKnifeRound)
                {
                    Server.ExecuteCommand($"sv_alltalk {Globals.sv_alltalk}; sv_deadtalk {Globals.sv_deadtalk}; sv_full_alltalk {Globals.sv_full_alltalk}; sv_talk_enemy_dead {Globals.sv_talk_enemy_dead}; sv_talk_enemy_living {Globals.sv_talk_enemy_living}");
                }

                Server.ExecuteCommand($"mp_restartgame 3");
                if (!string.IsNullOrEmpty(Localizer["chat.message.knife.ignored"]))
                {
                    Helper.AdvancedPrintToServer(Localizer["chat.message.knife.ignored"], Helper.GetPlayersNeeded(), Configs.GetConfigData().MinimumPlayersToEnableKnifePlugin);
                }

                Globals.DisableKnife = true;
                return HookResult.Continue;
            }
            Globals.RemoveWeapons = true;
        }
        return HookResult.Continue;
    }
    
    
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(Globals.DisableKnife || @event == null)return HookResult.Continue;

        if(Globals.PrepareKnifeRound)
        {
            Globals.BlockTeam = true;
            Globals.KnifeModeStartMessage = true;
            Globals.KnifeRoundStarted = true;
            AddTimer(Configs.GetConfigData().GiveHUDMessageOnStartKnifeRoundForXSecs, () =>
            {
                Globals.KnifeModeStartMessage = false;
            }, TimerFlags.STOP_ON_MAPCHANGE);

            AddTimer(1.0f, () =>
            {
                if (!string.IsNullOrEmpty(Localizer["chat.message.knife.start"]))
                {
                    Helper.AdvancedPrintToServer(Localizer["chat.message.knife.start"]);
                }
            }, TimerFlags.STOP_ON_MAPCHANGE);
            
            Globals.PrepareKnifeRound = false;
        }

        if(!Globals.OnWarmUp)
        {
            Globals.mp_roundtime = ConVar.Find("mp_roundtime")!.GetPrimitiveValue<float>();
            Globals.mp_roundtime_defuse = ConVar.Find("mp_roundtime_defuse")!.GetPrimitiveValue<float>();
            Globals.mp_team_intro_time = ConVar.Find("mp_team_intro_time")!.GetPrimitiveValue<float>();
            Globals.sv_alltalk = ConVar.Find("sv_alltalk")!.GetPrimitiveValue<bool>();
            Globals.sv_full_alltalk = ConVar.Find("sv_full_alltalk")!.GetPrimitiveValue<bool>();
            Globals.sv_talk_enemy_dead = ConVar.Find("sv_talk_enemy_dead")!.GetPrimitiveValue<bool>();
            Globals.sv_talk_enemy_living = ConVar.Find("sv_talk_enemy_living")!.GetPrimitiveValue<bool>();
            Globals.sv_deadtalk = ConVar.Find("sv_deadtalk")!.GetPrimitiveValue<bool>();
            Globals.PrepareKnifeRound = true;
            Globals.OnWarmUp = true;
        }

        if(Globals.PrepareKnifeRound)
        {
            Server.NextFrame(() =>
            {
                Server.ExecuteCommand($"mp_team_intro_time 0.0; sv_buy_status_override 3; mp_roundtime {Configs.GetConfigData().KnifeRoundXTimeInMins}; mp_roundtime_defuse {Configs.GetConfigData().KnifeRoundXTimeInMins}; mp_give_player_c4 0");
                if(Configs.GetConfigData().AllowAllTalkOnKnifeRound)
                {
                    AddTimer(1.0f, () =>
                    {
                        Server.ExecuteCommand($"sv_alltalk true; sv_deadtalk true; sv_full_alltalk true; sv_talk_enemy_dead true; sv_talk_enemy_living true");
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                    
                }
            });
        }

        return HookResult.Continue;
    }

    private HookResult OnCommandJoinTeam(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (Configs.GetConfigData().BlockTeamChangeOnVotingAndKnifeRound && Globals.BlockTeam)
        {
            return HookResult.Handled;
        }
        return HookResult.Continue;
    }

    public HookResult OnEventPlayerChat(EventPlayerChat @event, GameEventInfo info)
    {
        if(Globals.DisableKnife || !Configs.GetConfigData().EnableVoteTeamSideAfterWinning || @event == null)return HookResult.Continue;

        var eventplayer = @event.Userid;
        var eventmessage = @event.Text;
        var Caller = Utilities.GetPlayerFromUserid(eventplayer);
        

        if (Caller == null || !Caller.IsValid)return HookResult.Continue;

        if (string.IsNullOrWhiteSpace(eventmessage)) return HookResult.Continue;
        string trimmedMessageStart = eventmessage.TrimStart();
        string message = trimmedMessageStart.TrimEnd();
        
        string[] CommandInGameToVoteCT = Configs.GetConfigData().CommandsInGameToVoteCT.Split(',');
        if (CommandInGameToVoteCT.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            if(Globals.CTWINNER && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
            {
                if (Globals.VoteCountT.Contains(Caller.SteamID))
                {
                    Globals.VoteCountT.Remove(Caller.SteamID);
                    Globals.currentVotesT =  Globals.currentVotesT - 1;
                }
                if (Globals.VoteCountCT.Contains(Caller.SteamID))return HookResult.Continue;
                Globals.VoteCountCT.Add(Caller.SteamID);
                Globals.currentVotesCT = Globals.VoteCountCT.Count;

                var counct = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist && !p.IsBot && !p.IsHLTV);
                var required = (int)Math.Ceiling(counct * 0.6);
                
                if (Globals.currentVotesCT >= required)
                {
                    ResetValuesAndExecuteCommands();
                }

            }else if(Globals.TWINNER && Caller.TeamNum == (byte)CsTeam.Terrorist)
            {
                if (Globals.VoteCountT.Contains(Caller.SteamID))
                {
                    Globals.VoteCountT.Remove(Caller.SteamID);
                    Globals.currentVotesT =  Globals.currentVotesT - 1;
                }
                if (Globals.VoteCountCT.Contains(Caller.SteamID))return HookResult.Continue;
                Globals.VoteCountCT.Add(Caller.SteamID);
                Globals.currentVotesCT = Globals.VoteCountCT.Count;

                var countt = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist && !p.IsBot && !p.IsHLTV);
                var required = (int)Math.Ceiling(countt * 0.6);
                
                if (Globals.currentVotesCT >= required)
                {
                    Helper.SwitchTeams(Globals.TWINNER ? CsTeam.CounterTerrorist : CsTeam.Terrorist);
                    ResetValuesAndExecuteCommands(); 
                }
            }
        }

        string[] CommandInGameToVoteT = Configs.GetConfigData().CommandsInGameToVoteT.Split(',');
        if (CommandInGameToVoteT.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            if(Globals.CTWINNER && Caller.TeamNum == (byte)CsTeam.CounterTerrorist)
            {
                if (Globals.VoteCountCT.Contains(Caller.SteamID))
                {
                    Globals.VoteCountCT.Remove(Caller.SteamID);
                    Globals.currentVotesCT =  Globals.currentVotesCT - 1;
                }
                if (Globals.VoteCountT.Contains(Caller.SteamID))return HookResult.Continue;
                Globals.VoteCountT.Add(Caller.SteamID);
                Globals.currentVotesT = Globals.VoteCountT.Count;

                var counct = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.CounterTerrorist && !p.IsBot && !p.IsHLTV);
                var required = (int)Math.Ceiling(counct * 0.6);
                
                if (Globals.currentVotesT >= required)
                {
                    Helper.SwitchTeams(Globals.TWINNER ? CsTeam.CounterTerrorist : CsTeam.Terrorist);
                    ResetValuesAndExecuteCommands(); 
                }

            }else if(Globals.TWINNER && Caller.TeamNum == (byte)CsTeam.Terrorist)
            {
                if (Globals.VoteCountCT.Contains(Caller.SteamID))
                {
                    Globals.VoteCountCT.Remove(Caller.SteamID);
                    Globals.currentVotesCT =  Globals.currentVotesCT - 1;
                }
                if (Globals.VoteCountT.Contains(Caller.SteamID))return HookResult.Continue;
                Globals.VoteCountT.Add(Caller.SteamID);
                Globals.currentVotesT = Globals.VoteCountT.Count;

                var countt = Utilities.GetPlayers().Count(p => p.TeamNum == (int)CsTeam.Terrorist && !p.IsBot && !p.IsHLTV);
                var required = (int)Math.Ceiling(countt * 0.6);
                
                if (Globals.currentVotesT >= required)
                {
                    ResetValuesAndExecuteCommands(); 
                }
            }
        }

        return HookResult.Continue;
    }

    public void ResetValuesAndExecuteCommands()
    {
        Server.NextFrame(() =>
        {
            Globals.VoteCountT.Clear();
            Globals.VoteCountCT.Clear();
            Globals.TWINNER = false;
            Globals.CTWINNER = false;
            Globals.BlockTeam = false;
            Globals.RemoveWeapons = false;

            string roundtime = Globals.mp_roundtime.ToString().Replace(',', '.');
            string roundtimeDefuse = Globals.mp_roundtime_defuse.ToString().Replace(',', '.');
            string teamIntroTime = Globals.mp_team_intro_time.ToString().Replace(',', '.');
            Server.ExecuteCommand($"mp_team_intro_time {teamIntroTime}; sv_buy_status_override -1; mp_roundtime {roundtime}; mp_roundtime_defuse {roundtimeDefuse}; mp_give_player_c4 1");
            if (Configs.GetConfigData().AllowAllTalkOnKnifeRound)
            {
                Server.ExecuteCommand($"sv_alltalk {Globals.sv_alltalk}; sv_deadtalk {Globals.sv_deadtalk}; sv_full_alltalk {Globals.sv_full_alltalk}; sv_talk_enemy_dead {Globals.sv_talk_enemy_dead}; sv_talk_enemy_living {Globals.sv_talk_enemy_living}");
            }

            int x = Configs.GetConfigData().AfterWinningRestartXTimes;
            for (int i = 1; i <= x; i++)
            {
                float interval = i * 2.0f;
                int currentI = i;
                AddTimer(interval, () =>
                {
                    Server.ExecuteCommand($"mp_restartgame 1");
                    if (currentI == x)
                    {
                        AddTimer(4.0f, () =>
                        {
                            if (!string.IsNullOrEmpty(Localizer["chat.message.match.start"]))
                            {
                                Helper.AdvancedPrintToServer(Localizer["chat.message.match.start"]);
                            }
                        }, TimerFlags.STOP_ON_MAPCHANGE);
                    }
                }, TimerFlags.STOP_ON_MAPCHANGE);
            }
        });
    }


    private void OnMapEnd()
    {
        Helper.ClearVariables();
    }

    public override void Unload(bool hotReload)
    {
        Helper.ClearVariables();
    }
    
}