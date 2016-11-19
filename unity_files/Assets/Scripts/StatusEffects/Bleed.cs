using UnityEngine;
using System.Collections;

public class Bleed : StatusEffect {

	public float totalDamage;
	public float damagePerTick;
	public int durationInTurns;
	public float durationInTime;
	// Use this for initialization
	void Start ()
	{
		subject.PlaySound(subject.character.bleedSound);
		statusEffectName = "Bleed";

		durationInTurns = 3;
		durationInTime = subject.maxPhase * durationInTurns;

		totalDamage = 30;
		damagePerTick = totalDamage / durationInTime;
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (initialized)
		{
			if (durationInTime <= 0)
			{
				subject.statusEffects.Remove(this);
				Destroy(this);
			}
			else if (subject.curState == CharacterStateMachine.characterState.PHASING_IN)
			{
				durationInTime -= (subject.character.curSpeed * Time.deltaTime * BSM.timeScale);
				subject.TakeDamage(subject.character.curSpeed * Time.deltaTime * BSM.timeScale, canBeDefended: false, isSilent: true);

				tooltipString = Mathf.Round(damagePerTick * subject.character.curSpeed) + " damager per second for " + Mathf.Round(durationInTime) + " seconds";
			}
		}
	}
}
