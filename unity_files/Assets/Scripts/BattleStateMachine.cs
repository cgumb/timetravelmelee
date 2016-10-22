using UnityEngine;
using UnityEngine.UI; // for Text and UI Classes (e.g., Button)
using System.Collections;
using System.Collections.Generic; // for List

public class BattleStateMachine : MonoBehaviour {

	public enum PerformAction
	{
		WAIT,
		TAKEACTION,
		PERFORMACTION,
		WIN,
		LOSE
	}

	public PerformAction battleState;

	public List<GameObject> Characters = new List<GameObject> ();
	public List<GameObject> Enemies= new List<GameObject> ();

	public List<HandleTurn> PerformList = new List<HandleTurn> ();


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

	public GameObject ActionSelectPanel;
	public Transform actionSelectSpacer;
	public GameObject actionButton;
	private List<GameObject> actionButtons = new List<GameObject> ();

	public GameObject EnemySelectPanel;

	public playerGUI playerInput;

	public List<GameObject> CharactersToManage = new List<GameObject> ();
	public HandleTurn CharacterChoice;


	// Use this for initialization
	void Start () {
		battleState = PerformAction.WAIT;
		playerInput = playerGUI.ACTIVE;
		Characters.AddRange (GameObject.FindGameObjectsWithTag ("Character"));
		Enemies.AddRange (GameObject.FindGameObjectsWithTag ("Enemy"));

		ActionSelectPanel.SetActive (false);
		EnemySelectPanel.SetActive (false);

		CreateBars ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// battleState tracker
		switch (battleState)
		{
		case(PerformAction.WAIT):
			if (PerformList.Count > 0)
			{
				battleState = PerformAction.TAKEACTION;
			}
			break;

		case(PerformAction.TAKEACTION):
			GameObject Performer = PerformList [0].PerformersGameObject;
			if (PerformList [0].Type == "Enemy")
			{
				EnemyStateMachine ESM = Performer.GetComponent<EnemyStateMachine> ();
				ESM.target = PerformList [0].PerformersTarget;
				ESM.curState = EnemyStateMachine.TurnState.ACTION;
			}
			if (PerformList [0].Type == "Character")
			{
				CharacterStateMachine CSM = Performer.GetComponent<CharacterStateMachine> ();
				CSM.target = PerformList [0].PerformersTarget;
				CSM.curState = CharacterStateMachine.TurnState.ACTION;
			}
			battleState = PerformAction.PERFORMACTION;
			break;

		case(PerformAction.PERFORMACTION):

			break;

		case(PerformAction.WIN):
			HidePanels ();
			Debug.Log ("You Win!");
			break;

		case(PerformAction.LOSE):
			HidePanels ();
			Debug.Log ("Game Over :(");
			break;
		}

		// playerGUI state tracker
		switch (playerInput)
		{
		case (playerGUI.ACTIVE):
			if (CharactersToManage.Count > 0)
			{
				CharactersToManage [0].transform.FindChild ("Selector").gameObject.SetActive (true);
				CharacterChoice = new HandleTurn ();
				ActionSelectPanel.SetActive (true);
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

	public void collectActions(HandleTurn turn)
	{
		PerformList.Add (turn);
		Debug.Log ("turn added!");
	}




	// creates status bars for all characters
	void CharacterBars()
	{
		foreach (GameObject character in Characters)
		{
			GameObject newBar = Instantiate (characterBar) as GameObject;
			CharacterBar bar = newBar.GetComponent<CharacterBar>();

			CharacterStateMachine curCharacter = character.GetComponent<CharacterStateMachine> ();

			Text barText = newBar.transform.FindChild ("Name").gameObject.GetComponent<Text>();
			barText.text = curCharacter.character.name;

			bar.CharacterPrefab = character;

			curCharacter.ProgressBar = newBar.transform.FindChild("Border/ProgressBar").gameObject.GetComponent<Image>();
			curCharacter.LifeBar = newBar.transform.FindChild("Border/LifeBar").gameObject.GetComponent<Image>();

			newBar.transform.SetParent (characterSpacer);
		}
	}

	// creates status bars for all enemies
	void EnemyBars()
	{
		foreach (GameObject enemy in Enemies)
		{
			GameObject newBar = Instantiate (enemyBar) as GameObject;
			EnemyBar bar = newBar.GetComponent<EnemyBar>();

			EnemyStateMachine curEnemy = enemy.GetComponent<EnemyStateMachine> ();

			Text barText = newBar.transform.FindChild ("Name").gameObject.GetComponent<Text>();
			barText.text = curEnemy.enemy.name;

			bar.EnemyPrefab = enemy;

			curEnemy.ProgressBar = newBar.transform.FindChild("Border/ProgressBar").gameObject.GetComponent<Image>();
			curEnemy.LifeBar = newBar.transform.FindChild("Border/LifeBar").gameObject.GetComponent<Image>();

			newBar.transform.SetParent (enemySpacer);
		}
	}

	// wrapper function
	void CreateBars()
	{
		CharacterBars ();
		EnemyBars ();
	}

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
		CharacterChoice.PerformersGameObject = CharactersToManage [0];
		CharacterChoice.Type = "Character";
		CharacterChoice.ChosenAction = CharactersToManage[0].GetComponent<CharacterStateMachine>().character.Actions[0];

		ActionSelectPanel.SetActive (false);
		EnemySelectPanel.SetActive (true);
	}
		

	public void SelectTarget (GameObject Target)
	{
		//this just defaults to the only enemy at the moment
		CharacterChoice.PerformersTarget = Target;
		playerInput = playerGUI.DONE;
	}

	void PlayerInputDone()
	{
		PerformList.Add (CharacterChoice);
		EnemySelectPanel.SetActive (false);
		CharactersToManage [0].transform.FindChild ("Selector").gameObject.SetActive (false);
		CharactersToManage.RemoveAt (0);
		playerInput = playerGUI.ACTIVE;

		// clean up attack panel
		foreach (GameObject button in actionButtons) 
		{
			Destroy (button);
		}
		actionButtons.Clear ();
	}

	void HidePanels()
	{
		ActionSelectPanel.SetActive (false);
		EnemySelectPanel.SetActive (false);
	}
}
	