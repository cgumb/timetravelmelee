using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyStateMachine : CharacterStateMachine {

	// this was throwing errors, added a dirty fix in CSM's TimeForAction() for now
	//new protected float positionOffset = -3f;	// offset is different for enemies

	protected override void Update ()
	{
		SetScale();
		switch (curState)
		{
		case(characterState.PHASING_IN):
			base.PhaseIn ();
			break;

		case(characterState.ADDTOLIST):
			ChooseAction ();
			curState = characterState.WAITING;
			break;

		case(characterState.WAITING):
			// idle
			break;


		case(characterState.ACTING):
			StartCoroutine (TimeForAction ());
			break;

		case(characterState.DEAD):
			if (!alive)
			{
				return;
			}
			else
			{
				die();
				// win if no enemies left alive
				if (BSM.enemies.TrueForAll( c => c.IsAlive() == false))
				{
					BSM.battleState = BattleStateMachine.battleStates.WIN; // you win!

				}
			}
			break;
		}
	}


	// different chooseAction for enemies; they select a random action/target
	new void ChooseAction ()
	{
		Action myAction = new Action ();
		myAction.type = "Enemy";
		myAction.agent = this;

		//myAction.target = BSM.characters [Random.Range (0, BSM.characters.Count)];
		List<CharacterStateMachine> possibleTargets = BSM.characters.FindAll(c => c.IsAlive());
		if (possibleTargets.Count > 0)
		{
			myAction.target = possibleTargets [Random.Range(0, possibleTargets.Count)];
		}

		// select a random action
		int rand = Random.Range (0, character.actions.Count);
		myAction.chosenAction = character.actions[rand];

		BSM.collectActions(myAction);
	}

	// set this enemy as target if it's alive and the player is in targeting state
	void OnMouseDown()
	{
		if (this.alive == true && BSM.playerInput == BattleStateMachine.playerGUI.TARGETING)
		{
			BSM.SelectTarget(this);
		}
	}

	// highlight if currently targetable
	protected override void OnMouseOver()
	{
		SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
		if (IsTargetable())
		{
			float colorValue = (Mathf.Sin(Time.time * 8) + 1f) / 6.0f;
			Color myColor = Color.white;
			myColor.g = colorValue;
			//myColor.b = colorValue;

			renderer.color = myColor;
			//renderer.color = new Color(1f, colorValue2, 1f, 1f);

			//renderer.color = Color.yellow;
		}
		else
		{
			renderer.color = Color.white;
		}
		// for tooltip
		showTooltip = true;
		if (mouseOverTime == 0)
		{
			if (IsTargetable())
			{
				BSM.PlaySound(BSM.hoverSound);
			}
			mouseOverTime = Time.time;
		}
	}

	/*
	protected void OnMouseEnter()
	{
		if (this.alive == true && BSM.playerInput == BattleStateMachine.playerGUI.TARGETING)
		{
			BSM.PlaySound(BSM.hoverSound);
		}
	}
	*/


	// reset color on mouse exit (if alive)
	protected override void OnMouseExit()
	{
		if (this.alive == true)
		{
			SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
			renderer.color = new Color (255f, 255f, 255f, 255f);
		}
		// for tooltip
		showTooltip = false;
		mouseOverTime = 0;
	}

	public bool IsTargetable()
	{
		return this.alive == true && BSM.playerInput == BattleStateMachine.playerGUI.TARGETING;
	}

}
