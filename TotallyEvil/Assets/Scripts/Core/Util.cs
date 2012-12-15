using UnityEngine;
using System.Collections;

public class Util {
	public enum Side {
		None,
		Left,
		Right
	}
	
	public static float CheckSideSign(Vector2 up1, Vector2 up2) {
		return Vector2DCross(up1, up2) < 0 ? -1 : 1;
	}
	
	/// <summary>
	/// Checks which side up1 is in relation to up2
	/// </summary>
	public static Side CheckSide(Vector2 up1, Vector2 up2) {
		float s = Vector2DCross(up1, up2);
		return s == 0 ? Side.None : s < 0 ? Side.Right : Side.Left;
	}
	
	public static Vector2 Vector2DRot(Vector2 v, float r) {
		float c = Mathf.Cos(r);
		float s = Mathf.Sin(r);
		
		return new Vector2(v.x*c+v.y*s, -v.x*s+v.y*c);
	}
	
	public static float Vector2DCross(Vector2 v1, Vector2 v2) {
		return (v1.x*v2.y) - (v1.y*v2.x);
	}
	
	/// <summary>
	/// Caps given destDir with angleLimit (degree) on either side of srcDir, returns which side the destDir is capped relative to srcDir.
	/// </summary>
	/// <returns>
	/// The side destDir is relative to srcDir. (-1 or 1)
	/// </returns>
	public static float Vector2DDirCap(Vector2 srcDir, ref Vector2 destDir, float angleLimit) {
		
		float side = CheckSideSign(srcDir, destDir);
		
		float angle = Mathf.Acos(Vector2.Dot(srcDir, destDir));
		
		float limitAngle = angleLimit*Mathf.Deg2Rad;
		
		if(angle > limitAngle) {
			destDir = Vector2DRot(srcDir, -side*limitAngle);
		}
		
		return side;
	}
	
	public static Vector2 MouseToScreen() {
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
}
