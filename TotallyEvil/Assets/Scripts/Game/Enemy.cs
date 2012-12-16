using UnityEngine;
using System.Collections;

public class AIState : Sequencer.StateInstance {
	public Vector2 curPlanetDir;
	public Vector2 velocityHolder;
	public float d;
}

public class Enemy : Entity {
	public enum Melee {
		Off,
		PointToPlayer,
		PointToDir
	}
	
	public string[] afterSpawnAIState;
	
	public float dieDelay = 0.25f;
	public float hurtDelay = 0.5f;
	
	public float gibletMinScale = 1.0f;
	public float gibletMaxScale = 1.0f;
	public int numGiblets = 1;
	
	private AIState mAIStateInstance = null;
	private string mAICurState;
	private string mLastAIState;
	
	private Melee mMeleeMode = Melee.Off;
	private EnemyProjectile mMelee;
	
	public Melee meleeMode {
		get { return mMeleeMode; }
		set {
			mMeleeMode = value;
			if(mMelee != null) {
				switch(mMeleeMode) {
				case Melee.Off:
					mMelee.gameObject.SetActiveRecursively(false);
					break;
					
				case Melee.PointToDir:
				case Melee.PointToPlayer:
					mMelee.gameObject.SetActiveRecursively(true);
					break;
				}
			}
		}
	}
	
	//AI
	public void AIStop() {
		if(mAIStateInstance != null) {
			mAIStateInstance.terminate = true;
			mAIStateInstance = null;
			
			mAICurState = null;
		}
	}
	
	public void AISetPause(bool pause) {
		if(mAIStateInstance != null) {
			mAIStateInstance.pause = pause;
		}
	}
	
	public void AISetState(string state) {
		AIStop();
		
		mAICurState = state;
		
		mAIStateInstance = new AIState();
		AIManager.instance.states.Start(this, mAIStateInstance, state);
	}
	
	public void AIRestart() {
		if(!string.IsNullOrEmpty(mAICurState)) {
			AISetState(mAICurState);
		}
	}
	
	void AIChangeState(string state) {
		AISetState(state);
	}
	
	
	public string aiCurState {
		get {
			return mAICurState;
		}
	}
	
	public bool aiActive {
		get {
			return mAIStateInstance != null;
		}
	}
	
	protected override void SpawnFinish() {
		if(afterSpawnAIState.Length > 0) {
			AISetState(afterSpawnAIState[Random.Range(0, afterSpawnAIState.Length)]);
		}
	}
	
	protected override void SetBlink(bool blink) {
		if(state == Entity.State.die && !blink) {
			//spawn giblets
			
			//spawn essence
			EnemyStat es = (EnemyStat)stat;
			Essence.Generate(transform.position, es.essencePoints, es.essenceScale);
			
			Release();
		}
	}
	
	protected override void StateChanged() {
		switch(state) {
		case State.spawning:
			meleeMode = Melee.Off;
			break;
			
		case State.die:
			Blink(dieDelay);
			break;
		}
	}
	
	public override void Release() {
		AIStop();
		
		base.Release();
	}
			
	protected override void Awake () {
		base.Awake ();
		
		if(stat != null) {
			stat.hpChangeCallback += OnHPChange;
		}
		
		//projectiles within enemies are considered melees
		//default off
		mMelee = GetComponentInChildren<EnemyProjectile>();
		meleeMode = Melee.Off;
	}
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	void OnDestroy() {
		mAIStateInstance = null;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		switch(state) {
		case State.die:
			break;
		case State.spawning:
			break;
			
		default:
			if(mMelee != null) {
				switch(mMeleeMode) {	
				case Melee.PointToDir:
					mMelee.transform.up = entMove.dir;
					break;
					
				case Melee.PointToPlayer:
					Player p = Player.instance;
					Vector3 dir = (p.transform.position - transform.position).normalized;
					mMelee.transform.up = dir;
					break;
				}
			}
			break;
		}
	}
	
	void OnHPChange(EntityStat stat, float delta) {
		if(stat.curHP == 0) {
			state = State.die;
		}
		else if(delta < 0) {
			Blink(hurtDelay);
		}
	}
}
