using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyStateMachine : MonoBehaviour {

	private BattleStateMachine BSM;
	public Character enemy;
	public GameObject EnemySelector;
	private bool EnemySelectorActivity;

	public enum TurnState
	{
		PROCESSING,
		CHOOSEACTION,
		WAITING,
		SELECTING,
		ACTION,
		DEAD
	}

	public TurnState curState;
	private Vector2 startPosition;
	private bool alive = true;
	private float curCooldown = 0f;
	private float maxCooldown = 5f;
	public Image LifeBar;
	public Image ProgressBar;

	//timeForActionStuff
	private bool actionStarted;
	public GameObject target;
	private float positionOffset = 3f;
	private float moveSpeed = 5f;

	void Start ()
	{
		curState = TurnState.PROCESSING;
		startPosition = transform.position;
		BSM = GameObject.Find ("BattleManager").GetComponent<BattleStateMachine> ();

	}

	void Update ()
	{
		switch (curState)
		{
		case(TurnState.PROCESSING):
			updateProgressBar();

			break;

		case(TurnState.CHOOSEACTION):
			chooseAction ();
			curState = TurnState.WAITING;
			break;

		case(TurnState.WAITING):

			break;

		
		case(TurnState.ACTION):
			StartCoroutine (TimeForAction ());
			break;

		case(TurnState.DEAD):
			if (!alive) {
				return;
			} else
			{
				this.gameObject.tag = "DeadEnemy";
				BSM.Enemies.Remove (this.gameObject);
				// disable selecter
				EnemySelector.SetActive(false);
				// remove future turns
				for (int i = 0; i < BSM.PerformList.Count; i++)
				{
					if (BSM.PerformList [i].PerformersGameObject == this.gameObject)
					{
						BSM.PerformList.Remove(BSM.PerformList[i]);
					}
				}
				// change color
				SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
				renderer.color = new Color(156f,0f,0f,255f);
				alive = false;
				BSM.battleState = BattleStateMachine.PerformAction.WIN;
			}
			break;
		}
	}

	void updateLife ()
	{
		float lifePercent = enemy.curLife / enemy.baseLife;
		LifeBar.transform.localScale = new Vector2 (Mathf.Clamp (lifePercent, 0, 1), LifeBar.transform.localScale.y);
	}

	void updateProgressBar ()
	{
		curCooldown = curCooldown + (enemy.curSpeed * Time.deltaTime);
		float progressBarPercent = curCooldown / maxCooldown;
		ProgressBar.transform.localScale = new Vector2 (Mathf.Clamp (progressBarPercent, 0, 1), ProgressBar.transform.localScale.y);
		if (curCooldown >= maxCooldown)
		{
			curState = TurnState.CHOOSEACTION;
		}
	}

	void chooseAction ()
	{
		HandleTurn myAction = new HandleTurn ();
		myAction.Performer = enemy.name;
		myAction.Type = "Enemy";
		myAction.PerformersGameObject = this.gameObject;

		myAction.PerformersTarget = BSM.Characters [Random.Range (0, BSM.Characters.Count)];

		// select a random action
		int rand = Random.Range (0, enemy.Actions.Count);
		myAction.ChosenAction = enemy.Actions[rand];

		BSM.collectActions(myAction);
	}
	private IEnumerator TimeForAction()
	{
		if (actionStarted)
		{
			yield break;
		}
		actionStarted = true;

		//animate enemy to move near target
		Vector2 targetPosition = new Vector2 (target.transform.position.x + positionOffset, target.transform.position.y);
		while (MoveTowardTarget (targetPosition)){yield return null;}

		//wait
		yield return new WaitForSeconds(0.4f);
		//deal damage
		DealDamage();
	

		//return to starting location
		while (MoveTowardTarget (startPosition)){yield return null;}

		//remove performance from list
		BSM.PerformList.RemoveAt(0);
		//reset BSM to WAIT
		BSM.battleState = BattleStateMachine.PerformAction.WAIT;

		//end coroutine
		actionStarted = false;

		//reset this enemy state
		curCooldown = 0;
		curState = TurnState.PROCESSING;
	}

	private bool MoveTowardTarget(Vector2 target)
	{
		return target != (Vector2)(transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime));
	}

	void DealDamage()
	{
		float calculatedDamage = enemy.curAttack + BSM.PerformList [0].ChosenAction.actionDamage;
		target.GetComponent<CharacterStateMachine>().TakeDamage (calculatedDamage);
	}

	public void TakeDamage(float rawDamage)
	{
		float damage; // actual damage to be done

		// determine damage reduction
		float damageReduction = enemy.curDefense / 10;

		// cap it at 90%
		damageReduction = (damageReduction > 0.9 ? 0.9f : damageReduction);

		// calculate final damage
		damage = rawDamage * (1 - damageReduction);

		EnemySelectorToggle ();
		enemy.curLife -= damage;
		updateLife();
		if (enemy.curLife <= 0)
		{
			
			curState = TurnState.DEAD;
		}
		EnemySelectorToggle ();
	} 

	public void EnemySelectorToggle ()
	{
		EnemySelectorActivity = !EnemySelectorActivity;
		EnemySelector.SetActive (EnemySelectorActivity);
	}

	private IEnumerator Wait( float seconds )
	{
		yield return new WaitForSeconds (seconds);
	}
}
