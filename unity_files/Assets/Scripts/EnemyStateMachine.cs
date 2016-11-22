using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyStateMachine : CharacterStateMachine {

	// this was throwing errors, added a dirty fix in CSM's TimeForAction() for now
	//new protected float positionOffset = -3f;	// offset is different for enemies

	Action myAction;
	Color resetColor;

	void Awake() {
		myAction = new Action ();
		resetColor = new Color (255f, 255f, 255f, 255f);
		this.startPosition = this.transform.position;
		this.startPosition.x -= 4.1f;
		this.startPosition.y -= 2.28f;

	}

	protected override void Update ()
	{
		SetScale();
		switch (curState)
		{
		case(characterState.PHASING_IN):
			base.PhaseIn ();
			break;

		case(characterState.ADDTOLIST):
			MakeChoice ();
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
	void MakeChoice ()
	{

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
		/*
		if (this.alive == true && BSM.playerInput == BattleStateMachine.playerGUI.TARGETING)
		{
			BSM.SelectTarget(this);
		}
		*/

		if (IsTargetable() == true)
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

			renderer.color = myColor;
		}
		else if (IsAlive() == true )
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


	// reset color on mouse exit (if alive)
	protected override void OnMouseExit()
	{
		if (IsAlive() == true)
		{
			SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
			renderer.color = resetColor;
		}
		// for tooltip
		showTooltip = false;
		mouseOverTime = 0;
	}

	public bool IsTargetable()
	{
		return IsAlive() == true && BSM.playerInput == BattleStateMachine.playerGUI.TARGETING;
	}

}
