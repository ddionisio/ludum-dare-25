using UnityEngine;
using System.Collections;

public class EntityCollider : MonoBehaviour {
	public delegate void OnCollide(EntityCollider collider, RaycastHit hit);
	
	public event OnCollide collideCallback;
	
	private int mLayerMasks = 0;
	private float mRadius;
	
	public float radius {
		get { return mRadius; }
		
		set {
			mRadius = value;
		}
	}
	
	public int layerMasks {
		get { return mLayerMasks; }
		
		set {
			mLayerMasks = value;
		}
	}
	
	public void DoCollision(Vector3 pos, Vector3 dir, float dist) {
		RaycastHit hit;
		if(Physics.SphereCast(pos, mRadius, dir, out hit, dist, mLayerMasks)) {
			if(collideCallback != null) {
				collideCallback(this, hit);
			}
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
