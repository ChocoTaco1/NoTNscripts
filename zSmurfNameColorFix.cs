// zSmurfNameColorFix.cs
//
// Takes out the Alias color change for smurfs
// So when youre running a server without tribesnext
// Every name isnt Alias blue
//

package SmurfFix
{

function GameConnection::onConnect( %client, %name, %raceGender, %skin, %voice, %voicePitch )
{
   %client.setMissionCRC($missionCRC);
   sendLoadInfoToClient( %client );

   //%client.setSimulatedNetParams(0.1, 30);

   // z0dd - ZOD, 9/29/02. Removed T2 demo code from here

   // ---------------------------------------------------
   // z0dd - ZOD, 9/29/02. Removed T2 demo code from here

   // if hosting this server, set this client to superAdmin
   if(%client.getAddress() $= "Local")
   {   
      %client.isAdmin = true;
      %client.isSuperAdmin = true;
   }
   // Get the client's unique id:
   %authInfo = %client.getAuthInfo();
   %client.guid = getField( %authInfo, 3 );

   // check admin and super admin list, and set status accordingly
   if ( !%client.isSuperAdmin )
   {
      if ( isOnSuperAdminList( %client ) )
      {   
         %client.isAdmin = true;
         %client.isSuperAdmin = true;   
      }
      else if( isOnAdminList( %client ) )
      {
         %client.isAdmin = true;
      }
   }

   // Sex/Race defaults
   switch$ ( %raceGender )
   {
      case "Human Male":
         %client.sex = "Male";
         %client.race = "Human";
      case "Human Female":
         %client.sex = "Female";
         %client.race = "Human";
      case "Bioderm":
         %client.sex = "Male";
         %client.race = "Bioderm";
      default:
         error("Invalid race/gender combo passed: " @ %raceGender);
         %client.sex = "Male";
         %client.race = "Human";
   }
   %client.armor = "Light";

   // Override the connect name if this server does not allow smurfs:
   %realName = getField( %authInfo, 0 );
   if ( $PlayingOnline && $Host::NoSmurfs )
      %name = %realName;

   if ( strcmp( %name, %realName ) == 0 )
   {
      %client.isSmurf = false;

      //make sure the name is unique - that a smurf isn't using this name...
      %dup = -1;
      %count = ClientGroup.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %test = ClientGroup.getObject( %i );
         if (%test != %client)
         {
	    %rawName = stripChars( detag( getTaggedString( %test.name ) ), "\cp\co\c6\c7\c8\c9" );
            if (%realName $= %rawName)
            {
               %dup = %test;
               %dupName = %rawName;
               break;
            }
         }
      }

      //see if we found a duplicate name
      if (isObject(%dup))
      {
         //change the name of the dup
         %isUnique = false;
         %suffixCount = 1;
         while (!%isUnique)
         {
            %found = false;
            %testName = %dupName @ "." @ %suffixCount;
            for (%i = 0; %i < %count; %i++)
            {
               %cl = ClientGroup.getObject(%i);
	       %rawName = stripChars( detag( getTaggedString( %cl.name ) ), "\cp\co\c6\c7\c8\c9" );
               if (%rawName $= %testName)
               {
                  %found = true;
                  break;
               }
            }

            if (%found)
               %suffixCount++;
            else
               %isUnique = true;
         }

         //%testName will now have the new unique name...
         %oldName = %dupName;
         %newName = %testName;

         //MessageAll( 'MsgSmurfDupName', '\c2The real \"%1\" has joined the server.', %dupName );
         //MessageAll( 'MsgClientNameChanged', '\c2The smurf \"%1\" is now called \"%2\".', %oldName, %newName, %dup );

         %dup.name = addTaggedString(%newName);
         setTargetName(%dup.target, %dup.name);
      }

      // Add the tribal tag:
      %tag = getField( %authInfo, 1 );
      %append = getField( %authInfo, 2 );
      if ( %append )
         %name = "\cp\c6" @ %name @ "\c7" @ %tag @ "\co";
      else
         %name = "\cp\c7" @ %tag @ "\c6" @ %name @ "\co";

      %client.sendGuid = %client.guid;
   }
   else
   {
      %client.isSmurf = true;
      %client.sendGuid = 0;
      %name = stripTrailingSpaces( strToPlayerName( %name ) );
      if ( strlen( %name ) < 3 )
         %name = "Poser";
      
      // Make sure the alias is unique:
      %isUnique = true;
      %count = ClientGroup.getCount();
      for ( %i = 0; %i < %count; %i++ )
      {
         %test = ClientGroup.getObject( %i );
         %rawName = stripChars( detag( getTaggedString( %test.name ) ), "\cp\co\c6\c7\c8\c9" );
         if ( strcmp( %name, %rawName ) == 0 )
         {
            %isUnique = false;
            break;
         }
      }

      // Append a number to make the alias unique:
      if ( !%isUnique )
      {
         %suffix = 1;
         while ( !%isUnique )
         {
            %nameTry = %name @ "." @ %suffix;
            %isUnique = true;

            %count = ClientGroup.getCount();
            for ( %i = 0; %i < %count; %i++ )
            {
               %test = ClientGroup.getObject( %i );
               %rawName = stripChars( detag( getTaggedString( %test.name ) ), "\cp\co\c6\c7\c8\c9" );
               if ( strcmp( %nameTry, %rawName ) == 0 )
               {
                  %isUnique = false;
                  break;
               }
            }

            %suffix++;
         }

         // Success!
         %name = %nameTry;
      }

      %smurfName = %name;
      // Tag the name with the "smurf" color:
      //%name = "\cp\c8" @ %name @ "\co";
   }

   %client.name = addTaggedString(%name);
   if(%client.isSmurf)
      %client.nameBase = %smurfName;
   else
      %client.nameBase = %realName;

   //Allow - ChocoTaco
   // Make sure that the connecting client is not trying to use a bot skin:
   //%temp = detag( %skin );
   //if ( %temp $= "basebot" || %temp $= "basebbot" )
   //   %client.skin = addTaggedString( "base" );
   //else
        %client.skin = addTaggedString( %skin );

   %client.voice = %voice;
   %client.voiceTag = addtaggedString(%voice);
	   
   //set the voice pitch based on a lookup table from their chosen voice
   %client.voicePitch = getValidVoicePitch(%voice, %voicePitch);
   // z0dd - ZOD, 9/29/02. Removed T2 demo code from here
   // ---------------------------------------------------

   %client.justConnected = true;
   %client.isReady = false;

   // full reset of client target manager
   clientResetTargets(%client, false);

   %client.target = allocClientTarget(%client, %client.name, %client.skin, %client.voiceTag, '_ClientConnection', 0, 0, %client.voicePitch);
   %client.score = 0;
   %client.team = 0;
   
   $instantGroup = ServerGroup;
   $instantGroup = MissionCleanup;

   echo("CADD: " @ %client @ " " @ %client.getAddress());

   %count = ClientGroup.getCount();
   for(%cl = 0; %cl < %count; %cl++)
   {
      %recipient = ClientGroup.getObject(%cl);
      if((%recipient != %client))
      {
         // These should be "silent" versions of these messages...
         messageClient(%client, 'MsgClientJoin', "", 
               %recipient.name, 
               %recipient, 
               %recipient.target, 
               %recipient.isAIControlled(), 
               %recipient.isAdmin, 
               %recipient.isSuperAdmin, 
               %recipient.isSmurf, 
               %recipient.sendGuid);

         messageClient(%client, 'MsgClientJoinTeam', "", %recipient.name, $teamName[%recipient.team], %recipient, %recipient.team ); 
      }
   }

//   commandToClient(%client, 'getManagerID', %client);

   commandToClient(%client, 'setBeaconNames', "Target Beacon", "Marker Beacon", "Bomb Target");

   if ( $CurrentMissionType !$= "SinglePlayer" ) 
   {
      // z0dd - ZOD, 5/08/04. Send message of any gameplay changes
//      messageClient( %client, 'MsgClassic', 'Classic \c2Sniper Mod: \c3%1.', ($Host::ClassicLoadSniperChanges ? 'Enabled' : 'Disabled') );
//      messageClient( %client, 'MsgClassic', 'Classic \c2Missile Mod: \c3%1.', ($Host::ClassicLoadMissileChanges ? 'Enabled' : 'Disabled') );
//      messageClient( %client, 'MsgClassic', 'Classic \c2Mortar Mod: \c3%1.', ($Host::ClassicLoadMortarChanges ? 'Enabled' : 'Disabled') );
//      messageClient( %client, 'MsgClassic', 'Classic \c2Blaster Mod: \c3%1.', ($Host::ClassicLoadBlasterChanges ? 'Enabled' : 'Disabled') );
//      messageClient( %client, 'MsgClassic', 'Classic \c2Plasma Turret Mod: \c3%1.', ($Host::ClassicLoadPlasmaTurretChanges ? 'Enabled' : 'Disabled') );
//      messageClient( %client, 'MsgClassic', 'Classic \c2Player Mod: \c3%1.', ($Host::ClassicLoadPlayerChanges ? 'Enabled' : 'Disabled') );
//      messageClient( %client, 'MsgClassic', 'Classic \c2Havoc Mod: \c3%1.', ($Host::ClassicLoadHavocChanges ? 'Enabled' : 'Disabled') );
//      messageClient( %client, 'MsgClassic', 'Classic \c2Mine Mod: \c3%1.', ($Host::ClassicLoadMineChanges ? 'Enabled' : 'Disabled') );
//      messageClient( %client, 'MsgClassic', 'Classic \c2V-Ramming Mod: \c3%1.', ($Host::ClassicLoadVRamChanges ? 'Enabled' : 'Disabled') );

      // z0dd - ZOD, 9/29/02. Removed T2 demo code from here
      messageClient(%client, 'MsgClientJoin', 'Welcome to Tribes2 %1.', 
                    %client.name, 
                    %client, 
                    %client.target, 
                    false,   // isBot 
                    %client.isAdmin, 
                    %client.isSuperAdmin, 
                    %client.isSmurf, 
                    %client.sendGuid );
       // z0dd - ZOD, 9/29/02. Removed T2 demo code from here

      messageAllExcept(%client, -1, 'MsgClientJoin', '\c1%1 joined the game.', 
                       %client.name, 
                       %client, 
                       %client.target, 
                       false,   // isBot 
                       %client.isAdmin, 
                       %client.isSuperAdmin, 
                       %client.isSmurf,
                       %client.sendGuid );
   }
   else
      messageClient(%client, 'MsgClientJoin', "\c0Mission Insertion complete...", 
            %client.name, 
            %client, 
            %client.target, 
            false,   // isBot 
            false,   // isAdmin 
            false,   // isSuperAdmin 
            false,   // isSmurf
            %client.sendGuid );

   //Game.missionStart(%client);
   setDefaultInventory(%client);

   if($missionRunning)
      %client.startMission();
   $HostGamePlayerCount++;
   // z0dd - ZOD, 9/29/02. Removed T2 demo code from here

   if( $Host::ClassicConnectLog )
   {
      // z0dd - ZOD, 5/07/04. New logging method based on AurLogging by Aureole
      %file = $Host::ClassicConnLogPath @"/"@ formatTimeString("mm.dd.yy") @ "Connect.csv";
      %conn = new FileObject();
      %conn.openForAppend(%file);
      %conn.writeLine("\"" @ formatTimeString("mm.dd.yy - h:nn:ss A") @ "\"," @ %client.nameBase @ "\"," @ %client.guid @ "," @ getSubStr(%client.getAddress(), 3, strlen(%client.getAddress())));
      %conn.close();
      %conn.delete();
      echo( "exporting client info to connect.csv..." );

      // z0dd - ZOD - Founder, 5/25/03. Connect log
      //$conn::new[$ConnectCount++] = "Player: " @ %client.nameBase @ " Real Name: " @ %realName @ " Guid: " @ %client.guid @ " Connected from: " @ %client.getAddress();
      //%file = formatTimeString("mm.dd.yy") @ "Connect.log";
      //export("$conn::*", $Host::ClassicConnLogPath @"/"@ %file, true);
   }

   // z0dd - ZOD 4/29/02. Activate the clients Classic Huds
   // and start off with 0 SAD access attempts.
   %client.SadAttempts = 0;
   messageClient(%client, 'MsgBomberPilotHud', ""); // Activate the bomber pilot hud

   // z0dd - ZOD, 8/10/02. Get player hit sounds etc.
   commandToClient(%client, 'GetClassicModSettings', 1);

   //---------------------------------------------------------
   // z0dd - ZOD, 7/12/02. New AutoPW server function. Sets
   // server join password when server reaches x player count.
   if($Host::ClassicAutoPWEnabled)
   {
      if(($Host::ClassicAutoPWPlayerCount != 0 && $Host::ClassicAutoPWPlayerCount !$= "") && ($HostGamePlayerCount >= $Host::ClassicAutoPWPlayerCount))
         AutoPWServer(1);
   }
   // z0dd - ZOD, 5/12/04. Kick a bot for every client join if balanced bots are set
   if( $Host::BotsEnabled )
   {
      if($Host::ClassicBalancedBots)
      {
         for(%i = 0; %i < ClientGroup.getCount(); %i++)
         {
            %cl = ClientGroup.getObject(%i);
            if(%cl.isAIControlled())
            {
               %kick = %cl;
               break;
            }
         }
         if(%kick !$= "")
         {
            $HostGameBotCount--;
            %kick.drop();
         }
      }
   }
}

};

// Prevent package from being activated if it is already
if (!isActivePackage(SmurfFix))
    activatePackage(SmurfFix);