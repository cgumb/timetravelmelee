using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class BaseAction : MonoBehaviour {

	public string actionName;
	public string actionDescription;
	public float actionDamage;
	public int actionEnergyCost;
	public bool takesTarget;
	public AudioClip sound;

	// this s happehould be where we put what happens!
	public abstract void ActionEffect ();
}
