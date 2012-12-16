using UnityEngine;
using System.Collections;

public class ModalGameOver : UIController {
	public UIEventListener buttonRetry;
	public UIEventListener buttonQuit;
	
	void OnButtonRetry(GameObject go) {
		Main.instance.sceneManager.ReloadLevel();
	}
	
	void OnButtonQuit(GameObject go) {
		Main.instance.sceneManager.LoadScene(SceneManager.Scene.start);
	}

	void Awake() {
		buttonRetry.onClick += OnButtonRetry;
		buttonQuit.onClick += OnButtonQuit;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
