package ReExecEvo
{

function CreateServer( %mission, %missionType )
{
	parent::CreateServer( %mission, %missionType );
	
	deactivate(evolution_package);
	exec("scripts/autoexec/evolution.cs");
}

};

// Prevent package from being activated if it is already
if (!isActivePackage(ReExecEvo))
	activatePackage(ReExecEvo);