using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {
	public enum State {
		idle,
		move,
		jump,
		fall,
		
		numState
	}
	
	private EntityMovement mEntMove = null;
	
	public EntityMovement entMove {
		get { return mEntMove; }
	}
	
	protected virtual void Awake() {
		mEntMove = GetComponent<EntityMovement>();
	}

	// Use this for initialization
	protected virtual void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
