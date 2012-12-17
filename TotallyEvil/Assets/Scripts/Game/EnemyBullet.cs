using UnityEngine;
using System.Collections;

public class EnemyBullet : Entity {
	
	public float damage = 1.0f;
	
	public float bounceSpeed; //set to 0 to die when it hits the ground
	public float bounceSpeedScaleDecay = 0.8f;
	
	public float lifeDelay = 2.0f;
	
	private float mCurLifeTime = 0;
	
	private Vector2 mSpawnVelocity = Vector2.zero;
	private float mBounceSpeed;
	
	public static void Generate(string type, Vector3 startPos, Vector2 velocity) {
		Transform t = EntityManager.instance.Spawn(type, null, null, null);
		
		EnemyBullet bullet = t.GetComponentInChildren<EnemyBullet>();
		
		Vector3 pos = bullet.transform.position;
		pos.x = startPos.x;
		pos.y = startPos.y;
		bullet.transform.position = pos;
		
		bullet.mSpawnVelocity = velocity;
	}

	protected override void Awake () {
		base.Awake ();
		
		if(entMove != null) {
			entMove.onLandGround += OnLand;
		}
	}
	
	protected override void Start () {
		base.Start();
	}
	
	//
	protected override void SpawnFinish() {
		mCurLifeTime = 0;
		
		state = Entity.State.move;
	}
	
	public override void Release () {
		mSpawnVelocity = Vector2.zero;
		
		base.Release ();
	}
	
	protected override void StateChanged() {
		switch(state) {
		case State.spawning:
			mBounceSpeed = bounceSpeed;
			
			if(entMove != null) {
				entMove.ResetAll();
			}
			
			entMove.velocity = mSpawnVelocity;
			break;
		}
	}
	
	void LateUpdate () {
		switch(state) {
		case State.move:
			mCurLifeTime += Time.deltaTime;
			if(mCurLifeTime >= lifeDelay) {
				Release();
			}
			break;
		}
	}
	
	void OnLand(EntityMovement entMove) {
		if(mBounceSpeed == 0) {
			Release();
		}
		else {
			entMove.Jump(mBounceSpeed, false);
			mBounceSpeed *= bounceSpeedScaleDecay;
		}
	}
}
