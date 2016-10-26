using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// controls behavior of a character within the battle
public class CharacterStateMachine : MonoBehaviour {

	protected BattleStateMachine BSM;	// connection to BattleStateMachine
	public Character character;
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
	protected Vector2 startPosition;		// where character begins on the battlefield
	protected bool alive = true;
	protected float curPhase = 0f;		//
	protected float maxPhase = 5f;
	public Image lifeBar;			// visual representation of life
	public Image phaseBar;			// visual representation of phase
	public GameObject selector;

	//TimeForAction() stuff
	// variables used when performing an action
	private bool actionStarted;			// have we already started the action?
	public GameObject target;			// target of action
	protected float positionOffset = -3f; // you want to face your target, not land right on of them! (calculate dynamically in the future)
	private float moveSpeed = 7f;		// how quickly you move around the battlefield (have speed affect this?)

	// when a CharacterStateMachine is created
	void Start ()
	{
		selector.SetActive (false); 			// hide Selector
		curState = characterState.PHASING_IN;	// set to PHASING_IN state
		startPosition = transform.position;		// move to starting location on battlefield
		BSM = GameObject.Find ("BattleManager").GetComponent<BattleStateMachine> ();	// connect to the BSM
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
				BSM.charactersToManage.Add (this.gameObject);
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
				if (!alive) {return;}
				else
				{
					this.gameObject.tag = "DeadCharacter";	// tag as dead
					BSM.enemies.Remove (this.gameObject);	// BSM no longer recognizes this character
					selector.SetActive(false);				// disable selecter

					// remove any future actions if they are in BSM's queue
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
					BSM.battleState = BattleStateMachine.battleStates.LOSE; // lose!
				}
				break;
			}
		}

	// used by TakeDamage() to update lifeBar
	protected void updateLife ()
	{
		float lifePercent = character.curLife / character.baseLife;
		lifeBar.transform.localScale = new Vector2 (Mathf.Clamp (lifePercent, 0, 1), lifeBar.transform.localScale.y);
	}

	// phase in based on character speed and deltaTime, change state to ADDTOLIST when phase full
	protected void PhaseIn ()
	{
		curPhase += (character.curSpeed * Time.deltaTime);	// increase phase over time
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
	protected void chooseAction ()
	{
		Action myAction = new Action ();		// create new Action object
		myAction.agent = this.gameObject;		// set agent
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

		//animate agent to move near target
		// dirty fix that will need to be done a cleaner way later
		if (this.gameObject.CompareTag("Enemy") && positionOffset < 0) { positionOffset *= -1; }

		Vector2 targetPosition = new Vector2 (target.transform.position.x + positionOffset, target.transform.position.y);
		while (MoveTowardTarget (targetPosition)){yield return null;}

		//wait
		yield return new WaitForSeconds(0.5f);
		//deal damage
		DealDamage();

		//return to starting location
		while (MoveTowardTarget (startPosition)){yield return null;}

		//remove action from list
		BSM.performList.RemoveAt(0);
		//reset BSM to WAIT state
		BSM.battleState = BattleStateMachine.battleStates.WAIT;

		//end coroutine
		actionStarted = false;

		//reset the character's phase and set their state to PHASING_IN
		curPhase = 0;
		curState = characterState.PHASING_IN;
	}

	// returns true if we still have space to go to get to target
	protected bool MoveTowardTarget(Vector2 target)
	{
		return target != (Vector2)(transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime));
	}

	// deals damage to target based on character's attack and the action's damage
	protected void DealDamage()
	{
		Debug.Log("Dealing Damage...");
		float calculatedDamage = character.curAttack + BSM.performList[0].chosenAction.actionDamage;
		Debug.Log ("Taking Damage...");
		// play a silly sound! =D
		AudioSource audio = this.gameObject.GetComponent<AudioSource> ();
		audio.clip = BSM.performList [0].chosenAction.sound;
		audio.Play ();
		target.GetComponent<CharacterStateMachine>().TakeDamage (calculatedDamage);
		Debug.Log ("Damage Done!");
	}

	public void TakeDamage(float rawDamage)
	{
		float damage; // actual damage to be done

		// determine damage reduction
		float damageReduction = character.curDefense / 10;

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


}
