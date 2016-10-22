using UnityEngine;
using System.Collections;

public class CharacterBar : MonoBehaviour {

	public GameObject CharacterPrefab;

	public void selectCharacter()
	{
		GameObject.Find ("BattleManager").GetComponent<BattleStateMachine> ();
	}
}
