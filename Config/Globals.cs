using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using System.Diagnostics;

namespace Knife_Round_GoldKingZ;

public class Globals
{
    public class PlayerDataClass
    {
        public CCSPlayerController Player { get; set; }
        public CsTeam TeamVoted { get; set; }        
        public PlayerDataClass(CCSPlayerController player, CsTeam teamVoted)
        {
            Player = player;
            TeamVoted = teamVoted;
        }
    }
    public Dictionary<CCSPlayerController, PlayerDataClass> Player_Data = new Dictionary<CCSPlayerController, PlayerDataClass>();

    public CounterStrikeSharp.API.Modules.Timers.Timer? WarmUpTimer;
    public CounterStrikeSharp.API.Modules.Timers.Timer? ForceStripe;

    public int mp_free_armor = 0;

    public float mp_roundtime = 0.0f;
    public float mp_roundtime_defuse = 0.0f;
    public float mp_team_intro_time = 0.0f;
    public float mp_warmuptime = 0.0f;
    


    public bool OneTime = false;
    public bool BlockTeamChange = false;
    public bool sv_alltalk = false;
    public bool sv_full_alltalk = false;
    public bool sv_talk_enemy_dead = false;
    public bool sv_talk_enemy_living = false;
    public bool sv_deadtalk = false;


    public int GameMode = 0;
    public CsTeam WinerTeam = CsTeam.None;
    public CsTeam WantedTeam = CsTeam.None;

    public bool ShowCenter = false;
    public int TickTime;
    public DateTime LastTickTime;
}