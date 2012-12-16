using UnityEngine;
using System.Collections;

public class EntityRotateVelocity : MonoBehaviour {
	public EntityMovement entityMovement;
	
	public float rotatePerMeter = 1;
	
	private float mRotatePerMeterRad;
	
	void Awake() {
		mRotatePerMeterRad = rotatePerMeter*Mathf.Deg2Rad;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		mRotatePerMeterRad = rotatePerMeter*Mathf.Deg2Rad;
#endif
		
		if(entityMovement.velocity != Vector2.zero) {
			Vector2 vel = entityMovement.velocity;
			if(vel.x != 0) {
				vel.y += entityMovement.curYVel;
				
				float spd = vel.y == 0 ? vel.x : vel.x < 0 ? -vel.magnitude : vel.magnitude;
										
				float rotate = mRotatePerMeterRad*spd*Time.deltaTime;
				
				Vector2 rotDir = Util.Vector2DRot(transform.up, rotate);
				
				Quaternion q = Quaternion.FromToRotation(transform.up, rotDir);
				transform.rotation = q*transform.rotation;
			}
		}
	}
}
