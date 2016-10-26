using UnityEngine;
using UnityEngine.UI; // for Text and UI Classes (e.g., Button)
using System.Collections;
using System.Collections.Generic; // for List


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


	// player's interface
	public enum playerGUI
	{
		ACTIVE,
		WAITING,
		DONE
	}

	public GameObject enemyBar; 	// enemy status bar to create
	public Transform enemySpacer;	// where to place it

	public GameObject characterBar; 	// character bar to create
	public Transform characterSpacer;	// where to place it

	public GameObject actionSelectPanel;	// where player selects a character
	public Transform actionSelectSpacer;	// spacer holds each action option
	public GameObject actionButton;				// perfab of action button
	private List<GameObject> actionButtons = new List<GameObject> ();

	public GameObject enemySelectPanel;		// where player selecters a target
	public playerGUI playerInput;					// current state of playerGUI
	public Action characterChoice;			// action player has selected


	// Use this for initialization
	void Start ()
	{
		allCSMs = GameObject.FindObjectsOfType<CharacterStateMachine> ();
		foreach (CharacterStateMachine CSM in allCSMs)
		{
			if (CSM.CompareTag("Character")) {characters.Add(CSM);}
			if (CSM.CompareTag("Enemy")) {enemies.Add(CSM);}	
		}
		// BSM
		battleState = battleStates.WAIT;
		//characters.AddRange (GameObject.FindGameObjectsWithTag ("Character"));
		//enemies.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
		// GUI
		playerInput = playerGUI.ACTIVE;
		actionSelectPanel.SetActive (false);
		enemySelectPanel.SetActive (false);
		CreateBars ();
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
				charactersToManage [0].transform.FindChild ("Selector").gameObject.SetActive (true);
				characterChoice = new Action ();
				actionSelectPanel.SetActive (true);
				CreateActionButtons ();
				playerInput = playerGUI.WAITING;
			}
			break;

		case (playerGUI.WAITING):
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
		Debug.Log ("action added!");
	}

	// GUI methods
	// creates status bars for all characters
	void CharacterBars()
	{
		foreach (CharacterStateMachine curCharacter in characters)
		{
			GameObject newBar = Instantiate (characterBar) as GameObject;
			CharacterBar bar = newBar.GetComponent<CharacterBar>();

			Text barText = newBar.transform.FindChild ("Name").gameObject.GetComponent<Text>();
			barText.text = curCharacter.character.name;	

			bar.CharacterPrefab = curCharacter.gameObject;

			curCharacter.phaseBar = newBar.transform.FindChild("Border/PhaseBar").gameObject.GetComponent<Image>();
			curCharacter.lifeBar = newBar.transform.FindChild("Border/LifeBar").gameObject.GetComponent<Image>();

			newBar.transform.SetParent (characterSpacer);
		}
	}

	// creates status bars for all enemies
	void EnemyBars()
	{
		foreach (CharacterStateMachine curEnemy in enemies)
		{
			GameObject newBar = Instantiate (enemyBar) as GameObject;
			EnemyBar bar = newBar.GetComponent<EnemyBar>();

			Text barText = newBar.transform.FindChild ("Name").gameObject.GetComponent<Text>();
			barText.text = curEnemy.character.name;

			bar.EnemyPrefab = curEnemy.gameObject;

			curEnemy.phaseBar = newBar.transform.FindChild("Border/ProgressBar").gameObject.GetComponent<Image>();
			curEnemy.lifeBar = newBar.transform.FindChild("Border/LifeBar").gameObject.GetComponent<Image>();

			newBar.transform.SetParent (enemySpacer);
		}
	}

	// wrapper function
	void CreateBars()
	{
		CharacterBars ();
		EnemyBars ();
	}

	// populate the action panel with actions available to the character
	void CreateActionButtons ()
	{
		List<BaseAction> actions = charactersToManage[0].character.actions;

		foreach (BaseAction action in actions)
		{
			GameObject newButton = Instantiate (actionButton) as GameObject;
			Text actionButtonText = newButton.transform.FindChild ("Text").gameObject.GetComponent<Text>();
			actionButtonText.text = action.actionName;
			Debug.Log ("Making Button for " + action.actionName + " which has an index of " + actions.IndexOf(action));
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
		actionButtons[2].gameObject.GetComponent<Button>().onClick.AddListener (() => Action (2));

	}

	// sets characterChoice's agent and chosenAction, hides actionSelectPanel, and displays enemySelectPanel
	public void Action (int actionIndex)
	{
		CharacterStateMachine agent = charactersToManage[0];
		characterChoice.agent = agent;
		BaseAction action = agent.character.actions [actionIndex];
		characterChoice.chosenAction = action;

		actionSelectPanel.SetActive (false);	// hide actionSelectPanel
		// if action takes a target, show targetSelectPanel, otherwise end selecting state
		if (action.takesTarget == true) {
			enemySelectPanel.SetActive (true);
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
		enemySelectPanel.SetActive (false);
		charactersToManage [0].transform.FindChild ("Selector").gameObject.SetActive (false); // turn of selector
		charactersToManage.RemoveAt (0);	// remove from queue
		playerInput = playerGUI.ACTIVE;		// set gui state to active

		// clean up action panel after selection is made
		foreach (GameObject button in actionButtons)
		{
			Destroy (button);
		}
		actionButtons.Clear ();
	}

	// hide the action select and enemy select panels
	void HidePanels()
	{
		actionSelectPanel.SetActive (false);
		enemySelectPanel.SetActive (false);
	}

	void EndBattle()
	{
		foreach ( CharacterStateMachine CSM in allCSMs )
		{
			HidePanels ();	// hide player input panels
			CSM.curState = CharacterStateMachine.characterState.WAITING;	// make all idle
			performList.Clear();			// no future actions
			charactersToManage.Clear ();	// no future selections

		}
	}
}
