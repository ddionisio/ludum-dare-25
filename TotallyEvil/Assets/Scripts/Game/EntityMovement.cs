using UnityEngine;
using System.Collections;

public class EntityMovement : MonoBehaviour {
	public delegate void Callback(EntityMovement entMove);
	
	public const float maxYVelCap = 600.0f;
	
	public bool applyGravity = true;
	public bool applyOrientation = true;
	
	public tk2dBaseSprite orientXSprite = null; //for horizontal orientation
	
	public float radius;
		
	public float maxSpeed;
	
	[System.NonSerialized]
	public Vector2 velocity;
	
	[System.NonSerialized]
	public Vector2 accel;
	
	public Callback onLandGround = null;
	public Callback onDirXChange = null;
	
	private int mJumpCounter = 0;
	private bool mIsGround = false;
	
	private float mCurYVel;
	private float mMaxSpeedSq;
	private Vector2 mDir = Vector2.zero;
	
	private bool mIsMaxSpeed = false;
	
	private float mDX = 0;
	
	public Vector2 dir {
		get { return mDir; }
	}
	
	public float curYVel {
		get { return mCurYVel; }
	}
	
	public bool isMaxSpeed {
		get { return mIsMaxSpeed; }
	}
	
	public int jumpCounter {
		get { return mJumpCounter; }
	}
	
	//set velocity and accel to 0
	public void ResetMotion() {
		accel = velocity = Vector2.zero;
	}
	
	//fake physics
	public void Jump(float vel, bool incJumpCounter=true) {
		mIsGround = false;
		mCurYVel = vel;
		
		if(incJumpCounter)
			mJumpCounter++;
	}
	
	public void ResetJumpCounter() {
		mJumpCounter = 0;
	}
	
	public void ResetCurYVel() {
		mCurYVel = 0.0f;
	}
	
	void Awake() {
		mMaxSpeedSq = maxSpeed*maxSpeed;
		
		if(radius == 0) {
			SphereCollider sc = GetComponentInChildren<SphereCollider>();
			if(sc != null) {
				radius = sc.radius;
			}
		}
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		mMaxSpeedSq = maxSpeed*maxSpeed;
#endif
		World world = World.instance;
						
		float dt = Time.deltaTime;
		
		if(!mIsGround && applyGravity) {
			mCurYVel += world.gravity*dt;
			
			if(mCurYVel > maxYVelCap) {
				mCurYVel = maxYVelCap;
			}
		}
		
		if(accel != Vector2.zero) {
			velocity += accel*Time.deltaTime;
		}
		
		//cap velocity
		if(maxSpeed > 0) {
			if(velocity.y == 0) {
				if(velocity.x > maxSpeed) {
					velocity.x = maxSpeed;
				}
				else if(velocity.x < -maxSpeed) {
					velocity.x = -maxSpeed;
				}
				
				mIsMaxSpeed = false;
			}
			else {
				mIsMaxSpeed = velocity.sqrMagnitude > mMaxSpeedSq;
				if(mIsMaxSpeed) {
					velocity.Normalize();
					velocity *= maxSpeed;
				}
			}
		}
		
		//move
		if(velocity != Vector2.zero || mCurYVel != 0.0f) {
			Vector3 pos = transform.position;
			Vector2 curPos = new Vector2(pos.x, pos.y);
			Vector2 newPos = curPos;
			
			Vector2 dPos = new Vector2(velocity.x*dt, (velocity.y+mCurYVel)*dt);
			
			float dist = dPos.magnitude;
			mDir = dPos/dist;
															
			//adjust to ground
			//for now it's flat
			RaycastHit hit;
			if((!mIsGround || dPos.y > 0.0f) && Physics.SphereCast(curPos, radius, mDir, out hit, dist, Main.layerMaskGround)) {
				newPos.x += dPos.x;
				newPos.y = hit.point.y + radius;
				
				mCurYVel = 0;
				mJumpCounter = 0;
				
				if(!mIsGround) {
					mIsGround = true;
					
					if(onLandGround != null) {
						onLandGround(this);
					}
				}
			}
			else {
				newPos += dPos;
			}
			
			//direction X changed
			float dX = newPos.x - curPos.x;
			if(dX != 0) {
				if(mDX == 0) {
					mDX = dX;
				}
				else {
					float dirSignX = Mathf.Sign(dX);
					if(Mathf.Sign(mDX) != dirSignX) {
						mDX = dX;
						
						//orient sprite horizontally
						if(orientXSprite != null && dirSignX != 0.0f) {
							Vector3 s = orientXSprite.scale;
							s.x = dirSignX*Mathf.Abs(s.x);
						}
						
						//callback
						if(onDirXChange != null) {
							onDirXChange(this);
						}
					}
				}
			}
			
			pos.x = newPos.x;
			pos.y = newPos.y;
			transform.position = pos;
		}
	}
}
