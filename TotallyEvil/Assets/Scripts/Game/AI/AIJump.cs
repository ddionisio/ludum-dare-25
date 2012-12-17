using UnityEngine;
using System.Collections;

public class AIJump : SequencerAction {

	public float speedMin=0;
	public float speedMax=0;
	
	public Entity.State state = Entity.State.jump;
	public Entity.State endState = Entity.State.idle;
	
	public override void Start(MonoBehaviour behaviour, Sequencer.StateInstance aState) {
		Entity ai = (Entity)behaviour;
		EntityMovement em = ai.entMove;
		em.Jump(speedMin < speedMax ? Random.Range(speedMin, speedMax) : speedMin);
		ai.state = state;
	}
	
	public override bool Update(MonoBehaviour behaviour, Sequencer.StateInstance aState) {
		Entity ai = (Entity)behaviour;
		EntityMovement em = ai.entMove;
		
		bool done = em.isGround;
		
		if(done && endState != Entity.State.NumState) {
			ai.state = endState;
		}
		
		return done;
	}
	
	public override void Finish(MonoBehaviour behaviour, Sequencer.StateInstance state) {
	}
}
