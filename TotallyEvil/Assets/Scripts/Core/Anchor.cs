using UnityEngine;

/// <summary>
/// Anchor Utilities. Note: y = 0 is bottom.
/// </summary>
public class Anchor {
	public enum Type {
		Top,
		Left,
		Right,
		Bottom,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
		Center
	}
	
	public static Vector2 GetPosition(Vector2 src, Vector2 size, Type anchor) {
		float x, y;
		
		if(anchor == Type.Right || anchor == Type.TopRight || anchor == Type.BottomRight) {
			x = src.x - size.x;
		}
		else if(anchor == Type.Center || anchor == Type.Top || anchor == Type.Bottom) {
			x = src.x - size.x*0.5f;
		}
		else {
			x = src.x;
		}
		
		if(anchor == Type.Top || anchor == Type.TopLeft || anchor == Type.TopRight) {
			y = src.y - size.y;
		}
		else if(anchor == Type.Center || anchor == Type.Left || anchor == Type.Right) {
			y = src.y - size.y*0.5f;
		}
		else {
			y = src.y;
		}
		
		return new Vector2(x, y);
	}
	
	public static Vector2 GetPositionInRect(Rect r, Type anchor) {
		float x, y;
		
		if(anchor == Type.Right || anchor == Type.TopRight || anchor == Type.BottomRight) {
			x = r.xMax;
		}
		else if(anchor == Type.Center || anchor == Type.Top || anchor == Type.Bottom) {
			x = r.center.x;
		}
		else {
			x = r.xMin;
		}
		
		if(anchor == Type.Top || anchor == Type.TopLeft || anchor == Type.TopRight) {
			y = r.yMin;
		}
		else if(anchor == Type.Center || anchor == Type.Left || anchor == Type.Right) {
			y = r.center.y;
		}
		else {
			y = r.yMax;
		}
		
		return new Vector2(x, y);
	}
}
