using UnityEngine;
using System.Collections;

public class Lampoon : BaseAction
{
	public Lampoon ()
	{
		actionName = "Lampoon";
		actionDamage = 0;
		actionEnergyCost = 0;	// later will cost energy
		takesTarget = true;			
	}

	// this effect is permanent, in the future it will add a decaying status effect that can stack
	override public void ActionEffect ()
	{
	}
}
