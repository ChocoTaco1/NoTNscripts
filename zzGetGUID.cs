// GetGUID.cs
//
// To get the guid of the client(If they have one) for reference.
// Generally for normal server things that need it.
// Things like stats, banning, logging, etc.
//
// For servers that arnt running Tribesnext but still want to use the GUID system.
// NOTE: THIS IS NOT A TRIBESNEXT SYSTEM CERTIFICATION REPLACEMENT
// This is unsecure, as in no hashing or certificate checking occurs.
// GUIDs are not verified and run the risk of being altered clientside.
// Use at your own risk.

// This file is here
$GetGUID = true;

package TNspoof
{
	
function GameConnection::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch)
{
  if (%client.t2csri_serverChallenge $= "" && !%client.isAIControlled())
  {
	 if(!%client.poke)
	 {
		%client.tname = %name;
		%client.trgen = %raceGender;
		%client.tskin = %skin;
		%client.tvoic = %voice;
		%client.tvopi = %voicePitch;
		
		//poke the client
		commandToClient(%client, 't2csri_pokeClient', "T2CSRI 1.1 - 03/18/2009");
		//start the 15 second count down
		%client.tterm = schedule(15000, 0, t2csri_expireClient, %client);
		%client.poke = 1;
	 }
	 return; //reject until we get sendChallenge from client
  }
	 
  if (isEventPending(%client.tterm))
		cancel(%client.tterm);
		
  error("GUID =" SPC getField(%client.t2csri_cert, 1) SPC "Name =" SPC getField(%client.t2csri_cert, 0));
  //%client.nameBase = getField(%client.t2csri_cert, 0);// only for testing 
  //%client.guid = getField(%client.t2csri_cert, 1); // only for testing
  parent::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch);  
  
  %client.doneAuthenticating = 1;
}
	
};

// Prevent package from being activated if it is already
if (!isActivePackage(TNspoof))
    activatePackage(TNspoof);

// TN cert data we only really care about name and guid field 0 and 1
function serverCmdt2csri_sendCertChunk(%client, %chunk)
{
	if (%client.doneAuthenticating)
		return;
	
	%client.t2csri_cert = %client.t2csri_cert @ %chunk;
	
	if (strlen(%client.t2csri_cert) > 20000)
	{
	   %client.setDisconnectReason("Account certificate too long. Check your account key for corruption.");
	   %client.delete();
	}
}

// Only needed this to mark the end of the data and to allow the client in 
function serverCmdt2csri_sendChallenge(%client, %clientChallenge)
{ 
   %client.t2csri_serverChallenge = %clientChallenge;
   %client.onConnect(%client.tname, %client.trgen, %client.tskin, %client.tvoic, %client.tvopi); // Retry 
}

// Delete a client if they spend more than 15 seconds authenticating
function t2csri_expireClient(%client)
{
	if (!isObject(%client))
		return;
	
	%client.setDisconnectReason("This is a TribesNext server. You must install the TribesNext client to play. See www.tribesnext.com for info.");
	%client.delete();
}

// Format
function GameConnection::getAuthInfo(%client)
{
	%user = getField(%client.t2csri_cert, 0);
	%guid = getField(%client.t2csri_cert, 1);
	return %user @ "\t\t0\t" @ %guid @ "\n0\n";
}

// Unused, aka shutup
function serverCmdt2csri_sendCommunityCertChunk(%client, %chunk)
{
	return;
}

// Unused, aka shutup
function serverCmdt2csri_comCertSendDone(%client)
{
	return;  
}