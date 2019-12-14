// IP Based AdminList for servers without TribesNext (No GUID)
// $Host::AdminList = "IP:XXX.XXX.XXX.XXX";
// Multiple Admins ( \t is for tabs )
// $Host::AdminList = "IP:XXX.XXX.XXX.XXX\tIP:XXX.XXX.XXX.XXX";
// Same rules applay to SuperAdminList
//
// If you use Add Admin to list feature. It will save the ports as well with %client.getAddress().
// So, IP:XXX.XXX.XXX.XXX:XXXXX (IP:Port)
// It does still work as intended on reconnect.

package IPAdminList
{

function CreateServer( %mission, %missionType )
{
	parent::CreateServer( %mission, %missionType );
	
	// Prevent package from being activated if it is already
	if (!isActivePackage(PopUpWorkAround))
		activatePackage(PopUpWorkAround);
}

function isOnAdminList(%client)
{
   if( !%totalRecords = getFieldCount( $Host::AdminList ) )
   {   
      return false;
   }
   
   for(%i = 0; %i < %totalRecords; %i++)
   {
      %record = getField( getRecord( $Host::AdminList, 0 ), %i);
      if(%record == %client.getAddress())
         return true;
   }
   
   return false;
}   

function isOnSuperAdminList(%client)
{
   if( !%totalRecords = getFieldCount( $Host::superAdminList ) )
   {   
      return false;
   }
   
   for(%i = 0; %i < %totalRecords; %i++)
   {
      %record = getField( getRecord( $Host::superAdminList, 0 ), %i);
      if(%record == %client.getAddress())
         return true;
   }
   
   return false;
}

function ServerCmdAddToAdminList( %admin, %client )
{
   if( !%admin.isSuperAdmin )
      return;
   
   %count = getFieldCount( $Host::AdminList );

   for ( %i = 0; %i < %count; %i++ )
   {
      %id = getField( $Host::AdminList, %i );
      if ( %id == %client.getAddress() )
      {   
         return;  // They're already there!
      }
   }

   if( %count == 0 )
      $Host::AdminList = %client.getAddress();
   else
      $Host::AdminList = $Host::AdminList TAB %client.getAddress();

   // z0dd - ZOD, 4/29/02. Was not exporting to serverPrefs and did not message admin
   export( "$Host::*", $serverprefs, false );
   messageClient(%admin, 'MsgAdmin', '\c3\"%1\"\c2 added to Admin list: \c3%2\c2.', %client.name, %client.getAddress());
   logEcho(%admin.nameBase @ " added " @ %client.nameBase @ " " @ %client.getAddress() @ " to Admin list.", 1);
}

function ServerCmdAddToSuperAdminList( %admin, %client )
{
   if( !%admin.isSuperAdmin )
      return;

   %count = getFieldCount( $Host::SuperAdminList );

   for ( %i = 0; %i < %count; %i++ )
   {
      %id = getField( $Host::SuperAdminList, %i );
      if ( %id == %client.getAddress() )
         return;  // They're already there!
   }

   if( %count == 0 )
      $Host::SuperAdminList = %client.getAddress();
   else
      $Host::SuperAdminList = $Host::SuperAdminList TAB %client.getAddress();

   // z0dd - ZOD, 4/29/02. Was not exporting to serverPrefs and did not message admin
   export( "$Host::*", $serverprefs, false );
   messageClient(%admin, 'MsgAdmin', '\c3\"%1\"\c2 added to Super Admin list: \c3%2\c2.', %client.name, %client.getAddress());
   logEcho(%admin.nameBase @ " added " @ %client.nameBase @ " " @ %client.getAddress() @ " to Super Admin list.", 1);
}

};

// Prevent package from being activated if it is already
if (!isActivePackage(IPAdminList))
	activatePackage(IPAdminList);


package PopUpWorkAround
{

function DefaultGame::sendGamePlayerPopupMenu(%game, %client, %targetClient, %key)
{
   Parent::sendGamePlayerPopupMenu(%game, %client, %targetClient, %key);
   
   if(%client.isSuperAdmin)
   {
      messageClient(%client, 'MsgPlayerPopupItem', "", %key, "addAdmin", "", 'Add to Admin List', 10);
      messageClient(%client, 'MsgPlayerPopupItem', "", %key, "addSuperAdmin", "", 'Add to SuperAdmin List', 11);
   }
}

};