using UnityEngine;
using System.Collections;

//Amber is the color of your energy
public class Essence : Entity {
	public float delay;
	
	private float mPoints;
	private float mScale;
	
	private float mCurTime;
	private Vector3 mStartPos;
	
	public static void Generate(Vector3 startPos, float points, float scale) {
		Transform t = EntityManager.instance.Spawn("essence", null, null, null);
		Essence essence = t.GetComponentInChildren<Essence>();
		essence.mPoints = points;
		essence.mScale = scale;
		
		essence.mStartPos = startPos;
		essence.mStartPos.z = t.position.z;
		t.position = essence.mStartPos;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		switch(state) {
		case State.move:
			Player player = Player.instance;
			Vector3 dest = player.transform.position;
			
			mCurTime += Time.deltaTime;
			if(mCurTime >= delay) {
				PlayerStat ps = player.stat as PlayerStat;
				ps.curLevelPts += mPoints;
				
				Release();
			}
			else {
				float t = Ease.In(mCurTime, delay, 0.0f, 1.0f);
				
				Vector3 newPos = Vector3.Lerp(mStartPos, dest, t);
				newPos.z = mStartPos.z;
				transform.position = newPos;
			}
			break;
		}
	}
	
	protected override void SpawnFinish() {
		mCurTime = 0;
		state = Entity.State.move;
	}
	
	protected override void StateChanged() {
		switch(state) {
		case State.spawning:
			tk2dBaseSprite spr = GetComponentInChildren<tk2dBaseSprite>();
			Vector3 s = spr.scale;
			s.x = s.y = mScale;
			spr.scale = s;
			break;
		}
	}
}
