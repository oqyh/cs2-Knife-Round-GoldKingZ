using System.Diagnostics;

namespace Knife_Round_GoldKingZ;

public class Globals
{
    public static List<ulong> VoteCountCT = new();
    public static List<ulong> VoteCountT = new();
    
    public static Stopwatch Stopwatch = new Stopwatch();

    public static bool CTWINNER = false;
    public static bool TWINNER = false;
    public static float Timer = 0;

    public static int currentVotesT;
    public static int currentVotesCT;

    public static float mp_roundtime;
    public static float mp_roundtime_defuse;
    public static float mp_team_intro_time;
    public static bool sv_alltalk;
    public static bool sv_deadtalk;
    public static bool sv_full_alltalk;
    public static bool sv_talk_enemy_dead;
    public static bool sv_talk_enemy_living;

    public static bool BlockTeam = false;
    public static bool OnWarmUp = false;
    public static bool PrepareKnifeRound = false;
    public static bool KnifeRoundStarted = false;
    public static bool KnifeModeStartMessage = false;
    public static bool RemoveWeapons = false;
}