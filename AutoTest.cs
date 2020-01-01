//Function used to find out the order in which T2 is loading autoexec files

function autoTest()
{ 
   %folderPath = "scripts/autoexec/*.cs";
   %count = getFileCount(%folderPath);
   for (%i = 1; %i < %count; %i++)
   {
      %file = findNextfile(%folderPath);
      echo(%file);
   }
}