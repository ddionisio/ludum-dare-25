using UnityEngine;
using System.Collections;

public class AISetVelocity : SequencerAction {
	public float speedMin = 0;
	public float speedMax = 0;
	
	public bool toPlayer=true;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance state) {
		Entity ai = (Entity)behaviour;
		EntityMovement em = ai.entMove;
		
		float speed = speedMin < speedMax ? Random.Range(speedMin, speedMax) : speedMin;
		if(speed > 0) {
			em.velocity.x = toPlayer ? Player.instance.transform.position.x < ai.transform.position.x ? -speed : speed : speed;
			em.velocity.y = 0;
			
			ai.state = Entity.State.move;
		}
		else {
			em.velocity = Vector2.zero;
			
			ai.state = Entity.State.idle;
		}
	}
}
