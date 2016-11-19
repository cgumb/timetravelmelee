using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(AudioSource))]

public class PlayMovieOnSpace : MonoBehaviour {

	public MovieTexture movie;
	private AudioSource audio;

	void Start () {
		GetComponent<RawImage>().texture = movie as MovieTexture;
		audio = GetComponent<AudioSource>();
		audio.clip = movie.audioClip;
		movie.Play();
		audio.Play();


	}

	void Update () {
		if (Input.GetButtonDown ("Jump")) {

			Renderer r = GetComponent<Renderer>();
			MovieTexture movie = (MovieTexture)r.material.mainTexture;

			if (movie.isPlaying) {
				movie.Pause();
			}
			else {
				movie.Play();
			}
		}
	}
}