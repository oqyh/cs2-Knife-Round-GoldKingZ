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
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Translations;

namespace Knife_Round_GoldKingZ;

public class KnifeRoundGoldKingZ : BasePlugin
{
    public override string ModuleName => "Knife Round (Creates An Additional Round With Knifes After Warmup)";
    public override string ModuleVersion => "1.1.4";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
    public static KnifeRoundGoldKingZ Instance { get; set; } = new();
    public Globals g_Main = new();
    private readonly PlayerChat _PlayerChat = new();
    public override void Load(bool hotReload)
    {
        Instance = this;
        Configs.Load(ModuleDirectory);
        Configs.Shared.CookiesModule = ModuleDirectory;
        Configs.Shared.StringLocalizer = Localizer;

        RegisterEventHandler<EventRoundStart>(OnRoundStart);
        RegisterEventHandler<EventRoundPrestart>(OnEventRoundPrestart);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterEventHandler<EventRoundEnd>(OnEventRoundEnd);

        RegisterListener<Listeners.OnTick>(OnTick);
        RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
        
        AddCommandListener("say", OnPlayerChat, HookMode.Post);
        AddCommandListener("say_team", OnPlayerChatTeam, HookMode.Post);
        AddCommandListener("jointeam", OnCommandJoinTeam, HookMode.Pre);
    }

    public void OnTick()
    {
        if(!g_Main.ShowCenter)return;
        
        if(g_Main.GameMode == 1 || g_Main.GameMode == 2)
        {
            TimeSpan elapsedTime = DateTime.Now - g_Main.LastTickTime;
            if (elapsedTime.TotalSeconds >= 1)
            {
                if (g_Main.TickTime > 1)
                {
                    g_Main.TickTime -= 1;
                }else if (g_Main.TickTime <= 1)
                {
                    if(g_Main.GameMode == 1)
                    {
                        g_Main.ShowCenter = false;
                    }else if(g_Main.GameMode == 2)
                    {
                        Helper.GetWantedTeam();
                        Helper.SwitchTeams();
                        g_Main.GameMode = 3;
                    }
                    
                }
                g_Main.LastTickTime = DateTime.Now;
            }

            StringBuilder builder = new StringBuilder();

            int VotesToCt = g_Main.Player_Data.Values.Count(data => data.TeamVoted == CsTeam.CounterTerrorist);
            int VotesToT = g_Main.Player_Data.Values.Count(data => data.TeamVoted == CsTeam.Terrorist);
            bool winnerct = g_Main.WinerTeam == CsTeam.CounterTerrorist? true: false;
            bool winnert = g_Main.WinerTeam == CsTeam.Terrorist? true: false;
            int Votes_Requred = Helper.GetPlayersCount(false,false,winnerct,winnert);
            var required = (int)Math.Ceiling(Votes_Requred * 0.6);

            foreach (var players in Helper.GetPlayersController())
            {
                if(players == null || !players.IsValid)continue;
                if(g_Main.GameMode == 1)
                {
                    string localizeduse = Configs.Shared.StringLocalizer!["hud.message.kniferoundstarted"];
                    builder.AppendFormat(localizeduse);
                    var centerhtml = builder.ToString();
                    players.PrintToCenterHtml(centerhtml);
                }else if(g_Main.GameMode == 2)
                {
                    if(players.TeamNum == (byte)g_Main.WinerTeam)
                    {
                        string localizeduse = Configs.Shared.StringLocalizer!["hud.message.winnerteam", g_Main.TickTime, VotesToCt, VotesToT, required];
                        builder.AppendFormat(localizeduse);
                        var centerhtml = builder.ToString();
                        players.PrintToCenterHtml(centerhtml);
                    }else
                    {
                        string teamloc = players.TeamNum == (byte)CsTeam.Terrorist?"hud.message.loseteam.t":"hud.message.loseteam.ct";
                        string localizeduse = Configs.Shared.StringLocalizer![teamloc, g_Main.TickTime, VotesToCt, VotesToT, required];
                        builder.AppendFormat(localizeduse);
                        var centerhtml = builder.ToString();
                        players.PrintToCenterHtml(centerhtml);
                    }
                    
                }
            }
        }
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event == null)return HookResult.Continue;
        var player = @event.Userid;

        if (player == null || !player.IsValid)return HookResult.Continue;

        if (g_Main.Player_Data.ContainsKey(player))g_Main.Player_Data.Remove(player);

