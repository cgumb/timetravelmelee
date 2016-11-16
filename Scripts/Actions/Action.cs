using UnityEngine;
using System.Collections;

// these are the actions that end up in the queue, need better names for these classes...
[System.Serializable]
public class Action {
	
	public string type;
	public CharacterStateMachine agent;
	public CharacterStateMachine target;

	public BaseAction chosenAction;
}
