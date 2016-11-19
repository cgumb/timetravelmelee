using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class PlayVideo : MonoBehaviour {


	public MovieTexture movie;
	//private AudioSource audio;

	void Start () {
		GetComponent<RawImage>().texture = movie as MovieTexture;
		//audio = GetComponent<AudioSource>();
		//audio.clip = movie.audioClip;
		movie.Play ();
		//audio.Play ();


	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space) && movie.isPlaying) 
		{
			Destroy (GetComponent<RawImage> ());
			GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Load();
		}
		else if (Input.GetKeyDown (KeyCode.Space) && !movie.isPlaying) 
		{
			movie.Play();
		}
	}
}
