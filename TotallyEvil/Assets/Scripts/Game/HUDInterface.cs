using UnityEngine;
using System.Collections;

public class HUDInterface : MonoBehaviour {
	public string levelTextFormat = "LVL {0}";
	
	public UILabel levelLabel;
	
	public UISprite[] guards;
	
	public UISlider hpBar;
	
	public UISlider lvlBar;
	
	private static HUDInterface mInstance;
	
	public static HUDInterface instance {
		get { return mInstance; }
	}
	
	void OnDestroy() {
		mInstance = null;
	}
	
	void Awake() {
		mInstance = this;
	}
	
	// Use this for initialization
	void Start () {
		Player player = Player.instance;
		
		player.stat.hpChangeCallback += OnHPChange;
		
		((PlayerStat)player.stat).levelPointsChangeCallback += OnLevelPointsChange;
		((PlayerStat)player.stat).levelChangeCallback += OnLevelChange;
		
		player.guardUpdateCallback += OnGuardUpdate;
		
		hpBar.sliderValue = 1.0f;
		lvlBar.sliderValue = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnHPChange(EntityStat stat, float delta) {
		hpBar.sliderValue = stat.curHP/stat.maxHP;
	}
	
	void OnLevelPointsChange(PlayerStat stat, float delta) {
		lvlBar.sliderValue = stat.curLevelPts/stat.maxLevelPts;
	}
	
	void OnGuardUpdate(int cur, int max) {
		for(int i = 0; i < cur; i++) {
			if(!guards[i].gameObject.active) {
				guards[i].gameObject.active = true;
			}
		}
		
		for(int i = cur; i < max; i++) {
			if(guards[i].gameObject.active) {
				guards[i].gameObject.active = false;
			}
		}
	}
	
	void OnLevelChange(PlayerStat stat, int level) {
		levelLabel.text = string.Format(levelTextFormat, level+1);
	}
}
