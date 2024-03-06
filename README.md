# [CS2] Knife-Round (1.0.9)

### Creates An Additional Round With Knifes After Warmup

![Untitled](https://github.com/oqyh/cs2-Knife-Round/assets/48490385/5ca4d4a7-b103-42a6-9334-619d9c82a9f8)

![Untitled3](https://github.com/oqyh/cs2-Knife-Round/assets/48490385/c354f51b-3bfb-4e16-a571-4268e1edcc7b)

![Untitled4](https://github.com/oqyh/cs2-Knife-Round/assets/48490385/7d017fa3-6e10-4ea7-bab9-8a7204fb7a98)


## .:[ Dependencies ]:.
[Metamod:Source (2.x)](https://www.sourcemm.net/downloads.php/?branch=master)

[CounterStrikeSharp (152 And Above)](https://github.com/roflmuffin/CounterStrikeSharp/releases)


## .:[ Configuration ]:.
```json
{
  //Give Armor On Knife Round?
  //(0) = No
  //(1) = Give Armor
  //(2) = Give Armor + Helmet
  "GiveArmorOnKnifeRound": 2,
  
  //Freeze Players On Voting
  "FreezeOnVote": true,
  
  //Block Team Changing (To Avoid Loser Switch To Winner Team And Vote)
  "BlockTeamChangeOnVoteAndKnife": true,
  
  //Allow All Players To Hear Each Other On Knife Round Only
  "AllowAllTalkOnKnifeRound": true,
  
  //Knife Round Time (In Mins)
  "KnifeRoundTimer": 1,
  
  //Time To Vote Pick Team (In Secs)
  "VoteTimer": 50,
  
  //Message On Knife Round Start ("Knife_Start_Message") (In Secs)
  "MessageKnifeStartTimer": 25,
  
  //After Winner Pick Team How Many Restart Would You Like
  "AfterWinningRestartXTimes": 3,
  
//-----------------------------------------------------------------------------------------
  "ConfigVersion": 1
}
```

## .:[ Language ]:.
```json
{
	//==========================
	//        Colors
	//==========================
	//Red = {1}TEXT{0}
	//Cyan = {2}TEXT{0}
	//Blue = {3}TEXT{0}
	//DarkBlue = {4}TEXT{0}
	//LightBlue = {5}TEXT{0}
	//Purple = {6}TEXT{0}
	//Yellow = {7}TEXT{0}
	//Lime = {8}TEXT{0}
	//Magenta = {9}TEXT{0}
	//Pink = {10}TEXT{0}
	//Grey = {11}TEXT{0}
	//Green = {12}TEXT{0}
	//Orange = {13}TEXT{0}
	//==========================
	//        Others
	//==========================
	//Next Line = {14}
	//Current Votes On CT = {15}
	//Current Votes On T = {16}
	//Votes Needed = {17}
	//Time Vote = {18}
	//IMAGE = {19}IMAGE URL{20}

	"Knife_Start_Message": "{19}https://i.imgur.com/fV6lerG.png{20} {13}Knife Round{0} {19}https://i.imgur.com/BiIaTFk.png{20} {14} {14} {6}Winner Will Choose Team Side{0}",
	"Winner_Message": "{6}Vote Which Side To Pick{0} {14} {1}==Time Left To Vote: {18} Secs =={0} {14} {7}!ct{0} {11}To Go CT Side Team{0} {14} {7}!t{0} {11}To Go T Side Team{0} {14} Votes On {19}https://i.imgur.com/o2kWYOA.png{20} [{15} / {17}] {14} Votes On {19}https://i.imgur.com/m0b0TLv.png{20} [{16} / {17}]",
	"When_T_Lose": "{13}Waitng For{0} {3}CT's{0} {13}To Vote{0}",
	"When_CT_Lose": "{13}Waitng For{0} {1}T's{0} {13}To Vote{0}"
}
```

## .:[ Change Log ]:.
```
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
