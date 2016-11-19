using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrutalBayoneting : BaseAction {

	public StatusEffect bleed;	// bleed prefab goes here

	public BrutalBayoneting ()
	{
		actionName = "Brutal Bayoneting";
		// making the damage randomized was causing issues as a Random.Range()
		// method call only happens once at the beginning of the game
		// we'll have to puzzle this one out
		//	actionDamage = 5f;
		actionEnergyCost = 3;
		takesTarget = true;	
			
	}

	// this is where the actual function used in TimeForAction should go
	public override void ActionEffect()
	{
		actionEnergyCost = 3; // for some reason it is 0 if I don't add this :/
		Debug.Log("Energy Cost: " + actionEnergyCost);
		// should later find a way to load the bleed prefab automatically
		//StatusEffect newBleed = Instantiate(Resources.Load("Bleed") as StatusEffect);
		StatusEffect newBleed = Instantiate(bleed);
		newBleed.subject.statusEffects.Add(newBleed);
		newBleed.initialized = true;
	}
}
