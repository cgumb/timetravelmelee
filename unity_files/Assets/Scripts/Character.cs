using UnityEngine;
using System.Collections;
using System.Collections.Generic; // for List object

[System.Serializable]
public class Character {

	public string name;

	public float baseAttack;
	public float curAttack;

	public float baseDefense;
	public float curDefense;

	public float baseSpeed;
	public float curSpeed;

	public float baseLife;
	public float curLife;

	// list of actions available to character
	public List<BaseAction> actions = new List<BaseAction>();

}
