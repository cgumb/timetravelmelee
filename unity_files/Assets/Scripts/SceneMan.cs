using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMan : MonoBehaviour {
	void Start () {
		SceneManager.LoadScene ("Demo", LoadSceneMode.Additive);
	}
}