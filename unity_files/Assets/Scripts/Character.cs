using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public List<BaseAction> Actions = new List<BaseAction>();

}
