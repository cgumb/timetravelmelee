using UnityEngine;
using System.Collections;

public class EnemyBar : MonoBehaviour {

	public GameObject EnemyPrefab;

	public void selectEnemy()
	{ 
		// This is currently hardwired to select washington
		// EnemyPrefab is null even though it was set in the inspector
		// I'll have to sort this out later :/
		GameObject.Find ("BattleManager").GetComponent<BattleStateMachine> ().SelectTarget (GameObject.Find ("washington"));
	}
}
