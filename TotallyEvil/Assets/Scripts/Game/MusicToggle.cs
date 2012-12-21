using UnityEngine;
using System.Collections;

public class MusicToggle : MonoBehaviour {
	public UIEventListener clicky;

	// Use this for initialization
	void Start () {
		AudioListener.volume = 1.0f;
		clicky.onClick += Clicky;
	}
	
	void Clicky(GameObject go) {
		AudioListener.volume = AudioListener.volume > 0 ? 0.0f : 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
