using UnityEngine;
using System.Collections;

//respawn after entity has been claimed by entity manager
public class EntitySpawner : MonoBehaviour {
	
	public string type;
	
	public float delayRespawn = 1.0f;
	
	public bool activeOnStart = false;
	
	public int maxActive = 1;
	
	enum State {
		Inactive,
		Spawn,
		SpawnWait
	}
	
	private State mCurState = State.Inactive;
	private float mCurTime = 0;
	private int mCurNumActive = 0;
	private bool mSceneActive = true;
	
	public void Activate(bool yes) {
		if(yes) {
			ChangeState(State.SpawnWait);
		}
		else {
			ChangeState(State.Inactive);
		}
	}
	
	void Start() {
		if(activeOnStart && mSceneActive) {
			Activate(true);
		}
	}
	
	void OnSceneActivate(bool yes) {
		mSceneActive = yes;
		Activate(yes);
	}
	
	void OnDestroy() {
	}
	
	void ChangeState(State state) {
		mCurState = state;
		mCurTime = 0.0f;
		
		switch(state) {
		case State.Inactive:
			break;
		case State.Spawn:
			break;
		case State.SpawnWait:
			break;
		}
	}
	
	void Update() {
		switch(mCurState) {
		case State.Inactive:
			break;
		case State.Spawn:
			mCurNumActive++;
			
			Transform t = EntityManager.instance.Spawn(type, null, null, null);
			
			Vector3 pos = transform.position;
			
			t.position = new Vector3(pos.x, pos.y, t.position.z);
			
			Entity ent = t.GetComponentInChildren<Entity>();
			if(ent != null) {
				ent.releaseCallback += OnEntityRelease;
			}
						
			ChangeState(State.SpawnWait);
			break;
		case State.SpawnWait:
			if(mCurNumActive < maxActive) {
				mCurTime += Time.deltaTime;
				if(mCurTime >= delayRespawn) {
					ChangeState(State.Spawn);
				}
			}
			break;
		}
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.cyan;
		Gizmos.DrawIcon(transform.position, "spawner");
	}
	
	void OnEntityRelease(Entity ent) {
		ent.releaseCallback -= OnEntityRelease;
		mCurNumActive--;
	}
}
