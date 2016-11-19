using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.
using System.Linq;


// draws background tiles (we can scrap that later) and heroes and enemies
public class DrawBattle : MonoBehaviour
{

	// Grid Stuff
	public static int maxRows = 4;
	public static float bottomLeftX = -6.68f;
	public static float bottomLeftY = -3.63f;
	public  Vector2 bottomLeftPosition = new Vector2(bottomLeftX, bottomLeftY); 	// player's bottom row, back line

	public static float rowOffsetX = 0.3f;	// X difference between rows
	public static float rowOffsetY = 1.5f;	// Y difference between rows
	public Vector2 rowOffset = new Vector2(rowOffsetX, rowOffsetY);

	public float columnOffsetX  = 3.06f;	// distance between front and back rows

	// this wasn't working...
	//public float range = Mathf.Abs((bottomLeftY + (maxRows * rowOffsetY)) - bottomLeftY);
	public float range = 6;

	public BattleTemplate curBattle;	// the battle to be loaded (i.e., which characters to create)


	//SetupScene initializes our level and calls the previous functions to lay out the game board
	public void Draw()
	{
		//PLAYER
		//front row
		SetPosition(curBattle.player_row_1_front, 1, true);
		SetPosition(curBattle.player_row_2_front, 2, true);
		SetPosition(curBattle.player_row_3_front, 3, true);
		SetPosition(curBattle.player_row_4_front, 4, true);
		//back row
		SetPosition(curBattle.player_row_1_back, 1, false);
		SetPosition(curBattle.player_row_2_back, 2, false);
		SetPosition(curBattle.player_row_3_back, 3, false);
		SetPosition(curBattle.player_row_4_back, 4, false);

		//ENEMY
		//front row
		SetPosition(curBattle.enemy_row_1_front, 1, true);
		SetPosition(curBattle.enemy_row_2_front, 2, true);
		SetPosition(curBattle.enemy_row_3_front, 3, true);
		SetPosition(curBattle.enemy_row_4_front, 4, true);
		//back row
		SetPosition(curBattle.enemy_row_1_back, 1, false);
		SetPosition(curBattle.enemy_row_2_back, 2, false);
		SetPosition(curBattle.enemy_row_3_back, 3, false);
		SetPosition(curBattle.enemy_row_4_back, 4, false);
	}

	public void SetPosition(GameObject character, int row, bool front)
	{
		if (character == null)	// skip if position is empty
		{
			return;
		}

		Vector2 bottomBackPosition = bottomLeftPosition;
		int direction = 1;

		if (character.gameObject.CompareTag("Enemy"))
		{
			bottomBackPosition.x = -1 * bottomBackPosition.x;
			direction = -1;
		}

		Vector2 position = bottomBackPosition + (row - 1) * new Vector2(rowOffset.x * direction, rowOffset.y);

		if (front == true)
		{
			character.gameObject.GetComponent<CharacterStateMachine>().character.frontRow = true;
			position.x += columnOffsetX * direction;
		}
		else
		{
			character.gameObject.GetComponent<CharacterStateMachine>().character.frontRow = false;
		}

		character.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5 - row;

		GameObject instance = Instantiate (character, position, Quaternion.identity) as GameObject;
	}
}

