using UnityEngine;
using System.Collections;

public class PlayerStat : EntityStat {
	public delegate void OnLevelPointsChange(PlayerStat stat, float delta);
	
	public delegate void OnLevelChange(PlayerStat stat, int level);
	
	[System.Serializable]
	public class Level {
		public float maxHP;
		public float damage;
		public float nextLevelPoints;
	}
	
	public event OnLevelPointsChange levelPointsChangeCallback;
	public event OnLevelChange levelChangeCallback;
	
	private Level mCurLevel;
	private float mCurLevelPts;
	
	public float curLevelPts {
		get { return mCurLevelPts; }
		set { 
			if(mCurLevelPts != value) {
				float prevPts = mCurLevelPts;
				mCurLevelPts = value;
				if(mCurLevel != null && mCurLevelPts > mCurLevel.nextLevelPoints) {
					mCurLevelPts = mCurLevel.nextLevelPoints;
				}
				
				if(levelPointsChangeCallback != null) {
					levelPointsChangeCallback(this, mCurLevelPts - prevPts);
				}
			}
		}
	}
	
	public float maxLevelPts {
		get { return mCurLevel == null ? 0.0f : mCurLevel.nextLevelPoints; }
	}
	
	public override float damage {
		get { return mCurLevel == null ? 0.0f : mCurLevel.damage; }
	}
	
	public override float maxHP {
		get { return mCurLevel == null ? 1.0f : mCurLevel.maxHP; }
	}
	
	public override void Refresh() {
		base.Refresh();
		
		if(levelPointsChangeCallback != null) {
			levelPointsChangeCallback(this, 0);
		}
		
		if(levelChangeCallback != null) {
			levelChangeCallback(this, SceneWorld.instance.curLevel);
		}
	}
			
	protected override void Awake() {
		base.Awake();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnLevelChangeEnd(SceneWorld.LevelData level) {
		mCurLevel = level.stat;
		mCurHP = maxHP;
		mCurLevelPts = 0;
		
		Refresh();
	}
}
