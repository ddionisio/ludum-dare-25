using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public float moveAccel;
	public float jumpSpeed;
	public int maxJump = 2;
	
	public float maxSpeedScaleAttack = 1.0f;
	
	
	private Player mPlayer;
	private float mMoveX;
	private bool mReleased=true;
	
	private float mMaxSpeedAttackSqr;
	
	public void RefreshMaxSpeedAttackSqr() {
		mMaxSpeedAttackSqr = mPlayer.entMove.maxSpeed*mPlayer.entMove.maxSpeed*maxSpeedScaleAttack;
	}
	
	void Awake () {
		mPlayer = GetComponent<Player>();
		
		RefreshMaxSpeedAttackSqr();
	}
	
	// Use this for initialization
	void Start () {
		mPlayer.entMove.onDirXChange += OnDirXChange;
		mPlayer.entMove.onLandGround += OnLandGround;
	}
	
	void OnDisable() {
		mReleased = true;
		
		if(mPlayer != null && mPlayer.entMove != null)
			mPlayer.entMove.ResetMotion();
	}
	
	// Update is called once per frame
	void Update () {
		EntityMovement entMove = mPlayer.entMove;
		
		mMoveX = Input.GetAxis("Horizontal");
		
		if(mMoveX != 0.0f) {
			mMoveX = Mathf.Sign(mMoveX);
			
			if(entMove.accel.x == 0) {
				entMove.accel.x = mMoveX*moveAccel*mPlayer.scale;
			}
			else if(Mathf.Sign(entMove.accel.x) != mMoveX) {
				entMove.accel.x = 2.0f*mMoveX*moveAccel*mPlayer.scale; //change dir asap
			}
			
			mReleased = false;
		}
		else if(!mReleased) {
			if(entMove.jumpCounter == 0 && Mathf.Sign(entMove.accel.x) == Mathf.Sign(entMove.velocity.x)) {
				//Debug.Log("move accel: "+entMove.accel.x);
				entMove.accel.x *= -2; //"brakes"
			}
			
			mReleased = true;
		}
		
		if(entMove.jumpCounter < maxJump) {
			if(Input.GetButtonDown("Jump")) {
				entMove.Jump(jumpSpeed*mPlayer.scale);
			}
		}
		
		mPlayer.guardActive = Input.GetButton("Guard");
		
		if(!mPlayer.guardActive && (!entMove.isGround || entMove.curSpeedSqr >= mMaxSpeedAttackSqr)) {
			mPlayer.state = Entity.State.attack; 
		}
		else {
			mPlayer.state = mPlayer.guardActive ? Entity.State.guard : Entity.State.idle;
		}
	}
	
	void OnDirXChange(EntityMovement entMove) {
		if(mReleased) {
			entMove.accel.x = 0;
			entMove.velocity.x = 0;
		}
		else {
			mPlayer.entMove.accel.x = mMoveX*moveAccel*mPlayer.scale;
		}
	}
	
	void OnLandGround(EntityMovement entMove) {
		if(mReleased && Mathf.Sign(entMove.accel.x) == Mathf.Sign(entMove.velocity.x)) {
			//Debug.Log("land accel: "+entMove.accel.x);
			entMove.accel.x *= -2; //"brakes"
		}
	}
}
