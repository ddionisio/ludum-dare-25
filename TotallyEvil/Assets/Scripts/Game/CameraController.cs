using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	private static CameraController mInstance = null;
	
	private float mScale = 1.0f;
	
	private float mOrigSize;
	private Transform mAttach;
	private CameraBound mBound;
	
	public static CameraController instance {
		get { return mInstance; }
	}
	
	public float scale {
		get { return mScale; }
		set {
			mScale = value;
			camera.orthographicSize = mOrigSize*mScale;
		}
	}
	
	public Transform attach {
		get { return mAttach; }
		set {
			mAttach = value;
		}
	}
	
	public CameraBound bound {
		get { return mBound; }
		set { mBound = value; }
	}
	
	void OnDestroy() {
		mInstance = null;
	}
	
	void Awake() {
		mInstance = this;
		
		mOrigSize = camera.orthographicSize;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
				
		if(mAttach != null) {
			Vector3 newPos = mAttach.position;
			newPos.z = pos.z;
			
			//cap within bounds
			if(mBound != null) {
				float wRatio = camera.pixelWidth/camera.pixelHeight;
				
				transform.position = mBound.Cap(newPos, camera.orthographicSize*wRatio, camera.orthographicSize);
			}
			else {
				transform.position = newPos;
			}
		}
	}
}
