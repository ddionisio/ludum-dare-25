using UnityEngine;
using System.Collections;

public class Giblet : Entity {
	public float delayMin;
	public float delayMax;
	
	public float speedMin;
	public float speedMax;
	
	public float rotateSpeedMin;
	public float rotateSpeedMax;
	
	public float jumpSpdMin;
	public float jumpSpdMax;
	
	private float mScale;
	
	private float mCurTime;
	private float mMaxTime;
	private float mJumpSpd;
	
	private float mDefaultRadius;
	
	private tk2dBaseSprite mSpr;
	
	public static void Generate(Vector3 startPos, int numGibs, float scale) {
		//float worldScale = SceneWorld.instance.levels[SceneWorld.instance.curLevel].scale;
		
		for(int i = 0; i < numGibs; i++) {
			Transform t = EntityManager.instance.Spawn("giblet", null, null, null);
			
			Giblet giblet = t.GetComponentInChildren<Giblet>();
			
			Vector3 pos = giblet.transform.position;
			pos.x = startPos.x;
			pos.y = startPos.y;
			giblet.transform.position = pos;
			giblet.mScale = scale;
			
			if(giblet.mDefaultRadius == 0) {
				giblet.mDefaultRadius = giblet.entMove.radius;
			}
			
			giblet.entMove.radius = giblet.mDefaultRadius*scale;
		}
	}
	
	protected override void Awake () {
		base.Awake ();
		
		if(mDefaultRadius == 0) {
			mDefaultRadius = entMove.radius;
		}
		
		mSpr = GetComponentInChildren<tk2dBaseSprite>();
	}
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
		
		entMove.onLandGround += OnLand;
	}
	
	void LateUpdate () {
		switch(state) {
		case State.move:
			mCurTime += Time.deltaTime;
			if(mCurTime >= mMaxTime) {
				Release();
			}
			else {
				Color c = mSpr.color;
				c.a = 1.0f - mCurTime/mMaxTime;
				mSpr.color = c;
			}
			break;
		}
	}
	
	protected override void SpawnFinish() {
		float worldScale = SceneWorld.instance.levels[SceneWorld.instance.curLevel].scale;
		
		mMaxTime = Random.Range(delayMin, delayMax);
		
		mCurTime = 0;
		
		TransAnimSpinner spinner = GetComponentInChildren<TransAnimSpinner>();
		if(spinner != null) {
			spinner.rotatePerSecond = Random.Range(rotateSpeedMin, rotateSpeedMax);
		}
		
		entMove.velocity.x = Random.Range(speedMin, speedMax)*worldScale;
		
		mJumpSpd = Random.Range(jumpSpdMin, jumpSpdMax)*worldScale;
		
		entMove.Jump(mJumpSpd, false);
		
		state = Entity.State.move;
	}
	
	protected override void StateChanged() {
		switch(state) {
		case State.spawning:
			Vector3 s = mSpr.scale;
			s.x = s.y = mScale;
			mSpr.scale = s;
			break;
		}
	}
	
	void OnLand(EntityMovement entMove) {
		mJumpSpd *= 0.8f;
		entMove.Jump(mJumpSpd, false);
	}
}
