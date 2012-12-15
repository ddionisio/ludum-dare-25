using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	//only use these after awake
	public static int layerIgnoreRaycast;
	public static int layerGround;
	
	public static int layerMaskGround;
	
	[System.NonSerialized] public UserSettings userSettings;
	[System.NonSerialized] public UserData userData;
	[System.NonSerialized] public SceneManager sceneManager;
	
	private static Main mInstance = null;
	
	public static Main instance {
		get {
			return mInstance;
		}
	}
	
	public SceneController sceneController {
		get {
			return sceneManager.sceneController;
		}
	}
	
	void OnApplicationQuit() {
		mInstance = null;
	}
	
	void OnEnable() {
	}
			
	void Awake() {
		mInstance = this;
		
		layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
		layerGround = LayerMask.NameToLayer("Ground");
		
		layerMaskGround = 1<<layerGround;
		
		DontDestroyOnLoad(gameObject);
		
		userData = GetComponentInChildren<UserData>();
		userSettings = GetComponentInChildren<UserSettings>();
		
		sceneManager = GetComponentInChildren<SceneManager>();
	}
	
	void Start() {
		//TODO: maybe do other things before starting the game
		//go to start if we are in main scene
		SceneManager.Scene mainScene = SceneManager.Scene.main;
		if(Application.loadedLevelName == mainScene.ToString()) {
			sceneManager.LoadScene(SceneManager.Scene.start);
		}
		else {
			sceneManager.InitScene();
		}
	}
}
