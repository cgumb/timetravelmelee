using UnityEngine;
using System.Collections;
using System.Linq;			// for List.Where()

public class FiringLine : Passive
{
	public int bonusLevel;
	public float defenseBonus;

	void Awake() {
		statusEffectName = "Firing Line";
		bonusLevel = 0;
		defenseBonus = 1;
	}

	// calculate current bonus, add it to defense, and update tooltip string
	void Update ()
	{
		if (initialized)
		{
			bonusLevel = BSM.enemies.Where(e => e.name == subject.name && e.character.frontRow == subject.character.frontRow && e.IsAlive()).Count() - 1;
			subject.character.curDefense = subject.character.baseDefense + bonusLevel * defenseBonus;
			tooltipString = "+" + (bonusLevel * defenseBonus) + " to defense";
		}
	}
}
