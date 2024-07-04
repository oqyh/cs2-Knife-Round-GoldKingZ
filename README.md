## .:[ Join Our Discord For Support ]:.

![Discord Banner 2](https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2)

***
# [CS2] Knife-Round-GoldKingZ (1.1.0)

### Creates An Additional Round With Knifes After Warmup

![kniferound](https://github.com/oqyh/cs2-Knife-Round-GoldKingZ/assets/48490385/83968ac0-896c-40b1-8c59-602bc6962b01)

![knifewin](https://github.com/oqyh/cs2-Knife-Round-GoldKingZ/assets/48490385/fb2465cb-778f-4341-b633-8fa07d162b2a)


## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)

[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)


## .:[ Configuration ]:.
```json
{
  // Enable Vote Change Teams After Knife Round Win?
  "EnableVoteTeamSideAfterWinning": true,

  // if EnableVoteTeamSideAfterWinning Enabled Do You Like Everyone To Be Freeze On Voting?
  "FreezePlayersOnVoteStarted": true,

  // Block Team Changing On Voting And On Knife Round?
  "BlockTeamChangeOnVotingAndKnifeRound": true,

  // Allow All To Talk On Knife Round?
  "AllowAllTalkOnKnifeRound": true,

  // (0) = No
  // (2) = Give Armor Without Helmet
  // (3) = Give Armor With Helmet
  "GiveArmorOnKnifeRound": 2,

  // Time In Mins How Long Knife Round To Be
  "KnifeRoundXTimeInMins": 1,

  // Time In Secs How Long Voting To Be After Winning Knife Round
  "VoteXTimeInSecs": 50,

  // Time In Secs Show Message Knife Round Start On HUD
  "GiveHUDMessageOnStartKnifeRoundForXSecs": 15,

  // How Many Times Restart After Pick Side
  "AfterWinningRestartXTimes": 3,

  // Commands In Game To Vote CT Side
  "CommandsInGameToVoteCT": "!ct,.ct,ct",

  // Commands In Game To Vote T Side
  "CommandsInGameToVoteT": "!t,.t,t",
}
```

## .:[ Language ]:.
```json
{
	//==========================
	//        Colors
	//==========================
	//{Yellow} {Gold} {Silver} {Blue} {DarkBlue} {BlueGrey} {Magenta} {LightRed}
	//{LightBlue} {Olive} {Lime} {Red} {Purple} {Grey}
	//{Default} {White} {Darkred} {Green} {LightYellow}
	//HUD HTML Color Names List >> https://www.w3schools.com/colors/colors_names.asp
	//==========================
	//        Other
	//==========================
	//<br> = Next Line On Center HUD 
	//{nextline} = Print On Next Line
	//==========================
	
    "chat.message.knife.start": "{green}Gold KingZ | Knife Round! {nextline} {green}Gold KingZ | Knife Round! {nextline} {green}Gold KingZ | Knife Round!",
    "chat.message.match.start": "{green}Gold KingZ | LIVE! {nextline} {green}Gold KingZ | LIVE! {nextline} {green}Gold KingZ | LIVE!",
    
    "hud.message.kniferoundstarted": "<img src='https://raw.githubusercontent.com/oqyh/cs2-Knife-Round-GoldKingZ/main/Resources/knifeleft.png' class=''> <font color='orange'>Knife Round <img src='https://raw.githubusercontent.com/oqyh/cs2-Knife-Round-GoldKingZ/main/Resources/kniferight.png' class=''> <br> <br> <font color='blueviolet'>Winner Will Choose Team Side </font>",
	"hud.message.winnerteam": "<font color='green'>Vote Which Side To Pick <br> <font color='darkred'> = Time Left To Vote: {0} Secs = <br> <font color='yellow'>!ct <font color='grey'>To Go CT Side Team <br> <font color='yellow'>!t <font color='grey'>To Go T Side Team <br> <font color='grey'>Votes On <img src='https://raw.githubusercontent.com/oqyh/cs2-Knife-Round-GoldKingZ/main/Resources/ctimg.png' class=''> <font color='green'>[{1} <font color='grey'>/ <font color='green'>{3}] <br> <font color='grey'>Votes On <img src='https://raw.githubusercontent.com/oqyh/cs2-Knife-Round-GoldKingZ/main/Resources/timg.png' class=''> <font color='green'>[{2} <font color='grey'>/ <font color='green'>{3}] </font>",
    "hud.message.loseteam.ct": "<font color='yellow'>Waitng For <font color='red'>T's <font color='yellow'>To Vote </font>",
    "hud.message.loseteam.t": "<font color='yellow'>Waitng For <font color='RoyalBlue'>CT's <font color='yellow'>To Vote </font>"
}
```

## .:[ Change Log ]:.
```
(1.1.0)
-Upgrade Net.7 To Net.8
-Rework Knife-Round Plugin
-Added EnableVoteTeamSideAfterWinning
-Added CommandsInGameToVoteCT
-Added CommandsInGameToVoteT
-Added Lang chat.message.knife.start
-Added Lang chat.message.match.start
-Fix Remove Weapons
-Fix FreezePlayersOnVoteStarted
-Fix Lang HUD 

(1.0.9)
-Fix Some Bugs
-Added {19}IMAGE URL{20}

(1.0.8)
-Fix Some Bugs
-Fix Exploit Drop Gun Before Strip
-Fix [Lunix] KnifeRoundTimer Carry To All Rounds

(1.0.7)
-Fix Some Bugs
-Fix Remove Gloves
-Fix Remove Knifes

(1.0.6)
-Fix Some Bugs
-Added GiveArmorOnKnifeRound

(1.0.5)
-Fix Some Bugs
-Fix Windows Crash
-Remove mp_force_pick_time

(1.0.4)
-Fix Some Bugs
-Rework Knife Round For Better
-Added "AfterWinningRestartXTimes"

(1.0.3)
-Fix Some Bugs
-Fix Exploit Reconnect Spawn With Gun

(1.0.2)
-Fix Some Bugs
-Fix Exploit Droping Gun Before Round Start
-Fix Exploit Late Join To Spawn With Gun
-Fix Timer Carry 0 Timer to Next Map
-Fix Team Winner Counting
-Remove HLTV from Counting

(1.0.1)
-Fix Crash
-Fix Some Bugs
-Fix When Timer Hit 0 With No Vote Will Skip
-Added "MessageKnifeStartTimer"
-Added Multiple Languages

(1.0.0)
-Initial Release
```

## .:[ Donation ]:.

If this project help you reduce time to develop, you can give me a cup of coffee :)

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/oQYh)
