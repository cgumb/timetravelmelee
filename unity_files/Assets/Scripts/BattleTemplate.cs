using UnityEngine;
using System.Collections;
using System.Collections.Generic; // for List

// Used by Battle State Machine to load specific units for a given battle
public class BattleTemplate : MonoBehaviour
{
	public class BattlePlacement
	{
		public int row;
		public bool front;
		public GameObject character = null;

		public BattlePlacement(int row, bool front)
		{
			this.row = row;
			this.front = front;
		}
	}
	public GameObject player_row_1_front;
	public GameObject player_row_2_front;
	public GameObject player_row_3_front;
	public GameObject player_row_4_front;


	public GameObject player_row_1_back;
	public GameObject player_row_2_back;
	public GameObject player_row_3_back;
	public GameObject player_row_4_back;


	public GameObject enemy_row_1_front;
	public GameObject enemy_row_2_front;
	public GameObject enemy_row_3_front;
	public GameObject enemy_row_4_front;

	public GameObject enemy_row_1_back;
	public GameObject enemy_row_2_back;
	public GameObject enemy_row_3_back;
	public GameObject enemy_row_4_back;

	/*
	public BattlePlacement row_1_front = new BattlePlacement(1, true);
	public BattlePlacement row_2_front = new BattlePlacement(2, true);
	public BattlePlacement row_3_front = new BattlePlacement(3, true);
	public BattlePlacement row_4_front = new BattlePlacement(4, true);


	public BattlePlacement row_1_back = new BattlePlacement(1, false);
	public BattlePlacement row_2_back = new BattlePlacement(2, false);
	public BattlePlacement row_3_back = new BattlePlacement(3, false);
	public BattlePlacement row_4_back = new BattlePlacement(4, false);
	*/

}
