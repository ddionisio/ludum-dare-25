using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	public float gravity;
	
	private static World mInstance;
	
	private tk2dTileMap mTileMap;
	
	public static World instance {
		get { return mInstance; }
	}
	
	public tk2dTileMap tileMap {
		get { return mTileMap; }
	}
	
	void Awake() {
		mInstance = this;
		
		mTileMap = GetComponentInChildren<tk2dTileMap>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
