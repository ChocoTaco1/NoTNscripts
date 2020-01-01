package ReExecEvo
{

function CreateServer( %mission, %missionType )
{
	parent::CreateServer( %mission, %missionType );
	
	deactivate(evolution_package);
	activate(evolution_package);
}

};

// Prevent package from being activated if it is already
if (!isActivePackage(ReExecEvo))
	activatePackage(ReExecEvo);