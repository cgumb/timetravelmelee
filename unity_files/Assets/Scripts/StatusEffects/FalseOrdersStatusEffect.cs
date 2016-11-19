using UnityEngine;
using System.Collections;

public class FalseOrdersStatusEffect : StatusEffect {

	public float slowAmount;
//	public int durationInTurns;
//	public float durationInTime;
	// Use this for initialization
	void Start ()
	{

		statusEffectName = "False Orders";

	//	durationInTurns = 1;
	//	durationInTime = subject.maxPhase * durationInTurns;
		slowAmount = 0.5f;
	}

	// Update is called once per frame
	void Update ()
	{
		if (initialized)
		{
			if (subject.curPhase >= subject.maxPhase)
			{
				subject.statusEffects.Remove(this);
				Destroy(this);
			}

			// multiplying curSpeed by slowAmount gives a wacky number :/
			subject.character.curSpeed = slowAmount;

			tooltipString = "Half speed until next turn";

		}
	}
}
