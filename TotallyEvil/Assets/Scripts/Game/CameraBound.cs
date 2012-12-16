using UnityEngine;
using System.Collections;

public class CameraBound : MonoBehaviour {

	public float width;
	public float height;
	
	public Vector3 point;
	
	public Vector3 Cap(Vector3 center, float halfW, float halfH) {
		Vector3 pos = center;
		
		float hWorldW = width*0.5f;
		float hWorldH = height*0.5f;
		
		Vector3 wPos = transform.position;
		wPos.x -= hWorldW*0.5f;
		wPos.y -= hWorldH*0.5f;
		
		if(pos.x - halfW < wPos.x) {
			pos.x = wPos.x + halfW;
		}
		else if(pos.x + halfW > wPos.x + hWorldW) {
			pos.x = wPos.x + hWorldW - halfW;
		}
		
		if(pos.y - halfH < wPos.y) {
			pos.y = wPos.y + halfH;
		}
		else if(pos.y + halfH > wPos.y + hWorldH) {
			pos.y = wPos.y + hWorldH - halfH;
		}
		
		return pos;
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position, new Vector3(width*0.5f, height*0.5f, 0.1f));
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawIcon(transform.position + point, "point.png", true);
	}
}
