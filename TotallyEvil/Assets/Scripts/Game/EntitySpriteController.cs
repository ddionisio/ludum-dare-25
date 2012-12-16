using UnityEngine;
using System.Collections;

public class EntitySpriteController : MonoBehaviour {
	public float blinkDelay = 0.05f;
	
	protected tk2dBaseSprite mSprite;
	protected tk2dAnimatedSprite mSpriteAnim;
	protected tk2dStaticSpriteBatcher mSpriteBatcher; //
	
	private int[] mStateAnimIds;
	
	private float mPrevAlpha = 0;
	private bool mIsBlink = false;
	private float mBlinkTime = 0;
	
	protected bool HasAnim(Entity.State s) {
		return mStateAnimIds[(int)s] != -1;
	}
	
	protected void PlayAnim(Entity.State s) {
		if(mSpriteAnim != null) {
			if(mStateAnimIds == null) {
				mStateAnimIds = new int[(int)Entity.State.NumState];
				for(int i = 0; i < mStateAnimIds.Length; i++) {
					mStateAnimIds[i] = mSpriteAnim.GetClipIdByName(((Entity.State)i).ToString());
				}
			}
			
			int id = mStateAnimIds[(int)s];
			if(id != -1 && mSpriteAnim.clipId != id) {
				mSpriteAnim.Play(id);
			}
		}
	}
	
	void EntityStart(Entity ent) {
		ent.setStateCallback += OnSetState;
		ent.setBlinkCallback += OnSetBlink;
	}
	
	void Awake() {
		mSpriteBatcher = GetComponent<tk2dStaticSpriteBatcher>();
		mSprite = GetComponent<tk2dBaseSprite>();
		mSpriteAnim = mSprite != null ? mSprite as tk2dAnimatedSprite : null;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(mIsBlink) {
			mBlinkTime += Time.deltaTime;
			if(mBlinkTime >= blinkDelay) {
				mBlinkTime = 0;
				
				if(mSpriteBatcher != null) {
					mSpriteBatcher.renderer.enabled = !mSpriteBatcher.renderer.enabled;
				}
				else {
					Color c = mSprite.color;
					c.a = c.a == 0.0f ? mPrevAlpha : 0.0f;
					mSprite.color = c;
				}
			}
		}
	}
	
	void OnSetState(Entity ent, Entity.State state) {
		PlayAnim(state);
	}
	
	void OnSetBlink(Entity ent, bool b) {
		mIsBlink = b;
		
		if(b) {
			if(mSprite != null) {
				mPrevAlpha = mSprite.color.a;
			}
			
			mBlinkTime = 0;
		}
		else {
			if(mSpriteBatcher != null) {
				mSpriteBatcher.renderer.enabled = true;
			}
			else {
				Color clr = mSprite.color; clr.a = mPrevAlpha;
				mSprite.color = clr;
			}
		}
	}
}
