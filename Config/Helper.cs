using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using Knife_Round_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Timers;
using System.Linq.Expressions;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Core.Translations;

namespace Knife_Round_GoldKingZ;

public class Helper
{
    public static readonly string[] WeaponsList =
    {
        "weapon_ak47", "weapon_aug", "weapon_awp", "weapon_bizon", "weapon_cz75a", "weapon_deagle", "weapon_elite", "weapon_famas", "weapon_fiveseven", "weapon_g3sg1", "weapon_galilar",
        "weapon_glock", "weapon_hkp2000", "weapon_m249", "weapon_m4a1", "weapon_m4a1_silencer", "weapon_mac10", "weapon_mag7", "weapon_mp5sd", "weapon_mp7", "weapon_mp9", "weapon_negev",
        "weapon_nova", "weapon_p250", "weapon_p90", "weapon_revolver", "weapon_sawedoff", "weapon_scar20", "weapon_sg556", "weapon_ssg08", "weapon_tec9", "weapon_ump45", "weapon_usp_silencer", "weapon_xm1014",
        "weapon_decoy", "weapon_flashbang", "weapon_hegrenade", "weapon_incgrenade", "weapon_molotov", "weapon_smokegrenade","item_defuser", "item_cutters", "weapon_knife"
    };

    public static void AdvancedPlayerPrintToChat(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    player.PrintToChat(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToChat(message);
        }
    }
    public static void AdvancedPlayerPrintToConsole(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    player.PrintToConsole(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToConsole(message);
        }
    }
    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    Server.PrintToChatAll(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            Server.PrintToChatAll(message);
        }
    }
    
    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true) 
    {
        var playerList = Utilities
            .FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(p => p != null && p.IsValid && 
                        (IncludeBots || (!p.IsBot && !p.IsHLTV)) && 
                        p.Connected == PlayerConnectedState.PlayerConnected && 
                        ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
                        (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
                        (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
            .ToList();

        return playerList;
    }
    public static int GetPlayersCount(bool IncludeBots = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        return Utilities.GetPlayers().Count(p => 
            p != null && 
            p.IsValid && 
            p.Connected == PlayerConnectedState.PlayerConnected && 
            (IncludeBots || (!p.IsBot && !p.IsHLTV)) && 
            ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
            (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
            (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator))
        );
    }

    public static void ClearVariables(bool all = false)
    {
        var g_Main = KnifeRoundGoldKingZ.Instance.g_Main;

        g_Main.Player_Data.Clear();

        g_Main.WarmUpTimer?.Kill();
        g_Main.ForceStripe?.Kill();
        g_Main.WarmUpTimer = null;
        g_Main.ForceStripe = null;

        
        g_Main.GameMode = 0;
        g_Main.TickTime = 0;

        g_Main.OneTime = false;
        g_Main.BlockTeamChange = false;
        g_Main.ShowCenter = false;

        g_Main.WinerTeam = CsTeam.None;
        g_Main.WantedTeam = CsTeam.None;

        if(all)
        {
            g_Main.mp_free_armor = 0;

            g_Main.mp_roundtime = 0.0f;
            g_Main.mp_roundtime_defuse = 0.0f;
            g_Main.mp_team_intro_time = 0.0f;
            g_Main.mp_warmuptime = 0.0f;
            
            g_Main.sv_alltalk = false;
            g_Main.sv_full_alltalk = false;
            g_Main.sv_talk_enemy_dead = false;
            g_Main.sv_talk_enemy_living = false;
            g_Main.sv_deadtalk = false;
        }
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

    public static void DebugMessage(string message, bool prefix = true)
    {
        if (!Configs.GetConfigData().EnableDebug) return;

        Console.ForegroundColor = ConsoleColor.Magenta;
        string Prefix = "[Knife Round]: ";
        Console.WriteLine((prefix ? Prefix : "") + message);

        Console.ResetColor();
    }


    public static void GetOrginalConvars()
    {
        KnifeRoundGoldKingZ.Instance.g_Main.mp_free_armor = ConVar.Find("mp_free_armor")!.GetPrimitiveValue<int>();

        KnifeRoundGoldKingZ.Instance.g_Main.mp_roundtime = ConVar.Find("mp_roundtime")!.GetPrimitiveValue<float>();
        KnifeRoundGoldKingZ.Instance.g_Main.mp_roundtime_defuse = ConVar.Find("mp_roundtime_defuse")!.GetPrimitiveValue<float>();
        KnifeRoundGoldKingZ.Instance.g_Main.mp_team_intro_time = ConVar.Find("mp_team_intro_time")!.GetPrimitiveValue<float>();
        KnifeRoundGoldKingZ.Instance.g_Main.mp_warmuptime = ConVar.Find("mp_warmuptime")!.GetPrimitiveValue<float>();

        KnifeRoundGoldKingZ.Instance.g_Main.sv_alltalk = ConVar.Find("sv_alltalk")!.GetPrimitiveValue<bool>();
        KnifeRoundGoldKingZ.Instance.g_Main.sv_full_alltalk = ConVar.Find("sv_full_alltalk")!.GetPrimitiveValue<bool>();
        KnifeRoundGoldKingZ.Instance.g_Main.sv_talk_enemy_dead = ConVar.Find("sv_talk_enemy_dead")!.GetPrimitiveValue<bool>();
        KnifeRoundGoldKingZ.Instance.g_Main.sv_talk_enemy_living = ConVar.Find("sv_talk_enemy_living")!.GetPrimitiveValue<bool>();
        KnifeRoundGoldKingZ.Instance.g_Main.sv_deadtalk = ConVar.Find("sv_deadtalk")!.GetPrimitiveValue<bool>();
    }

    public static void ForceStripePlayer(CCSPlayerController player)
    {
        if (player == null || !player.IsValid || !player.PawnIsAlive) return;

        var playerPawn = player.PlayerPawn;
        if (playerPawn == null || !playerPawn.IsValid) return;

        var playerPawnValue = playerPawn.Value;
        if (playerPawnValue == null || !playerPawnValue.IsValid) return;

        var playerWeaponServices = playerPawnValue.WeaponServices;
        if (playerWeaponServices == null) return;

        var playerMyWeapons = playerWeaponServices.MyWeapons;
        if (playerMyWeapons == null) return;

        foreach (var weaponInventory in playerMyWeapons)
        {
            if (weaponInventory == null || !weaponInventory.IsValid) continue;

            var weaponInventoryValue = weaponInventory.Value;
            if (weaponInventoryValue == null || !weaponInventoryValue.IsValid) continue;

            var weaponDesignerName = weaponInventoryValue.DesignerName;
            if (weaponDesignerName == null || string.IsNullOrEmpty(weaponDesignerName)) continue;

            if (weaponDesignerName.Contains("weapon_knife")|| 
                weaponDesignerName.Contains("weapon_bayonet") ||
                weaponDesignerName.Contains("weapon_c4"))
            {
                continue;
            }

            RemoveWeaponByName(player, weaponDesignerName);
        }
        
    }

    public static void RemoveWeaponByName(CCSPlayerController? player, string weaponName)
    {
        if (string.IsNullOrEmpty(weaponName)) return;

        if (player == null || !player.IsValid) return;
        
        var playerpawn = player.PlayerPawn;
        if (playerpawn == null || !playerpawn.IsValid) return;

        var playerpawnvalue = playerpawn.Value;
        if (playerpawnvalue == null || !playerpawnvalue.IsValid) return;

        var playerWeaponServices = playerpawnvalue.WeaponServices;
        if (playerWeaponServices == null) return;

        var playerMyWeapons = playerWeaponServices.MyWeapons;
        if (playerMyWeapons == null) return;
        
        var WeaponInvetory = playerMyWeapons
            .Where(x => x != null && x.IsValid && x.Value != null && x.Value.IsValid && x.Value.DesignerName == weaponName).FirstOrDefault();

        if(WeaponInvetory == null || !WeaponInvetory.IsValid)return;
        
        var playerActiveWeapon = playerWeaponServices.ActiveWeapon;
        if(playerActiveWeapon == null || !playerActiveWeapon.IsValid || playerActiveWeapon.Value == null || !playerActiveWeapon.Value.IsValid)return;
        
        playerActiveWeapon.Raw = WeaponInvetory.Raw;
        
        var activeWeaponEntity = playerActiveWeapon.Value.As<CBaseEntity>();
        
        player.DropActiveWeapon();
        Server.NextFrame(() =>
        {
            if(activeWeaponEntity != null && activeWeaponEntity.IsValid)
            {
                activeWeaponEntity.AddEntityIOEvent("Kill", activeWeaponEntity, null, "", 0.3f);
            }
        });
    }

    public static void ClearGroundWeapons()
    {
        foreach (string Weapons in WeaponsList)
        {
            foreach (var entity in Utilities.FindAllEntitiesByDesignerName<CBaseEntity>(Weapons))
            {
                if (entity == null || !entity.IsValid) continue;
                if (entity.Entity == null) continue;
                if (entity.OwnerEntity == null || entity.OwnerEntity.IsValid) continue;

                entity.AcceptInput("Kill");
            }
        }
    }


    public static void ModifyConvar_KnifeStart()
    {
        Server.ExecuteCommand($"mp_team_intro_time 0.0; sv_buy_status_override 3; mp_roundtime {Configs.GetConfigData().Knife_Round_Time}; mp_roundtime_defuse {Configs.GetConfigData().Knife_Round_Time}; mp_give_player_c4 0; mp_free_armor {Configs.GetConfigData().Give_Armor_On_Knife_Round}");
        
        if(Configs.GetConfigData().Allow_All_Talk_On_Knife_Round)
        {
            Server.ExecuteCommand($"sv_alltalk true; sv_deadtalk true; sv_full_alltalk true; sv_talk_enemy_dead true; sv_talk_enemy_living true");  
        }
        if(!string.IsNullOrEmpty(Configs.GetConfigData().Execute_Cfg_On_KnifeRound))
        {
            Server.ExecuteCommand("execifexists "+ Configs.GetConfigData().Execute_Cfg_On_KnifeRound);
        }
    }
    public static void ModifyConvar_WaitingForVoting()
    {
        Server.ExecuteCommand($"mp_roundtime 60; mp_roundtime_defuse 60");
        
        if(Configs.GetConfigData().Allow_All_Talk_On_Knife_Round)
        {
            Server.ExecuteCommand($"sv_alltalk true; sv_deadtalk true; sv_full_alltalk true; sv_talk_enemy_dead true; sv_talk_enemy_living true");  
        }
        if(!string.IsNullOrEmpty(Configs.GetConfigData().Execute_Cfg_On_Voting))
        {
            Server.ExecuteCommand("execifexists "+ Configs.GetConfigData().Execute_Cfg_On_Voting);
        }
    }
    public static void ModifyConvar_MatchLive()
    {
        var g_Main = KnifeRoundGoldKingZ.Instance.g_Main;
        Server.ExecuteCommand($"mp_team_intro_time {g_Main.mp_team_intro_time}; sv_buy_status_override -1; mp_roundtime {g_Main.mp_roundtime}; mp_roundtime_defuse {g_Main.mp_roundtime_defuse}; mp_give_player_c4 1; mp_free_armor {g_Main.mp_free_armor}");
        if (Configs.GetConfigData().Allow_All_Talk_On_Knife_Round)
        {
            Server.ExecuteCommand($"sv_alltalk {g_Main.sv_alltalk}; sv_deadtalk {g_Main.sv_deadtalk}; sv_full_alltalk {g_Main.sv_full_alltalk}; sv_talk_enemy_dead {g_Main.sv_talk_enemy_dead}; sv_talk_enemy_living {g_Main.sv_talk_enemy_living}");
        }
        if(!string.IsNullOrEmpty(Configs.GetConfigData().Execute_Cfg_On_MatchLive))
        {
            Server.ExecuteCommand("execifexists "+ Configs.GetConfigData().Execute_Cfg_On_MatchLive);
        }
    }
    public static void MatchLive()
    {
        int x = Configs.GetConfigData().Restart_On_Match_Live;
        if (x < 1)
        {
            KnifeRoundGoldKingZ.Instance.AddTimer(1.0f, () =>
            {
                Server.ExecuteCommand($"mp_restartgame 1");
            }, TimerFlags.STOP_ON_MAPCHANGE);

            KnifeRoundGoldKingZ.Instance.AddTimer(4.0f, () =>
            {
                AdvancedServerPrintToChatAll(Configs.Shared.StringLocalizer!["chat.message.match.start"]);
            }, TimerFlags.STOP_ON_MAPCHANGE);
            return;
        }

        for (int i = 1; i <= x; i++)
        {
            float interval = i * 2.0f;
            int currentI = i;
            KnifeRoundGoldKingZ.Instance.AddTimer(interval, () =>
            {
                Server.ExecuteCommand($"mp_restartgame 1");
                if (currentI == x)
                {
                    KnifeRoundGoldKingZ.Instance.AddTimer(4.0f, () =>
                    {
                        AdvancedServerPrintToChatAll(Configs.Shared.StringLocalizer!["chat.message.match.start"]);
                    }, TimerFlags.STOP_ON_MAPCHANGE);
                }
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }

        ClearVariables();
    }
    
    public static void GetWinnerTeam()
    {
        var g_Main = KnifeRoundGoldKingZ.Instance.g_Main;
        int T_players_CT = 0;
        int T_players_T = 0;
        int T_Health_CT = 0;
        int T_Health_T = 0;

        foreach(var players in GetPlayersController(true,false))
        {
            if(players == null || !players.IsValid || 
            players.PlayerPawn == null || !players.PlayerPawn.IsValid ||
            players.PlayerPawn.Value == null || !players.PlayerPawn.Value.IsValid)continue;
            
            if(players.PlayerPawn.Value.LifeState != (byte)LifeState_t.LIFE_ALIVE)continue;

            if(players.TeamNum == (byte)CsTeam.CounterTerrorist)
            {
                T_players_CT++;
                T_Health_CT += (int)players.PawnHealth;
            }
            if(players.TeamNum == (byte)CsTeam.Terrorist)
            {
                T_players_T++;
                T_Health_T += (int)players.PawnHealth;
            }
            
        }


        if(Configs.GetConfigData().Get_Winner_Team < 2)
        {
            if(T_players_CT > T_players_T)
            {
                g_Main.WinerTeam = CsTeam.CounterTerrorist;
            }else if(T_players_T > T_players_CT)
            {
                g_Main.WinerTeam = CsTeam.Terrorist;
            }else
            {
                g_Main.WinerTeam = CsTeam.CounterTerrorist;
            }
        }else if(Configs.GetConfigData().Get_Winner_Team == 2)
        {
            if(T_players_CT > T_players_T)
            {
                g_Main.WinerTeam = CsTeam.CounterTerrorist;
            }else if(T_players_T > T_players_CT)
            {
                g_Main.WinerTeam = CsTeam.Terrorist;
            }else
            {
                g_Main.WinerTeam = CsTeam.Terrorist;
            }
        }
        
        else if(Configs.GetConfigData().Get_Winner_Team == 3)
        {
            if(T_Health_CT > T_Health_T)
            {
                g_Main.WinerTeam = CsTeam.CounterTerrorist;
            }else if(T_Health_T > T_Health_CT)
            {
                g_Main.WinerTeam = CsTeam.Terrorist;
            }else
            {
                g_Main.WinerTeam = CsTeam.CounterTerrorist;
            }
        }else if(Configs.GetConfigData().Get_Winner_Team == 4)
        {
            if(T_Health_CT > T_Health_T)
            {
                g_Main.WinerTeam = CsTeam.CounterTerrorist;
            }else if(T_Health_T > T_Health_CT)
            {
                g_Main.WinerTeam = CsTeam.Terrorist;
            }else
            {
                g_Main.WinerTeam = CsTeam.Terrorist;
            }
        }
        
        
        else if(Configs.GetConfigData().Get_Winner_Team == 5)
        {
            if(T_players_CT > T_players_T)
            {
                g_Main.WinerTeam = CsTeam.CounterTerrorist;
            }else if(T_players_T > T_players_CT)
            {
                g_Main.WinerTeam = CsTeam.Terrorist;
            }else 
            {
                if(T_Health_CT > T_Health_T)
                {
                    g_Main.WinerTeam = CsTeam.CounterTerrorist;
                }else if(T_Health_T > T_Health_CT)
                {
                    g_Main.WinerTeam = CsTeam.Terrorist;
                }else
                {
                    g_Main.WinerTeam = CsTeam.CounterTerrorist;
                }
                
            }
        }else if(Configs.GetConfigData().Get_Winner_Team == 6)
        {
            if(T_players_CT > T_players_T)
            {
                g_Main.WinerTeam = CsTeam.CounterTerrorist;
            }else if(T_players_T > T_players_CT)
            {
                g_Main.WinerTeam = CsTeam.Terrorist;
            }else 
            {
                if(T_Health_CT > T_Health_T)
                {
                    g_Main.WinerTeam = CsTeam.CounterTerrorist;
                }else if(T_Health_T > T_Health_CT)
                {
                    g_Main.WinerTeam = CsTeam.Terrorist;
                }else
                {
                    g_Main.WinerTeam = CsTeam.Terrorist;
                }
            }
        }

        string winnerteamstring = g_Main.WinerTeam == CsTeam.CounterTerrorist?"chat.message.winner.team.ct":"chat.message.winner.team.t";
        AdvancedServerPrintToChatAll(Configs.Shared.StringLocalizer![winnerteamstring]);
        AdvancedServerPrintToChatAll(Configs.Shared.StringLocalizer!["chat.message.winner.team.report"],T_players_CT,T_Health_CT,T_players_T,T_Health_T);
    }
    
    public static void GetWantedTeam()
    {
        var g_Main = KnifeRoundGoldKingZ.Instance.g_Main;
        int VotesToCt = g_Main.Player_Data.Values.Count(data => data.TeamVoted == CsTeam.CounterTerrorist);
        int VotesToT = g_Main.Player_Data.Values.Count(data => data.TeamVoted == CsTeam.Terrorist);

        if(VotesToCt > VotesToT)
        {
            g_Main.WantedTeam = CsTeam.CounterTerrorist;
        }else if(VotesToT > VotesToCt)
        {
            g_Main.WantedTeam = CsTeam.Terrorist;
        }else
        {
            g_Main.WantedTeam = g_Main.WinerTeam;
        }
    }

    public static void SwitchTeams()
    {
        var g_Main = KnifeRoundGoldKingZ.Instance.g_Main;
        CsTeam wantedteam = g_Main.WantedTeam == CsTeam.None ? g_Main.WinerTeam : g_Main.WantedTeam;
        CsTeam Loserteam = wantedteam == CsTeam.Terrorist ? CsTeam.CounterTerrorist : CsTeam.Terrorist;
        
        foreach(var player in GetPlayersController(true, false))
        {
            if (player == null || !player.IsValid) continue;

            if (player.TeamNum == (byte)g_Main.WinerTeam)
            {
                player.SwitchTeam(wantedteam);
            }
            else
            {
                player.SwitchTeam(Loserteam);
            }
        }

        ModifyConvar_MatchLive();
        g_Main.ShowCenter = false;
        Server.ExecuteCommand("mp_restartgame 1");
    }
    public static void FreezeCheck()
    {
        if(!Configs.GetConfigData().Freeze_Players_On_Vote_Started)return;
        var g_Main = KnifeRoundGoldKingZ.Instance.g_Main;

        if(g_Main.GameMode == 0)return;

        foreach(var players in GetPlayersController(true,false))
        {
            if (players == null || !players.IsValid)continue;
            if(g_Main.GameMode == 2)
            {
                FreezePlayer(players);
            }else
            {
                UnFreezePlayer(players);
            }
        }
    }

    public static void FreezePlayer(CCSPlayerController? player)
    {
        if (player == null || !player.IsValid ||
        player.Pawn == null || !player.Pawn.IsValid ||
        player.Pawn.Value == null || !player.Pawn.Value.IsValid) return;

        player.Pawn.Value!.MoveType = MoveType_t.MOVETYPE_NONE;
        Schema.SetSchemaValue(player.Pawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 0);
        Utilities.SetStateChanged(player.Pawn.Value, "CBaseEntity", "m_MoveType");
    }

    public static void UnFreezePlayer(CCSPlayerController? player)
    {
        if (player == null || !player.IsValid ||
        player.Pawn == null || !player.Pawn.IsValid ||
        player.Pawn.Value == null || !player.Pawn.Value.IsValid) return;

        player.Pawn.Value!.MoveType = MoveType_t.MOVETYPE_WALK;
        Schema.SetSchemaValue(player.Pawn.Value.Handle, "CBaseEntity", "m_nActualMoveType", 2);
        Utilities.SetStateChanged(player.Pawn.Value, "CBaseEntity", "m_MoveType");
    }
}