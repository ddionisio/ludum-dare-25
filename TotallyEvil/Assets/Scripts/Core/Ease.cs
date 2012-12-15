using UnityEngine;
using System.Collections;

public class Ease {
	public static float In(float t, float tMax, float start, float delta) {
		return start + delta*_in(t/tMax);
	}
	
	private static float _in(float r) {
		return r*r*r;
	}

	public static float Out(float t, float tMax, float start, float delta) {		
		return start + delta*_out(t/tMax);
	}
	
	private static float _out(float r) {
		float ir = r - 1.0f;
		return ir*ir*ir + 1.0f;
	}
	
	public static float OutElastic(float t, float tMax, float start, float delta) {
        return start + (delta * _outElastic(t/tMax));
	}
	
	private static float _outElastic(float ratio) {
        if(ratio == 0.0f || ratio == 1.0f) return ratio;
        
        float p = 0.3f;
        float s = p / 4.0f;
        return -1.0f * Mathf.Pow(2.0f, -10.0f*ratio) * Mathf.Sin((ratio-s)*2.0f*Mathf.PI/p) + 1.0f;
	}
	
	public static float InBounce(float t, float tMax, float start, float delta) {
		return start + (delta * _inBounce(t/tMax));
	}
	
	private static float _inBounce(float ratio) {
        return 1.0f - _outBounce(1.0f - ratio);
	}
	
	private static float _outBounce(float ratio) {
        float s = 7.5625f;
        float p = 2.75f;
        float l;
        if(ratio < (1.0f/p)) 
        	l = s * Mathf.Pow(ratio, 2.0f);
        else {
            if(ratio < (2.0f/p)) {
                ratio = ratio - (1.5f/p);
                l = s * Mathf.Pow(ratio, 2.0f) + 0.75f;
			} else {
                if(ratio < (2.5f/p)) {
                    ratio = ratio - (2.25f/p);
                    l = s * Mathf.Pow(ratio, 2.0f) + 0.9375f;
				} else {
                    ratio = ratio - (2.65f/p);
                    l = s * Mathf.Pow(ratio, 2.0f) + 0.984375f;
				}
            }
        }
        return l;
	}
	
	public static float InElastic(float t, float tMax, float start, float delta) {
		return start + (delta * _inElastic(t/tMax));
	}
	
	private static float _inElastic(float ratio) {
        if(ratio == 0.0f || ratio == 1.0f) return ratio;
        
        float p = 0.3f;
        float s = p / 4.0f;
        float invRatio = ratio - 1.0f;
        return -1 * Mathf.Pow(2.0f, 10.0f*invRatio) * Mathf.Sin((invRatio-s)*2.0f*Mathf.PI/p);
	}
}
