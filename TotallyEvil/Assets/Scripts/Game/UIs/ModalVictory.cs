using UnityEngine;
using System.Collections;

public class ModalVictory : UIController {
	public UIEventListener buttonQuit;
	
	void OnButtonQuit(GameObject go) {
		Main.instance.sceneManager.LoadScene(SceneManager.Scene.start);
	}
	
	void Awake() {
		buttonQuit.onClick += OnButtonQuit;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
