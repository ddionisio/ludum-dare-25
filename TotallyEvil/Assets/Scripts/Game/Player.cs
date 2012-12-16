using UnityEngine;
using System.Collections;

public class Player : Entity {
	public delegate void OnGuardUpdate(int cur, int max);
	
	[System.Serializable]
	public class GuardData {
		public GameObject guard;
		public float delay;
	}
	
	public event OnGuardUpdate guardUpdateCallback;
	
	public float hurtDelay = 1.0f;
	public float attackHitJumpSpd = 120;
	
	public GameObject thornsIdle;
	public GameObject thornsAttack;
			
	public GuardData[] guards;
	
	public float deathDelay;
	
	public float gibletMinScale = 1.0f;
	public float gibletMaxScale = 1.0f;
	public int numGiblets = 1;
	
	private static Player mInstance;
	
	private bool mGuardActive=false;
	private int mCurNumGuards;
	private float mCurGuardRechargeTime;
	
	private float mScale = 1.0f;
	
	private EntityRotateVelocity mRotVel;
	
	private PlayerController mController;
	
	private float mDefaultRadius = 0;
	private float mDefaultMaxSpeed = 0;
	private float mDefaultRotateSpd = 0;
	
	public static Player instance {
		get { return mInstance; }
	}
	
	public float scale {
		get { return mScale; }
		
		set {
			if(mScale != value) {
				mScale = value;
				
				Vector3 s = transform.localScale;
				s.x = s.y = mScale;
				transform.localScale = s;
				
				if(entCollider != null) {
					entCollider.radius = mDefaultRadius*mScale;
				}
				
				entMove.radius = mDefaultRadius*mScale;
				entMove.maxSpeed = mDefaultMaxSpeed*mScale;
				entMove.RefreshMaxSpdSqr();
				mController.RefreshMaxSpeedAttackSqr();
				
				mRotVel.rotatePerMeter = mDefaultRotateSpd*(1.0f/mScale);
			}
		}
	}
	
	public bool guardActive {
		get { return mGuardActive; }
		
		set {
			if(mGuardActive != value) {
				mGuardActive = value;
				
				for(int i = 0; i < mCurNumGuards; i++) {
					guards[i].guard.SetActiveRecursively(mGuardActive);
				}
			}
		}
	}
	
	public int guardCurNum {
		get { return mCurNumGuards; }
	}
	
	public float curGuardRechargeTime {
		get { return mCurGuardRechargeTime; }
	}
	
	public void GuardDec() {
		if(mCurNumGuards > 0) {
			mCurNumGuards--;
			
			guards[mCurNumGuards].guard.SetActiveRecursively(false);
			
			mCurGuardRechargeTime = 0;
			
			Blink(hurtDelay);
			
			if(guardUpdateCallback != null) {
				guardUpdateCallback(mCurNumGuards, guards.Length);
			}
		}
	}
	
	public void GuardReset() {
		foreach(GuardData g in guards) {
			g.guard.SetActiveRecursively(true);
		}
		
		mCurNumGuards = guards.Length;
		
		if(guardUpdateCallback != null) {
			guardUpdateCallback(mCurNumGuards, guards.Length);
		}
	}
	
	void OnDestroy() {
		mInstance = null;
	}
	
	protected override void Awake () {
		base.Awake();
		
		mRotVel = GetComponentInChildren<EntityRotateVelocity>();
		
		mDefaultRotateSpd = mRotVel.rotatePerMeter;
		
		thornsAttack.SetActiveRecursively(false);
		
		mCurNumGuards = guards.Length;
		
		foreach(GuardData g in guards) {
			g.guard.SetActiveRecursively(false);
		}
		
		mController = GetComponent<PlayerController>();
		
		mDefaultRadius = entMove.radius;
		mDefaultMaxSpeed = entMove.maxSpeed;
		
		mInstance = this;
		
		if(stat != null) {
			stat.hpChangeCallback += OnHPChange;
			
			((PlayerStat)stat).levelPointsChangeCallback += OnLevelPointsChange;
		}
	}
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
		
