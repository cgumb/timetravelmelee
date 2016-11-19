using UnityEngine;
using System.Collections;

public class BasicAttack : BaseAction
{
	public BasicAttack ()
	{
		actionName = "Basic Attack";
		// making the damage randomized was causing issues as a Random.Range()
		// method call only happens once at the beginning of the game
		// we'll have to puzzle this one out
		actionDamage = 5f;
		actionEnergyCost = 0;
		takesTarget = true;			
	}

	// this is where the actual function used in TimeForAction should go
	public override void ActionEffect()
	{
		
	}
}
