## .:[ Join Our Discord For Support ]:.

<a href="https://discord.com/invite/U7AuQhu"><img src="https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2"></a>

# [CS2] Knife-Round-GoldKingZ (1.1.4)

Creates An Additional Round With Knifes After Warmup

![Plugin Preview](https://github.com/oqyh/cs2-Knife-Round-GoldKingZ/assets/48490385/83968ac0-896c-40b1-8c59-602bc6962b01)

![Winner Selection](https://github.com/oqyh/cs2-Knife-Round-GoldKingZ/assets/48490385/fb2465cb-778f-4341-b633-8fa07d162b2a)

---

## 📦 Dependencies
[![Metamod:Source](https://img.shields.io/badge/Metamod:Source-2.x-2d2d2d?logo=sourceengine)](https://www.sourcemm.net/downloads.php?branch=dev)

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-83358F)](https://github.com/roflmuffin/CounterStrikeSharp)

---

## 📥 Installation

1. Download latest release
2. Extract to `csgo` directory
3. Configure `Knife-Round-GoldKingZ\config\config.json`
4. Restart server

---

## ⚙️ Configuration

> [!NOTE]
> Located In ..\Knife-Round-GoldKingZ\config\config.json                                           
>

| Property | Description | Values | Required |  
|----------|-------------|--------|----------|  
| `Minimum_Players_Needed` | Minimum players to start knife round | `Integer` (e.g., `5`) | - |  
| `Count_Bots_As_Players` | Count bots as players in `Minimum_Players_Needed` | `true`/`false` | - |  
| `After_Winning_Vote_Team_Side` | Team side selection after knife round |  `true` = Vote<br>`false` = Auto-assign | - |  
| `Send_Winner_Team_To_CT` | Auto-assign winner team to CT | `true` = Force CT<br>`false` = Keep original side | `After_Winning_Vote_Team_Side=false` |  
| `Get_Winner_Team` | Winner determination method | `1`-Alive players (CT if tie)<br>`2`-Alive players (T if tie)<br>`3`-Total health (CT if tie)<br>`4`-Total health (T if tie)<br>`5`-Alive→Health→CT<br>`6`-Alive→Health→T | - |  
| `Freeze_Players_On_Vote_Started` | Freeze players during voting | `true`/`false` | `After_Winning_Vote_Team_Side=true` |  
| `Block_Switching_Teams_On` | Restrict team switching | `0`=Never<br>`1`=Knife round<br>`2`=Knife+Voting<br>`3`=All phases | - |  
| `Allow_All_Talk_On_Knife_Round` | Global voice chat during knife round | `true`/`false` | - |  
| `Give_Armor_On_Knife_Round` | Player armor configuration | `0`=No armor<br>`1`=Vest<br>`2`=Vest + Helmet | - |  
| `Knife_Round_Time` | Knife round duration (minutes) | `Float` (e.g., `1.0`) | - |  
| `Vote_Team_Side_Time` | Voting phase duration (seconds) | `Integer` (e.g., `20`) | `After_Winning_Vote_Team_Side=true` |  
| `Knife_Round_Center_Message_Time` | Center message display time (seconds) | `Integer` (e.g., `5`) | - |  
| `Restart_On_Match_Live` | Match restarts before match live | `Integer` (e.g., `1`) | - |  
| `Commands_In_Game_To_Vote_CT` | Commands to vote for CT side | Comma-separated (e.g., `!ct, !stay`) | `After_Winning_Vote_Team_Side=true` |  
| `Commands_In_Game_To_Vote_T` | Commands to vote for T side | Comma-separated (e.g., `!t, !switch`) | `After_Winning_Vote_Team_Side=true` |  
| `Execute_Cfg_On_Warmup` | Warmup cfg file | Path (e.g., `cfg/KnifeRound/Warmup.cfg`) | - |  
| `Execute_Cfg_On_KnifeRound` | Knife round cfg file | Path (e.g., `cfg/KnifeRound/Knife.cfg`) | - |  
| `Execute_Cfg_On_Voting` | Voting choose side cfg file | Path (e.g., `cfg/KnifeRound/Vote.cfg`) | - |  
| `Execute_Cfg_On_MatchLive` | Live match cfg file | Path (e.g., `cfg/KnifeRound/Live.cfg`) | - |  
| `EnableDebug` | Debug mode | `true`/`false` | - |  


---

## 🌍 Language

> [!NOTE]
> Located In ..\Knife-Round-GoldKingZ\lang\en.json                                           
>

<details>
<summary>🖼️ Preview Colors In Game Codes (Click to expand 🔽)</summary>

![Color Preview](https://github.com/oqyh/cs2-Game-Manager/assets/48490385/3df7caa9-34a7-47da-94aa-8d682f59e85d)
</details>

```json
{
	//==========================
	//        Colors
	//==========================
	//{Yellow} {Gold} {Silver} {Blue} {DarkBlue} {BlueGrey} {Magenta} {LightRed}
	//{LightBlue} {Olive} {Lime} {Red} {Purple} {Grey}
	//{Default} {White} {Darkred} {Green} {LightYellow}
	//==========================
	//        Other
	//==========================
	//{nextline} = Print On Next Line
	//==========================

	"chat.message.knife.prepare": "{grey}[{green}Gold KingZ{grey}] After WarmUp Knife Round Will Start {green}Winner {grey}Choose Team Side",
	"chat.message.knife.ignored": "{grey}[{green}Gold KingZ{grey}] Knife Round Ignored Less Players [{green}{0} {grey}/ {green}{1} {grey}Needed{grey}]",
	"chat.message.knife.start": "{grey}[{green}Gold KingZ{grey}] {lime}Knife Round! {nextline} {grey}[{green}Gold KingZ{grey}] {lime}Knife Round! {nextline} {grey}[{green}Gold KingZ{grey}] {lime}Knife Round!",
	"chat.message.winner.team.ct": "{grey}[{green}Gold KingZ{grey}] Knife Round End Winner Team {darkblue}CounterTerrorist",
	"chat.message.winner.team.t": "{grey}[{green}Gold KingZ{grey}] Knife Round End Winner Team {darkred}Terrorist",
	"chat.message.winner.team.report": "{grey}----------------------{green}[Reports]{grey}---------------------- {nextline} {grey}{darkblue}[CounterTerrorist] {nextline} {grey}Total Alive Players: {yellow}{0} {grey}| Total Healths: {yellow}{1} {nextline} {grey}{darkred}[Terrorist] {nextline} {grey}Total Alive Players: {yellow}{2} {grey}| Total Healths: {yellow}{3} {nextline} {grey}-------------------------------------------------------",
	"chat.message.match.start": "{grey}[{green}Gold KingZ{grey}] {lime}Match Live! {nextline} {grey}[{green}Gold KingZ{grey}] {lime}Match Live! {nextline} {grey}[{green}Gold KingZ{grey}] {lime}Match Live!",

	"hud.message.kniferoundstarted": "<img src='https://raw.githubusercontent.com/oqyh/cs2-Knife-Round-GoldKingZ/main/Resources/knifeleft.png' class=''> <font color='orange'>Knife Round <img src='https://raw.githubusercontent.com/oqyh/cs2-Knife-Round-GoldKingZ/main/Resources/kniferight.png' class=''> <br> <br> <font color='blueviolet'>Winner Will Choose Team Side </font>",

	//{0} = Time Left For Voting 
	//{1} = Total Votes CT
	//{2} = Total Votes T
	//{3} = Total Votes Needed
	"hud.message.winnerteam": "<font color='green'>Vote Which Side To Pick <br> <font color='darkred'> = Time Left To Vote: {0} Secs = <br> <font color='yellow'>!ct <font color='grey'>To Vote CT Side Team <br> <font color='yellow'>!t <font color='grey'>To Vote T Side Team <br> <font color='grey'>Votes On <img src='https://raw.githubusercontent.com/oqyh/cs2-Knife-Round-GoldKingZ/main/Resources/ctimg.png' class=''> <font color='green'>[{1} <font color='grey'>/ <font color='green'>{3}] <br> <font color='grey'>Votes On <img src='https://raw.githubusercontent.com/oqyh/cs2-Knife-Round-GoldKingZ/main/Resources/timg.png' class=''> <font color='green'>[{2} <font color='grey'>/ <font color='green'>{3}] </font>",
	"hud.message.loseteam.ct": "<font color='yellow'>Waitng For <font color='red'>T's <font color='yellow'>To Vote <br> <font color='darkred'> = Time Left To Vote: {0} Secs = </font>",
	"hud.message.loseteam.t": "<font color='yellow'>Waitng For <font color='RoyalBlue'>CT's <font color='yellow'>To Vote <br> <font color='darkred'> = Time Left To Vote: {0} Secs = </font>"
}
```

---

## 📜 Changelog

<details>
<summary>📋 View Version History (Click to expand 🔽)</summary>

### [1.1.4]
- Rework Plugin
- Fix Knife Round Crash
- Fix Method Dropping Weapons
- Fix Give Armor On Knife Round
- Fix After_Winning_Vote_Team_Side
- Fix/Added FallBack Knife Round If Warmup Start Again 
- Added Send_Winner_Team_To_CT
- Added Get_Winner_Team
- Added Block_Switching_Teams_On 3 Modes
- Added Execute_Cfg_On_Warmup
- Added Execute_Cfg_On_KnifeRound
- Added Execute_Cfg_On_Voting
- Added Execute_Cfg_On_MatchLive
- Added EnableDebug
- Added In config.json info on each what it do
- Added In lang "chat.message.knife.prepare"
- Added In lang "chat.message.winner.team.ct"
- Added In lang "chat.message.winner.team.t"
- Added In lang "chat.message.winner.team.report"

### [1.1.3]
- Rework On Less MinimumPlayers (Removed Restart)

### [1.1.2]
- Fix Plugin Will Stop Working On Next Map

### [1.1.1]
- Added MinimumPlayersToEnableKnifePlugin
- Added CountBotsAsPlayers
- Added Lang chat.message.knife.ignored 

### [1.1.0]
- Upgrade Net.7 To Net.8
- Rework Knife-Round Plugin
- Added EnableVoteTeamSideAfterWinning
- Added CommandsInGameToVoteCT
- Added CommandsInGameToVoteT
- Added Lang chat.message.knife.start
- Added Lang chat.message.match.start
- Fix Remove Weapons
- Fix FreezePlayersOnVoteStarted
- Fix Lang HUD 

### [1.0.9]
- Fix Some Bugs
- Added {19}IMAGE URL{20}

### [1.0.8]
- Fix Some Bugs
- Fix Exploit Drop Gun Before Strip
- Fix [Lunix] KnifeRoundTimer Carry To All Rounds

### [1.0.7]
- Fix Some Bugs
- Fix Remove Gloves
- Fix Remove Knifes

### [1.0.6]
- Fix Some Bugs
- Added GiveArmorOnKnifeRound

### [1.0.5]
- Fix Some Bugs
- Fix Windows Crash
- Remove mp_force_pick_time

### [1.0.4]
- Fix Some Bugs
- Rework Knife Round For Better
- Added "AfterWinningRestartXTimes"

### [1.0.3]
- Fix Some Bugs
- Fix Exploit Reconnect Spawn With Gun

### [1.0.2]
- Fix Some Bugs
- Fix Exploit Droping Gun Before Round Start
- Fix Exploit Late Join To Spawn With Gun
- Fix Timer Carry 0 Timer to Next Map
- Fix Team Winner Counting
- Remove HLTV from Counting

### [1.0.1]
- Fix Crash
- Fix Some Bugs
- Fix When Timer Hit 0 With No Vote Will Skip
- Added "MessageKnifeStartTimer"
- Added Multiple Languages

### [1.0.0]
- Initial Release

</details>

---
