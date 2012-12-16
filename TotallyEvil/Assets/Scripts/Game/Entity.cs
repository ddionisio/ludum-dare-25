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
		
		NumState
	}
	
	public delegate void OnSetState(Entity ent, State state);
	public delegate void OnSetBool(Entity ent, bool b);
	
	public event OnSetState setStateCallback = null;
	public event OnSetBool setBlinkCallback = null;
	
	private EntityStat mEntStat = null;
	private EntityMovement mEntMove = null;
	private EntityCollider mEntCollider = null;
	
	private State mState = State.NumState;
	private State mPrevState = State.NumState;
	
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
	
	// Update is called once per frame
	void Update () {
	
	}
}
