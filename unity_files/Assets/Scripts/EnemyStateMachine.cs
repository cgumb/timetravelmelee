using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyStateMachine : CharacterStateMachine {


	public GameObject enemySelector;
	private bool enemySelectorActivity;

	// this was throwing errors, added a dirty fix in CSM's TimeForAction() for now
	//new protected float positionOffset = -3f;	// offset is different for enemies

	protected override void Update ()
	{
		switch (curState)
		{
		case(characterState.PHASING_IN):
			base.PhaseIn ();
			break;

		case(characterState.ADDTOLIST):
			chooseAction ();
			curState = characterState.WAITING;
			break;

		case(characterState.WAITING):
			// idle
			break;


		case(characterState.ACTING):
			StartCoroutine (TimeForAction ());
			break;

		case(characterState.DEAD):
			if (!alive) {
				return;
			} else
			{
				this.gameObject.tag = "DeadEnemy";
				BSM.enemies.Remove (this.gameObject);
				// disable selecter
				enemySelector.SetActive(false);
				// remove future turns
				for (int i = 0; i < BSM.performList.Count; i++)
				{
					if (BSM.performList [i].agent == this.gameObject)
					{
						BSM.performList.Remove(BSM.performList[i]);
					}
				}
				// change color
				SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
				renderer.color = new Color(156f,0f,0f,255f);
				alive = false;
				BSM.battleState = BattleStateMachine.battleStates.WIN;
			}
			break;
		}
	}


	// different chooseAction for enemies; they select a random action/target
	new void chooseAction ()
	{
		Action myAction = new Action ();
		myAction.type = "Enemy";
		myAction.agent = this.gameObject;

		myAction.target = BSM.characters [Random.Range (0, BSM.characters.Count)];

		// select a random action
		int rand = Random.Range (0, character.actions.Count);
		myAction.chosenAction = character.actions[rand];

		BSM.collectActions(myAction);
	}
}
