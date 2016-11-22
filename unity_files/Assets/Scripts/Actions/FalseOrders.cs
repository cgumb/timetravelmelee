using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FalseOrders : BaseAction {

	public StatusEffect falseOrders;	// FalseOrdersStatusEfect prefab goes here

	public FalseOrders ()
	{
		actionName = "False Orders";
		actionEnergyCost = 2;
		takesTarget = true;	

	}

	// this is where the actual function used in TimeForAction should go
	public override void ActionEffect()
	{
		BSM = GameObject.Find ("BattleManager").GetComponent<BattleStateMachine> ();
		CharacterStateMachine target = BSM.curAction.target;
		int direction = target.character.frontRow ? -1 : 1;	// negative means move left on x-axis, postive right
		if (target.gameObject.CompareTag("Enemy"))
		{
			direction *= -1;
		}
		float newX = target.transform.position.x + (BSM.Draw.columnOffsetX * direction);
		target.startPosition = new Vector2(newX, target.transform.position.y);
		target.transform.position = target.startPosition;
		target.character.frontRow = !target.character.frontRow;

		StatusEffect newFalseOrders = Instantiate(falseOrders);
		newFalseOrders.subject.statusEffects.Add(newFalseOrders);
		newFalseOrders.initialized = true;
	}
}
