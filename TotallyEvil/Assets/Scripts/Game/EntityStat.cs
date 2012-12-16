using UnityEngine;
using System.Collections;

public class EntityStat : MonoBehaviour {
	public delegate void OnHPChange(EntityStat stat, float delta);
	
	public event OnHPChange hpChangeCallback;
	
	[SerializeField] float _damage = 1.0f;
	[SerializeField] float _maxHP = 1.0f;
	
	protected float mCurHP;
	
	public virtual float damage {
		get { return _damage; }
	}
	
	public virtual float maxHP {
		get { return _maxHP; }
	}
	
	public virtual float curHP {
		get { return mCurHP; }
		
		set {
			if(mCurHP != value) {
				float prevHP = mCurHP;
				mCurHP = value;
				
				if(mCurHP < 0) {
					mCurHP = 0;
				}
				
				if(hpChangeCallback != null) {
					hpChangeCallback(this, value - prevHP);
				}
			}
		}
	}
	
	public virtual void Refresh() {
		if(hpChangeCallback != null) {
			hpChangeCallback(this, 0);
		}
	}
	
	public virtual void ResetStats() {
		mCurHP = _maxHP;
	}
	
	protected virtual void Awake() {
		mCurHP = maxHP;
	}
}
