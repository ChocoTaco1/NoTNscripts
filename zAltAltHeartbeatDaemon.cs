// zAltAltHeartbeatDaemon.cs
//
// Alternate Alternate Heartbeat Deamon
// The alternative to the alternative heartbeat for non tribesnext servers
// To still show up tho, on the tribesnext master server. 
//

// Tribesnext Beat Stock Values
// Dont change these
$Host::TN::beat = 3;
$Host::TN::echo = 1;


package AlternateAlternateHeartbeat
{
	
function CreateServer( %mission, %missionType )
{
	parent::CreateServer( %mission, %missionType );
	
	//Start the Alternate Alternate Heartbeat
	AlternateAlternateHeartbeat();
}

function AlternateAlternateHeartbeat()
{
	if(isEventPending($AltAltHBeat)) 
		cancel($AltAltHBeat);
	
    // Create an HTTP object for communications  
    if (!isObject(TNMasterServer))
      %watup = new HTTPObject(TNMasterServer){};
    else 
	  %watup = TNMasterServer;

    // Specify a URL to transmit to
    %server = "master.tribesnext.com:80";

    // Specify a URI to communicate with
	if ($Host::BindAddress !$= "")
      %URI = "/add/" @ $Host::Port @ "/" @ $Host::BindAddress;
    else 
	  %URI = "/add/" @ $Host::Port;

    // Send the GET command to the server
	%watup.mode = 1;
    %watup.get(%server, %URI);

    if ($Host::TN::echo)
		echo("-- Sent heartbeat to TN Master. (" @ %server @ ")" );

    //Call itself again.
    $AltAltHBeat = schedule($Host::TN::beat * 60000, 0, "AlternateAlternateHeartbeat" );
}

};

// Prevent package from being activated if it is already
if (!isActivePackage(AlternateAlternateHeartbeat))
    activatePackage(AlternateAlternateHeartbeat);