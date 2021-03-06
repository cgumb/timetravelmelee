﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class BaseAction : MonoBehaviour {

	public string actionName;
	public string actionDescription;
	public float actionDamage;
	public int actionEnergyCost;
	public bool takesTarget;
	public AudioClip sound;

	protected BattleStateMachine BSM;

	// this should be where we put what happens!
	virtual public void ActionEffect ()
	{

	}

	// connect to BSM
	void Awake ()
	{
		BSM = GameObject.Find ("BattleManager").GetComponent<BattleStateMachine> ();
	}
}
