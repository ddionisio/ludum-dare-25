using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
	public Vector2 cornerCap;
	
	public float gravity;
	
	private static World mInstance;
	
	private tk2dTileMap mTileMap;
	
	public static World instance {
		get { return mInstance; }
	}
	
	public tk2dTileMap tileMap {
		get { return mTileMap; }
	}
	
	public float CapGround(float y, float halfH) {
		float wY = transform.position.y - tileMap.data.tileOrigin.y;
		
		if(y - halfH < wY) {
			 y = wY + halfH;
		}
		
		return y;
	}
	
	public Vector3 Cap(Vector3 center, float halfW, float halfH) {
		Vector3 pos = center;
		
		Vector3 wPos = transform.position;
		wPos.x += cornerCap.x;
		wPos.y += cornerCap.y;
						
		float worldW = tileMap.width*tileMap.partitionSizeX - cornerCap.x*2.0f + tileMap.data.tileOrigin.x*2.0f;
		float worldH = tileMap.height*tileMap.partitionSizeY - cornerCap.y*2.0f + tileMap.data.tileOrigin.y*2.0f;
		
		if(pos.x - halfW < wPos.x) {
			pos.x = wPos.x + halfW;
		}
		else if(pos.x + halfW > wPos.x + worldW) {
			pos.x = wPos.x + worldW - halfW;
		}
		
		if(pos.y - halfH < wPos.y) {
			pos.y = wPos.y + halfH;
		}
		else if(pos.y + halfH > wPos.y + worldH) {
			pos.y = wPos.y + worldH - halfH;
		}
		
		return pos;
	}
	
	void OnDestroy() {
		mInstance = null;
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
