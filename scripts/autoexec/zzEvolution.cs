// Initialize variables
$MaxPlayers = $Host::MaxPlayers;
$BackupPassword = $Host::Password;
$EvoDefaultTimeLimit = $Host::TimeLimit;
$SaveCustomMapRotation = $Host::EvoCustomMapRotation;
$EvoNoBaseRape = 1;

// Load up our non packaged functions
exec("scripts/evolution/evoSupport.cs");
exec( $Host::EvoBanListFile );
exec("scripts/evolution/logs.cs");
exec("scripts/evolution/ParseCommands.cs");
exec("scripts/evolution/mapRotation.cs");
exec("scripts/evolution/voteOptions.cs");
exec("scripts/evolution/eTourney.cs");
exec("scripts/evolution/stats.cs");
exec("scripts/evolution/lease.cs");
exec( $Host::EvoCustomMapLimitsFile );

//
// Purpose: Create our packaged function overloads and execute them.
//
function createEvolutionPackage()
{
   // Load default prefs if they haven't been. Editing is done to ServerPrefs.cs
   if($Host::EvoDefaultsLoaded $= "" || !$Host::EvoDefaultsLoaded)
   {
      exec("evo_prefs.cs");
      $Host::EvoDefaultsLoaded = 1;
   }

   $PackageWrite = 1;
   %newfile = "scripts/evolution/evoPackage.cs";
   if(isFile(%newfile))
      return;

   %package = new fileObject();
   %package.openForWrite(%newFile);
   %package.writeLine("package evolution_package {");
   %fobject = new fileObject();
   %path = "scripts/evolution/*.ovl";
   for(%file = findFirstFile(%path); %file !$= ""; %file = findNextFile(%path))
   {
      %name = fileBase(%file);
      %fobject.openForRead(%file);
      while (!%fobject.isEOF()) 
      {
         %line = %fobject.readLine();
         if(getSubStr(%line, 0, 2) !$= "//")
            %package.writeLine(%line);
      }
      %fobject.close();
   }
   %fobject.delete();
   %package.writeLine("};");
   %package.writeLine("activatePackage(evolution_package);");
   %package.close();
   %package.delete();
}

if(!$PackageWrite)
   createEvolutionPackage();

exec("scripts/evolution/evoPackage.cs");

