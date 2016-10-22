using UnityEngine;
using System.Collections;

[System.Serializable]
public class HandleTurn {

	public string Performer;
	public string Type;
	public GameObject PerformersGameObject;
	public GameObject PerformersTarget;

	public BaseAction ChosenAction;
}
