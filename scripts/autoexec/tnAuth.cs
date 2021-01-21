// This file is here
$GetGUID = true;

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
   %client.onConnect(%client.tname, %client.trgen, %client.tskin, %client.tvoic, %client.tvopi);// retry
}

//delete a client if they spend more than 15 seconds authenticating
function t2csri_expireClient(%client){
	if (!isObject(%client))
		return;
	%client.setDisconnectReason("This is a TribesNext server. You must install the TribesNext client to play. See www.tribesnext.com for info.");
	%client.delete();
}

function GameConnection::getAuthInfo(%client){
   %user = getField(%client.t2csri_cert, 0);
   %guid = getField(%client.t2csri_cert, 1);
   return %user @ "\t\t0\t" @ %guid @ "\n0\n";
}

function serverCmdt2csri_sendCommunityCertChunk(%client, %chunk){//unused
	return;
}

function serverCmdt2csri_comCertSendDone(%client){//unused
	return;
}
};
if(!isActivePackage(TN))
   activatePackage(TN);

   //schedule(2000,0,"activatePackage", TN);
