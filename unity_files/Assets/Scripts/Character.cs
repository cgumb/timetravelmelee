﻿using UnityEngine;
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

	public AudioClip moveSound;
	public AudioClip deathSound;
	public AudioClip damagedSound;
	public AudioClip bleedSound;

	/* true = front line
	 * false = back line */
	public bool frontRow;

	/* variables for energy production */
	public bool makesEnergy = false;
	public float baseEnergyProduction = 1f;
	public float curEnergyProduction = 1f;

	// list of actions available to character
	public List<BaseAction> actions;

	public List<Passive> passives;

	void Awake() {
		actions = new List<BaseAction>();
		passives = new List<Passive>();
	}
}