using UnityEngine;
using System.Collections;

public class AIMoveToPlayer : AIMoveRandom {

	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance aState) {
		Entity ai = (Entity)behaviour;
		AIState aiState = (AIState)aState;
		
		ai.state = state;
		
		Vector2 src = ai.transform.position;
		Vector2 dest = Player.instance.transform.position;
		
		switch(type) {
		case Type.xOnly:
			dest.y = src.y;
			break;
			
		case Type.yOnly:
			dest.x = src.x;
			break;
		}
		
		Vector2 dir = dest - src;
		float dist = dir.magnitude;
		dir = dir/dist;
		
		float spd = minSpeed < maxSpeed ? Random.Range(minSpeed, maxSpeed) : minSpeed;
		ai.entMove.velocity = dir*spd;
		
		aiState.d = dist/spd;
	}
}