        return HookResult.Continue;
    }
    private HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo info)
	{
        if (player == null || !player.IsValid)return HookResult.Continue;

        _PlayerChat.OnPlayerChat(player, info, false);

        return HookResult.Continue;
    }
    private HookResult OnPlayerChatTeam(CCSPlayerController? player, CommandInfo info)
	{
        if (player == null || !player.IsValid)return HookResult.Continue;

        _PlayerChat.OnPlayerChat(player, info, true);

        return HookResult.Continue;
    }


    public HookResult OnEventRoundPrestart(EventRoundPrestart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;


        if(Helper.IsWarmup())
        {
            Helper.FreezeCheck();
            if(g_Main.GameMode != 0)Helper.ClearVariables();
            if(!g_Main.OneTime)
            {
                if(!string.IsNullOrEmpty(Configs.GetConfigData().Execute_Cfg_On_Warmup))
                {
                    Server.ExecuteCommand("execifexists "+ Configs.GetConfigData().Execute_Cfg_On_Warmup);
                    Server.ExecuteCommand($"mp_restartgame 1");
                }
                Helper.GetOrginalConvars();
                g_Main.WarmUpTimer = AddTimer(g_Main.mp_warmuptime - 1.0f, CheckPlayer_Callback, TimerFlags.STOP_ON_MAPCHANGE);
                Helper.AdvancedServerPrintToChatAll(Configs.Shared.StringLocalizer!["chat.message.knife.prepare"]);
                g_Main.OneTime = true;
            }
            return HookResult.Continue;
        }

        switch(g_Main.GameMode)
        {
            case -1:
                g_Main.BlockTeamChange = false;
                Helper.AdvancedServerPrintToChatAll(Configs.Shared.StringLocalizer!["chat.message.knife.ignored"], Helper.GetPlayersCount(Configs.GetConfigData().Count_Bots_As_Players), Configs.GetConfigData().Minimum_Players_Needed);
                break;
            case 1:
                g_Main.BlockTeamChange = false;
                if(Configs.GetConfigData().Block_Switching_Teams_On == 1 || Configs.GetConfigData().Block_Switching_Teams_On == 2 || Configs.GetConfigData().Block_Switching_Teams_On == 3)
                {
                    g_Main.BlockTeamChange = true;
                }
                g_Main.TickTime = Configs.GetConfigData().Knife_Round_Center_Message_Time;
                g_Main.LastTickTime = DateTime.Now;
                g_Main.ShowCenter = true;
                Helper.AdvancedServerPrintToChatAll(Configs.Shared.StringLocalizer!["chat.message.knife.start"]);
                break;
            case 2:
                g_Main.BlockTeamChange = false;
                if(Configs.GetConfigData().Block_Switching_Teams_On == 2 || Configs.GetConfigData().Block_Switching_Teams_On == 3)
                {
                    g_Main.BlockTeamChange = true;
                }
                g_Main.TickTime = Configs.GetConfigData().Vote_Team_Side_Time;
                g_Main.LastTickTime = DateTime.Now;
                g_Main.ShowCenter = true;
                break;
            case 3:
                g_Main.BlockTeamChange = false;
                if(Configs.GetConfigData().Block_Switching_Teams_On == 3)
                {
                    g_Main.BlockTeamChange = true;
                }
                Helper.MatchLive();
                g_Main.GameMode = 4;
                break;
        }
        return HookResult.Continue;
    }

    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        if(g_Main.GameMode == 1)
        {
            g_Main.ForceStripe?.Kill();
            g_Main.ForceStripe = null;
            g_Main.ForceStripe = AddTimer(1.0f, ForceStripe_Callback, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
        }
        Helper.FreezeCheck();
        return HookResult.Continue;
    }

    public HookResult OnEventRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;        
        
        switch(g_Main.GameMode)
        {
            case -1:
                g_Main.BlockTeamChange = false;
                g_Main.GameMode = -2;
                break;
            case 1:
                g_Main.BlockTeamChange = false;
                g_Main.ForceStripe?.Kill();
                g_Main.ForceStripe = null;
                Helper.GetWinnerTeam();
                if(Configs.GetConfigData().After_Winning_Vote_Team_Side)
                {
                    g_Main.GameMode = 2;
                    Helper.ModifyConvar_WaitingForVoting();
                }else
                {
                    if(Configs.GetConfigData().Send_Winner_Team_To_CT)
                    {
                        g_Main.WantedTeam = CsTeam.CounterTerrorist;
                    }
                    Helper.SwitchTeams();
                    g_Main.GameMode = 3;
                    Helper.ModifyConvar_MatchLive();
                }
                break;
        }

        return HookResult.Continue;
    }
    
    private void ForceStripe_Callback()
    {
        if(g_Main.GameMode != 1)
        {
            g_Main.ForceStripe?.Kill();
            g_Main.ForceStripe = null;
        }

        foreach(var players in Helper.GetPlayersController(true, false))
        {
            if (g_Main.GameMode != 1) return;
            if (players == null || !players.IsValid) continue;
            Helper.ForceStripePlayer(players);
        }

        Helper.ClearGroundWeapons();
    }
    private void CheckPlayer_Callback()
    {
        if(Helper.GetPlayersCount(Configs.GetConfigData().Count_Bots_As_Players) < Configs.GetConfigData().Minimum_Players_Needed)
        {
            g_Main.GameMode = -1;
        }else
        {
            if(g_Main.GameMode == 0)
            {
                g_Main.GameMode = 1;
                Helper.ModifyConvar_KnifeStart();
            }
        }

        g_Main.ForceStripe?.Kill();
        g_Main.ForceStripe = null;
    }


    private HookResult OnCommandJoinTeam(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (Configs.GetConfigData().Block_Switching_Teams_On > 0 && g_Main.BlockTeamChange)
        {
            return HookResult.Handled;
        }
        return HookResult.Continue;
    }


    private void OnMapEnd()
    {
        Helper.ClearVariables(true);
    }

    public override void Unload(bool hotReload)
    {
        Helper.ClearVariables(true);
    }


    /* [ConsoleCommand("css_test", "test")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void test(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(player == null || !player.IsValid )return;
    } */
    
}