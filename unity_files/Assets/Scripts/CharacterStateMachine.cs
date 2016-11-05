using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// controls behavior of a character within the battle
[System.Serializable]
public class CharacterStateMachine : MonoBehaviour {

	protected BattleStateMachine BSM;	// connection to BattleStateMachine
	public Character character;			// state machine's character (combine these in the future?)
	public int player;

	// different states a character can be in
	public enum characterState
	{
		PHASING_IN,		// phase bar filling up, gets a chance to select action when full
		ADDTOLIST,		// being added actionSelectQueue (in BattleStateMachine)
		WAITING,		// idle
		SELECTING,		// selecting an action
		ACTING,			// carrying out action
		DEAD			// :(
	}

	public characterState curState;
	public Vector2 startPosition;		// where character begins on the battlefield
	protected bool alive = true;
	protected float curPhase = 0f;		//
	protected float maxPhase = 5f;
	public Image lifeBar;			// visual representation of life
	public Image phaseBar;			// visual representation of phase

	public float backRowBonus = 3f;				// bonus to defense for being in back row
	public float backRowPenalty = 0.5f;			// % of damage lost for being in back row

	//TimeForAction() stuff
	// variables used when performing an action
	public bool actionStarted;				// have we already started the action?
	public CharacterStateMachine target;	// target of action
	public float positionOffset = -1f;		// don't want to land right on our targets!
	protected float moveSpeed = 8f;			// how quickly you move around the battlefield (have speed affect this?)

	//Tooltip Stuff
	public GUIStyle style = new GUIStyle();
	public bool showTooltip = false;
	public Rect rect;
	public string tooltip;
	Texture2D texture = new Texture2D(256, 128);
	protected float mouseOverTime = 0f;				// used to track when mouseover occured

	// when a CharacterStateMachine is created
	void Start ()
	{
		curState = characterState.PHASING_IN;	// set to PHASING_IN state
		startPosition = transform.position;		// move to starting location on battlefield
		BSM = GameObject.Find ("BattleManager").GetComponent<BattleStateMachine> ();	// connect to the BSM

		//Tooltip stuff
		rect = new Rect (0, 0, 256, 128);
		for (int y = 0; y < texture.height; ++y)
		{
			for (int x = 0; x < texture.width; ++x)
			{
				texture.SetPixel(x, y, Color.white);
			}
		}
		texture.Apply();
		style.normal.background = texture;
	}

	// executes on every frame
	protected virtual void Update ()
	{
		switch (curState)
		{
			// phase in until phase is full
			case(characterState.PHASING_IN):
				PhaseIn();
				break;

			// tell BSM you need to select and action, then wait
			case(characterState.ADDTOLIST):
				BSM.charactersToManage.Add (this);
				curState = characterState.WAITING;
				break;

			case(characterState.WAITING):
				// idle state
				break;

			case(characterState.SELECTING):
				// idle (code here could be used to prevent enemies from attacking while selecting an action)
				break;

			// performing an action
			case(characterState.ACTING):
				StartCoroutine (TimeForAction ());
				break;

			// life = 0, can no longer perform any actions
			case(characterState.DEAD):
				if (!alive) {return;}	// only die once :p
				else
				{
					die();
					// lose if no characters left alive
					if (BSM.characters.TrueForAll( c => c.IsAlive() == false))
					{
						BSM.battleState = BattleStateMachine.battleStates.LOSE; // lose!

					}
				}
				break;
			}
		}

	// used by TakeDamage() to update lifeBar
	protected void updateLife ()
	{
		float lifePercent = character.curLife / character.baseLife;
		lifeBar.transform.localScale = new Vector2 (Mathf.Clamp (lifePercent, 0, 1), lifeBar.transform.localScale.y);
		StartCoroutine(Flasher());
	}

