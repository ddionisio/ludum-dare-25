using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NGUILayoutFlow : MonoBehaviour {
	
	public enum Arrangement
	{
		Horizontal,
		Vertical,
	}
	
	public Arrangement arrangement = Arrangement.Horizontal;
	public float padding;
	public bool repositionNow = false;
	public bool relativeLineup = false;
	
	//TODO: use enums
	public bool centerLineup = false;
	public bool endLineup = false; //right align for horizontal, bottom align for vertical

	// Use this for initialization
	void Start () {
		Reposition();
	}
	
	// Update is called once per frame
	void Update () {
		if (repositionNow)
			Reposition();
	}
	
	public void Reposition () {		
		int count = transform.childCount;
		
		Bounds[] bounds = new Bounds[count];
		
		float bMax = float.MinValue, bMin = float.MaxValue;
		
		for (int i = 0; i < count; ++i) {
			Transform t = transform.GetChild(i);
			
			Bounds b = NGUIMath.CalculateRelativeWidgetBounds(t);
			Vector3 scale = t.localScale;
			b.min = Vector3.Scale(b.min, scale);
			b.max = Vector3.Scale(b.max, scale);
			bounds[i] = b;
			
			switch(arrangement) {
			case Arrangement.Horizontal:
				if(bMax < b.max.y)
					bMax = b.max.y;
				if(bMin > b.min.y)
					bMin = b.min.y;
				break;
				
			case Arrangement.Vertical:
				if(bMax < b.max.x)
					bMax = b.max.x;
				if(bMin > b.min.x)
					bMin = b.min.x;
				break;
			}
		}
		
		float offset = 0;
		
		for (int i = 0; i < count; ++i) {
			Transform t = transform.GetChild(i);
			
			if (!t.gameObject.active) continue;
			
			Bounds b = bounds[i];
									
			Vector3 pos = t.localPosition;
			
			switch(arrangement) {
			case Arrangement.Horizontal:
				pos.x = offset + b.extents.x - b.center.x;
				pos.y = relativeLineup ? 0 : -(b.extents.y + b.center.y) + (b.max.y - b.min.y - bMax + bMin) * 0.5f;
				
				offset += b.max.x - b.min.x + padding;
				break;
				
			case Arrangement.Vertical:
				pos.x = relativeLineup ? 0 : (b.extents.x - b.center.x) + (b.min.x - bMin);
				pos.y = -(offset + b.extents.y + b.center.y);
				
				offset += b.size.y + padding;
				break;
			}
			
			pos.x = Mathf.Round(pos.x);
			pos.y = Mathf.Round(pos.y);

			t.localPosition = pos;
		}
		
		if(centerLineup) {
			Bounds b = NGUIMath.CalculateRelativeWidgetBounds(transform);
			
			switch(arrangement) {
			case Arrangement.Horizontal:
				foreach(Transform t in transform) {
					Vector3 pos = t.localPosition;
					
					pos.x = Mathf.Round(pos.x - b.extents.x);
					
					t.localPosition = pos;
				}
				break;
				
			case Arrangement.Vertical:
				foreach(Transform t in transform) {
					Vector3 pos = t.localPosition;
					
					pos.y = Mathf.Round(pos.y + b.extents.y);
					
					t.localPosition = pos;
				}
				break;
			}
		}
		else if(endLineup) {
			Bounds b = NGUIMath.CalculateRelativeWidgetBounds(transform);
			
			switch(arrangement) {
			case Arrangement.Horizontal:
				//TODO
				foreach(Transform t in transform) {
					Vector3 pos = t.localPosition;
					
					pos.x = Mathf.Round(pos.x - b.size.x);
					
					t.localPosition = pos;
				}
				break;
				
			case Arrangement.Vertical:
				foreach(Transform t in transform) {
					Vector3 pos = t.localPosition;
					
					pos.y = Mathf.Round(pos.y + b.size.y);// .extents.y);
					
					t.localPosition = pos;
				}
				break;
			}
		}
		
		repositionNow = false;
	}
}