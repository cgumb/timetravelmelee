using UnityEngine;
using System.Collections;
using System.Collections.Generic; // for List

// Used by Battle State Machine to load specific units for a given battle
public class Battle : MonoBehaviour
{
	public List<GameObject> charactersInBattle; // list of heroes and enemies in the battle
}
