using CounterStrikeSharp.API.Core;
using Knife_Round_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API;

namespace Knife_Round_GoldKingZ;

public class PlayerChat
{
    public HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo info, bool TeamChat)
	{
        var g_Main = KnifeRoundGoldKingZ.Instance.g_Main;
        if (g_Main.GameMode != 2 || player == null || !player.IsValid || player.TeamNum != (byte)g_Main.WinerTeam)return HookResult.Continue;

        var eventmessage = info.ArgString;
        eventmessage = eventmessage.TrimStart('"');
        eventmessage = eventmessage.TrimEnd('"');
        
        if (string.IsNullOrWhiteSpace(eventmessage)) return HookResult.Continue;
        string trimmedMessageStart = eventmessage.TrimStart();
        string message = trimmedMessageStart.TrimEnd();

        

        string[] CommandsInGameToVoteCTs = Configs.GetConfigData().Commands_In_Game_To_Vote_CT.Split(',');
        if (CommandsInGameToVoteCTs.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            if (!g_Main.Player_Data.ContainsKey(player))
            {
                g_Main.Player_Data.Add(player, new Globals.PlayerDataClass(player, CsTeam.CounterTerrorist));
            }else
            {
                g_Main.Player_Data[player].Player = player;
                g_Main.Player_Data[player].TeamVoted = CsTeam.CounterTerrorist;
            }
        }

        string[] CommandsInGameToVoteTs = Configs.GetConfigData().Commands_In_Game_To_Vote_T.Split(',');
        if (CommandsInGameToVoteTs.Any(cmd => cmd.Equals(message, StringComparison.OrdinalIgnoreCase)))
        {
            if (!g_Main.Player_Data.ContainsKey(player))
            {
                g_Main.Player_Data.Add(player, new Globals.PlayerDataClass(player, CsTeam.Terrorist));
            }else
            {
                g_Main.Player_Data[player].Player = player;
                g_Main.Player_Data[player].TeamVoted = CsTeam.Terrorist;
            }
        }


        int VotesToCt = g_Main.Player_Data.Values.Count(data => data.TeamVoted == CsTeam.CounterTerrorist);
        int VotesToT = g_Main.Player_Data.Values.Count(data => data.TeamVoted == CsTeam.Terrorist);
        bool winnerct = g_Main.WinerTeam == CsTeam.CounterTerrorist? true: false;
        bool winnert = g_Main.WinerTeam == CsTeam.Terrorist? true: false;
        int Votes_Requred = Helper.GetPlayersCount(false,false,winnerct,winnert);
        var required = (int)Math.Ceiling(Votes_Requred * 0.6);

        if( VotesToCt >= required ||  VotesToT >= required)
        {
            Helper.GetWantedTeam();
            Helper.SwitchTeams();
            g_Main.GameMode = 3;
        }



        return HookResult.Continue;
    }
}