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

	public List<GameObject> characters = new List<GameObject> ();
	public List<GameObject> enemies= new List<GameObject> ();

	public List<Action> performList = new List<Action> ();
	public List<GameObject> charactersToManage = new List<GameObject> ();


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
		// BSM
		battleState = battleStates.WAIT;
		characters.AddRange (GameObject.FindGameObjectsWithTag ("Character"));
		enemies.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));
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
			HidePanels ();
			Debug.Log ("You Win!");
			break;

		case(battleStates.LOSE):
			HidePanels ();
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
		foreach (GameObject character in characters)
		{
			GameObject newBar = Instantiate (characterBar) as GameObject;
			CharacterBar bar = newBar.GetComponent<CharacterBar>();

			CharacterStateMachine curCharacter = character.GetComponent<CharacterStateMachine> ();

			Text barText = newBar.transform.FindChild ("Name").gameObject.GetComponent<Text>();
			barText.text = curCharacter.character.name;

			bar.CharacterPrefab = character;

			curCharacter.phaseBar = newBar.transform.FindChild("Border/PhaseBar").gameObject.GetComponent<Image>();
			curCharacter.lifeBar = newBar.transform.FindChild("Border/LifeBar").gameObject.GetComponent<Image>();

			newBar.transform.SetParent (characterSpacer);
		}
	}

	// creates status bars for all enemies
	void EnemyBars()
	{
		foreach (GameObject enemy in enemies)
		{
			GameObject newBar = Instantiate (enemyBar) as GameObject;
			EnemyBar bar = newBar.GetComponent<EnemyBar>();

			EnemyStateMachine curEnemy = enemy.GetComponent<EnemyStateMachine> ();

			Text barText = newBar.transform.FindChild ("Name").gameObject.GetComponent<Text>();
			barText.text = curEnemy.character.name;

			bar.EnemyPrefab = enemy;

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

	// populate the action panel of actions available to the character
	void CreateActionButtons ()
	{
		GameObject newButton = Instantiate (actionButton) as GameObject;
		Text actionButtonText = newButton.transform.FindChild ("Text").gameObject.GetComponent<Text>();
		actionButtonText.text = "Attack";
		newButton.GetComponent<Button> ().onClick.AddListener (() => Action1 ());
		newButton.transform.SetParent (actionSelectSpacer, false);
		actionButtons.Add (newButton);
	}


	public void Action1 ()
	{
		characterChoice.agent = charactersToManage [0];
	//	characterChoice.type = "Character";
		characterChoice.chosenAction = charactersToManage[0].GetComponent<CharacterStateMachine>().character.actions[0];

		actionSelectPanel.SetActive (false);
		enemySelectPanel.SetActive (true);
	}


	public void SelectTarget (GameObject Target)
	{
		//this just defaults to the only enemy at the moment :(
		characterChoice.target = Target;
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
}
