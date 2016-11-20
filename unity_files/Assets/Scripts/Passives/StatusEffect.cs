using UnityEngine;
using System.Collections;

public class StatusEffect : MonoBehaviour
{
	public string statusEffectName;
	public string tooltipString;

	public CharacterStateMachine subject;
	public BattleStateMachine BSM;
	public bool initialized = false;

	void Awake()
	{
		Debug.Log("Status Effect Awake!");
		BSM = GameObject.Find ("BattleManager").GetComponent<BattleStateMachine> ();
		subject = BSM.curAction.target;
		this.transform.SetParent(subject.transform);
	}
}
