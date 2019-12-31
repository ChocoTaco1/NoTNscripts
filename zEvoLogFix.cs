package StartEvoLogFix
{

function CreateServer( %mission, %missionType )
{
	parent::CreateServer( %mission, %missionType );
	
	// Prevent package from being activated if it is already
	if (!isActivePackage(EvoLogFix))
		activatePackage(EvoLogFix);
}

};

// Prevent package from being activated if it is already
if (!isActivePackage(StartEvoLogFix))
	activatePackage(StartEvoLogFix);
	
package EvoLogFix
{

// connectLog(%client, %realname, %tag)
// Info: Logs the connections
function connectLog(%client)
{
   if($Host::EvoConnectLogging)
   {
	  // get the client info
	  %authInfo = getAuthInfo(%client);
	  %ip = getField(strreplace(%client.getAddress(),":","\t"),1);
	  
	  // connect info
	  %ConnectLogPlayerCount = $HostGamePlayerCount;
	  $ConnectLog = formatTimeString("d-M-yy") SPC formatTimeString("[HH:nn]") SPC %client.nameBase SPC "(" @ getField(%authInfo, 0) @ "," SPC getField(%authInfo, 1) @ "," @ "," SPC %ip @ ")" SPC "Pop[" @ %ConnectLogPlayerCount @ "]" SPC "Map[" @ $CurrentMission @ "]";

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
}

};