using UnityEngine;
using System.Collections;

public class ModalPause : UIController {
	
	public UIEventListener buttonResume;
	public UIEventListener buttonQuit;
	
	void OnButtonResume(GameObject go) {
		Main.instance.sceneManager.Resume();
		UIModalManager.instance.ModalCloseAll();
	}
	
	void OnButtonQuit(GameObject go) {
		//TODO: confirm
		Main.instance.sceneManager.Resume();
		Main.instance.sceneManager.LoadScene(SceneManager.Scene.start);
	}
	
	void Awake() {
		buttonResume.onClick += OnButtonResume;
		buttonQuit.onClick += OnButtonQuit;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