	// phase in based on character speed and deltaTime, change state to ADDTOLIST when phase full
	protected void PhaseIn ()
	{
		curPhase += (character.curSpeed * Time.deltaTime * BSM.timeScale);	// increase phase over time
		// update phaseBar
		float progressBarPercent = curPhase / maxPhase;
		phaseBar.transform.localScale = new Vector2 (Mathf.Clamp (progressBarPercent, 0, 1), phaseBar.transform.localScale.y);
		// change state if phase is full
		if (curPhase >= maxPhase)
		{
			curState = characterState.ADDTOLIST;
		}
	}

	// bundles the details of a chosen action and sends it to the BSM to be added to the queue
	protected void ChooseAction ()
	{
		Action myAction = new Action ();		// create new Action object
		myAction.agent = this;					// set agent
		BSM.collectActions(myAction);			// send action to BSM
	}

	// coroutine that runs when in the ACTING state
	protected IEnumerator TimeForAction()
	{
		if (actionStarted)
		{
			yield break;
		}
		actionStarted = true;

		BaseAction chosenAction = BSM.performList [0].chosenAction;
		// we will want these action functions to be the ActionEffect() method within each action
		// right now this poses a problem as many of the variables they quirere are not accessible within each action object
		// we'll think of something! :^)
		switch (chosenAction.actionName)
		{
			case ("Basic Attack"):
				//animate agent to move near target
				// dirty fix that will need to be done a cleaner way later
				int offset = this.gameObject.CompareTag ("Enemy")? 1: -1;
		//		if (this.gameObject.CompareTag ("Enemy") && positionOffset < 0) {positionOffset *= -1;}
				Vector2 targetPosition = new Vector2 (target.transform.position.x + offset, target.transform.position.y);
				while (MoveTowardTarget (targetPosition))
				{
					yield return null;
				}
				//wait
				yield return new WaitForSeconds (0.5f);
				//deal damage
				PlayActionSound();	// play a silly sound! =D
				DealDamage ();

				//return to starting location
				while (MoveTowardTarget (startPosition))
				{
					yield return null;
				}
				break;
			case ("Move"):
				float toMove = 2f; // distance to move (i.e., distance between front and back rows)
				int direction = character.frontRow ? -1 : 1;	// negative means move left on x-axis, postive right 
				if (this.gameObject.CompareTag ("Enemy")) {direction *= -1;} // direction is reversed for enemies
				startPosition = new Vector2 (transform.position.x + direction * toMove, transform.position.y);
				PlayActionSound();	// play a move sound
				while (MoveTowardTarget (startPosition))
				{
					yield return null;
				}
				character.frontRow = !character.frontRow; // toggle frontRow
				break;
			case ("Lampoon"):
				// this effect is permanent, in the future it will add a decaying status effect that can stack
				float attackToSteal = 2f;
				if (BSM.performList [0].target.character.curAttack >= 3f)
				{
					BSM.performList [0].target.character.curAttack -= attackToSteal;
				}
				else // don't put target below 1 attack power
				{
					attackToSteal = BSM.performList [0].target.character.curAttack - 1f;	// only add amount stolen
					BSM.performList [0].target.character.curAttack = 1f;
					BSM.performList [0].target.character.curAttack -= attackToSteal;
					attackToSteal = attackToSteal < 0 ? 0 : attackToSteal;				// can't steal a negative amount!
				}
				BSM.performList [0].agent.character.curAttack += attackToSteal;
				Debug.Log (this.character.name + " stole " + attackToSteal + " attack power from " + target.character.name + "!");
				PlayActionSound();	// play a move sound
				break;
		}


		//remove action from list (if it exists, a loss/win will have already removed it :/
		if (BSM.performList.Count > 0) {BSM.performList.RemoveAt(0);}
		//reset BSM to WAIT state
		BSM.battleState = BattleStateMachine.battleStates.WAIT;

		//end coroutine
		actionStarted = false;

		//reset the character's phase and set their state to PHASING_IN
		curPhase = 0;
		curState = characterState.PHASING_IN;
	}

