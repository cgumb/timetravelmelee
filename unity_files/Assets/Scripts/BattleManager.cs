using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.


// draws background tiles (we can scrap that later) and heroes and enemies
public class BattleManager : MonoBehaviour
{
	// Using Serializable allows us to embed a class with sub properties in the inspector.
	[Serializable]
	public class Count
	{
		public int minimum; 			//Minimum value for our Count class.
		public int maximum; 			//Maximum value for our Count class.


		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public int level = 7;											// determins number of enemies generated
	public int columns = 13; 										//Number of columns in our game board.
	public int rows = 8;											//Number of rows in our game board.
	public int division = 4;
	public Count enemyCount = new Count(1, 4);						//Number of enemies to be generated
	public int heroCount = 3;
	public GameObject[] floorTiles;									//Array of floor prefabs.
	public GameObject[] enemyTiles;									//Array of enemy prefabs.
	public GameObject[] heroTiles;									//Array of hero prefabs.

	public GameObject[] outerWallTiles;								//Array of outer tile prefabs.

	private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
	public List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.
	public List <Vector3> heroPositions = new List <Vector3> ();	//A list of possible locations to place tiles.
	public List <Vector3> enemyPositions = new List <Vector3> ();	//A list of possible locations to place tiles.
	public int heroFrontRow = 4;
	public int heroBackRow = 1;
	public int enemyFrontRow = 8;
	public int enemyBackRow = 11;


	//Clears our list gridPositions and prepares it to generate a new board.
	void InitialiseList ()
	{
		//Clear our list gridPositions.
		gridPositions.Clear ();

		//Loop through x axis (columns).
		for(int x = 1; x < columns-1; x++)
		{
			//Within each column, loop through y axis (rows).
			for(int y = 1; y < rows-1; y++)
			{
				//At each index add a new Vector3 to our list with the x and y coordinates of that position.
				if (y < division)
				{
					gridPositions.Add(new Vector3 (x, y, 0f));
				}
			}
		}
		foreach (Vector3 position in gridPositions)
		{
			if (position.x == heroFrontRow || position.x == heroBackRow)
			{
				heroPositions.Add(position);
			}
			if (position.x == enemyFrontRow || position.x == enemyBackRow)
			{
				enemyPositions.Add(position);
			}
		}
	}


	//Sets up the outer walls and floor (background) of the game board.
	void BoardSetup ()
	{
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;

		//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
		for(int x = -1; x < columns + 1; x++)
		{
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for(int y = -1; y < rows + 1; y++)
			{
				//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
				GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];

				//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
				if (x == -1 || x == columns || y == -1 || y == rows || y == division)
				{
					toInstantiate = outerWallTiles [Random.Range(0, outerWallTiles.Length)];
				}

				//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

				//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
				instance.transform.SetParent (boardHolder);
			}
		}
	}


	//RandomPosition returns a random position from positions parameter.
	Vector3 RandomPosition (List<Vector3> positions)
	{
		//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
		int randomIndex = Random.Range (0, positions.Count);

		//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
		Vector3 randomPosition = positions[randomIndex];

		// remove all other positions in the same row
		positions.RemoveAll(position => position.y == randomPosition.y);

		//Return the randomly selected Vector3 position.
		return randomPosition;
	}


	// LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
	//the positions parameter are the valid positions the objects can be placed
	void PlaceUnitsAtRandom (GameObject[] tileArray, int minimum, int maximum, List<Vector3> positions)
	{
		//Choose a random number of objects to instantiate within the minimum and maximum limits
		int objectCount = Random.Range (minimum, maximum+1);
		//Instantiate objects until the randomly chosen limit objectCount is reached
		for(int i = 0; i < objectCount; i++)
		{
			//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
			Vector3 randomPosition = RandomPosition(positions);

			//Choose a random tile from tileArray and assign it to tileChoice
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];

			// mark the row of the newly placed unit
			int row = (int)randomPosition.x;
			if (row == heroFrontRow || row == enemyFrontRow)
			{
				tileChoice.gameObject.GetComponent<CharacterStateMachine>().character.frontRow = true;
			}
			else
			{
				tileChoice.gameObject.GetComponent<CharacterStateMachine>().character.frontRow = false;
			}

			//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}
		
	//SetupScene initializes our level and calls the previous functions to lay out the game board
	public void Awake ()
	{
		
		//Creates the outer walls and floor.
		BoardSetup ();

		//Reset our list of gridpositions.
		InitialiseList ();

		//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
		PlaceUnitsAtRandom (enemyTiles, enemyCount.minimum, enemyCount.maximum, enemyPositions);

		//Instantiate a random number of heroes based on minimum and maximum, at randomized positions.
		PlaceUnitsAtRandom (heroTiles, heroCount, heroCount, heroPositions);
	}
}

