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
	parent::GameConnection::onConnect( %client, %name, %raceGender, %skin, %voice, %voicePitch );

	// Use regular white color no matter what
	%name = "\cp\c6" @ %name @ "\co";
}

};

// Prevent package from being activated if it is already
if (!isActivePackage(SmurfFix))
    activatePackage(SmurfFix);