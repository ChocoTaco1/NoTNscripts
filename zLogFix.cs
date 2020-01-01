//Enable Console Logs
setLogMode(1);

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
function connectLog(%client, %logthis)
{
   if(%logthis)
   {
		parent::connectLog(%client, %logthis);
   }
}

function GameConnection::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch)
{
   // Call the standard procedure ...
   Parent::onConnect( %client, %name, %raceGender, %skin, %voice, %voicePitch );

   // Log the connection
   %logthis = true;
   connectLog(%client, %logthis);
}

};