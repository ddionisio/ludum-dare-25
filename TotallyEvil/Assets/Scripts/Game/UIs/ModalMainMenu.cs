using UnityEngine;
using System.Collections;

public class ModalMainMenu : UIController {
	public UIEventListener buttonStart;
	public UIEventListener buttonHowTo;
	
	
	void OnButtonStart(GameObject go) {
		Main.instance.sceneManager.LoadLevel(0);
	}
	
	void OnButtonHowTo(GameObject go) {
		UIModalManager.instance.ModalOpen(UIModalManager.Modal.HowToPlay);
	}
	
	void Awake() {
		buttonStart.onClick += OnButtonStart;
		buttonHowTo.onClick += OnButtonHowTo;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
