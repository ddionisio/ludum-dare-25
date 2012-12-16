using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {
	public enum State {
		idle,
		move,
		jump,
		fall,
		attack,
		guard,
		
		spawning, //once finish, calls OnEntitySpawnFinish to listeners
		
		hurt,
		die,
		
		NumState
	}
	
	public delegate void OnSetState(Entity ent, State state);
	public delegate void OnSetBool(Entity ent, bool b);
	public delegate void OnFinish(Entity ent);
	
	public float spawnDelay = 0.1f;
	
	public event OnSetState setStateCallback = null;
	public event OnSetBool setBlinkCallback = null;
	public event OnFinish spawnFinishCallback = null;
	
	private EntityStat mEntStat = null;
	private EntityMovement mEntMove = null;
	private EntityCollider mEntCollider = null;
	
	private State mState = State.NumState;
	private State mPrevState = State.NumState;
	
	private float mEntCurTime = 0;
	private float mBlinkCurTime = 0;
	private float mBlinkDelay = 0;
	
	public State state {
		get { return mState; }
		
		set {
			if(mState != value) {
				mPrevState = mState;
				mState = value;
				
				if(setStateCallback != null) {
					setStateCallback(this, value);
				}
				
				StateChanged();
			}
		}
	}
	
	public State prevState {
		get { return mPrevState; }
	}
	
	public EntityStat stat {
		get { return mEntStat; }
	}
	
	public EntityMovement entMove {
		get { return mEntMove; }
	}
	
	public EntityCollider entCollider {
		get { return mEntCollider; }
	}
	
	public bool isBlinking {
		get { return mBlinkDelay > 0 && mBlinkCurTime < mBlinkDelay; }
	}
	
	public void Blink(float delay) {
		mBlinkDelay = delay;
		mBlinkCurTime = 0;
		
		bool doBlink = delay > 0;
		
		if(setBlinkCallback != null) {
			setBlinkCallback(this, doBlink);
		}
		
		SetBlink(doBlink);
	}
	
	/// <summary>
	/// Spawn this entity, resets stats, set action to spawning, then later calls OnEntitySpawnFinish.
	/// NOTE: calls after an update to ensure Awake and Start is called.
	/// </summary>
	public void Spawn() {
		mState = mPrevState = State.NumState; //avoid invalid updates
		//ensure start is called before spawning if we are freshly allocated from entity manager
		StartCoroutine(DoSpawn());
	}
	
	public virtual void Release() {
		StopAllCoroutines();
		EntityManager.instance.Release(transform);
	}
	
	protected virtual void Awake() {
		mEntMove = GetComponent<EntityMovement>();
		mEntStat = GetComponent<EntityStat>();
		mEntCollider = GetComponent<EntityCollider>();
	}

	// Use this for initialization
	protected virtual void Start () {
		BroadcastMessage("EntityStart", this, SendMessageOptions.DontRequireReceiver);
	}
	
	protected virtual void StateChanged() {
	}
	
	protected virtual void SpawnFinish() {
	}
	
	protected virtual void SetBlink(bool blink) {
	}
	
	// Update is called once per frame
	private void Update () {
		switch(mState) {
		case State.spawning:
			mEntCurTime += Time.deltaTime;
			if(mEntCurTime >= spawnDelay) {
				mState = State.NumState; //need to be set by something
				
				if(spawnFinishCallback != null) {
					spawnFinishCallback(this);
				}
				
				SpawnFinish();
			}
			break;
		}
		
		if(mBlinkDelay > 0) {
			mBlinkCurTime += Time.deltaTime;
			if(mBlinkCurTime >= mBlinkDelay) {
				mBlinkDelay = mBlinkCurTime = 0;
				
				if(setBlinkCallback != null) {
					setBlinkCallback(this, false);
				}
				
				SetBlink(false);
			}
		}
	}
	
	//////////internal
		
	IEnumerator DoSpawn() {
		yield return new WaitForFixedUpdate();
		
		if(mEntMove != null) {
			mEntMove.ResetAll();
		}
		
		if(stat != null) {
			stat.ResetStats();
		}
		
		mEntCurTime = 0;
		
		state = State.spawning;
		
		yield break;
	}
}
