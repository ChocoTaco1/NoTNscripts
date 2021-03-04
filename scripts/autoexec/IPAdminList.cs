// IPAdminList.cs
//
// IP Based AdminList for servers without TribesNext (GUID from GetGUID.cs)
// Client is identified by their IP Address in the front followed by their 7 digit guid
// $Host::AdminList = "XXX.XXX.XXX.XXX:1234567";
// Multiple Admins ( \t is for tabs ) in between separate ids
// $Host::AdminList = "XXX.XXX.XXX.XXX:1234567\tXXX.XXX.XXX.XXX:1234567";
// Same rules apply to SuperAdminList

package IPAdminListStart
{

function CreateServer( %mission, %missionType )
{
	parent::CreateServer( %mission, %missionType );
	
	// Prevent package from being activated if it is already
	if (!isActivePackage(IPAdminList) && $GetGUID) //Need GetGUID.cs to run
		activatePackage(IPAdminList);
}

};

// Prevent package from being activated if it is already
if (!isActivePackage(IPAdminListStart))
	activatePackage(IPAdminListStart);

package IPAdminList
{

function isOnAdminList(%client)
{
   %ip = getField(strreplace(%client.getAddress(),":","\t"),1);
   //%guidip = %ip @ ":" @ %client.guid;
   
   if( !%totalRecords = getFieldCount( $Host::AdminList ) )
   {   
      return false;
   }
   
   for(%i = 0; %i < %totalRecords; %i++)
   {
      %record = getField( getRecord( $Host::AdminList, 0 ), %i);
	  //Separate the two fields
	  %recordip = getField(strreplace(%record,":","\t"),0);
	  %recordguid = getField(strreplace(%record,":","\t"),1);
	  //Both are compared individully
      if(%recordip == %ip && %recordguid == %client.guid)
         return true;
   }
   
   return false;
}   

function isOnSuperAdminList(%client)
{
   %ip = getField(strreplace(%client.getAddress(),":","\t"),1);
   //%guidip = %ip @ ":" @ %client.guid;
   
   if( !%totalRecords = getFieldCount( $Host::superAdminList ) )
   {   
      return false;
   }
   
   for(%i = 0; %i < %totalRecords; %i++)
   {
      %record = getField( getRecord( $Host::superAdminList, 0 ), %i);
	  //Separate the two fields
	  %recordip = getField(strreplace(%record,":","\t"),0);
	  %recordguid = getField(strreplace(%record,":","\t"),1);
	  //Both are compared individully
      if(%recordip == %ip && %recordguid == %client.guid)
         return true;
   }
   
   return false;
}

function ServerCmdAddToAdminList( %admin, %client )
{
   if( !%admin.isSuperAdmin )
      return;
   
   %count = getFieldCount( $Host::AdminList );
   %ip = getField(strreplace(%client.getAddress(),":","\t"),1);
   %guidip = %ip @ ":" @ %client.guid;

   for ( %i = 0; %i < %count; %i++ )
   {
      %id = getField( $Host::AdminList, %i );
      if ( %id == %guidip)
      {   
         return;  // They're already there!
      }
   }

   if( %count == 0 )
      $Host::AdminList = %guidip;
   else
      $Host::AdminList = $Host::AdminList TAB %guidip;

   // z0dd - ZOD, 4/29/02. Was not exporting to serverPrefs and did not message admin
   export( "$Host::*", $serverprefs, false );
   messageClient(%admin, 'MsgAdmin', '\c3\"%1\"\c2 added to Admin list: \c3%2\c2.', %client.name, %guidip);
   logEcho(%admin.nameBase @ " added " @ %client.nameBase @ " " @ %guidip @ " to Admin list.", 1);
}

function ServerCmdAddToSuperAdminList( %admin, %client )
{
   if( !%admin.isSuperAdmin )
      return;

   %count = getFieldCount( $Host::SuperAdminList );
   %ip = getField(strreplace(%client.getAddress(),":","\t"),1);
   %guidip = %ip @ ":" @ %client.guid;

   for ( %i = 0; %i < %count; %i++ )
   {
      %id = getField( $Host::SuperAdminList, %i );
      if ( %id == %guidip)
         return;  // They're already there!
   }

   if( %count == 0 )
      $Host::SuperAdminList = %guidip;
   else
      $Host::SuperAdminList = $Host::SuperAdminList TAB %guidip;

   // z0dd - ZOD, 4/29/02. Was not exporting to serverPrefs and did not message admin
   export( "$Host::*", $serverprefs, false );
   messageClient(%admin, 'MsgAdmin', '\c3\"%1\"\c2 added to Super Admin list: \c3%2\c2.', %client.name, %guidip);
   logEcho(%admin.nameBase @ " added " @ %client.nameBase @ " " @ %guidip @ " to Super Admin list.", 1);
}

};

