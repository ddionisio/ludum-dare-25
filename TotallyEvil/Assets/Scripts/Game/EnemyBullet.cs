using UnityEngine;
using System.Collections;

public class EnemyBullet : Entity {
	
	public float damage = 1.0f;
	
	public float bounceSpeed; //set to 0 to die when it hits the ground
	public float bounceSpeedScaleDecay = 0.8f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
