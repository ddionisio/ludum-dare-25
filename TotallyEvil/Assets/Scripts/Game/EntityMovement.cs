using UnityEngine;
using System.Collections;

public class EntityMovement : MonoBehaviour {
	public delegate void Callback(EntityMovement entMove);
	
	public const float maxYVelCap = 7500.0f;
	
	public bool applyGravity = true;
	public bool applyOrientation = true;
	public bool capToWorld = true;
	public bool capToCameraBounds = true;
	public bool wrapOnBounds = false;
	
	public tk2dBaseSprite orientXSprite = null; //for horizontal orientation
	
	public float radius;
		
	public float maxSpeed;
	
	[System.NonSerialized]
	public Vector2 velocity;
	
	[System.NonSerialized]
	public Vector2 accel;
	
	public event Callback onLandGround = null;
	public event Callback onDirXChange = null;
	
	private int mJumpCounter = 0;
	private bool mIsGround = false;
	
	private float mCurYVel;
	private float mMaxSpeedSqr;
	private Vector2 mDir = Vector2.zero;
	
	private float mCurSpeedSqr = 0;
	
	private float mDX = 0;
	
	private EntityCollider mEntityCollider;
	
	public Vector2 dir {
		get { return mDir; }
	}
	
	public float curYVel {
		get { return mCurYVel; }
	}
	
	public float curSpeedSqr {
		get { return mCurSpeedSqr; }
	}
	
	public int jumpCounter {
		get { return mJumpCounter; }
	}
	
	public bool isGround {
		get { return mIsGround; }
	}
	
	public void ResetAll() {
		ResetMotion();
		ResetJumpCounter();
		ResetCurYVel();
		mIsGround = false;
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
	
	public void RefreshMaxSpdSqr() {
		mMaxSpeedSqr = maxSpeed*maxSpeed;
	}
	
	void Awake() {
		RefreshMaxSpdSqr();
		
		if(radius == 0) {
			SphereCollider sc = GetComponentInChildren<SphereCollider>();
			if(sc != null) {
				radius = sc.radius;
			}
		}
		
		mEntityCollider = GetComponent<EntityCollider>();
		if(mEntityCollider != null) {
			mEntityCollider.radius = radius;
		}
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		RefreshMaxSpdSqr();
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
		
		mCurSpeedSqr = velocity.sqrMagnitude;
		
		//cap velocity
		if(maxSpeed > 0 && mCurSpeedSqr > mMaxSpeedSqr) {
			if(velocity.y == 0) {
				if(velocity.x > 0) {
					velocity.x = maxSpeed;
					mCurSpeedSqr = mMaxSpeedSqr;
				}
				else if(velocity.x < 0) {
					velocity.x = -maxSpeed;
					mCurSpeedSqr = mMaxSpeedSqr;
				}
			}
			else {
				velocity.Normalize();
				velocity *= maxSpeed;
				mCurSpeedSqr = mMaxSpeedSqr;
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
			float newY = newPos.y + dPos.y;
			float newCapY = capToWorld ? world.CapGround(newY, radius) : newY;
			
			if(newY != newCapY) {
				newPos.x += dPos.x;
				newPos.y = newCapY;
				
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
				newPos.x = newPos.x + dPos.x;
				newPos.y = newY;
			}
								
			//direction X changed
			float dX = newPos.x - curPos.x;
			if(dX != 0) {
				if(mDX == 0) {
					mDX = dX;
				}
				
				float dirSignX = Mathf.Sign(dX);
				if(Mathf.Sign(mDX) != dirSignX) {
					mDX = dX;
					
					//orient sprite horizontally
					if(orientXSprite != null && dirSignX != 0.0f) {
						Vector3 s = orientXSprite.scale;
						s.x = -dirSignX*Mathf.Abs(s.x);
						orientXSprite.scale = s;
					}
					
					//callback
					if(onDirXChange != null) {
						onDirXChange(this);
					}
				}
			}
			
			pos.x = newPos.x;
			pos.y = newPos.y;
			
			if(capToCameraBounds) {
				CameraBound camBound = CameraController.instance.bound;
				if(camBound != null) {
					Vector3 capPos = camBound.Cap(pos, radius, radius, wrapOnBounds);
					if(capPos != pos && !wrapOnBounds) {
						accel = velocity = Vector2.zero;
					}
					
					transform.position = capPos;
				}
				else {
					transform.position = pos;
				}
			}
			else {
				transform.position = pos;
			}
		}
	}
}
