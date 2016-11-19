using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour {

	public BattleStateMachine BSM;	// connection to BattleStateMachine
	public int energyCost;

	// Use this for initialization
	void Start ()
	{
		BSM = GameObject.Find ("BattleManager").GetComponent<BattleStateMachine> ();	// connect to the BSM
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (energyCost > BSM.playerEnergy)
		{
			this.GetComponent<Button>().interactable = false;
		}
		else
		{
			this.GetComponent<Button>().interactable = true;
		}
	}
}
