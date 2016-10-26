using UnityEngine;
using System.Collections;

public class Move : BaseAction
{
	public Move ()
	{
		actionName = "Move";
		actionDescription = "Move this character from front to back, or back to front row";
		actionDamage = 0f;
		actionEnergyCost = 0;
		takesTarget = false;
	}

	// this is where the actual function used in TimeForAction should go
	public override void ActionEffect ()
	{

	}
}