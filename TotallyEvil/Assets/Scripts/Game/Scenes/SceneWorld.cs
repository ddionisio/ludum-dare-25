using UnityEngine;
using System.Collections;

public class SceneWorld : SceneController {
	[System.Serializable]
	public class LevelData {
		public CameraBound bound;
		public float scale = 1.0f;
		public PlayerStat.Level stat;
	}
	
	public LevelData[] levels;
	
	public float levelChangeDelay = 1.0f;
	
	private int mCurLevel = 0;
	private int mNextLevel = 0;
	
	private float mDefaultGravity;
	
	private float mCurChangeTime = 0;
	private Vector3 mPlayerChangeStartPos;
	private Vector3 mPlayerChangeEndPos;
	private Vector3 mCamChangeStartPos;
	private Vector3 mCamChangeEndPos;
	
	public static SceneWorld instance {
		get {
			return Main.instance != null && Main.instance.sceneController != null ? (SceneWorld)Main.instance.sceneController : null;
		}
	}
	
	public int curLevel {
		get { return mCurLevel; }
		
		set {
			mNextLevel = value;
			
			mCurChangeTime = 0;
			
			mPlayerChangeStartPos = Player.instance.transform.position;
			mPlayerChangeEndPos = levels[mNextLevel].bound.transform.position + levels[mNextLevel].bound.point;
			mPlayerChangeEndPos.z = mPlayerChangeStartPos.z;
			
			mCamChangeStartPos = CameraController.instance.transform.position;
			mCamChangeEndPos = mPlayerChangeEndPos;
			mCamChangeEndPos.z = mCamChangeStartPos.z;
			
			CameraController.instance.attach = null;
			CameraController.instance.bound = null;
			
			BroadcastMessage("OnLevelChangeStart", null, SendMessageOptions.DontRequireReceiver);
			//HUDInterface.instance
		}
	}
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
		
		mDefaultGravity = World.instance.gravity;
		
		//start at the first level
		SetLevelData();
	}
	
	// Update is called once per frame
	void Update () {
		if(mCurLevel != mNextLevel) {
			mCurChangeTime += Time.deltaTime;
			if(mCurChangeTime >= levelChangeDelay) {
				mCurLevel = mNextLevel;
				SetLevelData();
			}
			else {
				float t = Ease.In(mCurChangeTime, levelChangeDelay, 0.0f, 1.0f);
				
				float s = levels[mCurLevel].scale + t*(levels[mNextLevel].scale - levels[mCurLevel].scale);
				
				Player.instance.scale = s;
				Player.instance.transform.position = Vector3.Lerp(mPlayerChangeStartPos, mPlayerChangeEndPos, t);
				CameraController.instance.transform.position = Vector3.Lerp(mCamChangeStartPos, mCamChangeEndPos, t);
				
				CameraController.instance.scale = s;
			}
		}
		
		if(Input.GetKeyDown(KeyCode.F1)) {
			curLevel = curLevel+1;
		}
	}
	
	private void SetLevelData() {
		//look for enemies > cur level, they need to be released
		Enemy[] enemies = GetComponentsInChildren<Enemy>(false);
		foreach(Enemy enemy in enemies) {
			PoolDataController poolDat = enemy.GetComponentInChildren<PoolDataController>();
			if(poolDat != null && poolDat.claimed) {
				continue; //just in case...
			}
			
			EnemyStat stat = enemy.stat != null ? enemy.stat as EnemyStat : null;
			if(stat != null && stat.level > mCurLevel) {
				enemy.Release();
			}
		}
		
		Player.instance.scale = levels[mCurLevel].scale;
		Vector3 newP = levels[mCurLevel].bound.transform.position + levels[mCurLevel].bound.point;
		newP.z = Player.instance.transform.position.z;
		Player.instance.transform.position = newP;
		
		CameraController.instance.scale = levels[mCurLevel].scale;
		CameraController.instance.bound = levels[mCurLevel].bound;
		CameraController.instance.attach = Player.instance.transform;
		
		World.instance.gravity = mDefaultGravity*levels[mCurLevel].scale;
		
		BroadcastMessage("OnLevelChangeEnd", levels[mCurLevel], SendMessageOptions.DontRequireReceiver);
	}
}
