using UnityEngine;
using System.Collections;
using System.Linq;			// for List.Where()

public class FiringLine : Passive
{
	public int bonusLevel;
	public float baseBonus;
	public float totalBonus;

	void Awake() {
		statusEffectName = "Firing Line";
		bonusLevel = 0;
		baseBonus = 3;
	}

	// calculate current bonus, add it to defense, and update tooltip string
	void Update ()
	{
		if (initialized)
		{
			bonusLevel = BSM.enemies.Where(e => e.name == subject.name && e.character.frontRow == subject.character.frontRow && e.IsAlive()).Count() - 1;
			if (bonusLevel > 1)
			{
				totalBonus = Mathf.Pow(baseBonus, bonusLevel);
			}
			else
			{
				totalBonus = 0;
			}
			subject.character.curDefense = subject.character.baseDefense + totalBonus;
			tooltipString = "+" + totalBonus + " to defense";
		}
	}
}
