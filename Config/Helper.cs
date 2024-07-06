using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using Knife_Round_GoldKingZ.Config;

namespace Knife_Round_GoldKingZ;

public class Helper
{
    public static void AdvancedPrintToChat(CCSPlayerController player, string message, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                player.PrintToChat(" " + messages);
            }
        }else
        {
            player.PrintToChat(message);
        }
    }
    public static void AdvancedPrintToServer(string message, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                Server.PrintToChatAll(" " + messages);
            }
        }else
        {
            Server.PrintToChatAll(message);
        }
    }
    
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        var excludedGroups = groups.Split(',');
        foreach (var group in excludedGroups)
        {
            if (group.StartsWith("#"))
            {
                if (AdminManager.PlayerInGroup(player, group))
                    return true;
            }
            else if (group.StartsWith("@"))
            {
                if (AdminManager.PlayerHasPermissions(player, group))
                    return true;
            }
        }
        return false;
    }
    public static void DropAllWeaponsAndDelete(CCSPlayerController player)
    {
        if(player == null || !player.IsValid)return;
        if(player.PlayerPawn == null || !player.PlayerPawn.IsValid)return;
        if(!player.PawnIsAlive)return;
        if(player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid)return;
        if(player.PlayerPawn.Value.WeaponServices == null || player.PlayerPawn.Value.WeaponServices.MyWeapons == null)return;

        foreach (var weapon in player.PlayerPawn.Value.WeaponServices.MyWeapons)
        {
            if (weapon == null || !weapon.IsValid) continue;
            var weaponValue = weapon.Value;
            if (weaponValue == null || !weaponValue.IsValid) continue;
            if (weaponValue.DesignerName != null && weaponValue.DesignerName.Contains("weapon_knife") || weaponValue.DesignerName != null && weaponValue.DesignerName.Contains("weapon_c4"))continue;
            if(weaponValue.DesignerName == null)continue;
            player.DropActiveWeapon();
            if(weapon.Value != null)weapon.Value.Remove();
        }
    }
    public static List<CCSPlayerController> GetCounterTerroristController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && p.Connected == PlayerConnectedState.PlayerConnected && p.Team == CsTeam.CounterTerrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetTerroristController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && p.Connected == PlayerConnectedState.PlayerConnected && p.Team == CsTeam.Terrorist).ToList();
        return playerList;
    }
    public static List<CCSPlayerController> GetAllController() 
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").Where(p => p != null && p.IsValid && p.Connected == PlayerConnectedState.PlayerConnected).ToList();
        return playerList;
    }
    public static int GetCounterTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.TeamNum == (byte)CsTeam.CounterTerrorist);
    }
    public static int GetTerroristCount()
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV && p.Connected == PlayerConnectedState.PlayerConnected && p.TeamNum == (byte)CsTeam.Terrorist);
    }
    public static int GetPlayersCount(bool IncludeBots = false)
    {
        return Utilities.GetPlayers().Count(p => p != null && p.IsValid && p.Connected == PlayerConnectedState.PlayerConnected && (IncludeBots || (!p.IsBot && !p.IsHLTV)));
    }
    public static int GetPlayersNeeded()
    {
        if (Configs.GetConfigData().CountBotsAsPlayers)
        {
            return GetPlayersCount(true);
        }
        else
        {
            return GetPlayersCount(false);
        }
    }
    public static void ClearVariables()
    {
        Globals.DisableKnife = false;
        Globals.VoteCountCT.Clear();
        Globals.VoteCountT.Clear();
        
        Globals.Stopwatch.Reset();
        
        Globals.CTWINNER = false;
        Globals.TWINNER = false;
        Globals.Timer = 0.0f;
        
        Globals.currentVotesT = 0;
        Globals.currentVotesCT = 0;
        
        Globals.mp_roundtime = 0.0f;
        Globals.mp_roundtime_defuse = 0.0f;
        Globals.mp_team_intro_time = 0.0f;
        Globals.sv_alltalk = false;
        Globals.sv_deadtalk = false;
        Globals.sv_full_alltalk = false;
        Globals.sv_talk_enemy_dead = false;
        Globals.sv_talk_enemy_living = false;
        
        Globals.BlockTeam = false;
        Globals.OnWarmUp = false;
        Globals.PrepareKnifeRound = false;
        Globals.KnifeRoundStarted = false;
        Globals.KnifeModeStartMessage = false;
        Globals.RemoveWeapons = false;
    }

    private static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch
        {
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }

    public static void SwitchTeams(CsTeam team)
    {
        foreach (var pl in Utilities.GetPlayers().FindAll(x => x != null && x.IsValid))
        {
            if (pl.TeamNum == 3)
            {
                pl.SwitchTeam(CsTeam.Terrorist);
            }
            else if (pl.TeamNum == 2)
            {
                pl.SwitchTeam(CsTeam.CounterTerrorist);
            }
        }
    }
}