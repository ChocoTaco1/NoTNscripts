//This file is here
$GetGUID = 1;

//built to help linux server performance 
package TN{
	function GameConnection::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch){
	   if(%client.getAddress() !$= "Local"){   
         if (%client.t2csri_serverChallenge $= "" && !%client.isAIControlled()){
            if(!%client.poke){
               %client.tname = %name;
               %client.trgen = %raceGender;
               %client.tskin = %skin;
               %client.tvoic = %voice;
               %client.tvopi = %voicePitch;
               commandToClient(%client, 't2csri_pokeClient', "T2CSRI 1.1 - 03/18/2009");//poke the client
               %client.tterm = schedule(15000, 0, t2csri_expireClient, %client);// start the 15 second count down
               %client.poke = 1;
            }
            return;//reject untill we get sendChallenge
         }
            
         if (isEventPending(%client.tterm))
            cancel(%client.tterm);
            
         error("GUID =" SPC getField(%client.t2csri_cert, 1) SPC "Name =" SPC getField(%client.t2csri_cert, 0)); // only for testing
         %client.nameBase = getField(%client.t2csri_cert, 0);// only for testing 
         %client.guid = getField(%client.t2csri_cert, 1); // only for testing
         parent::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch);  
             
         %client.doneAuthenticating = 1;
	   }
	   else{
	      parent::onConnect(%client, %name, %raceGender, %skin, %voice, %voicePitch);  
	   }
	}
//TN cert data we only really care about name and guid field 0 and 1
function serverCmdt2csri_sendCertChunk(%client, %chunk){
	if (%client.doneAuthenticating)
		return;
	%client.t2csri_cert = %client.t2csri_cert @ %chunk;
	if (strlen(%client.t2csri_cert) > 20000){
	   %client.setDisconnectReason("Account certificate too long. Check your account key for corruption.");
		%client.delete();
	}
}

//only needed this to mark the end of the data and to allow the client in 
function serverCmdt2csri_sendChallenge(%client, %clientChallenge){ 
   %client.t2csri_serverChallenge = %clientChallenge;
   %user = strReplace(getField(%client.t2csri_cert, 0), "\x27", "\\\x27");
	%guid = getField(%client.t2csri_cert, 1);
	
	%client.t2csri_authinfo = %user @ "\t\t0\t" @ %guid @ "\n0\n";
	
   %comCert = %client.t2csri_comCert;
	if (strLen(%comCert) > 0)
	{
		// assuming there is a comCert, and we aren't running in bare mode
		if (getField(%comCert, 3) $= %guid)
		{
			// GUID in the community cert matches that of the account cert
			%client.t2csri_authinfo = %client.t2csri_comInfo;
		}
		else
		{
			// uh oh... someone's being naughty.. valid cert, but for a different player. kill them!
			%client.setDisconnectReason("Community supplemental certificate doesn't match account certificate.");
			%client.delete();
			return;
		}
	}
   %client.onConnect(%client.tname, %client.trgen, %client.tskin, %client.tvoic, %client.tvopi);// retry 
}

//delete a client if they spend more than 15 seconds authenticating
function t2csri_expireClient(%client){
	if (!isObject(%client))
		return;
	%client.setDisconnectReason("This is a TribesNext server. You must install the TribesNext client to play. See www.tribesnext.com for info.");
	%client.delete();
}

function serverCmdt2csri_sendCommunityCertChunk(%client, %chunk){//unused
	// client can only send in one community cert
	if (%client.t2csri_sentComCertDone)
		return;

	%client.t2csri_comCert = %client.t2csri_comCert @ %chunk;
	if (strlen(%client.t2csri_comCert) > 20000)
	{
		%client.setDisconnectReason("Community certificate is too long.");
		%client.delete();
		return;
	}
}

function serverCmdt2csri_comCertSendDone(%client){//unused
if (%client.t2csri_sentComCertDone)
		return;

	%comCert = %client.t2csri_comCert;
	if (getFieldCount(%comCert) != 6)
	{
		%client.setDisconnectReason("Community certificate format is invalid.");
		%client.delete();
		return;
	}

	// parse
	%dceNum = getField(%comCert, 0);
	%issued = getField(%comCert, 1);
	%expire = getField(%comCert, 2);
	%guid = getField(%comCert, 3);
	%blob = getField(%comCert, 4);
	%sig = getField(%comCert, 5);
	%sumStr = getFieldS(%comCert, 0, 4);
	%calcSha = sha1Sum(%sumStr);

	// valid cert... set the field for processing in the auth-phase code
	%len = strlen(%blob);
	for (%i = 0; %i < %len; %i += 2)
	{
		%decoded = %decoded @ collapseEscape("\\x" @ getSubStr(%blob, %i, 2));
	}
	%client.t2csri_comInfo = %decoded @ "\n";
	%client.t2csri_sentComCertDone = 1;
}
};
if(!isActivePackage(TN))
   activatePackage(TN);
   
   //schedule(2000,0,"activatePackage", TN);