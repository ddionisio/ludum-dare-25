using UnityEngine;
using System.Collections;

public class AIMoveRandom : SequencerAction {
	public enum Type {
		xOnly,
		yOnly,
		Both
	}
	
	public Type type = Type.xOnly;
	public Entity.State state = Entity.State.move;
	public Entity.State endState = Entity.State.idle;
	
	public float minSpeed;
	public float maxSpeed;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance aState) {
		Entity ai = (Entity)behaviour;
		AIState aiState = (AIState)aState;
		
		ai.state = state;
		
		CameraBound bound = CameraController.instance.bound;
		if(bound != null && ai.entMove != null) {
			float r = ai.entMove.radius;
			Vector2 src = ai.transform.position;
			Vector2 dest = bound.RandomLocation(r, r);
			
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
		else {
			aiState.d = 0;
		}
	}
	
	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance aState) {
		AIState aiState = (AIState)aState;
		
		return Time.time - aiState.startTime >= aiState.d;
	}
	
	//do clean ups here, don't set any states dependent from outside
	public override void Finish(MonoBehaviour behaviour, Sequencer.StateInstance aState) {
		Entity ai = (Entity)behaviour;
		ai.state = endState;
		
		if(ai.entMove != null) {
			ai.entMove.velocity = Vector2.zero;
		}
	}
}
