using UnityEngine;
using UnityEngine.UI; // for Text
using System.Collections;
using System;		// for String

[System.Serializable]
public class Damage : MonoBehaviour {

	public int fadeTime = 150;
	public int curTime;
	float newGama = 1f;
	float newY;
	public float toRise = 1f;
	public Vector2 startPosition;
	private bool initialized = false;

	// Use this for initialization
	void Start () 
	{
		curTime = fadeTime;
		newY = 0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (initialized)
		{
			if (curTime > 0)
			{
				Debug.Log("Counting Down curTime");
				newGama = (curTime / (float)fadeTime) * newGama;

				this.GetComponent<TextMesh>().color = new Color(1, 1, 1, newGama);

				newY = (1 - curTime / (float) fadeTime) * toRise;
				this.transform.position = new Vector2 (startPosition.x, startPosition.y + newY);

				curTime = curTime - 1;
			}
			else
			{
				Destroy(this.gameObject);
			}
		}
	}

	public static void DrawDamage( float damageAmount, CharacterStateMachine damagedCharacter )
	{
		GameObject damage = Instantiate(Resources.Load("Damage")) as GameObject;

		Damage newDamage = damage.GetComponent<Damage>();

		newDamage.startPosition = damagedCharacter.transform.position;
		newDamage.gameObject.GetComponent<TextMesh>().text = String.Format("-{0}", damageAmount);
		damage.GetComponent<Renderer>().sortingLayerName = "UI";
	
		newDamage.initialized = true;
	}
}