		if(entCollider != null) {
			entCollider.layerMasks = Main.layerMaskEnemy | Main.layerMaskEnemyProjectile | Main.layerMaskStructure;
			entCollider.collideCallback += OnCollide;
		}
	}
	
	protected override void SetBlink(bool blink) {
		if(!blink) {
			if(state == Entity.State.die) {
				gameObject.SetActiveRecursively(false);
				
				UIModalManager.instance.ModalOpen(UIModalManager.Modal.GameOver);
			}
		}
	}
	
	protected override void StateChanged() {
		switch(prevState) {
		case State.attack:
			thornsIdle.SetActiveRecursively(true);
			thornsAttack.SetActiveRecursively(false);
			break;
		}
		
		switch(state) {
		case State.die:
			mController.enabled = false;
			
			entMove.ResetAll();
			entMove.enabled = false;
			
			entCollider.enabled = false;
			
			Blink(deathDelay);
			
			Giblet.Generate(transform.position, numGiblets, Random.Range(gibletMinScale, gibletMaxScale)*mScale);
			break;
			
		case State.idle:
			break;
			
		case State.attack:
			thornsIdle.SetActiveRecursively(false);
			thornsAttack.SetActiveRecursively(true);
			break;
		}
	}
	
	void LateUpdate () {
		if(mCurNumGuards < guards.Length) {
			mCurGuardRechargeTime += Time.deltaTime;
			if(mCurGuardRechargeTime >= guards[mCurNumGuards].delay) {
				mCurGuardRechargeTime -= guards[mCurNumGuards].delay;
				
				if(mGuardActive) {
					guards[mCurNumGuards].guard.SetActiveRecursively(true);
				}
				
				mCurNumGuards++;
				
				if(guardUpdateCallback != null) {
					guardUpdateCallback(mCurNumGuards, guards.Length);
				}
			}
		}
	}
	
	void OnCollide(EntityCollider collider, RaycastHit hit) {
		float hurtAmt = 0;
		
		if(hit.transform.gameObject.layer == Main.layerEnemy) {
			Enemy enemy = hit.transform.GetComponentInChildren<Enemy>();
			if(enemy.state != State.die) {
				EnemyStat enemyStat = enemy.stat != null ? enemy.stat as EnemyStat : null;
				if(enemyStat != null) {
					if(state == State.attack) {
						if(!isBlinking
							&& !enemy.isBlinking 
							&& enemy.state != State.spawning) {
							enemyStat.curHP -= stat.damage;
						}
					}
					else if(enemyStat.damage > 0) {
						hurtAmt = enemyStat.damage;
					}
					
					if(enemyStat.level >= SceneWorld.instance.curLevel) {
						Vector3 enemyPos = hit.transform.position;
						Vector3 pos = transform.position;
														
						if(pos.y > enemyPos.y) {
							entMove.Jump(attackHitJumpSpd*mScale, false);
						}
						else {
							entMove.ResetCurYVel();
							entMove.Jump(-attackHitJumpSpd*mScale, false);
						}
					}
				}
			}
		}
		else if(!isBlinking && hit.transform.gameObject.layer == Main.layerEnemyProjectile) {
			EnemyBullet bullet = hit.transform.GetComponentInChildren<EnemyBullet>();
			if(bullet != null) {
				hurtAmt = bullet.damage;
			}
			else {
				EnemyProjectile proj = hit.transform.GetComponentInChildren<EnemyProjectile>();
				if(proj == null) { //try parent
					proj = hit.transform.parent.GetComponentInChildren<EnemyProjectile>();
				}
				
				hurtAmt = proj.damage;
			}
		}
		
		if(hurtAmt > 0) {
			if(guardActive && mCurNumGuards > 0) {
				GuardDec();
			}
			else {
				//decrease from points first
				PlayerStat ps = (PlayerStat)stat;
				if(ps.curLevelPts > 0) {
					ps.curLevelPts -= hurtAmt;
				}
				else {
					stat.curHP -= hurtAmt;
				}
			}
		}
	}
	
	void OnHPChange(EntityStat stat, float delta) {
		PlayerStat pstat = (PlayerStat)stat;
		
		if(pstat.curHP == 0) {
			//decrease level
			SceneWorld sw = SceneWorld.instance;
			if(sw.curLevel > 0) {
				sw.curLevel--;
			}
			else {
				state = Entity.State.die;
			}
		}
		else if(delta < 0) {
			Blink(hurtDelay);
		}
	}
	
	void OnLevelPointsChange(PlayerStat stat, float delta) {
		if(stat.curLevelPts >= stat.maxLevelPts) {
			//Debug.Log("level up!");
			SceneWorld sw = SceneWorld.instance;
			sw.curLevel++;
		}
		else if(delta < 0) {
			Blink(hurtDelay);
		}
	}
	
	void OnLevelChangeStart() {
		mController.enabled = false;
		
		entMove.ResetAll();
		entMove.enabled = false;
		
		entCollider.enabled = false;
	}
	
	void OnLevelChangeEnd(SceneWorld.LevelData level) {
		mController.enabled = true;
		entMove.enabled = true;
		entCollider.enabled = true;
		
		entMove.ResetAll();
	}
	
	void OnUIModalActive() {
		mController.enabled = false;
	}
	
	void OnUIModalInactive() {
		mController.enabled = true;
	}
}
