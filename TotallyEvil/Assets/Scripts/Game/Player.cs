using UnityEngine;
using System.Collections;

public class Player : Entity {
	[System.Serializable]
	public class GuardData {
		public GameObject guard;
		public float delay;
	}
	
	public float attackHitJumpSpd = 120;
	
	public GameObject thornsIdle;
	public GameObject thornsAttack;
			
	public GuardData[] guards;
	
	private static Player mInstance;
	
	private bool mGuardActive=false;
	private int mCurNumGuards;
	private float mCurGuardRechargeTime;
	
	private float mScale = 1.0f;
	
	private tk2dBaseSprite[] mSprites;
	private Vector2[] mSpriteDefaultScales;
	private tk2dStaticSpriteBatcher[] mSpriteBatches;
	
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
				
				for(int i = 0; i < mSprites.Length; i++) {
					tk2dBaseSprite spr = mSprites[i];
					Vector3 s = spr.scale;
					s.x = Mathf.Sign(s.x)*mSpriteDefaultScales[i].x*mScale;
					s.y = Mathf.Sign(s.y)*mSpriteDefaultScales[i].y*mScale;
					spr.scale = s;
				}
				
				foreach(tk2dStaticSpriteBatcher sprB in mSpriteBatches) {
					Vector3 s = sprB.scale;
					s.x = Mathf.Sign(s.x)*mScale;
					s.y = Mathf.Sign(s.y)*mScale;
					sprB.scale = s;
				}
				
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
		}
	}
	
	public void GuardReset() {
		foreach(GuardData g in guards) {
			g.guard.SetActiveRecursively(true);
		}
		
		mCurNumGuards = guards.Length;
	}
	
	void OnDestroy() {
		mInstance = null;
	}
	
	protected override void Awake () {
		base.Awake();
		
		mSprites = GetComponentsInChildren<tk2dBaseSprite>(true);
		mSpriteDefaultScales = new Vector2[mSprites.Length];
		for(int i = 0; i < mSprites.Length; i++) {
			mSpriteDefaultScales[i] = mSprites[i].scale;
		}
		
		mSpriteBatches = GetComponentsInChildren<tk2dStaticSpriteBatcher>(true);
		
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
	
	protected override void StateChanged() {
		switch(prevState) {
		case State.attack:
			thornsIdle.SetActiveRecursively(true);
			thornsAttack.SetActiveRecursively(false);
			break;
		}
		
		switch(state) {
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
				
				guards[mCurNumGuards].guard.SetActiveRecursively(true);
				
				mCurNumGuards++;
			}
		}
	}
	
	void OnCollide(EntityCollider collider, RaycastHit hit) {
		if(hit.transform.gameObject.layer == Main.layerEnemy) {
			Enemy enemy = hit.transform.GetComponentInChildren<Enemy>();
			if(enemy.state != State.die) {
				EnemyStat enemyStat = enemy.stat != null ? enemy.stat as EnemyStat : null;
												
				if(state == State.attack
					&& !enemy.isBlinking 
					&& enemy.state != State.spawning
					&& enemyStat != null) {
					enemyStat.curHP -= stat.damage;
				}
				
				if(enemyStat != null && enemyStat.level >= SceneWorld.instance.curLevel) {
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
		else if(hit.transform.gameObject.layer == Main.layerEnemyProjectile) {
		}
	}
	
	void OnHPChange(EntityStat stat, float delta) {
		PlayerStat pstat = (PlayerStat)stat;
		
		if(pstat.curHP == 0) {
			//game over?
			//need to level down?
		}
		else {
		}
	}
	
	void OnLevelPointsChange(PlayerStat stat) {
	}
	
	void OnLevelChangeStart() {
		mController.enabled = false;
		entMove.ResetAll();
		entMove.enabled = false;
	}
	
	void OnLevelChangeEnd(SceneWorld.LevelData level) {
		mController.enabled = true;
		entMove.enabled = true;
	}
	
	void OnUIModalActive() {
		mController.enabled = false;
	}
	
	void OnUIModalInactive() {
		mController.enabled = true;
	}
}
