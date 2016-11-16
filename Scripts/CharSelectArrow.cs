using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;

[System.Serializable]
public class CharSelectArrow : MonoBehaviour
{
	private Vector2 position = new Vector2(0f,0f);
	public bool show = false;
	public Texture2D arrowTexture;
	public GUIStyle style = new GUIStyle();

	public void Start() {
		style.normal.background = arrowTexture;
	}
		
	public void moveTo(Vector2 destination)
	{
		position = destination;
	}

	public void toggleArrow()
	{
		show = !show;
	}

	protected void OnGUI()
	{
		if (show)
		{
			GUI.Box(new Rect(position.x, position.y, 32, 32), arrowTexture);
		}
	}

}

