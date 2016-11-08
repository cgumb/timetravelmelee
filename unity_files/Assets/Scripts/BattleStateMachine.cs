using UnityEngine;
using UnityEngine.UI; // for Text and UI Classes (e.g., Button)
using System.Collections;
using System.Collections.Generic; // for List
using System.Linq;


// TO-DO  Separate BSM from GUI elements into a separate class
public class BattleStateMachine : MonoBehaviour {
	// BattleStateMachine
	public enum battleStates
	{
		WAIT,
		TAKEACTION,
		PERFORMACTION,
		WIN,
		LOSE
	}

	public battleStates battleState;	// current battleState



	public CharacterStateMachine[] allCSMs;
	public List<CharacterStateMachine> characters = new List<CharacterStateMachine> ();
	public List<CharacterStateMachine> enemies= new List<CharacterStateMachine> ();

	public List<Action> performList = new List<Action> ();
	public List<CharacterStateMachine> charactersToManage = new List<CharacterStateMachine> ();

	private float baseTimeScale = 1f;			// the rate at which time is normally moving (for cooldowns)
	public float selectionTimeScale = 0f;		// rate of cooldown when player making a selection
	public float timeScale;				 		// current time scale
	public float startingPhaseCap = 0.2f;		// phase starts at filled to a random percent; this is the cap

	// player's interface
	public enum playerGUI
	{
		ACTIVE,
		WAITING,
		TARGETING,
		DONE
	}

	public GameObject battleCanvas; // should be set automatically :/


	public GameObject characterBar; 	// character bar to create
	public Transform characterSpacer;	// where to place bar for characters
	public Transform enemySpacer;		// where to place for enemies

	public GameObject actionSelectPanel;	// where player selects a character
	public Transform actionSelectSpacer;	// spacer holds each action option
	public GameObject actionButton;				// perfab of action button
	private List<GameObject> actionButtons = new List<GameObject> ();

	public playerGUI playerInput;					// current state of playerGUI
	public Action characterChoice;			// action player has selected


	// Use this for initialization
	void Start ()
	{
		// get characters to create from curBattle's array	
		allCSMs = GameObject.FindObjectsOfType<CharacterStateMachine> ();
		foreach (CharacterStateMachine CSM in allCSMs)
		{
			if (CSM.CompareTag("Character")) {characters.Add(CSM);}
			//characters.OrderByDescending(c => c.gameObject.transform.position.y); // sort by column

			if (CSM.CompareTag("Enemy")) {enemies.Add(CSM);}	
			//enemies.OrderByDescending(e => e.gameObject.transform.position.y);

			// set each character's phase to a random percent full (capped by startingPhaseCap) 
			CSM.curPhase = Random.value * startingPhaseCap * CSM.maxPhase;
		}
		// BSM
		timeScale = baseTimeScale;
		battleState = battleStates.WAIT;

		// GUI
		playerInput = playerGUI.ACTIVE;
		actionSelectPanel.SetActive (false);
		characters = characters.OrderByDescending(c => c.gameObject.transform.position.y).ToList();   // sort by column
		enemies = enemies.OrderByDescending(e => e.gameObject.transform.position.y).ToList();		// sort by column
		CreateBars(enemies, enemySpacer);
		CreateBars(characters, characterSpacer);
	}

	// Update is called once per frame
	void Update ()
	{
		// battleState tracker
		switch (battleState)
		{
			case(battleStates.WAIT):
				if (performList.Count > 0)
				{
					battleState = battleStates.TAKEACTION;
				}
				break;

			case(battleStates.TAKEACTION):
				Action curAction = performList[0];
				CharacterStateMachine CSM = curAction.agent.GetComponent<CharacterStateMachine>();
				CSM.target = curAction.target;
				CSM.curState = CharacterStateMachine.characterState.ACTING;

				battleState = battleStates.PERFORMACTION;
				break;

			case(battleStates.PERFORMACTION):
				break;

			case(battleStates.WIN):
				EndBattle ();
				Debug.Log ("You Win!");
				break;

			case(battleStates.LOSE):
				EndBattle ();
				Debug.Log ("Game Over :(");
				break;
		}

		// playerGUI state tracker
		switch (playerInput)
		{
		case (playerGUI.ACTIVE):
			if (charactersToManage.Count > 0)
			{
				timeScale = selectionTimeScale;
				characterChoice = new Action ();
				actionSelectPanel.SetActive (true);
				CreateActionButtons ();
				playerInput = playerGUI.WAITING;
			}
			break;

		case (playerGUI.WAITING):
			// idle state
			break;

		case (playerGUI.TARGETING):
			// idle state
			break;

		case (playerGUI.DONE):
			PlayerInputDone ();
			break;
		}
	}

// BSM methods
// add action to queue
	public void collectActions(Action curAction)
	{
		performList.Add (curAction);
	}