	// returns true if we still have space to go to get to target
	public bool MoveTowardTarget(Vector2 target)
	{
		return target != (Vector2)(transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime));
	}

	// deals damage to target based on character's attack and the action's damage
	// should really be part of a damaing action's ActionEffect()
	public void DealDamage()
	{
		float calculatedDamage = character.curAttack + BSM.performList[0].chosenAction.actionDamage;

		// damage penalty for being in back row
		calculatedDamage = character.frontRow ? calculatedDamage : calculatedDamage * (1 - backRowPenalty);

		target.GetComponent<CharacterStateMachine>().TakeDamage (calculatedDamage);
	}

	public void TakeDamage(float rawDamage)
	{
		float damage; // actual damage to be done

		// defense bonus if in the back row
		float totalDefense = character.frontRow? character.curDefense : character.curDefense + backRowBonus;

		// determine damage reduction
		float damageReduction = totalDefense / 10;

		// cap it at 90%
		damageReduction = (damageReduction > 0.9 ? 0.9f : damageReduction);

		// calculate final damage
		damage = rawDamage * (1 - damageReduction);

		Debug.Log (character.name + " takes " + damage + " of the original " + rawDamage + " (" + damageReduction * 100 + "% damage reduction)");
		character.curLife -= damage;
		updateLife();
		if (character.curLife <= 0)
		{

			curState = characterState.DEAD;
		}
	}
		
	public void PlayActionSound()
	{
		AudioSource audio = this.gameObject.GetComponent<AudioSource> ();
		audio.clip = BSM.performList [0].chosenAction.sound;
		audio.Play ();
	}

	public bool IsAlive()
	{
		return alive;
	}

	protected void die()
	{
		// remove action select panel if this character was selecting an action/target
		if (BSM.charactersToManage.Count > 0 && BSM.charactersToManage [0] == this)
		{
			BSM.ExitSelection();
		}

		// remove any future actions if they are in BSM's queue
		BSM.charactersToManage.Remove(this);
		BSM.performList.RemoveAll(a => a.agent == this);

		// if any future actions were tageting this unit, remove them and let
		// their agents choose new actions
		List<Action> incomingActions = BSM.performList.FindAll(a => a.target == this);
		for (int i = 0; i < incomingActions.Count; i++)
		{
			CharacterStateMachine agent = incomingActions[i].agent;
			BSM.performList.Remove(incomingActions [i]);
			agent.curState = characterState.ADDTOLIST;

		}

		// change color
		SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
		renderer.color = new Color (156f, 0f, 0f, 255f);
		alive = false;
	}

	// character flashes red 
	IEnumerator Flasher() 
	{
		SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
		for (int i = 0; i < 3; i++)
		{
			renderer.color = Color.red;
			yield return new WaitForSeconds(.1f);
			if (this.IsAlive()) // reseting color if dead causes problems :/
			{
				renderer.color = new Color (255f, 255f, 255f, 255f); 
			}
			yield return new WaitForSeconds(.1f);
		}
	}

	// Tooltip methods
	protected void OnGUI()
	{
		rect.x = Input.mousePosition.x+20;
		rect.y = Screen.height - Input.mousePosition.y-20;
		// hardcoded tooltip delay (0.75)
		// couldn't get this to work with a variable for some reason...
		if (showTooltip && ((mouseOverTime + 0.75) < Time.time))
		{
			GUI.Box(rect, tooltip, style);
		}
	}
	// Update tooltip text
	protected void OnMouseEnter() {
		tooltip = character.name;
		tooltip += "\n" + "Life: " + character.curLife + " (base: " + character.baseLife + ")";
		tooltip += "\n" + "Attack: " + character.curAttack + " (base: " + character.baseAttack + ")";
		tooltip += "\n" + "Speed: " + character.curSpeed + " (base: " + character.baseSpeed + ")";
	}
	// Continue displaying tooltip while hovered
	virtual protected void OnMouseOver() {
		showTooltip = true;
		if (mouseOverTime == 0)
		{
			mouseOverTime = Time.time;
		}
	}
	// Hide tooltip when mouse leaves
	virtual protected void OnMouseExit() {
		showTooltip = false;
		mouseOverTime = 0;
	}
}
