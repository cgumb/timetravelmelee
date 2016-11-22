using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;

[System.Serializable]
public class CharSelectArrow : MonoBehaviour
{
	public CharacterStateMachine owner;		// the character the arrow points to
	public int pulseSpeed;
	SpriteRenderer spriteRenderer;

	public void Awake()
	{
		this.owner = null;										// start without owner
		this.GetComponent<SpriteRenderer>().enabled = false;	// start invisible
		this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
		this.pulseSpeed = 8;

	}
		
	void Update()
	{
		if (owner != null)
		{
			Vector2 newPos = owner.transform.position;	// find owner's position
			newPos.y += 2.45f;							// add vertical offset
			this.transform.position = newPos;			// update position
		}

		Pulse();

	}

	public void Pulse()
	{
		float greenValue = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2.0f;
		spriteRenderer.color = new Color(1f, greenValue, 1f, 1f);
	}
}