	// GUI methods

	// populate the action panel with actions available to the character
	void CreateActionButtons ()
	{
		List<BaseAction> actions = charactersToManage[0].character.actions;

		foreach (BaseAction action in actions)
		{
			GameObject newButton = Instantiate (actionButton) as GameObject;
			Text actionButtonText = newButton.transform.FindChild ("Text").gameObject.GetComponent<Text>();
			actionButtonText.text = action.actionName;
			//Debug.Log ("Making Button for " + action.actionName + " which has an index of " + actions.IndexOf(action));
			// the following code to add a listen which calls Action with the index of the button's action wasn't working
			// both buttons ended up calling Move()
			// this could be a problem with the listener being given a parameter that's a local variable
			// but some sources on line show this should work...
			// in the mean time we have a dirty fix outside the foreach loop
			// ideally we'd want these listeners assigned dynamically here in the loop :(
			//newButton.GetComponent<Button> ().onClick.AddListener (() => Action (actions.IndexOf(action)));
			newButton.transform.SetParent (actionSelectSpacer, false);
			actionButtons.Add (newButton);
		}

		// dirty fix addressed above. To be resolved in the future
		actionButtons[0].gameObject.GetComponent<Button>().onClick.AddListener (() => Action (0));
		actionButtons[1].gameObject.GetComponent<Button>().onClick.AddListener (() => Action (1));

	}

	// sets characterChoice's agent and chosenAction, player can now click on a target
	public void Action (int actionIndex)
	{
		CharacterStateMachine agent = charactersToManage[0];
		characterChoice.agent = agent;
		BaseAction action = agent.character.actions [actionIndex];
		characterChoice.chosenAction = action;

		playerInput = playerGUI.TARGETING;
		// if action takes a target, enter targeting state, otherwise end selecting state
		if (action.takesTarget == true)
		{
			playerInput = playerGUI.TARGETING;
		}
		else 
		{
			playerInput = playerGUI.DONE;
		}
			
	}

	public void SelectTarget (CharacterStateMachine target)
	{
		//this just defaults to the only enemy at the moment :(
		characterChoice.target = target;
		playerInput = playerGUI.DONE;
	}

	// runs after player has selected an action and a target
	void PlayerInputDone()
	{
		performList.Add (characterChoice);		// add action to queue (work will need to be done here to separate from BSM)
		ExitSelection();
		charactersToManage.RemoveAt(0);	// remove from queue
		timeScale = baseTimeScale;
	}

	// hide the action select and enemy select panels
	public void ExitSelection()
	{
		actionSelectPanel.SetActive (false);
		// clean up action panel after selection is made
		foreach (GameObject button in actionButtons)
		{
			Destroy (button);
		}
		actionButtons.Clear ();
		playerInput = playerGUI.ACTIVE;		// set gui state to active
	}

	void EndBattle()
	{
		foreach ( CharacterStateMachine CSM in allCSMs )
		{
			ExitSelection();	//hide player input panels
			CSM.curState = CharacterStateMachine.characterState.WAITING;	// make all idle
			performList.Clear();			// no future actions
			charactersToManage.Clear ();	// no future selections

		}
	}

	void CreateBars(List<CharacterStateMachine> characters, Transform spacer)
	{
		foreach (CharacterStateMachine curCharacter in characters)
		{
			GameObject newBar = Instantiate (characterBar) as GameObject;
			CharacterBar bar = newBar.GetComponent<CharacterBar>();

			Text barText = newBar.transform.FindChild ("Name").gameObject.GetComponent<Text>();
			barText.text = curCharacter.character.name;	

			bar.CharacterPrefab = curCharacter.gameObject;	// link to character object

			curCharacter.phaseBar = newBar.transform.FindChild("Border/PhaseBar").gameObject.GetComponent<Image>();
			curCharacter.lifeBar = newBar.transform.FindChild("Border/LifeBar").gameObject.GetComponent<Image>();

			newBar.transform.SetParent(spacer);

		}
	}
}
