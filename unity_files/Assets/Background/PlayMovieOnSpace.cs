using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(AudioSource))]

public class PlayMovieOnSpace : MonoBehaviour {

	public MovieTexture movie;
	private AudioSource audioSource;

	void Start () {
		GetComponent<RawImage>().texture = movie as MovieTexture;
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = movie.audioClip;
		movie.Play();
		audioSource.Play();


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