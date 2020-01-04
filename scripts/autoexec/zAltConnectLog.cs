// zAltConnectLog
//
// This is to get around connectlog not working properly in evo in linux
// Just make our own and call it

//Enable Console Logs
setLogMode(1);

//All other connect logging is off
$Host::ClassicConnectLog = 0;
$Host::EvoConnectLogging = 0;

//********** Moved to zTNspoof.cs **********

//package AltConnectLog
//{
	
//function GameConnection::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch)
//{
//	parent::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch);
//	
//	// Log the connection
//	Alt_connectLog(%client);
//}
	
//};

// Prevent package from being activated if it is already
//if (!isActivePackage(AltConnectLog))
//    activatePackage(AltConnectLog);

//********************************************

// connectLog(%client, %realname, %tag)
// Info: Logs the connections
function Alt_connectLog(%client)
{
	// get the client info
	%authInfo = %client.getAuthInfo();

	// connect info
	%ConnectLogPlayerCount = $HostGamePlayerCount;
	$ConnectLog = formatTimeString("d-M-yy") SPC formatTimeString("[HH:nn]") SPC %client.nameBase SPC "(" @ getField(%authInfo, 0) @ "," SPC %client.guid @ "," SPC %client.getAddress() @ ")" SPC "Pop[" @ %ConnectLogPlayerCount @ "]" SPC "Map[" @ $CurrentMission @ "]";

	// log the message
	if($Host::EvoDailyLogs)
	{
		if(formatTimeString("HH") > getSubStr($Host::EvoDailyHour, 0, strstr($Host::EvoDailyHour, ":")) || (formatTimeString("HH") == getSubStr($Host::EvoDailyHour, 0, strstr($Host::EvoDailyHour, ":")) && formatTimeString("nn") >= getSubStr($Host::EvoDailyHour, strstr($Host::EvoDailyHour, ":")+1, 2)))
			export("$ConnectLog", "logs/Connect/ConnectLog-" @ formatTimeString("d-M-yy") @ ".txt", true);
		else
		{
			%yesterday = formatTimeString("d") - 1;
			export("$ConnectLog", "logs/Connect/ConnectLog-" @ %yesterday @ formatTimeString("-M-yy") @ ".txt", true);
		}
	}
	else
	export("$ConnectLog", "logs/Connect/ConnectLog.txt", true);
}