using UnityEngine;
using System.Collections;

public class ModalHowTo : UIController {
	
	public UIEventListener buttonReturn;
	
	void OnButtonReturn(GameObject go) {
		UIModalManager.instance.ModalCloseTop();
	}
	
	void Awake() {
		buttonReturn.onClick += OnButtonReturn;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
