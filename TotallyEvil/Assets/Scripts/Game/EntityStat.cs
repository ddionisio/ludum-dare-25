using UnityEngine;
using System.Collections;

public class EntityStat : MonoBehaviour {
	public delegate void OnHPChange(EntityStat stat);
	
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
				mCurHP = value;
				
				if(mCurHP < 0) {
					mCurHP = 0;
				}
				
				if(hpChangeCallback != null) {
					hpChangeCallback(this);
				}
			}
		}
	}
	
	protected virtual void Awake() {
		mCurHP = maxHP;
	}
}
