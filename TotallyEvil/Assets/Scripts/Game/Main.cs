using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	//only use these after awake
	public static int layerIgnoreRaycast;
	public static int layerGround;
	public static int layerEnemy;
	public static int layerEnemyProjectile;
	public static int layerStructure;
	
	public static int layerMaskGround;
	public static int layerMaskEnemy;
	public static int layerMaskEnemyProjectile;
	public static int layerMaskStructure;
	
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
		layerEnemy = LayerMask.NameToLayer("Enemy");
		layerEnemyProjectile = LayerMask.NameToLayer("EnemyProjectile");
		layerStructure = LayerMask.NameToLayer("Structure");
	
			
		layerMaskGround = 1<<layerGround;
		layerMaskEnemy = 1<<layerEnemy;
		layerMaskEnemyProjectile = 1<<layerEnemyProjectile;
		layerMaskStructure = 1<<layerStructure;
		
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
		
		//gonna lower the volume so your ears won't bleed from the terrible music ;)
		AudioListener.volume = 0.05f;
	}
}
